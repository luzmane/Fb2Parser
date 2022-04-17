using System.Linq;
using System.Xml.Linq;
using Fb2Parser.Utils;
using NLog;

namespace Fb2Parser.Model
{
    /// <summary>
    /// Any binary data that is required for the presentation of this book in base64 format.
    /// Currently only images are used
    /// </summary>
    public class BinaryElement : IFb2Element
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public const string AttributeContentType = "content-type";
        public const string AttributeId = "id";

        public string? ContentType { set; get; }
        public string? Id { set; get; }
        public string? Content { set; get; }

        public void Parse(XNode node)
        {
            if (node is XElement element)
            {
                ContentType = element.Attributes(AttributeContentType).GetSingleValueOrNull(Logger);
                Id = element.Attributes(AttributeId).GetSingleValueOrNull(Logger);
                Content = element.Value;
            }
        }
        public XNode ToXml()
        {
            XElement toReturn = new XElement(FictionBook.DefaultNamespace + FictionBook.TagBinary);

            toReturn.AddRequiredAttribute(AttributeContentType, ContentType, Logger, FictionBook.TagBinary);
            toReturn.AddRequiredAttribute(AttributeId, Id, Logger, FictionBook.TagBinary);
            toReturn.AddRequiredTagContent(Content, Logger);

            return toReturn;
        }
    }
}
