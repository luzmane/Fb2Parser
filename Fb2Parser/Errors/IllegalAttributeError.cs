
namespace Fb2Parser.Errors
{
    public class IllegalAttributeError : IFb2Error
    {
        public IllegalAttributeError(string tagName, string attributeName, string incorrectValue)
        {
            TagName = tagName;
            AttributeName = attributeName;
            IncorrectValue = incorrectValue;
        }
        public string TagName { get; }
        public string AttributeName { get; }
        public string IncorrectValue { get; }
        public override string ToString()
        {
            return $"'{AttributeName}' attribute of '{TagName}' tag is incorrect: '{IncorrectValue}'";
        }
    }
}
