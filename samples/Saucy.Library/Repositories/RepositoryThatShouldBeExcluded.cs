using Saucy.Common.Attributes;

namespace Saucy.Library.Repositories;

[WhenRegisteringWithContainerShouldExcluded]
public class RepositoryThatShouldBeExcluded : IRepository
{
	public void Get()
	{
		throw new NotImplementedException();
	}
}
