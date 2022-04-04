
namespace Fb2Parser.Errors
{
    public class NeedlessTagError : IFb2Error
    {
        public NeedlessTagError(string tagName, string needlessTagName)
        {
            TagName = tagName;
            NeedlessTagName = needlessTagName;
        }
        public string TagName { get; }
        public string NeedlessTagName { get; }
        public override string ToString()
        {
            return $"The tag '{TagName}' should not contain '{NeedlessTagName}' tag";
        }
    }
}
