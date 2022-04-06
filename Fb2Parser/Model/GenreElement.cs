using System.Diagnostics;
using System.Xml.Linq;
using Fb2Parser.Errors;
using Fb2Parser.Utils;
using NLog;

namespace Fb2Parser.Model
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class GenreElement : IFb2Element
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public const string AttributeMatch = "match";

        public int? Match { set; get; }
        public string? Content { set; get; }

        public void Parse(XNode node)
        {
            if (node is XElement genreElement)
            {
                string? match = genreElement.Attributes(AttributeMatch).GetSingleValueOrNull(Logger);
                if (match != null)
                {
                    if (int.TryParse(match, out int parsedMatch))
                    {
                        Match = parsedMatch;
                    }
                    else
                    {
                        FictionBook._parsingErrors.Value.Add(new IllegalAttributeError(TitleInfoElement.TagGenre, AttributeMatch, match));
                        Logger.Warn($"Unable to parse '{AttributeMatch}' attribute: {match}");
                    }
                }

                Content = genreElement.Value;
            }
        }
        public XNode ToXml()
        {
            XElement toReturn = new XElement(FictionBook.DefaultNamespace + TitleInfoElement.TagGenre);

            if (Match != null)
            {
                if (Match > 100)
                {
                    toReturn.AddOptionalAttribute(AttributeMatch, "100");

                }
                else if (Match < 0)
                {
                    toReturn.AddOptionalAttribute(AttributeMatch, "100");
                }
                else
                {
                    toReturn.AddOptionalAttribute(AttributeMatch, Match.ToString());
                }
            }

            toReturn.AddRequiredTagContent(Content, Logger, TitleInfoElement.TagGenre);

            return toReturn;
        }

        private string GetDebuggerDisplay()
        {
            string match = Match is null ? string.Empty : " match=" + Match + "\"";
            return $"<genre{match}>{Content}</genre>";
        }
    }
}
