using System;

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

        public override string ToString()
        {



            return base.ToString();
        }
    }
}
