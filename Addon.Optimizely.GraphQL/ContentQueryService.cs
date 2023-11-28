using System.Globalization;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Filters;
using EPiServer.Framework.Cache;
using EPiServer.ServiceLocation;

namespace Addon.Optimizely.GraphQL;

public sealed class ContentQueryService : IDisposable
{
    private readonly ISynchronizedObjectInstanceCache _cache;
    private readonly IContentEvents _contentEvents;
    private readonly IContentLoader _contentLoader;
    private readonly IContentModelUsage _contentModelUsage;
    private readonly IContentTypeRepository _contentTypeRepository;

    public ContentQueryService(
        IContentEvents contentEvents,
        IContentLoader contentLoader,
        IContentTypeRepository contentTypeRepository,
        IContentModelUsage contentModelUsage,
        ISynchronizedObjectInstanceCache cache)
    {
        _contentEvents = contentEvents;
        _contentLoader = contentLoader;
        _contentTypeRepository = contentTypeRepository;
        _contentModelUsage = contentModelUsage;
        _cache = cache;
        
        _contentEvents.PublishedContent += RemoveTypeFromCache;
        _contentEvents.DeletedContent += RemoveTypeFromCache;
    }

    private void RemoveTypeFromCache(object? sender, ContentEventArgs eventArgs)
    {
        _cache.Remove(eventArgs.Content.GetType().FullName);
    }
    
    public IEnumerable<T> GetContentOfType<T>(IList<int>? ids, string? language)
    {
        return GetContentOfType<T>(ids?.Select(x => new ContentReference(x)).ToList(), language);
    }

    public IEnumerable<T> GetContentOfType<T>(IList<ContentReference>? ids, string? language)
    {
        if (ids is null || ids.Count == 0)
        {
            var typeName = typeof(T).Name;
            ids = _cache.ReadThrough(
                typeName,
                () =>
                    _contentModelUsage
                        .ListContentOfContentType(_contentTypeRepository.Load(typeName))
                        .Select(x => x.ContentLink.ToReferenceWithoutVersion())
                        .Distinct()
                        .ToList(),
                ReadStrategy.Wait
            );
        }

        var languageLoaderOption = string.IsNullOrEmpty(language)
            ? LanguageLoaderOption.MasterLanguage()
            : LanguageLoaderOption.Specific(CultureInfo.GetCultureInfo(language));

        var items = _contentLoader.GetItems(
            ids,
            new LoaderOptions { languageLoaderOption });
        
        var filteredItems = FilterForVisitor(items);
        
        return filteredItems
            .OfType<T>();
    }

    public IEnumerable<T> GetChildrenOfType<T>(IContent content)
    {
        var loaderOptions = content is ILocalizable localizable
            ? new LoaderOptions { LanguageLoaderOption.Specific(localizable.Language) }
            : new LoaderOptions { LanguageLoaderOption.MasterLanguage() };
        
        return FilterForVisitor(_contentLoader.GetChildren<IContent>(content.ContentLink, loaderOptions))
            .OfType<T>();
    }

    public IContent? GetParent(IContent child)
    {
        var loaderOptions = child is ILocalizable localizable
            ? new LoaderOptions { LanguageLoaderOption.FallbackWithMaster(localizable.Language) }
            : new LoaderOptions { LanguageLoaderOption.MasterLanguage() };
        
        var parent = _contentLoader.Get<IContent>(child.ParentLink, loaderOptions);

        return GetFilters().ShouldFilter(parent)
            ? null
            : parent;
    }
    
    public IEnumerable<IContent> GetAncestors(IContent child)
    {
        var ancestors = _contentLoader.GetAncestors(child.ContentLink);
        return FilterForVisitor(ancestors);
    }
    
    private IEnumerable<IContent> FilterForVisitor(IEnumerable<IContent> contentItems)
    {
        var list = contentItems.ToList();
        
        GetFilters().Filter(list);
        
        return list;
    }

    private CompositeFilter GetFilters()
    {
        return new CompositeFilter(
            new FilterPublished(ServiceLocator.Current.GetInstance<IPublishedStateAssessor>()),
            new FilterAccess());
    }

    public void Dispose()
    {
        _contentEvents.PublishedContent -= RemoveTypeFromCache;
        _contentEvents.DeletedContent -= RemoveTypeFromCache;
    }
}