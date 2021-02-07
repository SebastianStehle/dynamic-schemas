using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace WebApplication1.TestSchema
{
    public static class Builder
    {
        public static void AddDynamicSchema(this IRequestExecutorBuilder builder)
        {
            var schemas = Enumerable.Range(0, 100).Select(x => $"Content{x}").ToHashSet();

            builder.AddType(new ContentInterfaceType());

            foreach (var schema in schemas)
            {
                var fields = Enumerable.Range(0, 100).Select(x => $"field{x}").ToHashSet();

                builder.AddType(new ContentType(schema, fields));
            }

            builder.AddQueryType(new QueryType(schemas));
        }
    }
}
