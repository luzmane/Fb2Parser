using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Fb2Parser.Errors;
using Fb2Parser.Utils;
using NLog;

namespace Fb2Parser.Model
{
    /// <summary>
    /// A cut-down version of <section> used in annotations
    /// </summary>
    public class AnnotationElement : IFb2Element
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public const string AttributeId = "id";
        public const string AttributeLang = "lang";
        public const string TagParagraph = "p";
        public const string TagPoem = "poem";
        public const string TagCite = "cite";
        public const string TagSubtitle = "subtitle";
        public const string TagTable = "table";
        public const string TagEmptyLine = "empty-line";

        /// <summary>
        /// Tag self name
        /// </summary>
        public string ElementName { get; }
        public string? Id { get; set; }
        public string? Lang { set; get; }
        public List<IFb2Element> Content { get; private set; } = new List<IFb2Element>();

        public AnnotationElement(string elementName)
        {
            ElementName = elementName;
        }

        public void Parse(XNode node)
        {
            if (node is XElement annotationElement)
            {
                Id = annotationElement.Attributes(AttributeId).GetSingleValueOrNull(Logger);
                Lang = annotationElement.Attributes(XNamespace.Xml + AttributeLang).GetSingleValueOrNull(Logger);

                foreach (XElement? item in annotationElement.Elements())
                {
                    switch (item.Name.LocalName)
                    {
                        case TagParagraph:
                            ParagraphType paragraph = new ParagraphType(TagParagraph);
                            paragraph.Parse(item);
                            Content.Add(paragraph);
                            break;
                        case TagPoem:
                            PoemElement poem = new PoemElement();
                            poem.Parse(item);
                            Content.Add(poem);
                            break;
                        case TagCite:
                            CiteElement cite = new CiteElement();
                            cite.Parse(item);
                            Content.Add(cite);
                            break;
                        case TagSubtitle:
                            ParagraphType subtitle = new ParagraphType(TagSubtitle);
                            subtitle.Parse(item);
                            Content.Add(subtitle);
                            break;
                        case TagTable:
                            TableElement table = new TableElement();
                            table.Parse(item);
                            Content.Add(table);
                            break;
                        case TagEmptyLine:
                            EmptyLineElement emptyLine = new EmptyLineElement();
                            emptyLine.Parse(item);
                            Content.Add(emptyLine);
                            break;
                        default:
                            Logger.Error($"The tag '{ElementName}' should not contain '{item.Name.LocalName}' tag, ignoring");
                            FictionBook._parsingErrors.Value.Add(new NeedlessTagError(ElementName, item.Name.LocalName));
                            break;
                    }
                }
            }
        }
        public XNode ToXml()
        {
            XElement toReturn = new XElement(FictionBook.DefaultNamespace + ElementName);

            toReturn.AddOptionalAttribute(XNamespace.Xml + AttributeLang, Lang);
            toReturn.AddOptionalAttribute(XNamespace.Xml + AttributeId, Id);
            toReturn.AddOptionalListToTag(Content);

            return toReturn;
        }
    }
}
