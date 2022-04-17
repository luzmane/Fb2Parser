using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Fb2Parser.Utils;
using NLog;

namespace Fb2Parser.Model
{
    /// <summary>
    /// Main content of the book, multiple bodies are used for additional information, like footnotes, that do not appear in the main book flow. The first body is presented to the reader by default, and content in the other bodies should be accessible by hyperlinks. 
    /// Name attribute should describe the meaning of this body, this is optional for the main body.
    /// </summary>
    public class BodyElement : IFb2Element
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public const string AttributeName = "name";
        public const string AttributeLang = "lang";
        public const string TagImage = "image";
        public const string TagTitle = "title";
        public const string TagEpigraph = "epigraph";
        public const string TagSection = "section";

        public string? Lang { set; get; }
        public string? Name { set; get; }
        /// <summary>
        /// Image to be displayed at the top of this section
        /// </summary>
        public ImageElement? Image { get; set; }
        /// <summary>
        /// A fancy title for the entire book, should be used if the simple text version 
        /// in <description> != adequate, e.g. the book title has multiple paragraphs
        /// and/or character styles
        /// </summary>
        public TitleElement? Title { get; set; }
        /// <summary>
        /// Epigraph(s) for the entire book, if any
        /// </summary>
        public List<EpigraphElement> Epigraphs { get; private set; } = new List<EpigraphElement>();
        public List<SectionElement> Sections { get; private set; } = new List<SectionElement>();

        public virtual void Parse(XNode node)
        {
            if (node is XElement bodyElement)
            {
                Lang = bodyElement.Attributes(XNamespace.Xml + AttributeLang).GetSingleValueOrNull(Logger);
                Name = bodyElement.Attributes(AttributeName).GetSingleValueOrNull(Logger);

                XElement? image = bodyElement.Elements(FictionBook.DefaultNamespace + TagImage).GetSingleValueOrNull(Logger);
                if (image != null)
                {
                    Image = new ImageElement();
                    Image.Parse(image);
                }

                XElement? title = bodyElement.Elements(FictionBook.DefaultNamespace + TagTitle).GetSingleValueOrNull(Logger);
                if (title != null)
                {
                    Title = new TitleElement();
                    Title.Parse(title);
                }

                foreach (XElement? item in bodyElement.Elements(FictionBook.DefaultNamespace + TagEpigraph))
                {
                    EpigraphElement epigraph = new EpigraphElement();
                    epigraph.Parse(item);
                    Epigraphs.Add(epigraph);
                }

                foreach (XElement? item in bodyElement.Elements(FictionBook.DefaultNamespace + TagSection))
                {
                    SectionElement section = new SectionElement();
                    section.Parse(item);
                    Sections.Add(section);
                }
            }
        }

        public XNode ToXml()
        {
            XElement toReturn = new XElement(FictionBook.DefaultNamespace + FictionBook.TagBody);

            toReturn.AddOptionalAttribute(XNamespace.Xml + AttributeLang, Lang);
            toReturn.AddOptionalAttribute(AttributeName, Name);
            toReturn.AddOptionalImageTag(Image, Logger);
            toReturn.AddOptionalTag(Title);
            toReturn.AddOptionalListToTag(Epigraphs);
            toReturn.AddOptionalListToTag(Sections);

            return toReturn;
        }
    }
}
