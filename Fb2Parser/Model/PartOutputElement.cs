using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Fb2Parser.Errors;
using Fb2Parser.Utils;
using NLog;

namespace Fb2Parser.Model
{
    /// <summary>
    /// Pointer to specific document section, explaining how to deal with it
    /// In XSD is 'partShareInstructionType'
    /// </summary>
    public class PartOutputElement : IFb2Element
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public const string AttributeType = "type";
        public const string AttributeHref = "href";
        public const string AttributeInclude = "include";

        public string? Type { set; get; }
        public string? Href { set; get; }
        public DocGenerationInstructionType? Include { set; get; }

        public void Parse(XNode node)
        {
            if (FictionBook.XlinkNamespace == XNamespace.None)
            {
                throw new XmlException("The book doesn't have 'xlink' namespace definition like: \"xmlns:xlink=\"http://www.w3.org/1999/xlink\"");
            }

            if (node is XElement partOutputElement)
            {
                Type = partOutputElement.Attributes(FictionBook.XlinkNamespace + AttributeType).GetSingleValueOrNull(Logger);
                Href = partOutputElement.Attributes(FictionBook.XlinkNamespace + AttributeHref).GetSingleValueOrNull(Logger);

                string? docType = partOutputElement.Attributes(AttributeInclude).GetSingleValueOrNull(Logger);
                if (docType != null)
                {
                    if (Enum.TryParse(docType, out DocGenerationInstructionType include))
                    {
                        Include = include;
                    }
                    else
                    {
                        Logger.Error($"'{AttributeInclude}' attribute of '{OutputElement.TagPart}' tag is incorrect: '{docType}'");
                        FictionBook._parsingErrors.Value.Add(new IllegalAttributeError(OutputElement.TagPart, AttributeInclude, docType));
                    }
                }
            }
        }
        public XNode ToXml()
        {
            XElement toReturn = new XElement(FictionBook.DefaultNamespace + OutputElement.TagPart);

            toReturn.AddOptionalAttribute(FictionBook.XlinkNamespace + AttributeType, Type);
            toReturn.AddRequiredAttribute(AttributeHref, Href, Logger, OutputElement.TagPart);
            toReturn.AddRequiredAttribute(AttributeInclude, Include?.ToString(), Logger, OutputElement.TagPart);

            return toReturn;
        }
    }
}
