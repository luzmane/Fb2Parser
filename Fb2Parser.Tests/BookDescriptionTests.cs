using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Fb2Parser.Model;
using NUnit.Framework;

namespace Fb2Parser.Tests
{
    /// <summary>
    /// Tests for 'stylesheet' and 'description'
    /// </summary>
    [TestFixture]
    public class BookDescriptionTests
    {
        private FictionBook? _fictionBook;
        private const string testFile = @"demo/description.fb2";

        [OneTimeSetUp]
        public void FictionBookSetUp()
        {
            _fictionBook = Fb2File.Parse(File.OpenText(testFile));
        }

        [Test]
        public void GenreIsSf()
        {
            Assert.AreEqual("sf",
                _fictionBook?.Description?.TitleInfo?.Genres[0].Content,
                "This book should have 'sf' genre");
        }

        [Test]
        public void AuthorLastNameIsTolstoy()
        {
            Assert.AreEqual("Толстой",
                _fictionBook?.Description?.TitleInfo?.Authors[0]?.LastName?.Content,
                "This book should have 'Толстой' as an author");
        }

        [Test]
        public void FileBeforeAndAfterAreEquals()
        {
            if (_fictionBook is null)
            {
                Assert.Fail("The fictionBook is null");
            }
            else
            {
                string parsedFilePath = testFile.Replace(".fb2", ".xml");
                XDocument toPrint = _fictionBook.ToXml();
                XmlWriter xmlWriter = XmlWriter.Create(parsedFilePath, new XmlWriterSettings()
                {
                    WriteEndDocumentOnClose = true,
                    Encoding = new UTF8Encoding(false),
                    Indent = true,
                    NewLineHandling = NewLineHandling.Entitize,
                    NewLineChars = "\n",
                    ConformanceLevel = ConformanceLevel.Document
                });
                toPrint.WriteTo(xmlWriter);
                xmlWriter.Flush();
                xmlWriter.Close();

                string originalFile = File.ReadAllText(testFile).Trim();
                string parsedFile = File.ReadAllText(parsedFilePath).Trim();
                Assert.AreEqual(originalFile, parsedFile, "Original and parsed files should be the same");
            }
        }

        [Test]
        public void CheckAuthor()
        {
            AuthorElement? author1 = _fictionBook?.Description?.TitleInfo?.Authors[0];
            Assert.IsNotNull(author1, "Unable to find an author");
            Assert.AreEqual("Лев", author1?.FirstName?.Content);
            Assert.AreEqual("Николаевич", author1?.MiddleName?.Content);
            Assert.AreEqual("Толстой", author1?.LastName?.Content);
            Assert.AreEqual("Лев Толстой", author1?.Nickname?.Content);
            Assert.IsTrue(author1?.Emails.All(email =>
                            "a@example.com".Equals(email, System.StringComparison.Ordinal)
                            || "b@test.com".Equals(email, System.StringComparison.Ordinal)),
                            "Email is not correct");
            Assert.IsTrue(author1?.HomePages.All(homePage =>
                            "http://example.com".Equals(homePage, System.StringComparison.Ordinal)
                            || "https://test.com".Equals(homePage, System.StringComparison.Ordinal)),
                            "Home page is not correct");
            Assert.AreEqual("1234-1234-1231", author1?.Id);

            AuthorElement? author2 = _fictionBook?.Description?.TitleInfo?.Authors[1];
            Assert.IsNotNull(author2, "Unable to find second author");
            Assert.AreEqual("Лёшка", author2?.Nickname?.Content);
            Assert.IsTrue(author2?.Emails.All(email =>
                            "тшсл@example.com".Equals(email, System.StringComparison.Ordinal)
                            || "фыва@test.com".Equals(email, System.StringComparison.Ordinal)),
                            "Email is not correct");
            Assert.IsTrue(author2?.HomePages.All(homePage =>
                            "http://example.com".Equals(homePage, System.StringComparison.Ordinal)
                            || "https://test.com".Equals(homePage, System.StringComparison.Ordinal)),
                            "Home page is not correct");
            Assert.AreEqual("4321-4321-1231", author2?.Id);
        }
    }
}
