using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Fb2Parser.Model;

namespace Fb2Parser
{
    public class Fb2File
    {
        private static readonly Regex _encodingRegex = new Regex("\\<\\?xml.*encoding=(?:'|\")(?<encoding>[a-zA-Z0-9-]+)(?:'|\").*\\?\\>", RegexOptions.IgnoreCase);

        /// <summary>
        /// Parse fb2 file
        /// </summary>
        /// <param name="bookStream">Fb2 file as a stream</param>
        /// <param name="settings">Parsing settings</param>
        /// <returns>FictionBook object</returns>
        public static FictionBook Parse(StreamReader bookStream, Fb2ParsingSettings? settings = null)
        {
            settings ??= new Fb2ParsingSettings();
            LoadOptions preserveWhitespace = settings.PreserveWhitespace ? LoadOptions.PreserveWhitespace : LoadOptions.None;
            return new FictionBook().Parse(XDocument.Load(bookStream, preserveWhitespace), settings);
        }

        /// <summary>
        /// Parse fb2 file
        /// </summary>
        /// <param name="bookStream">Fb2 file as a stream</param>
        /// <param name="settings">Parsing settings</param>
        /// <returns>FictionBook object</returns>
        public static Task<FictionBook> ParseAsync(StreamReader bookStream, Fb2ParsingSettings? settings = null)
        {
            return Task.Factory.StartNew(() => Parse(bookStream, settings));
        }
        public static FictionBook Parse(string filePath, Fb2ParsingSettings? settings = null)
        {
            settings ??= new Fb2ParsingSettings();
            settings.Encoding ??= DetectEncoding(filePath);
            using StreamReader bookStream = new StreamReader(filePath, Encoding.GetEncoding(settings.Encoding));
            return Parse(bookStream, settings);
        }
        public static Task<FictionBook> ParseAsync(string filePath, Fb2ParsingSettings? settings = null)
        {
            return Task.Factory.StartNew(() => Parse(filePath, settings));
        }
        private static string DetectEncoding(string filePath)
        {
            string? encoding = File.ReadAllLines(filePath)
                               .Where(line => !string.IsNullOrWhiteSpace(line))
                               .Where(line => line.Trim().StartsWith("<?xml", StringComparison.Ordinal))
                               .SelectMany(line => _encodingRegex.Matches(line))
                               .Select(match => match.Groups["encoding"].Value)
                               .FirstOrDefault();
            return encoding is null ? "utf-8" : encoding;
        }
    }
}
