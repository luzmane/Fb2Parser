using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Fb2Parser.Errors;
using Fb2Parser.Model;
using NUnit.Framework;

namespace Fb2Parser.Tests
{
    [TestFixture]
    public class IncorrectBooksTests
    {
        private FictionBook? _fictionBook;


        [OneTimeSetUp]
        public void FictionBookSetUp()
        {
            _fictionBook = Fb2File.Parse(File.OpenText(@"demo/incorrect.fb2"), new Fb2ParsingSettings()
            {
                LoadBookDescriptionOnly = true
            });
            _ = _fictionBook.ToXml();
        }

        [Test]
        public void VerifyNoGenre()
        {
            if (_fictionBook is null)
            {
                Assert.Fail("_fictionBook field is null");
            }
            else if (_fictionBook.ParsingErrors.Any(error =>
                error is RequiredElementInListError tag
                && string.Equals(tag.TagName, TitleInfoElement.TagGenre, System.StringComparison.Ordinal)))
            {
                Assert.Pass("Missing of 'genre' tag was detected");
            }
            else
            {
                Assert.Fail("The 'genre' tag is missing, but was not detected");
            }
        }
    }
}
