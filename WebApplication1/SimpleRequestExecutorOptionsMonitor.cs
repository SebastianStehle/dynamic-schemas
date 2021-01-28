using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplication1
{
    public partial class Startup
    {
        public sealed class SimpleRequestExecutorOptionsMonitor : IRequestExecutorOptionsMonitor
        {
            public ValueTask<RequestExecutorSetup> GetAsync(NameString schemaName, CancellationToken cancellationToken)
            {
                var setup = new RequestExecutorSetup();

                setup.SchemaServices.Add(services =>
                {
                    services.AddSingleton<IHttpRequestInterceptor, DefaultHttpRequestInterceptor>();
                });

                return new ValueTask<RequestExecutorSetup>(setup);
            }

            public IDisposable OnChange(Action<NameString> listener)
            {
                return new NoopDisposable();
            }

            class NoopDisposable : IDisposable
            {
                public void Dispose()
                {
                }
            }
        }
    }
}
