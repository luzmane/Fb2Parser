
namespace Fb2Parser.Errors
{
    public class IllegalTagError : IFb2Error
    {
        public IllegalTagError(string tagName, string incorrectValue)
        {
            TagName = tagName;
            IncorrectValue = incorrectValue;
        }
        public string TagName { get; }
        public string IncorrectValue { get; }
        public override string ToString()
        {
            return $"The value of tag '{TagName}' is incorrect: {IncorrectValue}";
        }
    }
}
