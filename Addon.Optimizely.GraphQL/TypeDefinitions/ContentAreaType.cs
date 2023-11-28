using EPiServer.Core;

namespace Addon.Optimizely.GraphQL;

public class ContentAreaType : ObjectType<ContentArea>
{
    protected override void Configure(IObjectTypeDescriptor<ContentArea> descriptor)
    {
        base.Configure(descriptor);
        
        descriptor.BindFieldsExplicitly();
        
        descriptor.Field(t => t.FilteredItems)
            .Type<ListType<ContentInterfaceType>>()
            .ResolveWith<Resolvers>(r => r.GetItemsForContentArea(default, default));

        descriptor.Field(t => t.Tag).Type<StringType>();
    }
}