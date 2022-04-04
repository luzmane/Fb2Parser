using System;
using System.Linq;
using System.Xml.Linq;
using Fb2Parser.Utils;
using NLog;

namespace Fb2Parser.Model
{
    public class TableDataElement : StyleType
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public const string AttributeId = "id";
        public const string AttributeStyle = "style";
        public const string AttributeColSpan = "colspan";
        public const string AttributeRowSpan = "rowspan";
        public const string AttributeAlign = "align";
        public const string AttributeVAlign = "valign";

        public string? Id { get; set; }
        public string? Style { get; set; }
        public int? ColSpan { get; set; }
        public int? RowSpan { get; set; }
        public AlignType? Align { get; set; }
        public VerticalAlignType? VAlign { get; set; }

        public TableDataElement(string elementName) : base(elementName)
        {
        }

        public override void Parse(XNode node)
        {
            if (node is XElement tableDataElement)
            {
                base.Parse(tableDataElement);

                Id = tableDataElement.Attributes(AttributeId).GetSingleValueOrNull(Logger);
                Style = tableDataElement.Attributes(AttributeStyle).GetSingleValueOrNull(Logger);

                string? colspan = tableDataElement.Attributes(AttributeColSpan).GetSingleValueOrNull(Logger);
                if (colspan != null)
                {
                    if (int.TryParse(colspan, out int colSpan))
                    {
                        ColSpan = colSpan;
                    }
                }
                string? rowspan = tableDataElement.Attributes(AttributeRowSpan).GetSingleValueOrNull(Logger);
                if (rowspan != null)
                {
                    if (int.TryParse(rowspan, out int rowSpan))
                    {
                        RowSpan = rowSpan;
                    }
                }

                string? align = tableDataElement.Attributes(AttributeAlign).GetSingleValueOrNull(Logger);
                if (align != null)
                {
                    if (Enum.TryParse(align, out AlignType parsedAlign))
                    {
                        Align = parsedAlign;
                    }
                }

                string? valign = tableDataElement.Attributes(AttributeVAlign).GetSingleValueOrNull(Logger);
                if (valign != null)
                {
                    if (Enum.TryParse(valign, out VerticalAlignType parsedValign))
                    {
                        VAlign = parsedValign;
                    }
                }
            }
        }
        public override XNode ToXml()
        {
            XElement toReturn = (XElement)base.ToXml();

            toReturn.AddOptionalAttribute(AttributeId, Id);
            toReturn.AddOptionalAttribute(AttributeStyle, Style);
            toReturn.AddOptionalAttribute(AttributeColSpan, ColSpan.ToString());
            toReturn.AddOptionalAttribute(AttributeRowSpan, RowSpan.ToString());
            toReturn.AddOptionalAttribute(AttributeAlign, Align.ToString());
            toReturn.AddOptionalAttribute(AttributeVAlign, VAlign.ToString());

            return toReturn;
        }
    }
}
