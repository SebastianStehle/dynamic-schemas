using HotChocolate;
using HotChocolate.Configuration;
using HotChocolate.Execution;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types.Descriptors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;

namespace WebApplication1
{
    public partial class Startup
    {
        public class Result
        {
            public string Value { get; set; }
        }

        public class QueryFoo
        {
            public List<Result> GetBoth()
            {
                return new List<Result>
                {
                    new Result
                    {
                        Value = "foo_both"
                    }
                };
            }

            public List<Result> GetFoo()
            {
                return new List<Result> 
                {
                    new Result 
                    {
                        Value = "foo_only"
                    }
                };
            }
        }
        public class QueryBar
        {
            public List<Result> GetBoth()
            {
                return new List<Result>
                {
                    new Result
                    {
                        Value = "bar_both"
                    }
                };
            }

            public List<Result> GetBar()
            {
                return new List<Result> 
                {
                    new Result
                    {
                        Value = "bar_only"
                    }
                };
            }
        }

        class Interceptor : ISchemaInterceptor
        {
            private readonly IHttpContextAccessor httpContextAccessor;

            public Interceptor(IHttpContextAccessor httpContextAccessor)
            {
                this.httpContextAccessor = httpContextAccessor;
            }
            public void OnAfterCreate(IDescriptorContext context, ISchema schema)
            {
            }

            public void OnBeforeCreate(IDescriptorContext context, ISchemaBuilder schemaBuilder)
            {
                // No idea how to get the name in a better way :( 
                var name = (string)httpContextAccessor.HttpContext.GetRouteValue("app");

                if (name == "bar")
                {
                    schemaBuilder.AddQueryType<QueryBar>();
                }
                else if (name == "foo")
                {
                    schemaBuilder.AddQueryType<QueryFoo>();
                }
            }

            public void OnError(IDescriptorContext context, Exception exception)
            {
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddMvc();
            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            services.AddSingleton<ISchemaInterceptor, Interceptor>();
            services.AddSingleton<GraphQLService>();
            services.AddGraphQLServer();
            services.AddSingletonWrapper<IRequestExecutorResolver, CachingRequestExecutorResolver>();
            services.AddSingleton<IRequestExecutorOptionsMonitor, SimpleRequestExecutorOptionsMonitor>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
