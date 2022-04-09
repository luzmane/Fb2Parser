using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using Fb2Parser.Utils;
using NLog;

namespace Fb2Parser.Model
{
    /// <summary>
    /// Generic information about the book
    /// </summary>
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class TitleInfoElement : IFb2Element
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public const string TagGenre = "genre";
        public const string TagAuthor = "author";
        public const string TagBookTitle = "book-title";
        public const string TagAnnotation = "annotation";
        public const string TagKeywords = "keywords";
        public const string TagDate = "date";
        public const string TagCoverpage = "coverpage";
        public const string TagLang = "lang";
        public const string TagSrcLang = "src-lang";
        public const string TagTranslator = "translator";
        public const string TagSequence = "sequence";

        /// <summary>
        /// Tag self name
        /// </summary>
        public string ElementName { get; }
        /// <summary>
        /// Genre of this book, with the optional match percentage
        /// </summary>
        public List<GenreElement> Genres { get; private set; } = new List<GenreElement>();
        /// <summary>
        /// Author(s) of this book
        /// </summary>
        public List<AuthorElement> Authors { get; private set; } = new List<AuthorElement>();
        /// <summary>
        /// Book title
        /// </summary>
        public TextFieldType? BookTitle { get; set; }
        /// <summary>
        /// Annotation for this book
        /// </summary>
        public AnnotationElement? Annotation { get; set; }
        /// <summary>
        /// Any keywords for this book, intended for use in search engines
        /// </summary>
        public TextFieldType? Keywords { get; set; }
        /// <summary>
        /// Date this book was written, can be not exact, e.g. 1863-1867. If an optional attribute is present,
        /// then it should contain some computer-readable date from the interval for use by search and indexingengines
        /// </summary>
        public DateType? Date { get; set; }
        /// <summary>
        /// Any coverpage items, currently only images
        /// </summary>
        public CoverpageElement? Coverpage { get; set; }
        /// <summary>
        /// Book's language
        /// </summary>
        public string? Lang { get; set; }
        /// <summary>
        /// Book's source language if this is a translation
        /// </summary>
        public string? SrcLang { get; set; }
        /// <summary>
        /// Translators if this is a translation
        /// </summary>
        public List<AuthorElement> Translators { get; private set; } = new List<AuthorElement>();
        /// <summary>
        /// Any sequences this book might be part of
        /// </summary>
        public List<SequenceElement> Sequences { get; private set; } = new List<SequenceElement>();

        public TitleInfoElement(string elementName)
        {
            ElementName = elementName;
        }

        public void Parse(XNode node)
        {
            if (node is XElement titleInfoElement)
            {
                foreach (XElement? item in titleInfoElement.Elements(FictionBook.DefaultNamespace + TagGenre))
                {
                    GenreElement genre = new GenreElement();
                    genre.Parse(item);
                    Genres.Add(genre);
                }

                foreach (XElement? item in titleInfoElement.Elements(FictionBook.DefaultNamespace + TagAuthor))
                {
                    AuthorElement author = new AuthorElement(TagAuthor);
                    author.Parse(item);
                    Authors.Add(author);
                }

                XElement? bookTitle = titleInfoElement.Elements(FictionBook.DefaultNamespace + TagBookTitle).GetSingleValueOrNull(Logger);
                if (bookTitle != null)
                {
                    BookTitle = new TextFieldType(TagBookTitle);
                    BookTitle.Parse(bookTitle);
                }

                XElement? annotation = titleInfoElement.Elements(FictionBook.DefaultNamespace + TagAnnotation).GetSingleValueOrNull(Logger);
                if (annotation != null)
                {
                    Annotation = new AnnotationElement(TagAnnotation);
                    Annotation.Parse(annotation);
                }

                XElement? keywords = titleInfoElement.Elements(FictionBook.DefaultNamespace + TagKeywords).GetSingleValueOrNull(Logger);
                if (keywords != null)
                {
                    Keywords = new TextFieldType(TagKeywords);
                    Keywords.Parse(keywords);
                }

                XElement? date = titleInfoElement.Elements(FictionBook.DefaultNamespace + TagDate).GetSingleValueOrNull(Logger);
                if (date != null)
                {
                    Date = new DateType();
                    Date.Parse(date);
                }

                XElement? coverpage = titleInfoElement.Elements(FictionBook.DefaultNamespace + TagCoverpage).GetSingleValueOrNull(Logger);
                if (coverpage != null)
                {
                    Coverpage = new CoverpageElement();
                    Coverpage.Parse(coverpage);
                }

                XElement? lang = titleInfoElement.Elements(FictionBook.DefaultNamespace + TagLang).GetSingleValueOrNull(Logger);
                if (lang != null)
                {
                    Lang = lang.Value;
                }

                XElement? srcLang = titleInfoElement.Elements(FictionBook.DefaultNamespace + TagSrcLang).GetSingleValueOrNull(Logger);
                if (srcLang != null)
                {
                    SrcLang = srcLang.Value;
                }

                foreach (XElement? item in titleInfoElement.Elements(FictionBook.DefaultNamespace + TagTranslator))
                {
                    AuthorElement translator = new AuthorElement(TagTranslator);
                    translator.Parse(item);
                    Translators.Add(translator);
                }

                foreach (XElement? item in titleInfoElement.Elements(FictionBook.DefaultNamespace + TagSequence))
                {
                    SequenceElement sequence = new SequenceElement();
                    sequence.Parse(item);
                    Sequences.Add(sequence);
                }
            }
        }
        public XNode ToXml()
        {
            XElement toReturn = new XElement(FictionBook.DefaultNamespace + ElementName);

            toReturn.AddRequiredListToTag(Genres, Logger, TagGenre, typeof(GenreElement));
            toReturn.AddRequiredListToTag(Authors, Logger, TagAuthor, typeof(AuthorElement));
            toReturn.AddRequiredTag(BookTitle, Logger, ElementName, TagBookTitle, typeof(TextFieldType));
            toReturn.AddOptionalTag(Annotation);
            toReturn.AddOptionalTag(Keywords);
            toReturn.AddOptionalTag(Date);
            toReturn.AddOptionalTag(Coverpage);
            toReturn.AddRequiredStringTag(FictionBook.DefaultNamespace + TagLang, Lang, Logger, ElementName);
            toReturn.AddOptionalStringTag(FictionBook.DefaultNamespace + TagSrcLang, SrcLang);
            toReturn.AddOptionalListToTag(Translators);
            toReturn.AddOptionalListToTag(Sequences);

            return toReturn;
        }

        private string GetDebuggerDisplay()
        {
            return ToString();
        }
    }
}
