using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Fb2Parser.Errors;
using Fb2Parser.Model;
using NLog;

namespace Fb2Parser.Utils
{
    public static class XElementExtensions
    {
        public static void AddRequiredTag(this XElement element, IFb2Element? field, ILogger logger, string parentTagName, string tagName)
        {
            if (field is null)
            {
                FictionBook._parsingErrors.Value.Add(new RequiredTagError(parentTagName, tagName));
                logger.Error($"Tag '{parentTagName}' requires to have '{tagName}' by standard");
            }
            else
            {
                element.Add(field.ToXml());
            }
        }
        public static void AddOptionalTag(this XElement element, IFb2Element? field)
        {
            if (field != null)
            {
                element.Add(field.ToXml());
            }
        }
        public static void AddOptionalStringTag(this XElement element, XName tagName, string? value)
        {
            if (value != null)
            {
                element.Add(new XElement(tagName, value));
            }
        }
        public static void AddRequiredStringTag(this XElement element, XName tagName, string? value, ILogger logger, string parentTagName)
        {
            if (value is null)
            {
                FictionBook._parsingErrors.Value.Add(new RequiredTagError(parentTagName, tagName.LocalName));
                logger.Error($"Tag '{parentTagName}' requires to have '{tagName.LocalName}' by standard");
            }
            else
            {
                element.Add(new XElement(tagName, value));
            }
        }
        public static void AddOptionalTagContent(this XElement element, string? content)
        {
            if (content != null)
            {
                element.Value = content;
            }
        }
        public static void AddRequiredTagContent(this XElement element, string? content, ILogger logger, string parentTagName)
        {
            if (content is null)
            {
                FictionBook._parsingErrors.Value.Add(new RequiredContentError(parentTagName));
                logger.Error($"Tag '{parentTagName}' should have content");
            }
            else
            {
                element.Value = content;
            }
        }
        public static void AddOptionalListToTag(this XElement element, IEnumerable<IFb2Element> list)
        {
            foreach (IFb2Element item in list)
            {
                element.Add(item.ToXml());
            }
        }
        public static void AddOptionalListOfStringsToTag(this XElement element, IEnumerable<string> list, XName tagName)
        {
            foreach (string item in list)
            {
                element.Add(new XElement(tagName, item));
            }
        }
        public static void AddRequiredListToTag(this XElement element, IEnumerable<IFb2Element> list, ILogger logger, string? tagName)
        {
            if (list.Any())
            {
                foreach (IFb2Element item in list)
                {
                    element.Add(item.ToXml());
                }
            }
            else
            {
                if (tagName is null)
                {
                    FictionBook._parsingErrors.Value.Add(new RequiredElementInListError(element.Name.LocalName));
                    logger.Error($"Tag '{element.Name.LocalName}' requires to have at least one child tag by standard");
                }
                else
                {
                    FictionBook._parsingErrors.Value.Add(new RequiredElementInListError(element.Name.LocalName, tagName));
                    logger.Error($"Tag '{element.Name.LocalName}' requires to have at least one '{tagName}' by standard");
                }
            }
        }
        public static void AddOptionalAttribute(this XElement element, XName attributeName, string? attribute)
        {
            if (attribute != null)
            {
                element.SetAttributeValue(attributeName, attribute);
            }
        }
        public static void AddRequiredAttribute(this XElement element, XName attributeName, string? attribute, ILogger logger, string tagName)
        {
            if (attribute is null)
            {
                FictionBook._parsingErrors.Value.Add(new RequiredAttributeError(tagName, attributeName.LocalName));
                logger.Error($"Attribute '{attributeName}' of tag '{tagName}' is required by standard");
            }
            else
            {
                element.SetAttributeValue(attributeName, attribute);
            }
        }
        public static XElement? GetSingleValueOrNull(this IEnumerable<XElement> enumerable, ILogger Logger)
        {
            if (enumerable.Count() > 0)
            {
                XElement toReturn = enumerable.First();
                if (enumerable.Count() > 1)
                {
                    FictionBook._parsingErrors.Value.Add(new IncorrectTagNumberError(toReturn.Parent.Name.LocalName, toReturn.Name.LocalName));
                    Logger.Warn($"The tag '{toReturn.Parent.Name.LocalName}' has more than one child tag '{toReturn.Name.LocalName}', ignoring rest");
                }
                return toReturn;
            }
            return null;
        }
    }
}
