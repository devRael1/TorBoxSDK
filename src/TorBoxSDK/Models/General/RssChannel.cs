using System.Xml.Serialization;

namespace TorBoxSDK.Models.General;

/// <summary>Represents the channel element of an RSS 2.0 feed.</summary>
public sealed class RssChannel
{
    /// <summary>Gets or sets the channel title.</summary>
    [XmlElement("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the channel link.</summary>
    [XmlElement("link")]
    public string Link { get; set; } = string.Empty;

    /// <summary>Gets or sets the channel description.</summary>
    [XmlElement("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>Gets or sets the channel language.</summary>
    [XmlElement("language")]
    public string Language { get; set; } = string.Empty;

    /// <summary>Gets or sets the last build date as a string.</summary>
    [XmlElement("lastBuildDate")]
    public string LastBuildDate { get; set; } = string.Empty;

    /// <summary>Gets or sets the list of RSS items.</summary>
    [XmlElement("item")]
    public List<RssItem> Items { get; set; } = [];
}
