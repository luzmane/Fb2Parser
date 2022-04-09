using System.Collections.Generic;
using System.Xml.Linq;
using Fb2Parser.Utils;
using NLog;

namespace Fb2Parser.Model
{
    public class CoverpageElement : IFb2Element
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public const string TagImage = "image";

        public List<InlineImageElement> Images { get; private set; } = new List<InlineImageElement>();

        public void Parse(XNode node)
        {
            if (node is XElement coverpageElement)
            {
                foreach (XElement? item in coverpageElement.Elements(FictionBook.DefaultNamespace + TagImage))
                {
                    InlineImageElement image = new InlineImageElement();
                    image.Parse(item);
                    Images.Add(image);
                }
            }
        }
        public XNode ToXml()
        {
            XElement toReturn = new XElement(FictionBook.DefaultNamespace + TitleInfoElement.TagCoverpage);

            toReturn.AddRequiredListToTag(Images, Logger, TagImage, typeof(InlineImageElement));

            return toReturn;
        }
    }
}
