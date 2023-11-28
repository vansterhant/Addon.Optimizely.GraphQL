using EPiServer.SpecializedProperties;

namespace Addon.Optimizely.GraphQL;

public class LinkItemType : ObjectType<LinkItem>
{
    protected override void Configure(IObjectTypeDescriptor<LinkItem> descriptor)
    {
        base.Configure(descriptor);
        descriptor.BindFieldsExplicitly();
        descriptor.Field(t => t.Href).Type<UrlType>();
        descriptor.Field(t => t.Text).Type<StringType>();
        descriptor.Field(t => t.Target).Type<StringType>();
        descriptor.Field(t => t.Title).Type<StringType>();
    }
}