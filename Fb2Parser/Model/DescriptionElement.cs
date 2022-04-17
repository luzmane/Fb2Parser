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
    public class DescriptionElement : IFb2Element
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public const string TagTitleInfo = "title-info";
        public const string TagSrcTitleInfo = "src-title-info";
        public const string TagDocumentInfo = "document-info";
        public const string TagPublishInfo = "publish-info";
        public const string TagCustomInfo = "custom-info";
        public const string TagOutput = "output";

        public TitleInfoElement? TitleInfo { set; get; }
        /// <summary>
        /// Generic information about the original book (for translations)
        /// </summary>
        public TitleInfoElement? SrcTitleInfo { set; get; }
        /// <summary>
        /// Information about this particular (xml) document
        /// </summary>
        public DocumentInfoElement? DocumentInfo { get; set; }
        /// <summary>
        /// Information about some paper/outher published document,
        /// that was used as a source of this xml document
        /// </summary>
        public PublishInfoElement? PublishInfo { get; set; }
        /// <summary>
        /// Any other information about the book/document that didnt fit in the above groups
        /// </summary>
        public List<CustomInfoElement> CustomInfos { get; private set; } = new List<CustomInfoElement>();
        /// <summary>
        /// Describes, how the document should be presented to end-user, what parts are free,
        /// what parts should be sold and what price should be used
        /// </summary>
        public List<OutputElement> Outputs { get; private set; } = new List<OutputElement>(2);

        public void Parse(XNode node)
        {
            if (node is XElement descriptionElement)
            {
                XElement? titleInfo = descriptionElement.Elements(FictionBook.DefaultNamespace + TagTitleInfo).GetSingleValueOrNull(Logger);
                if (titleInfo != null)
                {
                    TitleInfo = new TitleInfoElement(TagTitleInfo);
                    TitleInfo.Parse(titleInfo);
                }

                XElement? srcTitleInfo = descriptionElement.Elements(FictionBook.DefaultNamespace + TagSrcTitleInfo).GetSingleValueOrNull(Logger);
                if (srcTitleInfo != null)
                {
                    SrcTitleInfo = new TitleInfoElement(TagSrcTitleInfo);
                    SrcTitleInfo.Parse(srcTitleInfo);
                }

                XElement? documentInfo = descriptionElement.Elements(FictionBook.DefaultNamespace + TagDocumentInfo).GetSingleValueOrNull(Logger);
                if (documentInfo != null)
                {
                    DocumentInfo = new DocumentInfoElement();
                    DocumentInfo.Parse(documentInfo);
                }

                XElement? publishInfo = descriptionElement.Elements(FictionBook.DefaultNamespace + TagPublishInfo).GetSingleValueOrNull(Logger);
                if (publishInfo != null)
                {
                    PublishInfo = new PublishInfoElement();
                    PublishInfo.Parse(publishInfo);
                }

                foreach (XElement? item in descriptionElement.Elements(FictionBook.DefaultNamespace + TagCustomInfo))
                {
                    CustomInfoElement customInfo = new CustomInfoElement();
                    customInfo.Parse(item);
                    CustomInfos.Add(customInfo);
                }

                foreach (XElement? item in descriptionElement.Elements(FictionBook.DefaultNamespace + TagOutput))
                {
                    OutputElement output = new OutputElement();
                    output.Parse(item);
                    Outputs.Add(output);
                }
            }
        }
        public XNode ToXml()
        {
            XElement toReturn = new XElement(FictionBook.DefaultNamespace + FictionBook.TagDescription);

            toReturn.AddRequiredTag(TitleInfo, Logger, TagTitleInfo, typeof(TitleInfoElement));
            toReturn.AddOptionalTag(SrcTitleInfo);
            toReturn.AddRequiredTag(DocumentInfo, Logger, TagDocumentInfo, typeof(DocumentInfoElement));
            toReturn.AddOptionalTag(PublishInfo);
            toReturn.AddOptionalListToTag(CustomInfos);

            int cnt = 1;
            foreach (OutputElement output in Outputs)
            {
                if (cnt > 2)
                {
                    Logger.Error($"The '{TagOutput}' tag should present only 2 times, skipping the rest");
                    FictionBook._parsingErrors.Value.Add(new CountTimesTagError(TagOutput, 2));
                    break;
                }
                toReturn.Add(output.ToXml());
                cnt++;
            }

            return toReturn;
        }
    }
}
