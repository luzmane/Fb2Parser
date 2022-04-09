using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Fb2Parser.Errors;
using Fb2Parser.Model;
using NLog;

namespace Fb2Parser.Utils
{
    public static class XElementExtensions
    {
        public static void AddRequiredTag(this XElement element, IFb2Element? field, ILogger logger, string parentTagName, string tagName, Type type)
        {
            if (field is null)
            {
                if (FictionBook._fixMandatoryTags.Value)
                {
                    logger.Warn($"Creating mandatory tag '{tagName}' for '{parentTagName}'");
                    if (typeof(IFb2Element).IsAssignableFrom(type))
                    {
                        ConstructorInfo constructorInfo = type.GetConstructors().Where(ctor => ctor.GetParameters().Any()).FirstOrDefault();
                        if (constructorInfo != null)
                        {
                            field = (IFb2Element)constructorInfo.Invoke(new object[] { tagName });
                        }
                        else
                        {
                            constructorInfo = type.GetConstructors().FirstOrDefault();
                            field = (IFb2Element)constructorInfo.Invoke(new object[] { tagName });
                        }
                        element.Add(field.ToXml());
                    }
                    else
                    {
                        logger.Error($"The tag is not implementing {nameof(IFb2Element)}");
                    }
                }
                else
                {
                    logger.Error($"Tag '{parentTagName}' requires to have '{tagName}' by standard");
                    FictionBook._parsingErrors.Value.Add(new RequiredTagError(parentTagName, tagName));
                }
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
                if (FictionBook._fixMandatoryTags.Value)
                {
                    logger.Warn($"Creating mandatory string tag for '{tagName}'");
                    element.Add(new XElement(tagName, string.Empty));
                }
                else
                {
                    logger.Error($"Tag '{parentTagName}' requires to have '{tagName.LocalName}' by standard");
                    FictionBook._parsingErrors.Value.Add(new RequiredTagError(parentTagName, tagName.LocalName));
                }
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
                if (FictionBook._fixMandatoryTags.Value)
                {
                    logger.Warn($"Creating mandatory text node for '{parentTagName}'");
                    element.Value = string.Empty;
                }
                else
                {
                    logger.Error($"Tag '{parentTagName}' should have content");
                    FictionBook._parsingErrors.Value.Add(new RequiredContentError(parentTagName));
                }
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
        public static void AddRequiredListToTag(this XElement element, IEnumerable<IFb2Element> list, ILogger logger, string tagName, Type type)
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
                if (FictionBook._fixMandatoryTags.Value)
                {
                    logger.Warn($"Creating mandatory tag '{tagName}' for '{element.Name.LocalName}'");
                    if (typeof(IFb2Element).IsAssignableFrom(type))
                    {
                        IFb2Element field;
                        ConstructorInfo constructorInfo = type.GetConstructors().Where(ctor => ctor.GetParameters().Any()).FirstOrDefault();
                        if (constructorInfo != null)
                        {
                            field = (IFb2Element)constructorInfo.Invoke(new object[] { tagName });
                        }
                        else
                        {
                            constructorInfo = type.GetConstructors().FirstOrDefault();
                            field = (IFb2Element)constructorInfo.Invoke(new object[] { tagName });
                        }
                        element.Add(field.ToXml());
                    }
                    else
                    {
                        logger.Error($"The tag is not implementing {nameof(IFb2Element)}");
                    }
                }
                else
                {
                    logger.Error($"Tag '{element.Name.LocalName}' requires to have at least one '{tagName}' by standard");
                    FictionBook._parsingErrors.Value.Add(new RequiredElementInListError(element.Name.LocalName, tagName));
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
                logger.Error($"Attribute '{attributeName}' of tag '{tagName}' is required by standard");
                FictionBook._parsingErrors.Value.Add(new RequiredAttributeError(tagName, attributeName.LocalName));
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
                    Logger.Error($"The tag '{toReturn.Parent.Name.LocalName}' has more than one child tag '{toReturn.Name.LocalName}', ignoring rest");
                    FictionBook._parsingErrors.Value.Add(new IncorrectTagNumberError(toReturn.Parent.Name.LocalName, toReturn.Name.LocalName));
                }
                return toReturn;
            }
            return null;
        }
    }
}
