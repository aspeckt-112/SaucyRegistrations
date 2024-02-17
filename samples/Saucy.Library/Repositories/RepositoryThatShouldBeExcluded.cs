using Saucy.Common.Attributes;

namespace Saucy.Library.Repositories;

[ExcludeRegistration]
public class RepositoryThatShouldBeExcluded : IRepository
{
	public void Get()
	{
		throw new NotImplementedException();
	}
}
