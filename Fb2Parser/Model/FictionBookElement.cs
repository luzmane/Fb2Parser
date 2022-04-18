using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;
using Fb2Parser.Errors;
using Fb2Parser.Utils;
using NLog;

namespace Fb2Parser.Model
{
    /// <summary>
    /// Root element
    /// </summary>
    public class FictionBook
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        internal static readonly AsyncLocal<List<IFb2Error>> _parsingErrors = new AsyncLocal<List<IFb2Error>>();
        internal static readonly AsyncLocal<List<string>> _usedImages = new AsyncLocal<List<string>>();
        /// <summary>
        /// Contains parsing and fb2 book generation errors. 
        /// Full errors list only after execution of ToXml() method
        /// </summary>
        public List<IFb2Error> ParsingErrors { get; private set; } = new List<IFb2Error>(0);
        internal static AsyncLocal<bool> _fixMandatoryTags = new AsyncLocal<bool>();
        internal static AsyncLocal<List<string>> _binariesIds = new AsyncLocal<List<string>>();
        internal static AsyncLocal<bool> _removeNotExistingImageLinks = new AsyncLocal<bool>();

        public const string TagStylesheet = "stylesheet";
        public const string TagDescription = "description";
        public const string TagBody = "body";
        public const string TagBinary = "binary";
        public const string TagFictionBook = "FictionBook";

        public static XNamespace DefaultNamespace { set; get; } = XNamespace.None;
        public static XNamespace XlinkNamespace { set; get; } = XNamespace.None;
        public static string BookEncoding { set; get; } = "utf-8";

        /// <summary>
        /// This element contains an arbitrary stylesheet that is intepreted by a 
        /// some processing programs, e.g. text/css stylesheets can be used by XSLT 
        /// stylesheets to generate better looking html
        /// </summary>
        public List<StylesheetElement> Stylesheets { get; private set; } = new List<StylesheetElement>();
        public DescriptionElement? Description { get; set; }
        public List<BodyElement> Bodies { get; private set; } = new List<BodyElement>();
        public List<BinaryElement> Binaries { get; private set; } = new List<BinaryElement>();

        public FictionBook Parse(XDocument document, Fb2ParsingSettings? settings)
        {
            Logger.Debug("Start parsing file");
            if (settings is null)
            {
                Logger.Info("Using default parse settings");
                settings = new Fb2ParsingSettings();
            }

            Logger.Debug($"Load book description only is {settings.LoadBookDescriptionOnly}");

            _parsingErrors.Value = new List<IFb2Error>();
            _usedImages.Value = new List<string>();
            _binariesIds.Value = new List<string>();
            _fixMandatoryTags.Value = settings.AddMissingMandatoryTags;
            _removeNotExistingImageLinks.Value = settings.RemoveImagesNotInBinaries;

            BookEncoding = document.Declaration.Encoding;

            XElement fictionBook = document.Root!;

            LoadNamespaces(fictionBook);
            LoadStylesheet(fictionBook);
            LoadDescription(fictionBook);
            if (!settings.LoadBookDescriptionOnly)
            {
                LoadBodies(fictionBook);
            }
            LoadBinaries(fictionBook);
            CheckAllBinariesHasLinks();

            ParsingErrors.AddRange(_parsingErrors.Value);

            Logger.Debug("End parsing file");

            return this;
        }
        public XDocument ToXml()
        {
            Logger.Debug("Start xml generation");

            _parsingErrors.Value = new List<IFb2Error>();

            XDocument toReturn = new XDocument(new XDeclaration("1.0", BookEncoding, null));
            XElement fictionBook = new XElement(DefaultNamespace + "FictionBook",
                        new XAttribute("xmlns", DefaultNamespace),
                        new XAttribute(XNamespace.Xmlns + "l", @"http://www.w3.org/1999/xlink"));
            toReturn.Add(fictionBook);

            fictionBook.AddOptionalListToTag(Stylesheets);
            fictionBook.AddRequiredTag(Description, Logger, TagDescription, typeof(DescriptionElement));
            AddBodiesToXml(fictionBook);
            fictionBook.AddOptionalListToTag(Binaries);

            ParsingErrors.AddRange(_parsingErrors.Value);

            Logger.Debug("End xml generation");

            return toReturn;
        }

