
using NLog.LayoutRenderers.Wrappers;

namespace Fb2Parser.Errors
{
    public class RequiredElementInListError : IFb2Error
    {
        public RequiredElementInListError(string parentTagName, string? tagName = null)
        {
            ParentTagName = parentTagName;
            TagName = tagName;
        }
        public string ParentTagName { get; }
        public string? TagName { get; }
        public override string ToString()
        {
            return TagName is null ?
            $"Tag '{ParentTagName}' requires to have at least one child tag by standard" :
            $"Tag '{ParentTagName}' requires to have at least one '{TagName}' by standard";
        }
    }
}
