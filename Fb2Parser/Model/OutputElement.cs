using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Fb2Parser.Errors;
using Fb2Parser.Utils;
using NLog;

namespace Fb2Parser.Model
{
    /// <summary>
    /// In-document instruction for generating output free and payed documents.
    /// In XSD is 'shareInstructionType'
    /// </summary>
    public class OutputElement : IFb2Element
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public const string TagPart = "part";
        public const string TagOutputDocumentClass = "output-document-class";
        public const string AttributeMode = "mode";
        public const string AttributeIncludeAll = "include-all";
        public const string AttributePrice = "price";
        public const string AttributeCurrency = "currency";

        public List<IFb2Element> Content { get; private set; } = new List<IFb2Element>();
        public ShareModesType? Mode { get; set; }
        public DocGenerationInstructionType? IncludeAll { get; set; }
        public string? Price { get; set; }
        public string? Currency { get; set; }

        public void Parse(XNode node)
        {
            if (node is XElement outputElement)
            {
                string? docType = outputElement.Attributes(AttributeIncludeAll).GetSingleValueOrNull(Logger);
                if (docType != null)
                {
                    if (Enum.TryParse(docType, out DocGenerationInstructionType includeAll))
                    {
                        IncludeAll = includeAll;
                    }
                    else
                    {
                        FictionBook._parsingErrors.Value.Add(new IllegalAttributeError(DescriptionElement.TagOutput, AttributeIncludeAll, docType));
                        Logger.Warn($"'{AttributeIncludeAll}' attribute of '{DescriptionElement.TagOutput}' tag is incorrect: '{docType}'");
                    }
                }

                string? shareModesType = outputElement.Attributes(AttributeMode).GetSingleValueOrNull(Logger);
                if (shareModesType != null)
                {
                    if (Enum.TryParse(shareModesType, out ShareModesType mode))
                    {
                        Mode = mode;
                    }
                    else
                    {
                        FictionBook._parsingErrors.Value.Add(new IllegalAttributeError(DescriptionElement.TagOutput, AttributeMode, shareModesType));
                        Logger.Warn($"'{AttributeMode}' attribute of '{DescriptionElement.TagOutput}' tag is incorrect: '{shareModesType}'");
                    }
                }

                string? price = outputElement.Attributes(AttributePrice).GetSingleValueOrNull(Logger);
                if (price != null)
                {
                    if (float.TryParse(price, out _))
                    {
                        // verify attribute 'price' is valid number, but save the original format
                        Price = price;
                    }
                    else
                    {
                        FictionBook._parsingErrors.Value.Add(new IllegalAttributeError(DescriptionElement.TagOutput, AttributePrice, price));
                        Logger.Warn($"'{AttributePrice}' attribute of '{DescriptionElement.TagOutput}' tag is incorrect: '{price}'");
                    }
                }

                Currency = outputElement.Attributes(AttributeCurrency).GetSingleValueOrNull(Logger);

                foreach (XElement? item in outputElement.Elements())
                {
                    switch (item.Name.LocalName)
                    {
                        case TagPart:
                            PartOutputElement partOutputElement = new PartOutputElement();
                            partOutputElement.Parse(item);
                            Content.Add(partOutputElement);
                            break;
                        case TagOutputDocumentClass:
                            OutputDocumentClassElement outputDocumentClassElement = new OutputDocumentClassElement();
                            outputDocumentClassElement.Parse(item);
                            Content.Add(outputDocumentClassElement);
                            break;
                        default:
                            FictionBook._parsingErrors.Value.Add(new NeedlessTagError(DescriptionElement.TagOutput, item.Name.LocalName));
                            Logger.Warn($"The tag '{DescriptionElement.TagOutput}' should not contain '{item.Name.LocalName}' tag");
                            break;
                    }
                }
            }
        }

        public XNode ToXml()
        {
            XElement toReturn = new XElement(FictionBook.DefaultNamespace + DescriptionElement.TagOutput);

            toReturn.AddRequiredAttribute(AttributeMode, Mode?.ToString(), Logger, DescriptionElement.TagOutput);
            toReturn.AddRequiredAttribute(AttributeIncludeAll, IncludeAll?.ToString(), Logger, DescriptionElement.TagOutput);
            toReturn.AddOptionalAttribute(AttributePrice, Price);
            toReturn.AddOptionalAttribute(AttributeCurrency, Currency);
            toReturn.AddOptionalListToTag(Content);

            return toReturn;
        }
    }
}
