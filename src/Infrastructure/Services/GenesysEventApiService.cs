using Shared.Wrapper;
using Domain.Models.Genesys;
using Application.Common.Interfaces;
using static Domain.Errors.ErrorDto;
using PureCloudPlatform.Client.V2.Api;
using Application.Interfaces.Services;
using Application.Common.ExceptionHandlers;

namespace Infrastructure.Services;

public class GenesysEventApiService : IGenesysEventApiService
{
	private const string DatatableName = "WorkflowEvent";

	private readonly IGenesysApiClient _genesysApiClient;
	private readonly ArchitectApi _architectApi;

	public GenesysEventApiService(IGenesysApiClient genesysApiClient)
	{
		_genesysApiClient = genesysApiClient;
		_architectApi = _genesysApiClient.CreateApiInstance<ArchitectApi>();
	}

	public async Task<Result> CreateEvent(EventRequest eventsRequest)
	{
		try
		{
			var datatableId = await _genesysApiClient.ExecuteWithRetryAsync(() =>
				_architectApi.GetFlowsDatatablesAsync(name: DatatableName));

			if (datatableId.Entities.Count == 0)
				return Result.Failure(GenesysEventError.DatatableNameNotFound);

			var result = await _genesysApiClient.ExecuteWithRetryAsync(() =>
				_architectApi.PostFlowsDatatableRowsAsync(datatableId?.Entities?.FirstOrDefault()?.Id, eventsRequest));

			return Result.Success();
		}
		catch (Exception ex)
		{
			throw ex.With(ex.Message, ex.Source, ex.StackTrace)
					.DetailData(nameof(eventsRequest.Key), eventsRequest.Key)
					.DetailData(nameof(eventsRequest.Note), eventsRequest.Note ?? string.Empty)
					.DetailData(nameof(eventsRequest.Status), eventsRequest.Status ?? string.Empty);
		}
	}
}