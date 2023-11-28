using EPiServer.DataAnnotations;

namespace Addon.Optimizely.GraphQL.Generator.Sample.Models;

/// <summary>
///     Attribute used for site content types to set default attribute values
/// </summary>
public class SiteContentType : ContentTypeAttribute
{
    public SiteContentType()
    {
        GroupName = Globals.GroupNames.Default;
    }
}