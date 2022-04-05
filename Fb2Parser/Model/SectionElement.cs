using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Fb2Parser.Errors;
using Fb2Parser.Utils;
using NLog;

namespace Fb2Parser.Model
{
    /// <summary>
    /// A basic block of a book, can contain more child sections or textual content
    /// </summary>
    public class SectionElement : IFb2Element
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public const string AttributeId = "id";
        public const string AttributeLang = "lang";
        public const string TagTitle = "title";
        public const string TagEpigraph = "epigraph";
        public const string TagImage = "image";
        public const string TagAnnotation = "annotation";
        public const string TagSection = "section";
        public const string TagParagraph = "p";
        public const string TagPoem = "poem";
        public const string TagSubtitle = "subtitle";
        public const string TagEmptyLine = "empty-line";
        public const string TagTable = "table";
        public const string TagCite = "cite";

        public string? Id { get; set; }
        public string? Lang { set; get; }
        /// <summary>
        /// Section's title
        /// </summary>
        public TitleElement? Title { get; set; }
        /// <summary>
        /// Epigraph(s) for this section
        /// </summary>
        public List<EpigraphElement> Epigraphs { get; private set; } = new List<EpigraphElement>();
        /// <summary>
        /// Image to be displayed at the top of this section
        /// </summary>
        public ImageElement? Image { get; set; }
        /// <summary>
        /// Annotation for this section, if any
        /// </summary>
        public AnnotationElement? Annotation { get; set; }
        /// <summary>
        /// Child sections
        /// </summary>
        public List<SectionElement> Sections { get; private set; } = new List<SectionElement>();
        public List<IFb2Element> Content { get; private set; } = new List<IFb2Element>();

        public void Parse(XNode node)
        {
            if (node is XElement sectionElement)
            {
                Id = sectionElement.Attributes(AttributeId).GetSingleValueOrNull(Logger);
                Lang = sectionElement.Attributes(XNamespace.Xml + AttributeLang).GetSingleValueOrNull(Logger);

                int sectionStartItems = 0;

                XElement? title = sectionElement.Elements(FictionBook.DefaultNamespace + TagTitle).GetSingleValueOrNull(Logger);
                if (title != null)
                {
                    Title = new TitleElement();
                    Title.Parse(title);
                }

                foreach (XElement? item in sectionElement.Elements(FictionBook.DefaultNamespace + TagEpigraph))
                {
                    EpigraphElement epigraph = new EpigraphElement();
                    epigraph.Parse(item);
                    Epigraphs.Add(epigraph);
                }

                XElement? image = sectionElement.Elements(FictionBook.DefaultNamespace + TagImage).FirstOrDefault();
                int imageIndex = -1;
                if (image != null)
                {
                    Image = new ImageElement();
                    Image.Parse(image);
                    sectionStartItems++;
                    imageIndex = sectionElement.Elements().ToList().IndexOf(image);
                }

                XElement? annotation = sectionElement.Elements(FictionBook.DefaultNamespace + TagAnnotation).GetSingleValueOrNull(Logger);
                if (annotation != null)
                {
                    Annotation = new AnnotationElement(TagAnnotation);
                    Annotation.Parse(annotation);
                }

                IEnumerable<XElement> sectionsList = sectionElement.Elements(FictionBook.DefaultNamespace + TagSection);
                if (sectionsList.Any())
                {
                    foreach (XElement? item in sectionsList)
                    {
                        SectionElement section = new SectionElement();
                        section.Parse(item);
                        Sections.Add(section);
                    }
                }
                else
                {
                    XElement? firstElement = sectionElement.Elements()
                                                        .FirstOrDefault(x => _firstLineTags.Contains(x.Name.LocalName));
                    if (firstElement != null)
                    {
                        if (imageIndex >= 0)
                        {
                            int firstElementIndex = sectionElement.Elements().ToList().IndexOf(firstElement);
                            if (imageIndex > firstElementIndex)
                            {
                                sectionStartItems--;
                                Image = null;
                            }
                        }

                        ElementSwitch(firstElement);
                        // for the firstLineTag element
                        sectionStartItems++;

                        sectionElement.Elements()
                                                .Where(x => x.Name.LocalName switch
                                                    {
                                                        "p" => true,
                                                        "image" => true,
                                                        "poem" => true,
                                                        "subtitle" => true,
                                                        "cite" => true,
                                                        "empty-line" => true,
                                                        "table" => true,
                                                        _ => false
                                                    }
                                                )
                                                .Skip(sectionStartItems)
                                                .ToList()
                                                .ForEach(item => ElementSwitch(item));
                    }
                }
            }
        }
        private void ElementSwitch(XElement item)
        {
            switch (item.Name.LocalName)
            {
                case TagParagraph:
                    ParagraphType paragraph = new ParagraphType(TagParagraph);
                    paragraph.Parse(item);
                    Content.Add(paragraph);
                    break;
                case TagImage:
                    ImageElement image = new ImageElement();
                    image.Parse(item);
                    Content.Add(image);
                    break;
                case TagPoem:
                    PoemElement poem = new PoemElement();
                    poem.Parse(item);
                    Content.Add(poem);
                    break;
                case TagSubtitle:
                    ParagraphType subtitle = new ParagraphType(TagSubtitle);
                    subtitle.Parse(item);
                    Content.Add(subtitle);
                    break;
                case TagCite:
                    CiteElement cite = new CiteElement();
                    cite.Parse(item);
                    Content.Add(cite);
                    break;
                case TagEmptyLine:
                    EmptyLineElement emptyLine = new EmptyLineElement();
                    emptyLine.Parse(item);
                    Content.Add(emptyLine);
                    break;
                case TagTable:
                    TableElement table = new TableElement();
                    table.Parse(item);
                    Content.Add(table);
                    break;
                default:
                    FictionBook._parsingErrors.Value.Add(new NeedlessTagError(TagSection, item.Name.LocalName));
                    Logger.Warn($"The tag '{TagSection}' should not contain '{item.Name.LocalName}' tag");
                    break;
            }
        }
        public XNode ToXml()
        {
            XElement toReturn = new XElement(FictionBook.DefaultNamespace + TagSection);

            toReturn.AddOptionalAttribute(AttributeId, Id);
            toReturn.AddOptionalAttribute(XNamespace.Xml + AttributeLang, Lang);
            toReturn.AddOptionalTag(Title);
            toReturn.AddOptionalListToTag(Epigraphs);
            toReturn.AddOptionalTag(Image);
            toReturn.AddOptionalTag(Annotation);
            if (Sections.Count > 0)
            {
                toReturn.AddOptionalListToTag(Sections);
            }
            else
            {
                toReturn.AddOptionalListToTag(Content);
            }

            return toReturn;
        }
        private readonly List<string> _firstLineTags = new List<string>()
        {
            "p",
            "poem",
            "subtitle",
            "cite",
            "empty-line",
            "table",
        };
    }
}
