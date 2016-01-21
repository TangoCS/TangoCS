using System;
using System.Linq.Expressions;

namespace Nephrite.Multilanguage
{
	public interface IMultilingual
	{
		string LanguageCode { get; set; }
	}

	public interface IMultilingual<T, TKey> : IMultilingual
	{
		Expression<Func<T, bool>> ObjectDataSelector(TKey id);
	}
}
