using System.Xml.Serialization;

namespace TorBoxSDK.Models.General;

/// <summary>Represents the root element of an RSS 2.0 feed.</summary>
[XmlRoot("rss")]
public sealed class RssFeed
{
    /// <summary>Gets or sets the RSS version.</summary>
    [XmlAttribute("version")]
    public string Version { get; set; } = string.Empty;

    /// <summary>Gets or sets the channel metadata and items.</summary>
    [XmlElement("channel")]
    public RssChannel? Channel { get; set; }
}
