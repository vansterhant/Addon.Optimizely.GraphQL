using EPiServer.Core;
using HotChocolate.Data.Filters;
using HotChocolate.Data.Sorting;

namespace Addon.Optimizely.GraphQL;

public abstract class PageDataTypeBase<T> : ObjectType<T> where T : PageData
{
    protected override void Configure(IObjectTypeDescriptor<T> descriptor)
    {
        base.Configure(descriptor);

        descriptor.BindFields(BindingBehavior.Explicit);

        descriptor.Field(t => t.VisibleInMenu).Type<BooleanType>();

        descriptor.Field(t => t.Status).Type<EnumType<VersionStatus>>();
        descriptor.Field(t => t.IsPendingPublish).Type<BooleanType>();
        descriptor.Field(t => t.StartPublish).Type<DateType>();
        descriptor.Field(t => t.StopPublish).Type<DateType>();
        
        descriptor.Field(t => t.Category).Type<ListType<IntType>>();

        descriptor.Field(t => t.Language).Type<StringType>()
            .Resolve(context => context.Parent<PageData>().Language.Name);
        descriptor.Field(t => t.ExistingLanguages).Type<ListType<StringType>>()
            .Resolve(context => context.Parent<PageData>().ExistingLanguages.Select(x => x.Name));
        descriptor.Field(t => t.MasterLanguage).Type<StringType>()
            .Resolve(context => context.Parent<PageData>().MasterLanguage.Name);

        descriptor.Field(t => t.Name).Type<StringType>();
        descriptor.Field(t => t.ContentLink).Type<IdType>()
            .Resolve(context => context.Parent<PageData>().ContentLink.ToString());
        descriptor.Field(t => t.ParentLink).Type<StringType>()
            .Resolve(context => context.Parent<PageData>().ContentLink.ToString());
        descriptor.Field(t => t.ContentGuid).Type<UuidType>();
        descriptor.Field(t => t.IsDeleted).Type<BooleanType>();

        descriptor.Field(t => t.Created).Type<DateType>();
        descriptor.Field(t => t.CreatedBy).Type<StringType>();
        descriptor.Field(t => t.Changed).Type<DateType>();
        descriptor.Field(t => t.ChangedBy).Type<StringType>();
        descriptor.Field(t => t.Deleted).Type<DateType>();
        descriptor.Field(t => t.DeletedBy).Type<StringType>();
        descriptor.Field(t => t.Saved).Type<DateType>();
        
        descriptor.Field("_parent").Type<PageDataInterfaceType>()
            .ResolveWith<Resolvers>(r => r.GetParent(default!, default!));
        
        descriptor.Field("_ancestors").Type<ListType<PageDataInterfaceType>>()
            .ResolveWith<Resolvers>(r => r.GetAncestors(default!, default!));

        descriptor.Implements<PageDataInterfaceType>();
    }
}

public class PageDataType : PageDataTypeBase<PageData>
{
}