using EPiServer.Core;

namespace Addon.Optimizely.GraphQL.Generator.Sample.Models.Pages;

public interface IHasRelatedContent
{
    ContentArea RelatedContentArea { get; }
}