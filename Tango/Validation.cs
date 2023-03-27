using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tango.Localization;

namespace Tango
{
	public class ValidationMessage
	{
		public string Group { get; set; }
		public string Name { get; set; }
		public ValidationMessageSeverity Severity { get; private set; }
		public string Message { get; private set; }

		public ValidationMessage(string group, string name, string message, ValidationMessageSeverity severity = ValidationMessageSeverity.Error)
		{
			Group = group;
			Name = name;
			Message = message;
			Severity = severity;
		}

		//public ValidationMessage(string message, ValidationMessageSeverity severity = ValidationMessageSeverity.Error)
		//{
		//	Message = message;
		//	Severity = severity;
		//}
	}

	public class ValidationMessageCollection : ObservableCollection<ValidationMessage>
	{
		public const string DEF_GROUP = "entitycheck";

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
			if (e.NewItems != null)
				foreach (ValidationMessage m in e.NewItems)
					_messagesCount[m.Severity]++;

			if (e.OldItems != null)
				foreach (ValidationMessage m in e.OldItems)
					_messagesCount[m.Severity]--;
		}

		public void AddRange(IEnumerable<ValidationMessage> collection)
		{
			foreach (var i in collection)
				Add(i);
        }

		public void Add(string group, string name, string message, ValidationMessageSeverity severity = ValidationMessageSeverity.Error)
		{
			Add(new ValidationMessage(group, name, message, severity));
		}

		public void Add(string name, string message, ValidationMessageSeverity severity = ValidationMessageSeverity.Error)
		{
			Add(new ValidationMessage(DEF_GROUP, name, message, severity));
		}

		//public void Add(string message, ValidationMessageSeverity severity = ValidationMessageSeverity.Error)
		//{
		//	Add(new ValidationMessage(message, severity));
		//}

		public bool HasItems(params ValidationMessageSeverity[] types)
		{
			if (types == null || types.Length == 0) return Count > 0;

			foreach (var s in types)
				if (_messagesCount[s] > 0) return true;

			return false;
		}

		public ValidationBuilder<T> Check<T>(IResourceManager resources, string id, string name, T value)
		{
			return new ValidationBuilder<T>(resources, this, id, name, value);
		}
	}

	public class ValidationBuilder<T>
	{
		public string ElementID { get; private set; }
		public string ElementName { get; set; }
		public T Value { get; private set; }

		public ValidationMessageCollection Collection { get; private set; }
		public IResourceManager Resources { get; private set; }

		public ValidationBuilder(IResourceManager resources, ValidationMessageCollection c, string id, string name, T value)
		{
			Collection = c;
			ElementID = id;
			ElementName = name;
			Value = value;
			Resources = resources;
		}

		public string GetMesssageText(string msgType, string data = null)
		{
			return string.Format(Resources.Get($"common.validation.{msgType}"), ElementName, data);
		}

		public void AddMessage(string msgType, string data = null, ValidationMessageSeverity severity = ValidationMessageSeverity.Error)
		{
			Collection.Add(ValidationMessageCollection.DEF_GROUP, ElementID, GetMesssageText(msgType, data), severity);
		}
	}

	public enum ValidationMessageSeverity
	{
		Error,
		Warning,
		Information
	}
}
