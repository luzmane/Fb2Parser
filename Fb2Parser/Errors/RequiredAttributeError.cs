
namespace Fb2Parser.Errors
{
    public class RequiredAttributeError : IFb2Error
    {
        public RequiredAttributeError(string parentTagName, string attributeName)
        {
            ParentTagName = parentTagName;
            AttributeName = attributeName;
        }
        public string ParentTagName { get; }
        public string AttributeName { get; }
        public override string ToString()
        {
            return $"Attribute '{AttributeName}' of tag '{ParentTagName}' is required by standard";
        }
    }
}
