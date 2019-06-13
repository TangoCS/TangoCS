using System;
using System.Collections.Generic;
using System.Text;

namespace Tango.Data
{
	public interface IEntityAudit
	{
		void WriteObjectChange<T, TKey>(T entity, EntityAuditAction action, List<PropertyChange> propertyChanges) where T : IWithKey<T, TKey>;
	}

	public class PropertyChange
	{
		public string PropertyName { get; set; }
		public string OldValue { get; set; }
		public string NewValue { get; set; }
	}

	public enum EntityAuditAction
	{
		Insert,
		Update,
		Delete
	}

	public interface IObjectTracker
	{
		void StartTracking(object entity);
		List<PropertyChange> GetChanges(object entity);
	}
}
