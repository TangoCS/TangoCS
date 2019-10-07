using System;
using Tango.UI;

namespace Tango.Meta.Fluent
{
	public static class StandardOperations
	{
		public static AbstractMetaClassBuilder<T, TClass> OperationCreateNew<T, TClass>(this AbstractMetaClassBuilder<T, TClass> b, Action<IMetaOperation> attributes = null)
			where T : AbstractMetaClassBuilder<T, TClass>
		{
			var o = new MetaOperation { Name = "CreateNew", Image = "New" };
			b.MetaClass.AddOperation(o);

			if (attributes != null) attributes(o);

			if (o.Parameters.Count == 0)
				o.ParmString("returnurl");
			return b;
		}

		public static AbstractMetaClassBuilder<T, TClass> OperationEdit<T, TClass>(this AbstractMetaClassBuilder<T, TClass> b, Action<IMetaOperation> attributes = null)
			where T : AbstractMetaClassBuilder<T, TClass>
		{
			var o = new MetaOperation { Name = "Edit", Image = "Edit" };
			b.MetaClass.AddOperation(o);

			if (attributes != null) attributes(o);

			if (o.Parameters.Count == 0)
				o.Parm(b.MetaClass.Key.Type as IMetaParameterType, Constants.Id).ParmString("returnurl");
			return b;
		}

		public static AbstractMetaClassBuilder<T, TClass> OperationList<T, TClass>(this AbstractMetaClassBuilder<T, TClass> b, Action<IMetaOperation> attributes = null)
			where T : AbstractMetaClassBuilder<T, TClass>
		{
			var o = new MetaOperation { Name = "ViewList", Image = "List" };
			b.MetaClass.AddOperation(o);

			if (attributes != null) attributes(o);
			return b;
		}

		public static AbstractMetaClassBuilder<T, TClass> OperationView<T, TClass>(this AbstractMetaClassBuilder<T, TClass> b, Action<IMetaOperation> attributes = null)
			where T : AbstractMetaClassBuilder<T, TClass>
		{
			var o = new MetaOperation { Name = "View", Image = "Properties" };
			b.MetaClass.AddOperation(o);

			if (attributes != null) attributes(o);

			if (o.Parameters.Count == 0)
				o.Parm(b.MetaClass.Key.Type as IMetaParameterType, Constants.Id).ParmString("returnurl");
			return b;
		}

		public static AbstractMetaClassBuilder<T, TClass> OperationDelete<T, TClass>(this AbstractMetaClassBuilder<T, TClass> b, Action<IMetaOperation> attributes = null)
			where T : AbstractMetaClassBuilder<T, TClass>
		{
			var o = new MetaOperation { Name = "Delete", Image = "Delete" };
			b.MetaClass.AddOperation(o);

			if (attributes != null) attributes(o);

			if (o.Parameters.Count == 0)
				o.Parm(b.MetaClass.Key.Type as IMetaParameterType, Constants.Id).ParmString("returnurl");
			return b;
		}

		public static AbstractMetaClassBuilder<T, TClass> OperationUnDelete<T, TClass>(this AbstractMetaClassBuilder<T, TClass> b, Action<IMetaOperation> attributes = null)
			where T : AbstractMetaClassBuilder<T, TClass>
		{
			var o = new MetaOperation { Name = "UnDelete", Image = "Undelete" };
			b.MetaClass.AddOperation(o);

			if (attributes != null) attributes(o);

			if (o.Parameters.Count == 0)
				o.Parm(b.MetaClass.Key.Type as IMetaParameterType, Constants.Id).ParmString("returnurl");
			return b;
		}

		public static AbstractMetaClassBuilder<T, TClass> OperationMoveUp<T, TClass>(this AbstractMetaClassBuilder<T, TClass> b, Action<IMetaOperation> attributes = null)
			where T : AbstractMetaClassBuilder<T, TClass>
		{
			var o = new MetaOperation { Name = "MoveUp", Image = "MoveUp" };
			b.MetaClass.AddOperation(o);

			if (attributes != null) attributes(o);

			if (o.Parameters.Count == 0)
				o.Parm(b.MetaClass.Key.Type as IMetaParameterType, Constants.Id).ParmString("returnurl");
			return b;
		}

		public static AbstractMetaClassBuilder<T, TClass> OperationMoveDown<T, TClass>(this AbstractMetaClassBuilder<T, TClass> b, Action<IMetaOperation> attributes = null)
			where T : AbstractMetaClassBuilder<T, TClass>
		{
			var o = new MetaOperation { Name = "MoveDown", Image = "MoveDown" };
			b.MetaClass.AddOperation(o);

			if (attributes != null) attributes(o);

			if (o.Parameters.Count == 0)
				o.Parm(b.MetaClass.Key.Type as IMetaParameterType, Constants.Id).ParmString("returnurl");
			return b;
		}
		//public static IMetaClass StandardEntity<TUser>(this IMetaClass cls)
		//{
		//	cls.Title().TimeStamp<TUser>().OperationCreateNew().OperationList().OperationEdit().OperationDelete();
		//	return cls;
		//}
	}
}
