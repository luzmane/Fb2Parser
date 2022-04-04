
namespace Fb2Parser.Errors
{
    public class CountTimesTagError : IFb2Error
    {
        public CountTimesTagError(string tagName, int times)
        {
            TagName = tagName;
            Times = times;
        }
        public string TagName { get; }
        public int Times { get; }
        public override string ToString()
        {
            return $"The '{TagName}' tag should present only {Times} times";
        }
    }
}
