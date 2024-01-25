namespace Saucy.Console.Services;

public class ClassThatDerivesFromTheAbstractBaseService : AbstractBaseService
{
	public override void WriteToConsole()
	{
		System.Console.WriteLine("Hello from ClassThatDerivesFromTheAbstractBaseService!");
	}
}
