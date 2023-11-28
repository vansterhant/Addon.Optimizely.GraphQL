using EPiServer.Web;
using HotChocolate.Data.Filters;

namespace Addon.Optimizely.GraphQL;

public class SiteDefinitionType : ObjectType<SiteDefinition>
{
    protected override void Configure(IObjectTypeDescriptor<SiteDefinition> descriptor)
    {
        base.Configure(descriptor);
        
        descriptor.BindFields(BindingBehavior.Explicit);
        
        descriptor.Field(t => t.Id).Type<IdType>();
        descriptor.Field(t => t.Name).Type<StringType>();
        descriptor.Field(t => t.SiteUrl).Type<UrlType>();
        descriptor.Field(t => t.Hosts).Type<ListType<HostDefinitionType>>();
        
        descriptor.Field(t => t.RootPage).Type<StringType>()
            .Resolve(context => context.Parent<SiteDefinition>().RootPage?.ToString());
        descriptor.Field(t => t.StartPage).Type<StringType>()
            .Resolve(context => context.Parent<SiteDefinition>().StartPage?.ToString());
        descriptor.Field(t => t.ContentAssetsRoot).Type<StringType>()
            .Resolve(context => context.Parent<SiteDefinition>().ContentAssetsRoot?.ToString());
        descriptor.Field(t => t.GlobalAssetsRoot).Type<StringType>()
            .Resolve(context => context.Parent<SiteDefinition>().GlobalAssetsRoot?.ToString());
    }
}

public class SiteDefinitionFilterType : FilterInputType<SiteDefinition>
{
    protected override void Configure(IFilterInputTypeDescriptor<SiteDefinition> descriptor)
    {
        base.Configure(descriptor);
        
        descriptor.BindFields(BindingBehavior.Explicit);
        
        descriptor.Field(t => t.Id);
        descriptor.Field(t => t.Name);
        descriptor.Field(t => t.SiteUrl);
        descriptor.Field(t => t.RootPage);
        descriptor.Field(t => t.StartPage);
    }
}

public class HostDefinitionType  : ObjectType<HostDefinition>
{
    protected override void Configure(IObjectTypeDescriptor<HostDefinition> descriptor)
    {
        base.Configure(descriptor);
        
        descriptor.BindFields(BindingBehavior.Explicit);
        
        descriptor.Field(t => t.Name).Type<StringType>();
        descriptor.Field(t => t.Language).Type<StringType>()
            .Resolve(context => context.Parent<HostDefinition>().Language?.Name);
        descriptor.Field(t => t.Type).Type<EnumType<EPiServer.Web.HostDefinitionType>>();

    }
}