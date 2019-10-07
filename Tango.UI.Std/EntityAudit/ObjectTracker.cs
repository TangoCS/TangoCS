using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using Tango.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tango.UI.Std.EntityAudit
{
	public class ObjectTracker : IObjectTracker
	{
		Dictionary<object, Dictionary<PropertyInfo, object>> objects = new Dictionary<object, Dictionary<PropertyInfo, object>>();
		public void StartTracking(object entity)// where T : IEntity, IWithKey<T, TKey>, IWithTitle, IWithPropertyAudit
		{
			if (entity == null)
				return;
			objects[entity] = GetProperties(entity.GetType()).ToDictionary(o => o, o => o.GetValue(entity));
		}

		IEnumerable<PropertyInfo> GetProperties(Type t)
		{
			return t.GetProperties().Where(o => o.GetCustomAttribute<ColumnAttribute>() != null && !o.GetCustomAttributes(typeof(ComputedAttribute), false).Any());
		}

		public List<PropertyChange> GetChanges(object entity)// where T : IEntity, IWithKey<T, TKey>, IWithTitle
		{
			var pc = new List<PropertyChange>();
			if (objects.ContainsKey(entity))
			{
				foreach (var prop in objects[entity].Keys)
				{
					var oldValue = Convert.ToString(objects[entity][prop]);
					var newValue = Convert.ToString(prop.GetValue(entity));
					if (prop.PropertyType == typeof(DateTime) || prop?.PropertyType == typeof(DateTime?))
					{
						oldValue = objects[entity][prop] != null ? ((DateTime)objects[entity][prop]).ToString("dd.MM.yyyy HH:mm:ss") : null;
						newValue = prop.GetValue(entity) != null ? ((DateTime)prop.GetValue(entity)).ToString("dd.MM.yyyy HH:mm:ss") : null;
					}
					if (prop.PropertyType == typeof(decimal) || prop?.PropertyType == typeof(decimal?))
					{
						oldValue = objects[entity][prop] != null ? ((decimal)objects[entity][prop]).ToString("0.############") : null;
						newValue = prop.GetValue(entity) != null ? ((decimal)prop.GetValue(entity)).ToString("0.############") : null;
					}
					if (oldValue != null && !oldValue.Equals(newValue))
						pc.Add(new PropertyChange { PropertyName = prop.Name, OldValue = oldValue, NewValue = newValue });
				}
			}
			return pc;
		}
	}
}
