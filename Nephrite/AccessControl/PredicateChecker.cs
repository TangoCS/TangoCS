using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nephrite.AccessControl
{
	public class PredicateChecker
	{
		static Dictionary<string, Func<PredicateEvaluationContext, bool>> _predicates = null;
		IPredicateLoader _predicateLoader { get; set; }
		AccessControlOptions _options;

		static PredicateChecker _instanceHolder;
		static object LockObject = new object();

		public static void Init(IPredicateLoader loader, AccessControlOptions options = null)
		{
			if (_instanceHolder == null)
			{
				lock (LockObject)
				{
					if (_instanceHolder == null)
					{
						_instanceHolder = new PredicateChecker(loader, options);
						return;
					}
				}
			}

			throw new ApplicationException("PredicateChecker.Init() method should be called only once.");
		}

		public static PredicateChecker Instance
		{
			get
			{
				if (_instanceHolder == null)
				{
					throw new ApplicationException("PredicateChecker instance hasn't been initialized.");
				}

				return _instanceHolder;
			}
		}

		PredicateChecker(IPredicateLoader loader, AccessControlOptions options = null)
		{
			_options = options ?? new AccessControlOptions { Enabled = () => true };
			_predicateLoader = loader;
		}

		public BoolResult Check(string securableObjectKey, object predicateContext)
		{
			if (!_options.Enabled()) return BoolResult.True;
			string key = securableObjectKey.ToUpper();
			if (_predicates == null)
			{
				_predicates = new Dictionary<string, Func<PredicateEvaluationContext, bool>>();
				_predicateLoader.Load(_predicates);
			}
			if (!_predicates.ContainsKey(key)) return BoolResult.True;
			var pec = new PredicateEvaluationContext { PredicateContext = predicateContext };
			var result = _predicates[key](pec);

			return new BoolResult(result, pec.Message);
		}
	}

	public interface IPredicateLoader
	{
		void Load(Dictionary<string, Func<PredicateEvaluationContext, bool>> list);
	}

	public class PredicateEvaluationContext
	{
		public object PredicateContext { get; set; }
		public string Message { get; set; }
	}
}
