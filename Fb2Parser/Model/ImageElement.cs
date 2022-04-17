using System.Xml.Linq;
using Fb2Parser.Utils;
using NLog;

namespace Fb2Parser.Model
{
    /// <summary>
    /// An empty element with an image name as an attribute
    /// </summary>
    public class ImageElement : InlineImageElement
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public const string AttributeTitle = "title";
        public const string AttributeId = "id";

        public string? Title { set; get; }
        public string? Id { set; get; }

        public override void Parse(XNode node)
        {
            base.Parse(node);

            if (node is XElement imageElement)
            {
                Title = imageElement.Attributes(AttributeTitle).GetSingleValueOrNull(Logger);
                Id = imageElement.Attributes(AttributeId).GetSingleValueOrNull(Logger);
            }
        }
        public override XNode ToXml()
        {
            XElement toReturn = (XElement)base.ToXml();

            toReturn.AddOptionalAttribute(AttributeTitle, Title);
            toReturn.AddOptionalAttribute(AttributeId, Id);

            return toReturn;
        }
    }
}
