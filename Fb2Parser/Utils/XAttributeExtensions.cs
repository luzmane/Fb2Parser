
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Fb2Parser.Errors;
using Fb2Parser.Model;
using NLog;

namespace Fb2Parser.Utils
{
    public static class XAttributeExtensions
    {
        public static string? GetSingleValueOrNull(this IEnumerable<XAttribute> enumerable, ILogger Logger)
        {
            if (enumerable.Count() > 0)
            {
                XAttribute toReturn = enumerable.First();
                if (enumerable.Count() > 1)
                {
                    Logger.Error($"The tag '{toReturn.Parent.Name.LocalName}' has more than one attribute '{toReturn.Name.LocalName}', ignoring rest");
                    FictionBook._parsingErrors.Value.Add(new IncorrectAttributeNumberError(toReturn.Parent.Name.LocalName, toReturn.Name.LocalName));
                }
                return toReturn.Value;
            }
            return null;
        }
    }
}
