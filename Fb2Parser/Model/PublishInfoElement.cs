using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Fb2Parser.Utils;
using NLog;

namespace Fb2Parser.Model
{
    /// <summary>
    /// Information about some paper/outher published document,
    /// that was used as a source of this xml document
    /// </summary>
    public class PublishInfoElement : IFb2Element
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public const string TagBookName = "book-name";
        public const string TagPublisher = "publisher";
        public const string TagCity = "city";
        public const string TagYear = "year";
        public const string TagIsbn = "isbn";
        public const string TagSequence = "sequence";

        /// <summary>
        /// Original (paper) book name
        /// </summary>
        public TextFieldType? BookName { get; set; }
        /// <summary>
        /// Original (paper) book publisher
        /// </summary>
        public TextFieldType? Publisher { get; set; }
        /// <summary>
        /// City where the original (paper) book was published
        /// </summary>
        public TextFieldType? City { get; set; }
        /// <summary>
        /// Year of the original (paper) publication
        /// </summary>
        public string? Year { get; set; }
        public TextFieldType? Isbn { get; set; }
        public List<SequenceElement> Sequences { get; private set; } = new List<SequenceElement>();

        public virtual void Parse(XNode node)
        {
            if (node is XElement publishInfoElement)
            {
                XElement? bookName = publishInfoElement.Elements(FictionBook.DefaultNamespace + TagBookName).GetSingleValueOrNull(Logger);
                if (bookName != null)
                {
                    BookName = new TextFieldType(TagBookName);
                    BookName.Parse(bookName);
                }

                XElement? publisher = publishInfoElement.Elements(FictionBook.DefaultNamespace + TagPublisher).GetSingleValueOrNull(Logger);
                if (publisher != null)
                {
                    Publisher = new TextFieldType(TagPublisher);
                    Publisher.Parse(publisher);
                }

                XElement? city = publishInfoElement.Elements(FictionBook.DefaultNamespace + TagCity).GetSingleValueOrNull(Logger);
                if (city != null)
                {
                    City = new TextFieldType(TagCity);
                    City.Parse(city);
                }

                XElement? year = publishInfoElement.Elements(FictionBook.DefaultNamespace + TagYear).GetSingleValueOrNull(Logger);
                if (year != null)
                {
                    Year = year.Value;
                }

                XElement? isbn = publishInfoElement.Elements(FictionBook.DefaultNamespace + TagIsbn).GetSingleValueOrNull(Logger);
                if (isbn != null)
                {
                    Isbn = new TextFieldType(TagIsbn);
                    Isbn.Parse(isbn);
                }

                foreach (XElement? item in publishInfoElement.Elements(FictionBook.DefaultNamespace + TagSequence))
                {
                    SequenceElement sequence = new SequenceElement();
                    sequence.Parse(item);
                    Sequences.Add(sequence);
                }
            }
        }
        public XNode ToXml()
        {
            XElement toReturn = new XElement(FictionBook.DefaultNamespace + DescriptionElement.TagPublishInfo);

            toReturn.AddOptionalTag(BookName);
            toReturn.AddOptionalTag(Publisher);
            toReturn.AddOptionalTag(City);
            toReturn.AddOptionalStringTag(FictionBook.DefaultNamespace + TagYear, Year);
            toReturn.AddOptionalTag(Isbn);
            toReturn.AddOptionalListToTag(Sequences);

            return toReturn;
        }
    }
}
