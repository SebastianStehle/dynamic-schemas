using HotChocolate.Language;
using HotChocolate.Types;
using System.Collections.Generic;

namespace WebApplication1.TestSchema
{
    public class QueryType : ObjectType
    {
        private readonly HashSet<string> schemas;

        public QueryType(HashSet<string> schemas)
        {
            this.schemas = schemas;
        }

        protected override void Configure(IObjectTypeDescriptor descriptor)
        {
            foreach (var schema in schemas)
            {
                descriptor.Field($"get{schema}")
                    .Type(new NamedTypeNode(schema))
                    .Resolve(x => new object());
            }
        }
    }
}
