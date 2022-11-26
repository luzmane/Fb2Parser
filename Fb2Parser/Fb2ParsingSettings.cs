namespace Fb2Parser
{
    public class Fb2ParsingSettings
    {
        /// <summary>
        /// Load all tags tree except body
        /// </summary>
        public bool LoadBookDescriptionOnly { get; set; } = false;
        /// <summary>
        /// Add missing mandatory tags
        /// </summary>
        public bool AddMissingMandatoryTags { get; set; } = true;
        /// <summary>
        /// Remove links on images that doesn't exist in binary tags
        /// </summary>
        public bool RemoveImagesNotInBinaries { get; set; } = true;
        /// <summary>
        /// File encoding
        /// </summary>
        public string? Encoding { get; set; } = null;

        /// <summary>
        /// Transparent parameter for <see cref="System.Xml.Linq.LoadOptions"/> 
        /// of <see cref="System.Xml.Linq.XDocument"/> to preserve whitespace during xml processing
        /// </summary>
        public bool PreserveWhitespace { get; set; } = true;

        public override string ToString()
        {



            return base.ToString();
        }
    }
}
