using System.Collections.Generic;
using System.Xml.Linq;
using Fb2Parser.Errors;
using Fb2Parser.Utils;
using NLog;

namespace Fb2Parser.Model
{
    /// <summary>
    /// Markup
    /// </summary>
    public class StyleLinkType : IFb2Element
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public const string TagStrong = "strong";
        public const string TagEmphasis = "emphasis";
        public const string TagStyle = "style";
        public const string TagStrikethrough = "strikethrough";
        public const string TagSub = "sub";
        public const string TagSup = "sup";
        public const string TagCode = "code";
        public const string TagImage = "image";
        public const string Undefined = "Undefined";

        /// <summary>
        /// Tag self name
        /// </summary>
        public string ElementName { get; }
        public List<IFb2Element> Content { get; private set; } = new List<IFb2Element>();

        public StyleLinkType(string elementName)
        {
            ElementName = elementName;
        }

        public virtual void Parse(XNode node)
        {
            if (node is XElement styleType)
            {
                foreach (XNode item in styleType.Nodes())
                {
                    if (item is XText text)
                    {
                        SimpleText simpleText = new SimpleText();
                        simpleText.Parse(text);
                        Content.Add(simpleText);
                    }
                    else if (item is XElement formattedText)
                    {
                        switch (formattedText.Name.LocalName)
                        {
                            case TagStrong:
                            case TagEmphasis:
                            case TagStrikethrough:
                            case TagSub:
                            case TagSup:
                            case TagCode:
                            case TagStyle:
                                StyleLinkType taggedText = new StyleLinkType(formattedText.Name.LocalName);
                                taggedText.Parse(formattedText);
                                Content.Add(taggedText);
                                break;
                            case TagImage:
                                InlineImageElement image = new InlineImageElement();
                                image.Parse(formattedText);
                                Content.Add(image);
                                break;
                            default:
                                Logger.Error($"The tag '{ElementName}' should not contain '{formattedText.Name.LocalName}' tag");
                                FictionBook._parsingErrors.Value.Add(new NeedlessTagError(ElementName, formattedText.Name.LocalName));
                                break;
                        }
                    }
                }
            }
        }
        public virtual XNode ToXml()
        {
            XElement toReturn = new XElement(FictionBook.DefaultNamespace + ElementName);

            toReturn.AddOptionalListWithImageToTag(Content, Logger);

            return toReturn;
        }
    }
}
