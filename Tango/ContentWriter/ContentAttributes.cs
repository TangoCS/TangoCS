using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango
{
	public interface IContentItemAttributes<T>
		where T : IContentItemAttributes<T>
	{
		void SetWriter(IContentWriter writer);

		T ID<TValue>(TValue value);
		T Class(string value, bool replaceExisting = false);
		T Style(string value, bool replaceExisting = false);
		T Extended<TValue>(string key, TValue value);
		T Title(string value);
	}

	public interface IContentItemAttributes : IContentItemAttributes<IContentItemAttributes>
	{

	}

	public interface ITrAttributes : IContentItemAttributes<ITrAttributes>
	{
		
	}

	public interface ITdAttributes : IContentItemAttributes<ITdAttributes>
	{
		ITdAttributes ColSpan(int value);
		ITdAttributes RowSpan(int value);
	}

	public interface IThAttributes : IContentItemAttributes<IThAttributes>
	{
		IThAttributes ColSpan(int value);
		IThAttributes RowSpan(int value);
	}
}
