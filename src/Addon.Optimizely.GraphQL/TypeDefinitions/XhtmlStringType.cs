using EPiServer.Core;

namespace Addon.Optimizely.GraphQL;

public class XhtmlStringType : ObjectType<XhtmlString>
{
    protected override void Configure(IObjectTypeDescriptor<XhtmlString> descriptor)
    {
        base.Configure(descriptor);
        descriptor.BindFieldsExplicitly();
        descriptor.Field(t => t.ToHtmlString()).Name("html").Type<StringType>();
    }
}
