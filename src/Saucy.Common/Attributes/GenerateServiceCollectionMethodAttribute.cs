namespace Saucy.Common.Attributes
{
    [System.AttributeUsageAttribute(System.AttributeTargets.Class)]
    public class GenerateServiceCollectionMethodAttribute : System.Attribute
    {
        public string MethodName { get; }

        public GenerateServiceCollectionMethodAttribute(string methodName)
        {
            MethodName = methodName;
        }
    }
}