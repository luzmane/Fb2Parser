using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Fb2Parser.Errors;
using Fb2Parser.Utils;
using NLog;

namespace Fb2Parser.Model
{
    /// <summary>
    /// This element contains an arbitrary stylesheet that is intepreted 
    /// by a some processing programs, e.g. text/css stylesheets can be 
    /// used by XSLT stylesheets to generate better looking html
    /// </summary>
    public class DocumentInfoElement : IFb2Element
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public const string TagAuthor = "author";
        public const string TagProgramUsed = "program-used";
        public const string TagDate = "date";
        public const string TagSrcUrl = "src-url";
        public const string TagSrcOcr = "src-ocr";
        public const string TagId = "id";
        public const string TagVersion = "version";
        public const string TagHistory = "history";
        public const string TagPublisher = "publisher";

        /// <summary>
        /// Author(s) of this particular document
        /// </summary>
        public List<AuthorElement> Authors { get; private set; } = new List<AuthorElement>();
        /// <summary>
        /// Any software used in preparation of this document, in free format
        /// </summary>
        public TextFieldType? ProgramUsed { set; get; }
        /// <summary>
        /// Date this document was created, same guidelines as in the <title-info> section apply
        /// </summary>
        public DateType? Date { get; set; }
        /// <summary>
        /// Source URL if this document is a conversion of some other (online) document
        /// </summary>
        public List<string> SrcUrl { get; private set; } = new List<string>();
        /// <summary>
        /// Author of the original (online) document, if this is a conversion
        /// </summary>
        public TextFieldType? SrcOcr { get; set; }
        /// <summary>
        /// This is a unique identifier for a document. This must not change
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// Document version, in free format, should be incremented 
        /// if the document is changed and re-released to the public
        /// </summary>
        public string? Version { get; set; }
        /// <summary>
        /// Short description for all changes made to this document,
        /// like "Added missing chapter 6", in free form
        /// </summary>
        public AnnotationElement? History { get; set; }
        /// <summary>
        /// Owner of the fb2 document copyrights
        /// </summary>
        public List<AuthorElement> Publishers { get; private set; } = new List<AuthorElement>();

        public void Parse(XNode node)
        {
            if (node is XElement documentInfoElement)
            {
                foreach (XElement? item in documentInfoElement.Elements(FictionBook.DefaultNamespace + TagAuthor))
                {
                    AuthorElement author = new AuthorElement(TagAuthor);
                    author.Parse(item);
                    Authors.Add(author);
                }

                XElement? programUsed = documentInfoElement.Elements(FictionBook.DefaultNamespace + TagProgramUsed).GetSingleValueOrNull(Logger);
                if (programUsed != null)
                {
                    ProgramUsed = new TextFieldType(TagProgramUsed);
                    ProgramUsed.Parse(programUsed);
                }

                XElement? date = documentInfoElement.Elements(FictionBook.DefaultNamespace + TagDate).GetSingleValueOrNull(Logger);
                if (date != null)
                {
                    Date = new DateType();
                    Date.Parse(date);
                }

                foreach (XElement? item in documentInfoElement.Elements(FictionBook.DefaultNamespace + TagSrcUrl))
                {

                    SrcUrl.Add(item.Value);
                }

                XElement? srcOcr = documentInfoElement.Elements(FictionBook.DefaultNamespace + TagSrcOcr).GetSingleValueOrNull(Logger);
                if (srcOcr != null)
                {
                    SrcOcr = new TextFieldType(TagSrcOcr);
                    SrcOcr.Parse(srcOcr);
                }

                XElement? id = documentInfoElement.Elements(FictionBook.DefaultNamespace + TagId).GetSingleValueOrNull(Logger);
                if (id != null)
                {
                    Id = id.Value;
                }

                string? version = documentInfoElement.Elements(FictionBook.DefaultNamespace + TagVersion).GetSingleValueOrNull(Logger)?.Value;
                if (version != null)
                {
                    if (double.TryParse(version, out _))
                    {
                        // verify if 'version' is valid number, but save the original format
                        Version = version;
                    }
                    else
                    {
                        FictionBook._parsingErrors.Value.Add(new IllegalTagError(TagVersion, version));
                        Logger.Warn($"'{TagVersion}' tag is not valid number: {version}");
                    }
                }

                XElement? history = documentInfoElement.Elements(FictionBook.DefaultNamespace + TagHistory).GetSingleValueOrNull(Logger);
                if (history != null)
                {
                    History = new AnnotationElement(TagHistory);
                    History.Parse(history);
                }

                foreach (XElement? item in documentInfoElement.Elements(FictionBook.DefaultNamespace + TagPublisher))
                {
                    AuthorElement publisher = new AuthorElement(TagPublisher);
                    publisher.Parse(item);
                    Publishers.Add(publisher);
                }
            }
        }
        public XNode ToXml()
        {
            XElement toReturn = new XElement(FictionBook.DefaultNamespace + DescriptionElement.TagDocumentInfo);

            toReturn.AddRequiredListToTag(Authors, Logger, TagAuthor);
            toReturn.AddOptionalTag(ProgramUsed);
            toReturn.AddRequiredTag(Date, Logger, DescriptionElement.TagDocumentInfo, TagDate);
            toReturn.AddOptionalListOfStringsToTag(SrcUrl, FictionBook.DefaultNamespace + TagSrcUrl);
            toReturn.AddOptionalTag(SrcOcr);
            toReturn.AddRequiredStringTag(FictionBook.DefaultNamespace + TagId, Id, Logger, DescriptionElement.TagDocumentInfo);
            toReturn.AddRequiredStringTag(FictionBook.DefaultNamespace + TagVersion, Version, Logger, DescriptionElement.TagDocumentInfo);
            toReturn.AddOptionalTag(History);
            toReturn.AddOptionalListToTag(Publishers);

            return toReturn;
        }
    }
}
