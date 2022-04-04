namespace Fb2Parser.Errors
{
    public class RequiredContentError : IFb2Error
    {
        public RequiredContentError(string tagName)
        {
            TagName = tagName;
        }
        public string TagName { get; }
        public override string ToString()
        {
            return $"Tag '{TagName}' should have content";
        }
    }
}
