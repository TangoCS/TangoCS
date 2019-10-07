using NHibernate.Event;

namespace Abc
{
	public interface IFileStorageListener : IPostUpdateEventListener, IPostInsertEventListener
	{

	}
}
