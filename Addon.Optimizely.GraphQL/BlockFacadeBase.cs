using System.Globalization;
using EPiServer.Core;

namespace Addon.Optimizely.GraphQL;

public class BlockFacadeBase<T> : BlockFacadeBase where T : BlockData
{
    public BlockFacadeBase(T original) : base(original)
    {
        Original = original;
    }

    public new T Original { get; }
}

public class BlockFacadeBase
{
    public BlockFacadeBase(object original)
    {
        Original = original;
    }

    public object Original { get; }

    public CategoryList Category => (Original as ICategorizable).Category;
    public string Name => (Original as IContent).Name;
    public ContentReference ContentLink => (Original as IContent).ContentLink;
    public ContentReference ParentLink => (Original as IContent).ParentLink;
    public Guid ContentGuid => (Original as IContent).ContentGuid;
    public bool IsDeleted => (Original as IContent).IsDeleted;
    public CultureInfo Language => (Original as ILocalizable).Language;
    public IEnumerable<CultureInfo> ExistingLanguages => (Original as ILocalizable).ExistingLanguages;
    public CultureInfo MasterLanguage => (Original as ILocalizable).MasterLanguage;
    public VersionStatus Status => (Original as IVersionable).Status;
    public bool IsPendingPublish => (Original as IVersionable).IsPendingPublish;
    public DateTime? StartPublish => (Original as IVersionable).StartPublish;
    public DateTime? StopPublish => (Original as IVersionable).StopPublish;
    public DateTime Created => (Original as IChangeTrackable).Created;
    public string CreatedBy => (Original as IChangeTrackable).CreatedBy;
    public DateTime Changed => (Original as IChangeTrackable).Changed;
    public string ChangedBy => (Original as IChangeTrackable).ChangedBy;
    public DateTime Saved => (Original as IChangeTrackable).Saved;
    public string DeletedBy => (Original as IChangeTrackable).DeletedBy;
    public DateTime? Deleted => (Original as IChangeTrackable).Deleted;
}