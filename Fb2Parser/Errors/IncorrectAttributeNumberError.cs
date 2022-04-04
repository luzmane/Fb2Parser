
namespace Fb2Parser.Errors
{
    public class IncorrectAttributeNumberError : IFb2Error
    {
        public IncorrectAttributeNumberError(string tagName, string attributeName)
        {
            TagName = tagName;
            AttributeName = attributeName;
        }
        public string TagName { get; }
        public string AttributeName { get; }
        public override string ToString()
        {
            return $"The tag '{TagName}' has more than one attribute '{AttributeName}'";
        }
    }
}

