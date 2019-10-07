using System.Runtime;
using System.Xml;

namespace System.ServiceModel
{
	public static partial class DiagnosticUtility
	{
		private const string TraceSourceName = "TraceSourceNameToReplace";
		internal const string EventSourceName = TraceSourceName + " [COR_BUILD_MAJOR].[COR_BUILD_MINOR].[CLR_OFFICIAL_ASSEMBLY_NUMBER].0";
		internal const string DefaultTraceListenerName = "Default";

		private static bool s_shouldUseActivity = false;

		private static object s_lockObject = new object();

		private static ExceptionUtility s_exceptionUtility = null;

		private static void UpdateLevel()
		{
#pragma warning disable 618

#pragma warning restore 618
		}

		public static ExceptionUtility ExceptionUtility {
			get {
				return DiagnosticUtility.s_exceptionUtility ?? GetExceptionUtility();
			}
		}

		private static ExceptionUtility GetExceptionUtility()
		{
			lock (DiagnosticUtility.s_lockObject)
			{
				if (DiagnosticUtility.s_exceptionUtility == null)
				{
#pragma warning disable 618
					DiagnosticUtility.s_exceptionUtility = new ExceptionUtility(DiagnosticUtility.TraceSourceName, DiagnosticUtility.EventSourceName, FxTrace.Exception);
#pragma warning restore 618
				}
			}
			return DiagnosticUtility.s_exceptionUtility;
		}

		static internal bool ShouldUseActivity {
			get { return DiagnosticUtility.s_shouldUseActivity; }
		}


//		[Conditional("DEBUG")]
//		internal static void DebugAssert(bool condition, string message)
//		{
//			if (!condition)
//			{
//				DebugAssert(message);
//			}
//		}

//		[MethodImpl(MethodImplOptions.NoInlining)]
//		[Conditional("DEBUG")]
//		internal static void DebugAssert(string message)
//		{
//#pragma warning disable 618
//			Fx.Assert(message);
//#pragma warning restore 618
//		}
	}


	public class ExceptionUtility
	{
		private const string ExceptionStackAsStringKey = "System.ServiceModel.Diagnostics.ExceptionUtility.ExceptionStackAsString";

#pragma warning disable 649
		// This field should be only used for debug build.
		internal static ExceptionUtility mainInstance;
#pragma warning restore 649

		private ExceptionTrace _exceptionTrace;
		private string _name;
		private string _eventSourceName;

		[ThreadStatic]
		private static Guid s_activityId;

		[Obsolete("For SMDiagnostics.dll use only. Call DiagnosticUtility.ExceptionUtility instead")]
		internal ExceptionUtility(string name, string eventSourceName, object exceptionTrace)
		{
			_exceptionTrace = (ExceptionTrace)exceptionTrace;
			_name = name;
			_eventSourceName = eventSourceName;
		}


		public ArgumentException ThrowHelperArgument(string message)
		{
			return (ArgumentException)this.ThrowHelperError(new ArgumentException(message));
		}

		public ArgumentException ThrowHelperArgument(string paramName, string message)
		{
			return (ArgumentException)this.ThrowHelperError(new ArgumentException(message, paramName));
		}

		public ArgumentNullException ThrowHelperArgumentNull(string paramName)
		{
			return (ArgumentNullException)this.ThrowHelperError(new ArgumentNullException(paramName));
		}

		public ArgumentNullException ThrowHelperArgumentNull(string paramName, string message)
		{
			return (ArgumentNullException)this.ThrowHelperError(new ArgumentNullException(paramName, message));
		}

		public ArgumentException ThrowHelperArgumentNullOrEmptyString(string arg)
		{
			return (ArgumentException)this.ThrowHelperError(new ArgumentException(Res.S("StringNullOrEmpty"), arg));
		}

		public Exception ThrowHelperFatal(string message, Exception innerException)
		{
			return this.ThrowHelperError(new FatalException(message, innerException));
		}

		public Exception ThrowHelperInternal(bool fatal)
		{
			return fatal ? Fx.AssertAndThrowFatal("Fatal InternalException should never be thrown.") : Fx.AssertAndThrow("InternalException should never be thrown.");
		}

		public Exception ThrowHelperInvalidOperation(string message)
		{
			return ThrowHelperError(new InvalidOperationException(message));
		}

		public Exception ThrowHelperCallback(string message, Exception innerException)
		{
			return this.ThrowHelperCritical(new CallbackException(message, innerException));
		}

		public Exception ThrowHelperCallback(Exception innerException)
		{
			return this.ThrowHelperCallback(Res.S("GenericCallbackException"), innerException);
		}

		public Exception ThrowHelperCritical(Exception exception)
		{
			return exception;
		}

		public Exception ThrowHelperError(Exception exception)
		{
			return exception;
		}

		public Exception ThrowHelperWarning(Exception exception)
		{
			return exception;
		}

		internal Exception ThrowHelperXml(XmlReader reader, string message)
		{
			return this.ThrowHelperXml(reader, message, null);
		}

		internal Exception ThrowHelperXml(XmlReader reader, string message, Exception inner)
		{
			IXmlLineInfo lineInfo = reader as IXmlLineInfo;
			return this.ThrowHelperError(new XmlException(
				message,
				inner,
				(null != lineInfo) ? lineInfo.LineNumber : 0,
				(null != lineInfo) ? lineInfo.LinePosition : 0));
		}

		// On a single thread, these functions will complete just fine
		// and don't need to worry about locking issues because the effected
		// variables are ThreadStatic.
		internal static void UseActivityId(Guid activityId)
		{
			ExceptionUtility.s_activityId = activityId;
		}

		internal static void ClearActivityId()
		{
			ExceptionUtility.s_activityId = Guid.Empty;
		}
	}
}
