using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Serialization;
using HotChocolate.Execution;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using RequestDelegate = Microsoft.AspNetCore.Http.RequestDelegate;

namespace WebApplication1
{
    public class GraphQLService
    {
        private readonly IFileProvider fileProvider;
        private readonly IServiceProvider serviceProvider;
        private readonly IRequestExecutorResolver requestExecutorResolver;
        private readonly ConcurrentDictionary<string, Pipeline> pipelines = new ConcurrentDictionary<string, Pipeline>();

        public GraphQLService(IServiceProvider serviceProvider, IRequestExecutorResolver requestExecutorResolver)
        {
            this.serviceProvider = serviceProvider;
            this.requestExecutorResolver = requestExecutorResolver;
            this.requestExecutorResolver.RequestExecutorEvicted += RequestExecutorResolver_RequestExecutorEvicted;

            fileProvider = CreateFileProvider();
        }

        private void RequestExecutorResolver_RequestExecutorEvicted(object sender, RequestExecutorEvictedEventArgs e)
        {
            pipelines.TryRemove(e.Name, out var _);
        }

        class Pipeline
        {
            private readonly RequestDelegate requestDeelgate;

            public Pipeline(IServiceProvider serviceProvider, IFileProvider fileProvider, PathString path)
            {
                requestDeelgate =
                    new ApplicationBuilder(serviceProvider)
                        .UseMiddleware<WebSocketSubscriptionMiddleware>(Schema.DefaultName)
                        .UseMiddleware<HttpPostMiddleware>(Schema.DefaultName)
                        .UseMiddleware<HttpGetSchemaMiddleware>(Schema.DefaultName)
                        .UseMiddleware<ToolDefaultFileMiddleware>(fileProvider, path)
                        .UseMiddleware<ToolOptionsFileMiddleware>(Schema.DefaultName, path)
                        .UseMiddleware<ToolStaticFileMiddleware>(fileProvider, path)
                        .UseMiddleware<HttpGetMiddleware>(Schema.DefaultName)
                        .Use(next => context =>
                        {
                            context.Response.StatusCode = 404;

                            return Task.CompletedTask;
                        })
                        .Build();
            }

            public async Task ExecuteAsync(HttpContext httpContext, string basePath)
            {
                var request = httpContext.Request;

                var previousPath = request.Path;
                var previousPathBase = request.PathBase;
                try
                {
                    request.PathBase = basePath;
                    request.Path = previousPath.Value![basePath.Length..];

                    await requestDeelgate(httpContext);
                }
                finally
                {
                    request.PathBase = previousPathBase;
                    request.Path = previousPath;
                }
            }
        }

        public async Task ExecuteAsync(HttpContext httpContext, string name, string basePath, string path)
        {
            var pipeline = pipelines.GetOrAdd(name, x => new Pipeline(serviceProvider, fileProvider, path));

            await pipeline.ExecuteAsync(httpContext, basePath);
        }

        private static IFileProvider CreateFileProvider()
        {
            var resourceNamespace = $"{typeof(MiddlewareBase).Namespace}.Resources";

            
            return new EmbeddedFileProvider(typeof(MiddlewareBase).Assembly, resourceNamespace);
        }
    }
}