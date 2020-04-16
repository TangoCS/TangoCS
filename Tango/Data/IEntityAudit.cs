using System;
using System.Collections.Generic;
using System.Text;

namespace Tango.Data
{
	public interface IEntityAudit
	{
		//Guid? WriteObjectChange<T, TKey>(T entity, EntityAuditAction action, List<PropertyChange> propertyChanges, Guid? parentObjectChange_ID = null) 
		//	where T : IWithKey<TKey>;

		List<ObjectChangePackage> Packages { get; }

		void WriteObjectChange();
	}

	public static class IEntityAuditExtensions
	{
		public static void AddChanges(this IEntityAudit audit, ObjectChange primaryObject, List<ObjectChange> secondaryObjects = null)
		{
			var pack = new ObjectChangePackage {
				PrimaryObject = primaryObject
			};

			if (secondaryObjects != null)
				pack.SecondaryObjects = secondaryObjects;

			audit.Packages.Add(pack);
		}

		public static void AddChanges<T, TKey>(this IEntityAudit audit, T entity, EntityAuditAction action, List<PropertyChange> propertyChanges = null, List<ObjectChange> secondaryObjects = null)
			where T : IWithKey<TKey>
		{
			var pack = new ObjectChangePackage {
				PrimaryObject = ObjectChange.Create<T, TKey>(entity, action, propertyChanges)
			};

			if (secondaryObjects != null)
				pack.SecondaryObjects = secondaryObjects;

			audit.Packages.Add(pack);
		}
	}

	public class ObjectChangePackage
	{
		public ObjectChange PrimaryObject { get; set; }
		public List<ObjectChange> SecondaryObjects { get; set; } = new List<ObjectChange>();
	}

	public class ObjectChange
	{
		public static ObjectChange Create<T, TKey>(T entity, EntityAuditAction action, List<PropertyChange> propertyChanges = null)
			where T : IWithKey<TKey>
		{
			return new ObjectChange {
				Action = action,
				ID = () => entity.ID.ToString(),
				Title = (entity as IWithTitle)?.Title ?? $"Объект {typeof(T).Name}, ID = {entity.ID}",
				Type = typeof(T),
				PropertyChanges = propertyChanges
			};
		}

		public Func<string> ID { get; set; }
		public string Title { get; set; }
		public Type	Type { get; set; }

		public EntityAuditAction Action { get; set; }

		public List<PropertyChange> PropertyChanges { get; set; }
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
