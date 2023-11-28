using System.ComponentModel.DataAnnotations;
using EPiServer.DataAbstraction;

namespace Addon.Optimizely.GraphQL.Generator.Sample.Models.Pages;

/// <summary>
///     Used primarily for publishing news articles on the website
/// </summary>
[SiteContentType(
    GroupName = Globals.GroupNames.News,
    GUID = "AEECADF2-3E89-4117-ADEB-F8D43565D2F4")]
[SiteImageUrl(Globals.StaticGraphicsFolderPath + "page-type-thumbnail-article.png")]
public class ArticlePage : StandardPage
{
    [Display(
        Name = "My hard limit text",
        GroupName = SystemTabNames.Content,
        Order = 310)]
    [StringLength(20)]
    public virtual string MyHardLimitText { get; set; }

    [Display(
        Name = "My hard limit text area",
        GroupName = SystemTabNames.Content,
        Order = 311)]
    [StringLength(20)]
    public virtual string MyHardLimitTextArea { get; set; }

    [Display(
        Name = "My soft limit text",
        GroupName = SystemTabNames.Content,
        Order = 312)]
    public virtual string MySoftLimitText { get; set; }

    [Display(
        Name = "My soft limit text area",
        GroupName = SystemTabNames.Content,
        Order = 313)]
    public virtual string MySoftLimitTextArea { get; set; }


    public override void SetDefaultValues(ContentType contentType)
    {
        base.SetDefaultValues(contentType);

        VisibleInMenu = false;
    }
}