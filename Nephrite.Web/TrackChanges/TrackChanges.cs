using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using NHibernate.Event;

namespace Nephrite.Web.TrackChanges
{
	public class TrackChangesListener : IPreUpdateEventListener, IPreInsertEventListener, IPreDeleteEventListener
	{
		public bool OnPreUpdate(PreUpdateEvent e)
		{
			string[] names = e.Persister.PropertyNames;

			return false;
		}

		public bool OnPreInsert(PreInsertEvent e)
		{
			return false;
		}

		public bool OnPreDelete(PreDeleteEvent e)
		{
			return false;
		}
	}


}