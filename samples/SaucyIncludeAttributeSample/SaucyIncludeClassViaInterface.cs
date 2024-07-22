namespace SaucyIncludeAttributeSample;

[SaucyInclude(ServiceScope.Transient)]
public class SaucyIncludeClassViaInterface : ISaucyIncludeInterface
{
}