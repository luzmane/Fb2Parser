using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Fb2Parser.Utils;
using NLog;

namespace Fb2Parser.Model
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class TextFieldType : IFb2Element
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public const string AttributeLang = "lang";

        /// <summary>
        /// Tag self name
        /// </summary>
        public string ElementName { get; }
        public string? Lang { set; get; }
        public string? Content { set; get; }

        public TextFieldType(string elementName)
        {
            ElementName = elementName;
        }

        public virtual void Parse(XNode node)
        {
            if (node is XElement textFieldTypeElement)
            {
                Lang = textFieldTypeElement.Attributes(XNamespace.Xml + AttributeLang).GetSingleValueOrNull(Logger);
                Content = textFieldTypeElement.Value;
            }
        }
        public virtual XNode ToXml()
        {
            XElement toReturn = new XElement(FictionBook.DefaultNamespace + ElementName);

            toReturn.AddOptionalAttribute(XNamespace.Xml + AttributeLang, Lang);
            toReturn.AddTagContent(Content);

            return toReturn;
        }

        private string GetDebuggerDisplay()
        {
            return $"<{ElementName}>{Content}</{ElementName}>";
        }
    }
}
