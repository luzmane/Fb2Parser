using System.Linq;
using System.Xml.Linq;
using Fb2Parser.Utils;
using NLog;

namespace Fb2Parser.Model
{
    /// <summary>
    /// This element contains an arbitrary stylesheet that is intepreted 
    /// by a some processing programs, e.g. text/css stylesheets can be 
    /// used by XSLT stylesheets to generate better looking html
    /// </summary>
    public class StylesheetElement : IFb2Element
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public const string AttributeType = "type";

        public string? Type { set; get; }
        public string? Content { set; get; }

        public void Parse(XNode node)
        {
            if (node is XElement element)
            {
                Type = element.Attributes(AttributeType).GetSingleValueOrNull(Logger);
                Content = element.Value;
            }
        }
        public XNode ToXml()
        {
            XElement toReturn = new XElement(FictionBook.DefaultNamespace + FictionBook.TagStylesheet);

            toReturn.AddRequiredAttribute(AttributeType, Type, Logger, FictionBook.TagStylesheet);
            toReturn.AddTagContent(Content);

            return toReturn;
        }
    }
}
