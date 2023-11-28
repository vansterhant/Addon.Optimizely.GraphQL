using EPiServer.Web;

namespace Addon.Optimizely.GraphQL;

public class QueryBase
{
    [UseFiltering(typeof(SiteDefinitionFilterType))]
    public IEnumerable<SiteDefinition> GetSiteDefinition(
        [Service(ServiceKind.Synchronized)] ISiteDefinitionRepository siteDefinitionRepository)
    {
        return siteDefinitionRepository.List();
    }
}