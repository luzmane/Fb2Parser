
namespace Fb2Parser.Errors
{
    public class RequiredTagError : IFb2Error
    {
        public RequiredTagError(string parentTagName, string tagName)
        {
            ParentTagName = parentTagName;
            TagName = tagName;
        }
        public string ParentTagName { get; }
        public string TagName { get; }
        public override string ToString()
        {
            return $"Tag '{ParentTagName}' requires to have '{TagName}' by standard";
        }
    }
}
