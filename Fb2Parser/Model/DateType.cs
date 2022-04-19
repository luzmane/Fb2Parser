using System.Linq;
using System.Xml.Linq;
using Fb2Parser.Utils;
using NLog;

namespace Fb2Parser.Model
{
    /// <summary>
    /// A human readable date, maybe not exact, with an optional computer readable variant
    /// </summary>
    public class DateType : IFb2Element
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public const string AttributeValue = "value";
        public const string AttributeLang = "lang";

        public string? Value { set; get; }
        public string? Lang { set; get; }
        public string? Content { set; get; }

        public void Parse(XNode node)
        {
            if (node is XElement dateTypeElement)
            {
                Value = dateTypeElement.Attributes(AttributeValue).GetSingleValueOrNull(Logger);
                Lang = dateTypeElement.Attributes(XNamespace.Xml + AttributeLang).GetSingleValueOrNull(Logger);
                Content = dateTypeElement.Value;
            }
        }
        public XNode ToXml()
        {
            XElement toReturn = new XElement(FictionBook.DefaultNamespace + TitleInfoElement.TagDate);

            toReturn.AddOptionalAttribute(XNamespace.Xml + AttributeLang, Lang);
            toReturn.AddOptionalAttribute(AttributeValue, Value);
            toReturn.AddTagContent(Content);

            return toReturn;
        }
    }
}
