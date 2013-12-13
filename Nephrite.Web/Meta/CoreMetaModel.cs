using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Meta
{

	





	public interface IFeatureInfo
	{
		Guid ID { get; }
		string Name { get; }
		string Description { get; }
		string Version { get; }

		List<Guid> Dependencies { get; }
	}

	public interface IFeatureBootstrap
	{
		//void Do();
	}

	[global::System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class FeatureBootstrapAttribute : Attribute
	{

	}
}