using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Fb2Parser.Utils;
using NLog;

namespace Fb2Parser.Model
{
    /// <summary>
    /// Basic html-like tables
    /// </summary>
    public class TableElement : IFb2Element
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public const string AttributeId = "id";
        public const string AttributeStyle = "style";
        public const string TagTableRow = "tr";

        public string? Id { get; set; }
        public string? Style { get; set; }
        public List<TableRowElement> TableRows { get; private set; } = new List<TableRowElement>();

        public void Parse(XNode node)
        {
            if (node is XElement tableElement)
            {
                Id = tableElement.Attributes(AttributeId).GetSingleValueOrNull(Logger);
                Style = tableElement.Attributes(AttributeStyle).GetSingleValueOrNull(Logger);

                foreach (XElement? item in tableElement.Elements(FictionBook.DefaultNamespace + TagTableRow))
                {
                    TableRowElement tableRow = new TableRowElement();
                    tableRow.Parse(item);
                    TableRows.Add(tableRow);
                }
            }
        }
        public XNode ToXml()
        {
            XElement toReturn = new XElement(FictionBook.DefaultNamespace + CiteElement.TagTable);

            toReturn.AddOptionalAttribute(AttributeStyle, Style);
            toReturn.AddOptionalAttribute(AttributeId, Id);
            toReturn.AddRequiredListToTag(TableRows, Logger, TagTableRow);

            return toReturn;
        }
    }
}
