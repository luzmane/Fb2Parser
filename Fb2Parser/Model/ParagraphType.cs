using System.Linq;
using System.Xml.Linq;
using Fb2Parser.Utils;
using NLog;

namespace Fb2Parser.Model
{
    /// <summary>
    /// A basic paragraph, may include simple formatting inside
    /// </summary>
    public class ParagraphType : StyleType
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public const string AttributeId = "id";
        public const string AttributeStyle = "style";

        public string? Id { get; set; }
        public string? Style { get; set; }

        public ParagraphType(string tagName) : base(tagName)
        {
        }

        public override void Parse(XNode node)
        {
            if (node is XElement paragraphElement)
            {
                base.Parse(paragraphElement);

                Id = paragraphElement.Attributes(AttributeId).GetSingleValueOrNull(Logger);
                Style = paragraphElement.Attributes(AttributeStyle).GetSingleValueOrNull(Logger);
            }
        }
        public override XNode ToXml()
        {
            XElement toReturn = (XElement)base.ToXml();

            toReturn.AddOptionalAttribute(AttributeId, Id);
            toReturn.AddOptionalAttribute(AttributeStyle, Style);

            return toReturn;
        }
    }
}
