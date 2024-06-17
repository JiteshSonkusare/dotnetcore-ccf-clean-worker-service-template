using AutoMapper;
using Domain.Entities;
using Domain.Models.Genesys;

namespace Application.Tasks.SendEvent.Mapper;

public class EventMapper : Profile
{
	public EventMapper()
	{
		CreateMap<Event, EventRequest>()
			.ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.Id.ToString()));
	}
}