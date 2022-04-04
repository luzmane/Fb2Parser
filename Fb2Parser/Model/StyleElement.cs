using System.Linq;
using System.Xml.Linq;
using Fb2Parser.Utils;
using NLog;

namespace Fb2Parser.Model
{
    /// <summary>
    /// Markup
    /// In xsd is 'namedStyleType'
    /// </summary>
    public class StyleElement : StyleType
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public const string AttributeName = "name";

        public string? Name { set; get; }

        public StyleElement(string elementName) : base(elementName)
        {
        }

        public override void Parse(XNode node)
        {
            if (node is XElement styleElement)
            {
                base.Parse(styleElement);

                Name = styleElement.Attributes(AttributeName).GetSingleValueOrNull(Logger);
            }
        }
        public override XNode ToXml()
        {
            XElement toReturn = (XElement)base.ToXml();

            toReturn.AddRequiredAttribute(AttributeName, Name, Logger, ElementName);

            return toReturn;
        }
    }
}
