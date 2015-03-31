using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Linq;
using System.Collections;

namespace Nephrite.Web.Controls
{
    public partial class LinkedObjects : System.Web.UI.UserControl
    {
        IModelObject obj = null;
		public List<string> SkipProperties = new List<string>();

        protected IModelObject Object
        {
            get
            {
                if (obj == null)
                {
                    if (Parent is ViewControl)
                        obj = ((ViewControl)Parent).ViewData as IModelObject;
                }
                return obj;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void SetObject(IModelObject obj)
        {
            this.obj = obj;
        }

        List<IModelObject> lmo = null;

        public List<IModelObject> GetLinkedObjects()
        {
            return GetLinkedObjects(obj);
        }
		List<IModelObject> GetLinkedObjects(IModelObject obj)
		{
			if (lmo != null)
				return lmo;
			lmo = _GetLinkedObjects(obj);
			return lmo;
		}
        List<IModelObject> _GetLinkedObjects(IModelObject obj)
        {
            
            List<IModelObject>  list = new List<IModelObject>();
            Type t = obj.GetType();
			var skip = t.GetCustomAttributes(typeof(LinkedObjectsAttribute), false).OfType<LinkedObjectsAttribute>().Select(o => o.SkipProperty).ToArray();
            foreach (var p in t.GetProperties())
            {
				if (skip.Contains(p.Name))
					continue;
				if (SkipProperties.Contains(p.Name))
					continue;
                if (p.PropertyType.Name.StartsWith("EntitySet"))
                {
                    foreach (var o in (p.GetValue(obj, null) as IEnumerable).OfType<IModelObject>())
                    {
                        if (o.GetType().Name == "UserInfo")
                            continue;
                        if (o.GetType().Name == "HST_" + obj.GetType().Name)
                        {
                            list.AddRange(_GetLinkedObjects(o));
                            continue;
                        }
                            
                        list.Add(o);
                    }
                }
            }
			return list.OrderBy(o => o.Title).OrderBy(o => o.MetaClass.Caption).ToList();
        }
    }

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class LinkedObjectsAttribute : Attribute
	{
		readonly string skipProperty;

		public LinkedObjectsAttribute(string skipProperty)
		{
			this.skipProperty = skipProperty;
		}

		public string SkipProperty
		{
			get { return skipProperty; }
		}
	}
}