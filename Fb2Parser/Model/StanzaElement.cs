using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Fb2Parser.Utils;
using NLog;

namespace Fb2Parser.Model
{
    /// <summary>
    /// Each poem should have at least one stanza.
    /// Stanzas are usually separated with empty lines by user agents
    /// </summary>
    public class StanzaElement : IFb2Element
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public const string AttributeLang = "lang";
        public const string TagTitle = "title";
        public const string TagSubtitle = "subtitle";
        public const string TagLine = "v";

        public string? Lang { get; set; }
        public TitleElement? Title { get; set; }
        public ParagraphType? Subtitle { get; set; }
        /// <summary>
        /// An individual line in a stanza
        /// </summary>
        public List<ParagraphType> Vs { get; private set; } = new List<ParagraphType>();

        public void Parse(XNode node)
        {
            if (node is XElement stanzaElement)
            {
                Lang = stanzaElement.Attributes(XNamespace.Xml + AttributeLang).GetSingleValueOrNull(Logger);

                XElement? title = stanzaElement.Elements(FictionBook.DefaultNamespace + TagTitle).GetSingleValueOrNull(Logger);
                if (title != null)
                {
                    Title = new TitleElement();
                    Title.Parse(title);
                }

                XElement? subtitle = stanzaElement.Elements(FictionBook.DefaultNamespace + TagSubtitle).GetSingleValueOrNull(Logger);
                if (subtitle != null)
                {
                    Subtitle = new ParagraphType(TagSubtitle);
                    Subtitle.Parse(subtitle);
                }

                foreach (XElement? item in stanzaElement.Elements(FictionBook.DefaultNamespace + TagLine))
                {
                    ParagraphType section = new ParagraphType(TagLine);
                    section.Parse(item);
                    Vs.Add(section);
                }
            }
        }
        public XNode ToXml()
        {
            XElement toReturn = new XElement(FictionBook.DefaultNamespace + PoemElement.TagStanza);

            toReturn.AddOptionalAttribute(XNamespace.Xml + AttributeLang, Lang);
            toReturn.AddOptionalTag(Title);
            toReturn.AddOptionalTag(Subtitle);
            toReturn.AddRequiredListToTag(Vs, Logger, TagLine, typeof(ParagraphType));

            return toReturn;
        }
    }
}
