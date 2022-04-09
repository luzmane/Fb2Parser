using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Fb2Parser.Errors;
using Fb2Parser.Utils;
using NLog;

namespace Fb2Parser.Model
{
    /// <summary>
    /// Information about a single author
    /// </summary>
    public class AuthorElement : IFb2Element
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public const string TagFirstName = "first-name";
        public const string TagMiddleName = "middle-name";
        public const string TagLastName = "last-name";
        public const string TagNickname = "nickname";
        public const string TagHomePage = "home-page";
        public const string TagEmail = "email";
        public const string TagId = "id";

        public TextFieldType? FirstName { set; get; }
        public TextFieldType? MiddleName { set; get; }
        public TextFieldType? LastName { set; get; }
        public TextFieldType? Nickname { set; get; }
        public List<string> HomePages { private set; get; } = new List<string>();
        public List<string> Emails { private set; get; } = new List<string>();
        public string? Id { set; get; }
        /// <summary>
        /// Tag self name
        /// </summary>
        public string ElementName { get; }

        public AuthorElement(string elementName)
        {
            ElementName = elementName;
        }

        public void Parse(XNode node)
        {
            if (node is XElement authorElement)
            {
                XElement? firstName = authorElement.Elements(FictionBook.DefaultNamespace + TagFirstName).GetSingleValueOrNull(Logger);
                if (firstName != null)
                {
                    FirstName = new TextFieldType(TagFirstName);
                    FirstName.Parse(firstName);
                }

                XElement? lastName = authorElement.Elements(FictionBook.DefaultNamespace + TagLastName).GetSingleValueOrNull(Logger);
                if (lastName != null)
                {
                    LastName = new TextFieldType(TagLastName);
                    LastName.Parse(lastName);
                }

                XElement? middleName = authorElement.Elements(FictionBook.DefaultNamespace + TagMiddleName).GetSingleValueOrNull(Logger);
                if (middleName != null)
                {
                    MiddleName = new TextFieldType(TagMiddleName);
                    MiddleName.Parse(middleName);
                }

                XElement? nickname = authorElement.Elements(FictionBook.DefaultNamespace + TagNickname).GetSingleValueOrNull(Logger);
                if (nickname != null)
                {
                    Nickname = new TextFieldType(TagNickname);
                    Nickname.Parse(nickname);
                }

                foreach (XElement? item in authorElement.Elements(FictionBook.DefaultNamespace + TagHomePage))
                {
                    HomePages.Add(item.Value);
                }

                foreach (XElement? item in authorElement.Elements(FictionBook.DefaultNamespace + TagEmail))
                {
                    Emails.Add(item.Value);
                }

                XElement? id = authorElement.Elements(FictionBook.DefaultNamespace + TagId).GetSingleValueOrNull(Logger);
                if (id != null)
                {
                    Id = id.Value;
                }
            }
        }
        public XNode ToXml()
        {
            XElement toReturn = new XElement(FictionBook.DefaultNamespace + ElementName);

            if (FirstName != null || LastName != null)
            {
                toReturn.AddRequiredTag(FirstName, Logger, ElementName, TagFirstName, typeof(TextFieldType));
                toReturn.AddOptionalTag(MiddleName);
                toReturn.AddRequiredTag(LastName, Logger, ElementName, TagLastName, typeof(TextFieldType));
                toReturn.AddOptionalTag(Nickname);
                toReturn.AddOptionalListOfStringsToTag(HomePages, FictionBook.DefaultNamespace + TagHomePage);
                toReturn.AddOptionalListOfStringsToTag(Emails, FictionBook.DefaultNamespace + TagEmail);
                toReturn.AddOptionalStringTag(FictionBook.DefaultNamespace + TagId, Id);
            }
            else if (Nickname != null)
            {
                toReturn.AddRequiredTag(Nickname, Logger, ElementName, TagNickname, typeof(TextFieldType));
                toReturn.AddOptionalListOfStringsToTag(HomePages, FictionBook.DefaultNamespace + TagHomePage);
                toReturn.AddOptionalListOfStringsToTag(Emails, FictionBook.DefaultNamespace + TagEmail);
                toReturn.AddOptionalStringTag(FictionBook.DefaultNamespace + TagId, Id);
            }
            else
            {
                if (FictionBook._fixMandatoryTags.Value)
                {
                    Logger.Warn($"There is no suitable child set found for '{ElementName}' tag. Creating empty 'nickname' tag");
                    toReturn.AddRequiredTag(new TextFieldType(TagNickname), Logger, TagNickname, ElementName, typeof(TextFieldType));
                }
                else
                {
                    Logger.Error($"There is no suitable child set found for '{ElementName}' tag. The standard requires at least 'last-name', 'first-name' tags or 'nickname'");
                    FictionBook._parsingErrors.Value.Add(new NoSetTagError(ElementName));
                }
            }

            return toReturn;
        }
    }
}
