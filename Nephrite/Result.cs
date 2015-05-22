using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Nephrite
{
	public class Result
	{
		public Result(int code, string message)
		{
			Code = code;
			Message = message;
		}

		public int Code { get; private set; }
		public string Message { get; private set; }
	}

	public class BoolResult
	{
		static BoolResult _true = new BoolResult(true);
		public static BoolResult True
		{
			get { return _true; }
		}

		public BoolResult(bool value, string message = "")
		{
			Value = value;
			Message = message;
		}

		public bool Value { get; private set; }
		public string Message { get; private set; }
	}

	public class ValidationMessage
	{
		public string Name { get; set; }
		public ValidationMessageSeverity Severity { get; private set; }
		public string Message { get; private set; }

		public ValidationMessage(string name, string message, ValidationMessageSeverity severity = ValidationMessageSeverity.Error)
		{
			Name = name;
			Message = message;
			Severity = severity;
		}

		public ValidationMessage(string message, ValidationMessageSeverity severity = ValidationMessageSeverity.Error)
		{
			Message = message;
			Severity = severity;
		}
	}

	public class ValidationMessageCollection : ObservableCollection<ValidationMessage>
	{
		Dictionary<ValidationMessageSeverity, int> _messagesCount = new Dictionary<ValidationMessageSeverity, int>();

		public ValidationMessageCollection()
		{
			CollectionChanged += OnCollectionChanged;
			foreach (ValidationMessageSeverity s in Enum.GetValues(typeof(ValidationMessageSeverity)))
			{
				_messagesCount.Add(s, 0);
			}
		}

		void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			foreach (ValidationMessage m in e.NewItems)
				_messagesCount[m.Severity]++;

			foreach (ValidationMessage m in e.OldItems)
				_messagesCount[m.Severity]--;
		}

		public void Add(string name, string message, ValidationMessageSeverity severity = ValidationMessageSeverity.Error)
		{
			Add(new ValidationMessage(name, message, severity));
		}

		public void Add(string message, ValidationMessageSeverity severity = ValidationMessageSeverity.Error)
		{
			Add(new ValidationMessage(message, severity));
		}

		public bool HasItems(params ValidationMessageSeverity[] types)
		{
			if (types == null) return Count > 0;

			foreach (var s in types)
				if (_messagesCount[s] > 0) return true;

			return false;
		}
	}

	public enum ValidationMessageSeverity
	{
		Error,
		Warning,
		Information
	}
}