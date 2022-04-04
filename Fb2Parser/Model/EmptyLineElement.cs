using NLog;
using System.Xml.Linq;

namespace Fb2Parser.Model
{
    public class EmptyLineElement : IFb2Element
    {
        public void Parse(XNode node)
        {
        }
        public XNode ToXml()
        {
            return new XElement(FictionBook.DefaultNamespace + CiteElement.TagEmptyLine);
        }
    }
}
