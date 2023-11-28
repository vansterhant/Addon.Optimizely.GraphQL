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
            .ResolveWith<Resolvers>(r => r.GetParent(default, default));
        
        descriptor.Field("_ancestors").Type<ListType<PageDataInterfaceType>>()
            .ResolveWith<Resolvers>(r => r.GetAncestors(default, default));

        descriptor.Implements<PageDataInterfaceType>();
    }
}

public abstract class PageDataFilterTypeBase<T> : FilterInputType<T> where T : PageData
{
    protected override void Configure(IFilterInputTypeDescriptor<T> descriptor)
    {
        base.Configure(descriptor);

        descriptor.BindFields(BindingBehavior.Explicit);

        descriptor.Field(t => t.VisibleInMenu);
        
        descriptor.Field(t => t.Name);
        descriptor.Field(t => t.ContentLink);
        descriptor.Field(t => t.ParentLink);
        descriptor.Field(t => t.ContentGuid);
        descriptor.Field(t => t.IsDeleted);

        descriptor.Field(t => t.Status);
        descriptor.Field(t => t.IsPendingPublish);
        descriptor.Field(t => t.StartPublish);
        descriptor.Field(t => t.StopPublish);

        descriptor.Field(t => t.Language);

        descriptor.Field(t => t.ExistingLanguages);
        descriptor.Field(t => t.MasterLanguage);
        
        descriptor.Field(t => t.Created);
        descriptor.Field(t => t.CreatedBy);
        descriptor.Field(t => t.Changed);
        descriptor.Field(t => t.ChangedBy);
        descriptor.Field(t => t.Deleted);
        descriptor.Field(t => t.DeletedBy);
        descriptor.Field(t => t.Saved);
    }
}

public abstract class PageDataSortTypeBase<T> : SortInputType<T> where T : PageData
{
    protected override void Configure(ISortInputTypeDescriptor<T> descriptor)
    {
        base.Configure(descriptor);

        descriptor.BindFields(BindingBehavior.Explicit);

        descriptor.Field(t => t.VisibleInMenu);

        descriptor.Field(t => t.Status);
        descriptor.Field(t => t.IsPendingPublish);
        descriptor.Field(t => t.StartPublish);
        descriptor.Field(t => t.StopPublish);

        descriptor.Field(t => t.Name);
        descriptor.Field(t => t.IsDeleted);

        descriptor.Field(t => t.Created);
        descriptor.Field(t => t.CreatedBy);
        descriptor.Field(t => t.Changed);
        descriptor.Field(t => t.ChangedBy);
        descriptor.Field(t => t.Deleted);
        descriptor.Field(t => t.DeletedBy);
        descriptor.Field(t => t.Saved);
    }
}

public class PageDataType : PageDataTypeBase<PageData>
{
}