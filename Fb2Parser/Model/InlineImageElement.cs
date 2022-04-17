using System.Xml;
using System.Xml.Linq;
using Fb2Parser.Utils;
using NLog;

namespace Fb2Parser.Model
{
    public class InlineImageElement : IFb2Element
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public const string AttributeType = "type";
        public const string AttributeHref = "href";
        public const string AttributeAlt = "alt";

        public string? Type { set; get; }
        public string? Href { set; get; }
        public string? Alt { set; get; }

        public virtual void Parse(XNode node)
        {
            if (FictionBook.XlinkNamespace == XNamespace.None)
            {
                throw new XmlException("The book doesn't have 'xlink' namespace definition like: \"xmlns:xlink=\"http://www.w3.org/1999/xlink\"");
            }

            if (node is XElement imageElement)
            {
                Type = imageElement.Attributes(FictionBook.XlinkNamespace + AttributeType).GetSingleValueOrNull(Logger);
                Href = imageElement.Attributes(FictionBook.XlinkNamespace + AttributeHref).GetSingleValueOrNull(Logger);
                if (Href != null)
                {
                    FictionBook._usedImages.Value.Add(Href[1..]);
                }
                Alt = imageElement.Attributes(AttributeAlt).GetSingleValueOrNull(Logger);
            }
        }
        public virtual XNode ToXml()
        {
            XElement toReturn = new XElement(FictionBook.DefaultNamespace + StyleLinkType.TagImage);

            toReturn.AddOptionalAttribute(FictionBook.XlinkNamespace + AttributeType, Type);
            toReturn.AddOptionalAttribute(FictionBook.XlinkNamespace + AttributeHref, Href);
            toReturn.AddOptionalAttribute(AttributeAlt, Alt);

            return toReturn;
        }
    }
}
