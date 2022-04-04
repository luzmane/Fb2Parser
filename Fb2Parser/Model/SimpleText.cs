using System.Xml.Linq;
using NLog;

namespace Fb2Parser.Model
{
    public class SimpleText : IFb2Element
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public string? Text { get; set; }

        public void Parse(XNode node)
        {
            if (node is XText simpleText)
            {
                Text = simpleText.Value;
            }
        }
        public XNode ToXml()
        {
            XText toReturn;
            if (Text is null)
            {
                Logger.Error("Text is null, using empty string instead");
                toReturn = new XText(string.Empty);
            }
            else
            {
                toReturn = new XText(Text);
            }

            return toReturn;
        }
    }
}
