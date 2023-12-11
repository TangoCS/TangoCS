using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.AccessControl
{
	public interface IObjectChangeRequestManager<T>
	{
		bool IsEnabled(Type entity);
		bool IsCurrentUserModerator();
		void Save(string action, ObjectChangeRequestData<T> data, string comments);
		ObjectChangeRequestData<T> Load(string ochid);
	}

	public class ObjectChangeRequestData<T>
	{
		public List<string> ChangedFields { get; set; }
		public T Object { get; set; }
	}

	public enum ObjectChangeRequestStatus
	{
		New = 1,
		Approved = 2,
		Rejected = 3
	}
}
