using Infrastructure.MQ;
using Infrastructure.Services;
using Infrastructure.Genesys.Auth;
using Infrastructure.Respositories;
using Infrastructure.Genesys.Client;
using Application.Common.Interfaces;
using Application.Interfaces.Services;
using Application.Common.Interfaces.MQ;
using Application.Interfaces.Repositories;
using Application.Common.Interfaces.Genesys;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection InfrastructureDependencies(this IServiceCollection services)
    {
        return services
                .AddTransient<IDateTimeService, SystemDateTimeService>()
                .AddTransient(typeof(IRepositoryAsync<,>), typeof(RepositoryAsync<,>))
                .AddTransient(typeof(IUnitOfWork<>), typeof(UnitOfWork<>))
                .AddTransient<IGenesysApiClient, GenesysApiClient>()
                .AddTransient<IGenesysConfigurationHandler, GenesysConfigurationHandler>()
                .AddTransient<IGenesysEventApiService, GenesysEventApiService>()
                .AddTransient<IMQReaderClient, MQReaderClient>()
                .AddTransient<IMQWriterClient, MQWriterClient>();
    }
}