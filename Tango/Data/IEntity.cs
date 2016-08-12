using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Tango.Data
{
	public interface IEntity
	{
	}

	public interface IChildEntity : IEntity
	{
		string GetPath();
	}
}