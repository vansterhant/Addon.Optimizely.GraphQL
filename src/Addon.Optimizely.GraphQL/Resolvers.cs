using EPiServer.Core;

namespace Addon.Optimizely.GraphQL;

public class Resolvers
{
    public IEnumerable<T> GetChildren<T>(
        [Parent] IContent content,
        [Service(ServiceKind.Synchronized)] ContentQueryService contentQueryService)
    {
        return contentQueryService.GetChildrenOfType<T>(content);
    }

    public PageData? GetParent(
        [Parent] IContent content,
        [Service(ServiceKind.Synchronized)] ContentQueryService contentQueryService)
    {
        return contentQueryService.GetParent(content) as PageData;
    }
    
    public IEnumerable<PageData> GetAncestors(
        [Parent] IContent content,
        [Service(ServiceKind.Synchronized)] ContentQueryService contentQueryService)
    {
        return contentQueryService.GetAncestors(content).OfType<PageData>();
    }
    
    public IEnumerable<object> GetItemsForContentArea(
        [Parent] ContentArea contentArea,
        [Service] IContentAreaItemFactory contentAreaItemFactory)
    {
        return contentArea.FilteredItems
            .Select(item => contentAreaItemFactory.CreateFacade(item.LoadContent()));
    }
}