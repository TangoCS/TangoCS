using System;
using System.Collections.Generic;
using System.Linq;

namespace Tango.UI
{
	public interface IFieldGroup : IWithPropertyInjection
	{
		void Init(IViewElement form);
		void SetViewData<TViewData>(TViewData defaultViewData) where TViewData : class;
		void SetViewData<TViewData>(IEntityField<TViewData> field, TViewData viewData) where TViewData: class; 

		void ValidateFormData(ValidationMessageCollection val);
		void ProcessFormData(ValidationMessageCollection val);

		Dictionary<string, object> Args { get; }
		IEnumerable<IField> Fields { get; }
	}

	public abstract class FieldGroup : IFieldGroup
	{
		List<IField> fields = new List<IField>();
		IViewElement Form;

		public IEnumerable<IField> Fields => fields;

		protected F AddField<F>(F field)
			where F : InteractionFlowElement, IField
		{
			fields.Add(field);
			return field;
		}

		public virtual void OnValidateFormData(ValidationMessageCollection val) { }
		public virtual void OnProcessFormData(ValidationMessageCollection val) { }

		public void ValidateFormData(ValidationMessageCollection val)
		{
			foreach (var f in fields)
			{
				if (f.IsVisible && f is IEditableField ef && !ef.Disabled)
					ef.ValidateFormValue(val);
			}
			OnValidateFormData(val);
		}

		public void ProcessFormData(ValidationMessageCollection val)
		{
			foreach (var f in fields)
			{
				if (f.IsVisible && f is IEditableField ef && !ef.Disabled)
					ef.SubmitProperty(val);
				else if (f is IServerField sf)
					sf.SubmitProperty();
			}
			OnProcessFormData(val);
		}

		public void Init(IViewElement form)
		{
			Form = form;

			if (fields.Count == 0)
			{
				var fieldProps = GetType().GetProperties().Where(o => typeof(IField).IsAssignableFrom(o.PropertyType));
				foreach (var prop in fieldProps)
				{
					var f = Activator.CreateInstance(prop.PropertyType) as IField;
					fields.Add(f);
					prop.SetValue(this, f);
				}
			}

			//var allFields = new Dictionary<string, IField>();

			foreach (var f in fields)
			{
				//allFields.Add(f.GetType().Name, f);
				f.Context = Form.Context;
				f.EventReceiver = Form.ClientID;
				f.InjectProperties(Form.Context.RequestServices);
				//f.AllFields = allFields;
				f.Args = Args;
			}
		}

		Dictionary<string, object> _customViewData = new Dictionary<string, object>();

		public Dictionary<string, object> Args { get; } = new Dictionary<string, object>();

		public void SetViewData<TViewData>(TViewData defaultViewData)
            where TViewData : class
        {
			foreach (var f in fields)
			{
                if (f is IEntityField ef2)
                {
					var id = ef2.GetType().Name;
					if (_customViewData.ContainsKey(id))
						ef2.SetViewData(_customViewData[id]);
					else
						ef2.SetViewData(defaultViewData);
				}
            }
        }

        public void SetViewData<TViewData>(IEntityField<TViewData> field, TViewData viewData)
			where TViewData : class
		{
			if (field is IEntityField ef && ef.EntityType.IsAssignableFrom(typeof(TViewData)))
			{
				_customViewData[field.GetType().Name] = viewData;
				field.SetViewData(viewData);
			}
		}

		//public void AssignViewData<TViewData>(TViewData viewData)
		//	where TViewData : class
		//{
		//	_customViewData[GetType().Name] = viewData;
		//	foreach (var f in fields)
		//		if (f is IEntityField ef)
		//			ef.SetViewData(viewData);
		//}
	}

	[Obsolete]
	public abstract class FieldGroup<T> : FieldGroup
		where T : class
	{
	}

	public static class ValidationMessageCollectionExtensions
	{
		public static void Add(this ValidationMessageCollection coll, IField field, string message, 
			ValidationMessageSeverity severity = ValidationMessageSeverity.Error)
		{
			coll.Add("entitycheck", field.ID, message, severity);
		}

	}
}
