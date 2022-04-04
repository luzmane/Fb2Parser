using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Fb2Parser.Errors;
using Fb2Parser.Utils;
using NLog;

namespace Fb2Parser.Model
{
    /// <summary>
    /// A title, used in sections, poems and body elements
    /// </summary>
    public class TitleElement : IFb2Element
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public const string AttributeLang = "lang";
        public const string TagParagraph = "p";
        public const string TagEmptyLine = "empty-line";

        public List<IFb2Element> Content { get; private set; } = new List<IFb2Element>();
        public string? Lang { set; get; }

        public void Parse(XNode node)
        {
            if (node is XElement titleElement)
            {
                Lang = titleElement.Attributes(XNamespace.Xml + AttributeLang).GetSingleValueOrNull(Logger);

                foreach (XElement? item in titleElement.Elements())
                {
                    switch (item.Name.LocalName)
                    {
                        case TagParagraph:
                            ParagraphType paragraph = new ParagraphType(TagParagraph);
                            paragraph.Parse(item);
                            Content.Add(paragraph);
                            break;
                        case TagEmptyLine:
                            EmptyLineElement emptyLine = new EmptyLineElement();
                            emptyLine.Parse(item);
                            Content.Add(emptyLine);
                            break;
                        default:
                            FictionBook._parsingErrors.Value.Add(new NeedlessTagError(BodyElement.TagTitle, item.Name.LocalName));
                            Logger.Warn($"The tag '{BodyElement.TagTitle}' should not contain '{item.Name.LocalName}' tag");
                            break;
                    }
                }

            }
        }
        public XNode ToXml()
        {
            XElement toReturn = new XElement(FictionBook.DefaultNamespace + BodyElement.TagTitle);

            toReturn.AddOptionalAttribute(XNamespace.Xml + AttributeLang, Lang);
            toReturn.AddOptionalListToTag(Content);

            return toReturn;
        }
    }
}
