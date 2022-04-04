using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Fb2Parser.Errors;
using Fb2Parser.Utils;
using NLog;

namespace Fb2Parser.Model
{
    /// <summary>
    /// A citation with an optional citation author at the end
    /// </summary>
    public class CiteElement : IFb2Element
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public const string TagParagraph = "p";
        public const string TagPoem = "poem";
        public const string TagSubtitle = "subtitle";
        public const string TagTable = "table";
        public const string TagEmptyLine = "empty-line";
        public const string TagTextAuthor = "text-author";
        public const string AttributeId = "id";
        public const string AttributeLang = "lang";

        public List<IFb2Element> Content { get; private set; } = new List<IFb2Element>();
        public List<ParagraphType> TextAuthors { get; private set; } = new List<ParagraphType>();
        public string? Id { get; set; }
        public string? Lang { set; get; }

        public void Parse(XNode node)
        {
            if (node is XElement citeElement)
            {
                Id = citeElement.Attributes(AttributeId).GetSingleValueOrNull(Logger);
                Lang = citeElement.Attributes(XNamespace.Xml + AttributeLang).GetSingleValueOrNull(Logger);

                foreach (XElement? item in citeElement.Elements(FictionBook.DefaultNamespace + TagTextAuthor))
                {
                    ParagraphType paragraph = new ParagraphType(TagTextAuthor);
                    paragraph.Parse(item);
                    TextAuthors.Add(paragraph);
                }

                foreach (XElement? item in citeElement.Elements())
                {
                    switch (item.Name.LocalName)
                    {
                        case TagEmptyLine:
                            EmptyLineElement emptyLine = new EmptyLineElement();
                            emptyLine.Parse(item);
                            Content.Add(emptyLine);
                            break;
                        case TagPoem:
                            PoemElement poem = new PoemElement();
                            poem.Parse(item);
                            Content.Add(poem);
                            break;
                        case TagParagraph:
                            ParagraphType paragraph = new ParagraphType(TagParagraph);
                            paragraph.Parse(item);
                            Content.Add(paragraph);
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
                        case TagTextAuthor:
                            // already processed
                            break;
                        default:
                            FictionBook._parsingErrors.Value.Add(new NeedlessTagError(AnnotationElement.TagCite, item.Name.LocalName));
                            Logger.Warn($"The tag '{AnnotationElement.TagCite}' should not contain '{item.Name.LocalName}' tag");
                            break;
                    }
                }
            }
        }
        public XNode ToXml()
        {
            XElement toReturn = new XElement(FictionBook.DefaultNamespace + AnnotationElement.TagCite);

            toReturn.AddOptionalAttribute(XNamespace.Xml + AttributeLang, Lang);
            toReturn.AddOptionalAttribute(XNamespace.Xml + AttributeId, Id);
            toReturn.AddOptionalListToTag(Content);
            toReturn.AddOptionalListToTag(TextAuthors);

            return toReturn;
        }
    }
}
