using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Fb2Parser.Utils;
using NLog;

namespace Fb2Parser.Model
{
    /// <summary>
    /// Generic hyperlinks. Cannot be nested.
    /// Footnotes should be implemented by links referring to additional bodies in the same document
    /// </summary>
    public class LinkElement : StyleLinkType
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public const string AttributeType = "type";
        public const string AttributeHref = "href";

        public string? Type { set; get; }
        public string? XlinkType { set; get; }
        public string? Href { set; get; }

        public LinkElement(string tagName) : base(tagName)
        {
        }

        public override void Parse(XNode node)
        {
            if (FictionBook.XlinkNamespace == XNamespace.None)
            {
                throw new XmlException("The book doesn't have 'xlink' namespace definition like: \"xmlns:xlink=\"http://www.w3.org/1999/xlink\"");
            }

            if (node is XElement linkElement)
            {
                base.Parse(linkElement);

                Type = linkElement.Attributes(AttributeType).GetSingleValueOrNull(Logger);
                XlinkType = linkElement.Attributes(FictionBook.XlinkNamespace + AttributeType).GetSingleValueOrNull(Logger);
                Href = linkElement.Attributes(FictionBook.XlinkNamespace + AttributeHref).GetSingleValueOrNull(Logger);
            }
        }
        public override XNode ToXml()
        {
            XElement toReturn = (XElement)base.ToXml();

            toReturn.AddOptionalAttribute(FictionBook.XlinkNamespace + AttributeType, XlinkType);
            toReturn.AddRequiredAttribute(FictionBook.XlinkNamespace + AttributeHref, Href, Logger, StyleElement.TagLink);
            toReturn.AddOptionalAttribute(AttributeType, Type);

            return toReturn;
        }
    }
}
