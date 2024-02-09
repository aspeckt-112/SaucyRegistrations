using Saucy.Common.Attributes;
using Saucy.Common.Enums;

namespace Saucy.Library.Repositories;

[WhenRegisteringUseScope(ServiceScope.Scoped)]
public class ScopedScopedRepository : IRepository
{
	public void Get()
	{
		throw new NotImplementedException();
	}
}
