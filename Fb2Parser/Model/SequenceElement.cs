using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Fb2Parser.Errors;
using Fb2Parser.Utils;
using NLog;

namespace Fb2Parser.Model
{
    /// <summary>
    /// Book sequences
    /// </summary>
    public class SequenceElement : IFb2Element
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public const string TagSequence = "sequence";
        public const string AttributeName = "name";
        public const string AttributeNumber = "number";
        public const string AttributeLang = "lang";

        public string? Name { get; set; }
        public string? Lang { set; get; }
        public string? Number { get; set; }
        public List<SequenceElement> Sequences { get; private set; } = new List<SequenceElement>();

        public void Parse(XNode node)
        {
            if (node is XElement sequenceElement)
            {
                string? number = sequenceElement.Attributes(AttributeNumber).GetSingleValueOrNull(Logger);
                if (number != null)
                {
                    if (float.TryParse(number, out _))
                    {
                        // verify if 'number' is valid number, but save the original format
                        Number = number;
                    }
                    else
                    {
                        Logger.Error($"'{AttributeNumber}' tag is not valid number: {number}. Skipping");
                        FictionBook._parsingErrors.Value.Add(new IllegalAttributeError(TitleInfoElement.TagSequence, AttributeNumber, number));
                    }
                }

                Name = sequenceElement.Attributes(AttributeName).GetSingleValueOrNull(Logger);
                Lang = sequenceElement.Attributes(XNamespace.Xml + AttributeLang).GetSingleValueOrNull(Logger);

                foreach (XElement? item in sequenceElement.Elements(FictionBook.DefaultNamespace + TagSequence))
                {
                    SequenceElement sequence = new SequenceElement();
                    sequence.Parse(item);
                    Sequences.Add(sequence);
                }
            }
        }
        public XNode ToXml()
        {
            XElement toReturn = new XElement(FictionBook.DefaultNamespace + TitleInfoElement.TagSequence);

            toReturn.AddOptionalAttribute(AttributeName, Name);
            toReturn.AddOptionalAttribute(AttributeNumber, Number);
            toReturn.AddOptionalAttribute(XNamespace.Xml + AttributeLang, Lang);
            toReturn.AddOptionalListToTag(Sequences);

            return toReturn;
        }
    }
}
