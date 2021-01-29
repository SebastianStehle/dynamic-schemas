using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication1
{
    public partial class Startup
    {
        public class Result
        {
            public string Value { get; set; }
        }

        public class ResultType : ObjectType<Result>
        {
            protected override void Configure(IObjectTypeDescriptor<Result> descriptor)
            {
                descriptor.Name("Result");

                descriptor.Field(x => x.Value)
                    .Type<NonNullType<StringType>>();
            }
        }

        public class QueryFooType : ObjectType
        {
            protected override void Configure(IObjectTypeDescriptor descriptor)
            {
                descriptor.Field("both")
                    .Resolve(new FieldResolverDelegate(x =>
                    {
                        return new ValueTask<object>(new List<Result>
                        {
                            new Result
                            {
                                Value = "foo_both"
                            }
                        });
                    }))
                    .Type<NonNullType<ListType<NonNullType<ResultType>>>>();

                descriptor.Field("foo")
                    .Resolve(new FieldResolverDelegate(x =>
                    {
                        return new ValueTask<object>(new List<Result>
                        {
                            new Result
                            {
                                Value = "foo_only"
                            }
                        });
                    }))
                    .Type<NonNullType<ListType<NonNullType<ResultType>>>>();
            }
        }
        public class QueryBarType : ObjectType
        {
            protected override void Configure(IObjectTypeDescriptor descriptor)
            {
                descriptor.Field("both")
                    .Resolve(new FieldResolverDelegate(x =>
                    {
                        return new ValueTask<object>(new List<Result>
                        {
                            new Result
                            {
                                Value = "bar_both"
                            }
                        });
                    }))
                    .Type<NonNullType<ListType<NonNullType<ResultType>>>>();

                descriptor.Field("bar")
                    .Resolve(new FieldResolverDelegate(x =>
                    {
                        return new ValueTask<object>(new List<Result>
                        {
                            new Result
                            {
                                Value = "bar_only"
                            }
                        });
                    }))
                    .Type<NonNullType<ListType<NonNullType<ResultType>>>>();
            }
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
            services.AddGraphQLServer("myFooType")
                .AddQueryType<QueryFooType>();
            services.AddGraphQLServer("myBarType")
                .AddQueryType<QueryBarType>();
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
                endpoints.MapGraphQL("/graphql/fooType", "myFooType");
                endpoints.MapGraphQL("/graphql/barType", "myBarType");
            });
        }
    }
}
