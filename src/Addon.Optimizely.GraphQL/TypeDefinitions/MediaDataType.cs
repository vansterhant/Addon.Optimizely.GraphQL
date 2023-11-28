using EPiServer.Core;
using HotChocolate.Data.Filters;
using HotChocolate.Data.Sorting;

namespace Addon.Optimizely.GraphQL;

public abstract class MediaDataTypeBase<T> : ObjectType<T> where T : MediaData
{
    protected override void Configure(IObjectTypeDescriptor<T> descriptor)
    {
        base.Configure(descriptor);

        descriptor.BindFields(BindingBehavior.Explicit);

        descriptor.Field(t => t.ContentLink).Type<IdType>()
            .Resolve(context => context.Parent<MediaData>().ContentLink.ToString());
        descriptor.Field(t => t.ParentLink).Type<StringType>()
            .Resolve(context => context.Parent<MediaData>().ParentLink.ToString());
        descriptor.Field(t => t.ContentGuid).Type<UuidType>();
        descriptor.Field(t => t.IsDeleted).Type<BooleanType>();
        descriptor.Field(t => t.Language).Type<StringType>();
    }
}

public abstract class MediaDataFilterTypeBase<T> : FilterInputType<T> where T : MediaData
{
    protected override void Configure(IFilterInputTypeDescriptor<T> descriptor)
    {
        base.Configure(descriptor);

        descriptor.BindFields(BindingBehavior.Explicit);

        descriptor.Field(t => t.ContentLink);
        descriptor.Field(t => t.ParentLink);
        descriptor.Field(t => t.ContentGuid);
        descriptor.Field(t => t.IsDeleted);
        descriptor.Field(t => t.Language);
    }
}

public abstract class MediaDataSortTypeBase<T> : SortInputType<T> where T : MediaData
{
    protected override void Configure(ISortInputTypeDescriptor<T> descriptor)
    {
        base.Configure(descriptor);

        descriptor.BindFields(BindingBehavior.Explicit);

        descriptor.Field(t => t.Language);
    }
}