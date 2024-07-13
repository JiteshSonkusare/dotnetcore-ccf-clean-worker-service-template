using Shared.Wrapper;
using Domain.Models.Genesys;
using Domain.Config.Genesys;
using Microsoft.Extensions.Options;
using Application.Common.Interfaces;
using static Domain.Errors.ErrorDto;
using PureCloudPlatform.Client.V2.Api;
using Application.Interfaces.Services;
using Application.Common.ExceptionHandlers;

namespace Infrastructure.Services;

public class GenesysEventApiService : IGenesysEventApiService
{
    private readonly GenesysApiConfig _config;
    private readonly ArchitectApi _architectApi;
    private readonly IGenesysApiClient _genesysApiClient;

	public GenesysEventApiService(IOptions<GenesysApiConfig> config, IGenesysApiClient genesysApiClient)
    {
        _config = config.Value;
        _genesysApiClient = genesysApiClient;
        _architectApi = _genesysApiClient.CreateApiInstance<ArchitectApi>();
	}

    public async Task<Result> CreateEvent(EventRequest eventsRequest)
    {
        try
        {
			var datatableId = await _genesysApiClient.ExecuteAsync(() =>
                _architectApi.GetFlowsDatatablesAsync(name: _config.DatatableName),
                useRetry: true);

            if (datatableId.Entities.Count == 0)
                return Result.Failure(GenesysEventError.DatatableNameNotFound);

            var result = await _genesysApiClient.ExecuteAsync(() =>
                _architectApi.PostFlowsDatatableRowsAsync(datatableId?.Entities?.FirstOrDefault()?.Id, eventsRequest),
                useRetry: true);

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