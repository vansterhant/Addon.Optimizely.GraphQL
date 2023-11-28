using EPiServer.Core;

namespace Addon.Optimizely.GraphQL;

public interface IContentAreaItemFactory
{
    public object CreateFacade(IContentData content);
}