namespace Tango.Meta.Database
{
	public class SSequenceIdentity : MetaStereotype
	{
	}

	public static class SequenceExtensions
	{
		public static void SequenceIdentity<TClass, TValue>(this MetaAttribute<TClass, TValue> attribute, string seqName)
		{
			attribute.IsIdentity = true;
			attribute.AssignStereotype(new SSequenceIdentity { Name = seqName });
		}
	}
}
