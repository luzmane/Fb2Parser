using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Fb2Parser.Errors;
using Fb2Parser.Utils;
using NLog;

namespace Fb2Parser.Model
{
    public class TableRowElement : IFb2Element
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public const string AttributeAlign = "align";
        public const string TagTableHeader = "th";
        public const string TagTableData = "td";

        public AlignType? Align { get; set; }
        public List<TableDataElement> Content { get; private set; } = new List<TableDataElement>();

        public void Parse(XNode node)
        {
            if (node is XElement tableRowElement)
            {
                string? alignType = tableRowElement.Attributes(AttributeAlign).GetSingleValueOrNull(Logger);
                if (alignType != null)
                {
                    if (Enum.TryParse(alignType, out AlignType align))
                    {
                        Align = align;
                    }
                    else
                    {
                        Logger.Error($"'{AttributeAlign}' attribute of '{TableElement.TagTableRow}' tag is incorrect: '{alignType}'");
                        FictionBook._parsingErrors.Value.Add(new IllegalAttributeError(TableElement.TagTableRow, AttributeAlign, alignType));
                    }
                }

                foreach (XElement? item in tableRowElement.Elements())
                {
                    switch (item.Name.LocalName)
                    {
                        case TagTableHeader:
                        case TagTableData:
                            TableDataElement tableData = new TableDataElement(item.Name.LocalName);
                            tableData.Parse(item);
                            Content.Add(tableData);
                            break;
                        default:
                            Logger.Error($"The tag '{TableElement.TagTableRow}' should not contain '{item.Name.LocalName}' tag");
                            FictionBook._parsingErrors.Value.Add(new NeedlessTagError(TableElement.TagTableRow, item.Name.LocalName));
                            break;
                    }
                }
            }
        }
        public XNode ToXml()
        {
            XElement toReturn = new XElement(FictionBook.DefaultNamespace + TableElement.TagTableRow);

            toReturn.AddOptionalAttribute(AttributeAlign, Align?.ToString());
            toReturn.AddRequiredListToTag(Content, Logger, TagTableData, typeof(TableDataElement));

            return toReturn;
        }
    }
}
