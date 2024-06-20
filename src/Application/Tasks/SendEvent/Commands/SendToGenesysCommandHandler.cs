using MediatR;
using AutoMapper;
using Domain.Entities;
using Shared.Wrapper;
using Domain.Models.Genesys;
using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using static Domain.Errors.ErrorDto;
using Application.Interfaces.Services;
using Application.Interfaces.Repositories;
using Application.Common.ExceptionHandlers;

namespace Application.Tasks.SendEvent.Commands;

internal class SendToGenesysCommandHandler : IRequestHandler<SendEventCommand, Result>
{
	private readonly IMapper _mapper;
	private readonly IUnitOfWork<int> _unitOfWork;
	private readonly IDateTimeService _dateTimeService;
	private readonly IGenesysEventApiService _genesysEventApiService;

	public SendToGenesysCommandHandler(IMapper mapper, IUnitOfWork<int> unitOfWork, IDateTimeService dateTimeService, IGenesysEventApiService genesysEventApiService)
	{
		_mapper = mapper;
		_unitOfWork = unitOfWork;
		_dateTimeService = dateTimeService;
		_genesysEventApiService = genesysEventApiService;
	}

	public async Task<Result> Handle(SendEventCommand request, CancellationToken cancellationToken)
	{
		try
		{
			var events = await _unitOfWork.Repository<Event>().Entities
				.Where(m => m.ProcessedOnUtc == null)
				.ToListAsync(cancellationToken);

			if (events.Count == 0)
				return Result.Failure(TasksErrors.EventsNotFound);

			foreach (var item in events)
			{
				var mappedEvents = _mapper.Map<EventRequest>(item);
				var result = await _genesysEventApiService.CreateEvent(mappedEvents);

				if (result.Suceeded)	
				{
					item.ProcessedOnUtc = _dateTimeService.Now;
					await _unitOfWork.Repository<Event>().UpdateAsync(item);
				}
			}

			await _unitOfWork.CommitAsync(cancellationToken);
			return Result.Success();
		}
		catch (Exception ex)
		{
			throw ex.With(ex.Source, ex.Message, ex.StackTrace ?? string.Empty)
					.DetailData("command", nameof(request));
		}
	}
}