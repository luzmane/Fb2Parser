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
        /// <param name="loadBookDescriptionOnly">Load all parts except 'body' tags flag</param>
        /// <returns>FictionBook object</returns>
        public static FictionBook Parse(StreamReader bookStream, bool loadBookDescriptionOnly = false)
        {
            return new FictionBook().Parse(XDocument.Load(bookStream), loadBookDescriptionOnly);
        }

        /// <summary>
        /// Parse fb2 file
        /// </summary>
        /// <param name="bookStream">Fb2 file as a stream</param>
        /// <param name="loadBookDescriptionOnly">Load all parts except 'body' tags flag</param>
        /// <returns>FictionBook object</returns>
        public static Task<FictionBook> ParseAsync(StreamReader bookStream, bool loadBookDescriptionOnly = false)
        {
            return Task.Factory.StartNew(() => Parse(bookStream, loadBookDescriptionOnly));
        }
        public static FictionBook Parse(string filePath, string? encoding = null, bool loadBookDescriptionOnly = false)
        {
            if (encoding is null)
            {
                encoding = DetectEncoding(filePath);
            }
            using StreamReader bookStream = new StreamReader(filePath, Encoding.GetEncoding(encoding));
            return new FictionBook().Parse(XDocument.Load(bookStream), loadBookDescriptionOnly);
        }
        public static Task<FictionBook> ParseAsync(string filePath, string? encoding = null, bool loadBookDescriptionOnly = false)
        {
            return Task.Factory.StartNew(() => Parse(filePath, encoding, loadBookDescriptionOnly));
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
