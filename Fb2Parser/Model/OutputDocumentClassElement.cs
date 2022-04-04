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
    /// Selector for output documents. Defines, which rule to apply to any specific output documents
    /// </summary>
    public class OutputDocumentClassElement : IFb2Element
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public const string TagPart = "part";
        public const string AttributeName = "name";
        public const string AttributeCreate = "create";
        public const string AttributePrice = "price";

        public List<PartOutputElement> Parts { get; private set; } = new List<PartOutputElement>();
        public string? Name { get; set; }
        public DocGenerationInstructionType? Create { get; set; }
        public float? Price { get; set; }

        public void Parse(XNode node)
        {
            if (node is XElement outputDocumentClassElement)
            {
                foreach (XElement? item in outputDocumentClassElement.Elements(FictionBook.DefaultNamespace + TagPart))
                {
                    PartOutputElement partOutputElement = new PartOutputElement();
                    partOutputElement.Parse(item);
                    Parts.Add(partOutputElement);
                }

                Name = outputDocumentClassElement.Attributes(AttributeName).GetSingleValueOrNull(Logger);

                string? docType = outputDocumentClassElement.Attributes(AttributeCreate).GetSingleValueOrNull(Logger);
                if (docType != null)
                {
                    if (Enum.TryParse(docType, out DocGenerationInstructionType create))
                    {
                        Create = create;
                    }
                    else
                    {
                        FictionBook._parsingErrors.Value.Add(new IllegalAttributeError(OutputElement.TagOutputDocumentClass, AttributeCreate, docType));
                        Logger.Warn($"'{AttributeCreate}' attribute of '{OutputElement.TagOutputDocumentClass}' tag is incorrect: '{docType}'");
                    }
                }

                string? price = outputDocumentClassElement.Attributes(AttributePrice).GetSingleValueOrNull(Logger);
                if (price != null)
                {
                    if (float.TryParse(price, out float parsedPrice))
                    {
                        Price = parsedPrice;
                    }
                    else
                    {
                        FictionBook._parsingErrors.Value.Add(new IllegalAttributeError(OutputElement.TagOutputDocumentClass, AttributePrice, price));
                        Logger.Warn($"'{AttributePrice}' attribute of '{OutputElement.TagOutputDocumentClass}' tag is incorrect: '{price}'");
                    }
                }
            }
        }
        public XNode ToXml()
        {
            XElement toReturn = new XElement(FictionBook.DefaultNamespace + OutputElement.TagOutputDocumentClass);

            toReturn.AddRequiredAttribute(AttributeName, Name, Logger, OutputElement.TagOutputDocumentClass);
            toReturn.AddOptionalAttribute(AttributeCreate, Create.ToString());
            toReturn.AddOptionalAttribute(AttributePrice, Price.ToString());
            toReturn.AddOptionalListToTag(Parts);

            return toReturn;
        }
    }
}
