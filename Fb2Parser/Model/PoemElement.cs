using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Fb2Parser.Errors;
using Fb2Parser.Utils;
using NLog;

namespace Fb2Parser.Model
{
    /// <summary>
    /// A poem
    /// </summary>
    public class PoemElement : IFb2Element
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public const string AttributeId = "id";
        public const string AttributeLang = "lang";
        public const string TagTitle = "title";
        public const string TagEpigraph = "epigraph";
        public const string TagTextAuthor = "text-author";
        public const string TagDate = "date";
        public const string TagSubtitle = "subtitle";
        public const string TagStanza = "stanza";

        public string? Id { get; set; }
        public string? Lang { set; get; }
        /// <summary>
        /// Date this poem was written
        /// </summary>
        public DateType? Date { get; set; }
        public List<ParagraphType> TextAuthors { get; private set; } = new List<ParagraphType>();
        /// <summary>
        /// Poem title
        /// </summary>
        public TitleElement? Title { get; set; }
        /// <summary>
        /// Poem epigraph(s), if any
        /// </summary>
        public List<EpigraphElement> Epigraphs { get; private set; } = new List<EpigraphElement>();
        public List<IFb2Element> Content { get; private set; } = new List<IFb2Element>();

        public void Parse(XNode node)
        {
            if (node is XElement poemElement)
            {
                Id = poemElement.Attributes(AttributeId).GetSingleValueOrNull(Logger);
                Lang = poemElement.Attributes(XNamespace.Xml + AttributeLang).GetSingleValueOrNull(Logger);

                XElement? date = poemElement.Elements(FictionBook.DefaultNamespace + TagDate).GetSingleValueOrNull(Logger);
                if (date != null)
                {
                    Date = new DateType();
                    Date.Parse(date);
                }

                foreach (XElement? item in poemElement.Elements(FictionBook.DefaultNamespace + TagTextAuthor))
                {
                    ParagraphType author = new ParagraphType(TagTextAuthor);
                    author.Parse(item);
                    TextAuthors.Add(author);
                }

                XElement? title = poemElement.Elements(FictionBook.DefaultNamespace + TagTitle).GetSingleValueOrNull(Logger);
                if (title != null)
                {
                    Title = new TitleElement();
                    Title.Parse(title);
                }

                foreach (XElement? item in poemElement.Elements(FictionBook.DefaultNamespace + TagEpigraph))
                {
                    EpigraphElement epigraph = new EpigraphElement();
                    epigraph.Parse(item);
                    Epigraphs.Add(epigraph);
                }

                foreach (XElement? item in poemElement.Elements())
                {
                    switch (item.Name.LocalName)
                    {
                        case TagSubtitle:
                            ParagraphType subtitle = new ParagraphType(TagSubtitle);
                            subtitle.Parse(item);
                            Content.Add(subtitle);
                            break;
                        case TagStanza:
                            StanzaElement stanza = new StanzaElement();
                            stanza.Parse(item);
                            Content.Add(stanza);
                            break;
                        case TagTitle:
                        case TagEpigraph:
                        case TagTextAuthor:
                        case TagDate:
                            // already processed
                            break;
                        default:
                            Logger.Error($"The tag '{SectionElement.TagPoem}' should not contain '{item.Name.LocalName}' tag");
                            FictionBook._parsingErrors.Value.Add(new NeedlessTagError(SectionElement.TagPoem, item.Name.LocalName));
                            break;
                    }
                }
            }
        }
        public XNode ToXml()
        {
            XElement toReturn = new XElement(FictionBook.DefaultNamespace + SectionElement.TagPoem);

            toReturn.AddOptionalAttribute(XNamespace.Xml + AttributeLang, Lang);
            toReturn.AddOptionalAttribute(XNamespace.Xml + AttributeId, Id);
            toReturn.AddOptionalTag(Title);
            toReturn.AddOptionalListToTag(Epigraphs);
            toReturn.AddRequiredListToTag(Content, Logger, TagSubtitle, typeof(ParagraphType));
            toReturn.AddOptionalListToTag(TextAuthors);
            toReturn.AddOptionalTag(Date);

            return toReturn;
        }
    }
}
