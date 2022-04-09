using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Fb2Parser.Errors;
using Fb2Parser.Utils;
using NLog;

namespace Fb2Parser.Model
{
    /// <summary>
    /// An epigraph
    /// </summary>
    public class EpigraphElement : IFb2Element
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public const string TagParagraph = "p";
        public const string TagPoem = "poem";
        public const string TagCite = "cite";
        public const string TagEmptyLine = "empty-line";
        public const string TagTextAuthor = "text-author";
        public const string AttributeId = "id";

        public List<IFb2Element> Content { get; private set; } = new List<IFb2Element>();
        public List<ParagraphType> TextAuthors { get; private set; } = new List<ParagraphType>();
        public string? Id { get; set; }

        public void Parse(XNode node)
        {
            if (node is XElement epigraphElement)
            {
                Id = epigraphElement.Attributes(AttributeId).GetSingleValueOrNull(Logger);

                foreach (XElement? item in epigraphElement.Elements(FictionBook.DefaultNamespace + TagTextAuthor))
                {
                    ParagraphType paragraph = new ParagraphType(TagTextAuthor);
                    paragraph.Parse(item);
                    TextAuthors.Add(paragraph);
                }

                foreach (XElement? item in epigraphElement.Elements())
                {
                    switch (item.Name.LocalName)
                    {
                        case TagEmptyLine:
                            EmptyLineElement emptyLine = new EmptyLineElement();
                            emptyLine.Parse(item);
                            Content.Add(emptyLine);
                            break;
                        case TagCite:
                            CiteElement cite = new CiteElement();
                            cite.Parse(item);
                            Content.Add(cite);
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
                        case TagTextAuthor:
                            // already processed
                            break;
                        default:
                            Logger.Error($"The tag '{BodyElement.TagEpigraph}' should not contain '{item.Name.LocalName}' tag");
                            FictionBook._parsingErrors.Value.Add(new NeedlessTagError(BodyElement.TagEpigraph, item.Name.LocalName));
                            break;
                    }
                }
            }
        }
        public XNode ToXml()
        {
            XElement toReturn = new XElement(FictionBook.DefaultNamespace + BodyElement.TagEpigraph);

            toReturn.AddOptionalAttribute(XNamespace.Xml + AttributeId, Id);
            toReturn.AddOptionalListToTag(Content);
            toReturn.AddOptionalListToTag(TextAuthors);

            return toReturn;
        }
    }
}
