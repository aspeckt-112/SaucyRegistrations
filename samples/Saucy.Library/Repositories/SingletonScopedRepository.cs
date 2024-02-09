using Saucy.Common.Attributes;
using Saucy.Common.Enums;

namespace Saucy.Library.Repositories;

[WhenRegisteringUseScope(ServiceScope.Singleton)]
public class SingletonScopedRepository : IRepository
{
	public void Get()
	{
		throw new NotImplementedException();
	}
}
