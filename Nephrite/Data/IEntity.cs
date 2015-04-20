using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Nephrite.Data
{
	public interface IEntity
	{
	}

	public interface IChildEntity : IEntity
	{
		string GetPath();
	}
}