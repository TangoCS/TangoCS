namespace Nephrite.AccessControl
{
	public interface IPredicateChecker
	{
		BoolResult Check(string securableObjectKey, object predicateContext);
	}
}
