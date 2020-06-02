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

		public static void AddChanges<TKey>(this IEntityAudit audit, IWithKey<TKey> entity, EntityAuditAction action, List<PropertyChange> propertyChanges = null, List<ObjectChange> secondaryObjects = null)
		{
			var pack = new ObjectChangePackage {
				PrimaryObject = ObjectChange.RegisterAction(entity, action, propertyChanges)
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
		static string GetTitle<T>(T entity) => (entity as IWithTitle)?.Title ?? (entity as IWithName)?.Name;

		public static ObjectChange RegisterAction<TKey>(IWithKey<TKey> entity, EntityAuditAction action, List<PropertyChange> propertyChanges = null)
		{
			return new ObjectChange {
				Action = action,
				ID = () => entity.ID.ToString(),
				Title = GetTitle(entity),
				Type = entity.GetType(),
				PropertyChanges = propertyChanges
			};
		}

		public static ObjectChange Insert<TKey>(IWithKey<TKey> entity)
		{
			return new ObjectChange {
				Action = EntityAuditAction.Insert,
				ID = () => entity.ID.ToString(),
				Title = GetTitle(entity),
				Type = entity.GetType(),
				PropertyChanges = null
			};
		}

		public static ObjectChange Update<TKey>(IWithKey<TKey> entity, List<PropertyChange> propertyChanges)
		{
			return new ObjectChange {
				Action = EntityAuditAction.Update,
				ID = () => entity.ID.ToString(),
				Title = GetTitle(entity),
				Type = entity.GetType(),
				PropertyChanges = propertyChanges
			};
		}

		public static ObjectChange Delete<TKey>(IWithKey<TKey> entity)
		{
			return new ObjectChange {
				Action = EntityAuditAction.Delete,
				ID = () => entity.ID.ToString(),
				Title = GetTitle(entity),
				Type = entity.GetType(),
				PropertyChanges = null
			};
		}

		public static ObjectChange Delete<T>(object id)
		{
			return new ObjectChange {
				Action = EntityAuditAction.Delete,
				ID = () => id.ToString(),
				Title = null,
				Type = typeof(T),
				PropertyChanges = null
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
