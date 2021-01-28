using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Execution.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddMvc();
            services.AddMemoryCache();
            services.AddHttpContextAccessor();
#if DYNAMIC
            services.AddSingleton<ISchemaInterceptor, Interceptor>();
            services.AddSingleton<GraphQLService>();
            services.AddGraphQLServer();
            services.AddSingletonWrapper<IRequestExecutorResolver, CachingRequestExecutorResolver>();
            services.AddSingleton<IRequestExecutorOptionsMonitor, SimpleRequestExecutorOptionsMonitor>();
#else
            services.AddGraphQLServer("myFoo")
                .AddQueryType<QueryFoo>();
            services.AddGraphQLServer("myBar")
                .AddQueryType<QueryBar>();
#endif
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

                endpoints.MapGraphQL("/graphql/foo", "myFoo");
                endpoints.MapGraphQL("/graphql/bar", "myBar");
            });
        }
    }
}
