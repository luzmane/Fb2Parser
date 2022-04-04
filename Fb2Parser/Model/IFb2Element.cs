using System.Xml.Linq;

namespace Fb2Parser.Model
{
    public interface IFb2Element
    {
        /// <summary>
        /// Update Fb2 element from provided XNode
        /// </summary>
        /// <param name="node">data to update Fb2 element</param>
        void Parse(XNode node);

        /// <summary>
        /// Genereate XNode (XElement or EText) from Fb2 element
        /// </summary>
        /// <returns></returns>
        XNode ToXml();
    }
}
