using System.Linq;
using System.Xml.Linq;
using Fb2Parser.Utils;
using NLog;

namespace Fb2Parser.Model
{
    /// <summary>
    /// Information about some paper/outher published document,
    /// that was used as a source of this xml document
    /// </summary>
    public class CustomInfoElement : TextFieldType
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public const string AttributeInfoType = "info-type";

        public string? InfoType { get; set; }

        public CustomInfoElement() : base(DescriptionElement.TagCustomInfo)
        {
        }

        public override void Parse(XNode node)
        {
            base.Parse(node);

            if (node is XElement element)
            {
                InfoType = element.Attributes(AttributeInfoType).GetSingleValueOrNull(Logger);
            }
        }
        public override XNode ToXml()
        {
            XElement toReturn = (XElement)base.ToXml();

            toReturn.AddRequiredAttribute(AttributeInfoType, InfoType, Logger, ElementName);

            return toReturn;
        }
    }
}
