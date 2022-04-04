
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Fb2Parser.Model;
using NLog;

namespace Fb2Parser.Errors
{
    public interface IFb2Error
    {
        string ToString();
    }
}
