using EPiServer;
using EPiServer.Core;

namespace Addon.Optimizely.GraphQL;

public class PageDataInterfaceType : InterfaceType<PageData>
{
    protected override void Configure(IInterfaceTypeDescriptor<PageData> descriptor)
    {
        base.Configure(descriptor);

        descriptor.Name("Page");
        descriptor.ResolveAbstractType((c, r) => c.Schema.Types.OfType<ObjectType>()
                .FirstOrDefault(t => t.RuntimeType == r.GetOriginalType()));
        
        descriptor.BindFields(BindingBehavior.Explicit);
        
        descriptor.Field(t => t.VisibleInMenu).Type<BooleanType>();
        
        descriptor.Field("_parent").Type<PageDataInterfaceType>();
        descriptor.Field("_ancestors").Type<ListType<PageDataInterfaceType>>();

        descriptor.Implements<ContentInterfaceType>();
    }
}