        private void CheckAllBinariesHasLinks()
        {
            _binariesIds.Value.AddRange(Binaries.Where(bin => bin.Id != null).Select(binary => binary.Id!).ToList());
            List<string> imageLinks = _usedImages.Value;
            imageLinks.Except(_binariesIds.Value)
                      .ToList()
                      .ForEach(image => Logger.Warn(
                        $"Image with id '{image}' doesn't have appropriate image in '{TagBinary}' tags"
                            + (FictionBook._removeNotExistingImageLinks.Value ? ". Remove from the book" : "")));
            _binariesIds.Value.Except(imageLinks)
                            .ToList()
                            .ForEach(binary => Logger.Warn($"'{TagBinary}' has image with id '{binary}' that doesn't used in the book"));
        }
        private static void LoadNamespaces(XElement fictionBook)
        {
            DefaultNamespace = fictionBook.GetDefaultNamespace() ?? XNamespace.None;

            XPathNavigator navigator = fictionBook.CreateNavigator();
            _ = navigator.MoveToFollowing(XPathNodeType.Element);
            KeyValuePair<string, string> xlinkNamespacePair = navigator.GetNamespacesInScope(XmlNamespaceScope.All).FirstOrDefault(pair => pair.Value.Contains("xlink"));
            XlinkNamespace = fictionBook.GetNamespaceOfPrefix(xlinkNamespacePair.Key ?? "invalid") ?? XNamespace.None;
        }
        private void LoadStylesheet(XElement fictionBook)
        {
            foreach (XElement? item in fictionBook.Elements(DefaultNamespace + TagStylesheet))
            {
                StylesheetElement stylesheet = new StylesheetElement();
                stylesheet.Parse(item);
                Stylesheets.Add(stylesheet);
            }
        }
        private void LoadDescription(XElement fictionBook)
        {
            XElement? description = fictionBook.Elements(DefaultNamespace + TagDescription).GetSingleValueOrNull(Logger);
            if (description != null)
            {
                Description = new DescriptionElement();
                Description.Parse(description);
            }
        }
        private void LoadBodies(XElement fictionBook)
        {
            foreach (XElement? item in fictionBook.Elements(DefaultNamespace + TagBody))
            {
                BodyElement body = new BodyElement();
                body.Parse(item);
                Bodies.Add(body);
            }
        }
        private void LoadBinaries(XElement fictionBook)
        {
            foreach (XElement? item in fictionBook.Elements(DefaultNamespace + TagBinary))
            {
                BinaryElement binary = new BinaryElement();
                binary.Parse(item);
                Binaries.Add(binary);
            }
        }
        private void AddBodiesToXml(XElement fictionBook)
        {
            if (Bodies.Count == 0)
            {
                Logger.Error($"The book doesn't have '{FictionBook.TagBody}' tag");
                FictionBook._parsingErrors.Value.Add(new RequiredTagError(FictionBook.TagFictionBook, FictionBook.TagBody));
            }
            else if (Bodies.Count == 1)
            {
                fictionBook.Add(Bodies[0].ToXml());
            }
            else if (Bodies.Count == 2)
            {
                BodyElement first = Bodies[0];
                BodyElement second = Bodies[1];

                if (string.IsNullOrEmpty(first.Name))
                {
                    fictionBook.Add(first.ToXml());
                    fictionBook.Add(second.ToXml());
                }
                else if (string.IsNullOrEmpty(second.Name))
                {
                    Logger.Warn($"Incorrect order of tags '{FictionBook.TagBody}'");
                    fictionBook.Add(second.ToXml());
                    fictionBook.Add(first.ToXml());
                }
                else
                {
                    Logger.Warn($"Both tags '{FictionBook.TagBody}' has '{BodyElement.AttributeName}' attribute, removing attribute '{BodyElement.AttributeName}' from the first tag");
                    first.Name = null;
                    fictionBook.Add(first.ToXml());
                    fictionBook.Add(second.ToXml());
                }
            }
            else
            {
                BodyElement first = Bodies[0];
                if (string.IsNullOrEmpty(first.Name))
                {
                    Logger.Warn($"The first tags '{FictionBook.TagBody}' has '{BodyElement.AttributeName}' attribute, removing it");
                    first.Name = null;
                }
                Logger.Warn($"The book has too much '{FictionBook.TagBody}' tags");
                fictionBook.AddRequiredListToTag(Bodies, Logger, TagBody, typeof(BodyElement));
            }
        }
    }
}
