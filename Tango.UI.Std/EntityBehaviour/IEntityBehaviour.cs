using System;
using System.Collections.Generic;
using System.Text;
using Tango.Data;

namespace Tango.EntityBehaviour
{
	public interface IEntityBehaviour
	{
		void Invoke(Action next);
	}

	public class BehaviourContextAttribute : Attribute
	{
	}

	public interface IEntityBehaviourManager
	{
		void Add<TEntity, TEvent, TEntityBehaviour>()
			where TEntity: IEntity
			where TEvent: IEntityEvent
			where TEntityBehaviour: IEntityBehaviour;
		void Replace<TEntity, TEvent, TOldEntityBehaviour, TNewEntityBehaviour>()
			where TEntity: IEntity
			where TEvent: IEntityEvent
			where TOldEntityBehaviour: IEntityBehaviour
			where TNewEntityBehaviour : IEntityBehaviour;
		void InsertAfter<TEntity, TEvent, TEntityBehaviour, TNewEntityBehaviour>()
			where TEntity : IEntity
			where TEvent : IEntityEvent
			where TEntityBehaviour : IEntityBehaviour
			where TNewEntityBehaviour : IEntityBehaviour;
	}

	public class EntityBehaviourManager : IEntityBehaviourManager
	{
		public void Add<TEntity, TEvent, TEntityBehaviour>()
			where TEntity : IEntity
			where TEvent : IEntityEvent
			where TEntityBehaviour : IEntityBehaviour
		{
			
		}

		public void InsertAfter<TEntity, TEvent, TEntityBehaviour, TNewEntityBehaviour>()
			where TEntity : IEntity
			where TEvent : IEntityEvent
			where TEntityBehaviour : IEntityBehaviour
			where TNewEntityBehaviour : IEntityBehaviour
		{
			
		}

		public void Replace<TEntity, TEvent, TOldEntityBehaviour, TNewEntityBehaviour>()
			where TEntity : IEntity
			where TEvent : IEntityEvent
			where TOldEntityBehaviour : IEntityBehaviour
			where TNewEntityBehaviour : IEntityBehaviour
		{
			
		}
	}

	public interface IEntityEvent { }
	public interface IOnProcessFormData : IEntityEvent { }

}
