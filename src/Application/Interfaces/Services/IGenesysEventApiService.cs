using Domain.Models.Genesys;
using Shared.Wrapper;

namespace Application.Interfaces.Services;

public interface IGenesysEventApiService
{
	Task<Result> CreateEvent(EventRequest eventsRequest);
}