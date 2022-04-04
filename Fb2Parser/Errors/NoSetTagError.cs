
namespace Fb2Parser.Errors
{
    public class NoSetTagError : IFb2Error
    {
        public NoSetTagError(string tagName)
        {
            TagName = tagName;
        }
        public string TagName { get; }
        public override string ToString()
        {
            return $"There is no suitable child set found for '{TagName}' tag";
        }
    }
}
