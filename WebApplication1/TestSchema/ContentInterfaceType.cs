using HotChocolate.Types;

namespace WebApplication1.TestSchema
{
    public class ContentInterfaceType : InterfaceType
    {
        protected override void Configure(IInterfaceTypeDescriptor descriptor)
        {
            descriptor.Name("Content")
                .Description("The basic structure for all content types.");

            descriptor.Field("id")
                .Type<NonNullType<StringType>>()
                .Description("The id of the content.");
        }
    }
}
