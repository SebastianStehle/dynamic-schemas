using HotChocolate.Language;
using HotChocolate.Types;
using System.Collections.Generic;

namespace WebApplication1.TestSchema
{
    public class ContentType : ObjectType
    {
        private readonly string schemaName;
        private readonly HashSet<string> fields;

        public ContentType(string schemaName, HashSet<string> fields)
        {
            this.schemaName = schemaName;
            this.fields = fields;
        }

        protected override void Configure(IObjectTypeDescriptor descriptor)
        {
            descriptor.Name(schemaName);

            descriptor.Interface(new NamedTypeNode("Content"));

            descriptor.Field("id")
                .Type<NonNullType<StringType>>()
                .Description("The id of the content.")
                .Resolve(schemaName);

            foreach (var field in fields)
            {
                descriptor.Field(field).Resolve(field)
                    .Type<NonNullType<StringType>>()
                    .Description($"The {field} of the content.");
            }
        }
    }
}
