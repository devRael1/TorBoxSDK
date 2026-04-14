using System.Xml.Serialization;

namespace TorBoxSDK.Models.General;

/// <summary>Represents a single item in an RSS 2.0 feed.</summary>
public sealed class RssItem
{
    /// <summary>Gets or sets the item title.</summary>
    [XmlElement("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the item link.</summary>
    [XmlElement("link")]
    public string Link { get; set; } = string.Empty;

    /// <summary>Gets or sets the item description.</summary>
    [XmlElement("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>Gets or sets the publication date as a string.</summary>
    [XmlElement("pubDate")]
    public string PubDate { get; set; } = string.Empty;

    /// <summary>Gets or sets the content:encoded element value.</summary>
    [XmlElement("encoded", Namespace = "http://purl.org/rss/1.0/modules/content/")]
    public string? ContentEncoded { get; set; }
}
