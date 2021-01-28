using HotChocolate;
using HotChocolate.Configuration;
using HotChocolate.Types.Descriptors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace WebApplication1
{
    public partial class Startup
    {
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
    }
}
