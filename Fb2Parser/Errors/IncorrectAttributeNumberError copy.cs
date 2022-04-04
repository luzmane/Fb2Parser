
namespace Fb2Parser.Errors
{
    public class IncorrectTagNumberError : IFb2Error
    {
        public IncorrectTagNumberError(string parentTagName, string tagName)
        {
            ParentTagName = parentTagName;
            TagName = tagName;
        }
        public string ParentTagName { get; }
        public string TagName { get; }
        public override string ToString()
        {
            return $"The tag '{ParentTagName}' has more than one child tag '{TagName}'";
        }
    }
}

