using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace WebApplication1
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSingletonWrapper<TInterface, TImplementation>(this IServiceCollection services)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            var existing = services.First(x => x.ServiceType == typeof(TInterface));

            if (existing == null)
            {
                services.AddSingleton<TInterface, TImplementation>();
            }
            else if (existing.ImplementationType != null)
            {
                services.AddSingleton<TInterface>(c =>
                {
                    var inner = (TInterface)ActivatorUtilities.CreateInstance(c, existing.ImplementationType);

                    return ActivatorUtilities.CreateInstance<TImplementation>(c, inner);
                });
            }
            else if (existing.ImplementationFactory != null)
            {
                services.AddSingleton<TInterface>(c =>
                {
                    var inner = existing.ImplementationFactory(c);

                    return ActivatorUtilities.CreateInstance<TImplementation>(c, inner);
                });
            }

            return services;
        }
    }
}
