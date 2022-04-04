using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Fb2Parser.Model;

namespace Fb2Parser
{
    public class Fb2File
    {
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
    }
}
