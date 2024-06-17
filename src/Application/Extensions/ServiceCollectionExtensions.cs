using System.Reflection;
using Application.Common.Behaviors;
using Application.Common.ResilliencePolicies;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection ApplicationDependencies(this IServiceCollection services)
	{
		return services.AddAutoMapper(Assembly.GetExecutingAssembly())
						.AddMediatR(cfg =>
						{
							cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
							cfg.AddOpenBehavior(typeof(RequestResponseLoggingBehavior<,>));
						})
						.ResilliencePipelineExtension();
	}
}