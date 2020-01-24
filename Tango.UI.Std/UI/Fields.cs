using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Tango.Localization;
using Tango.Logic;

namespace Tango.UI
{
	public interface IField : IInteractionFlowElement, IWithPropertyInjection
	{
		string Caption { get; }
		string Description { get; }
		bool IsRequired { get; }
		bool IsVisible { get; }
		bool Disabled { get; }
		bool ReadOnly { get; }
		bool ShowDescription { get; }
		string StringValue { get; }

		//IInteractionFlowElement Form { get; set; }

		bool FireOnChangeEvent { get; }
		string EventReceiver { get; set; }

		//IReadOnlyDictionary<string, IField> AllFields { get; set; }
		IReadOnlyDictionary<string, object> Args { get; set; }
	}

	public interface IField<TValue> : IField
	{	
		TValue Value { get; }		
	}

	public interface IEntityField : IField
	{
        Type EntityType { get; }
        void SetViewData<TViewData>(TViewData viewData);
    }

	public interface IEntityField<TEntity> : IEntityField
		where TEntity : class
	{
		TEntity ViewData { get; set; }
		string[] Properties { get; }
    }

	public interface IFormField<TFormValue> : IField
	{
		TFormValue FormValue { get; }
	}

	public interface IEditableField : IField
	{
		void ValidateFormValue(ValidationMessageCollection val);
		void SubmitProperty(ValidationMessageCollection val);
	}

	public interface IServerField : IField
	{
		void SubmitProperty();
	}

	public interface IFormattedField : IField
	{
		string Format { get; }
	}

	public interface IFieldValueProvider<T>
	{
		T Value { get; }
	}

	public abstract class Field : InteractionFlowElement, IField
	{
		public abstract string Caption { get; }
		public virtual string Description => "";
		public virtual bool IsRequired => false;
		public virtual bool IsVisible => true;
		public virtual bool Disabled => false;
		public virtual bool ReadOnly => false;
		public virtual bool ShowDescription => false;
		public virtual string StringValue => "";

		//public IReadOnlyDictionary<string, IField> AllFields { get; set; }
		public IReadOnlyDictionary<string, object> Args { get; set; }

		//public IInteractionFlowElement Form { get; set; }

		public virtual bool FireOnChangeEvent => false;

		public string EventReceiver { get; set; }
	}

	public abstract class Field<TValue, TFormValue> : Field, IField<TValue>, IFormField<TFormValue>, IFieldValueProvider<TFormValue>
	{
		protected IFieldValueProvider<TFormValue> ValueProvider;

		public override bool IsRequired {
			get {
				var type = typeof(TFormValue);
				return type.IsValueType && type != typeof(bool) && (Nullable.GetUnderlyingType(type) == null);
			}
		}

		public virtual TValue DefaultValue => default;
		public virtual TValue Value => Context.RequestMethod == "POST" ? ProceedFormValue : DefaultValue;
		public override string StringValue => 
			Value is IWithTitle ? (Value as IWithTitle)?.Title : 
			typeof(TValue).IsEnum ? Enumerations.GetEnumDescription((Enum)(object)Value) :
			Value?.ToString();


		TFormValue formValueCache = default;
		bool formValueGot = false;

		public TFormValue FormValue
		{
			get
			{
				if (!formValueGot)
				{
					formValueCache = (ValueProvider ?? (this as IFieldValueProvider<TFormValue>)).Value;
					formValueGot = true;
				}
				return formValueCache;
			}
		}

		TFormValue IFieldValueProvider<TFormValue>.Value => ValueProvider == this ? GetFormValue() : ValueProvider.Value;
		public virtual TFormValue GetFormValue() => GetArg<TFormValue>(ID);

		public void SetValueProvider(IFieldValueProvider<TFormValue> provider)
		{
			ValueProvider = provider;
		}

		public virtual void ValidateFormValue(ValidationMessageCollection val)
		{
			if (IsRequired && ValidationFunc != null) ValidationFunc(val.Check(Resources, ID, Caption, FormValue));
		}
		public virtual Func<ValidationBuilder<TFormValue>, ValidationBuilder<TFormValue>> ValidationFunc=> vb => vb.NotEmpty();

		protected TValue ProceedFormValue
		{
			get
			{
				if (FormValue is string s)
					return (TValue)(object)s.Trim();
				else if (FormValue is TValue v)
                    return v;
                else if (FormValue == null)
                    return (TValue)(object)null;
                else if (FormValue is ICastable<TFormValue, TValue> obj)
                    return obj.Cast(FormValue);
                else
                    throw new NotImplementedException($"Cast from {typeof(TFormValue).Name} to {typeof(TValue).Name} is not implemented");
			}
		}

		public Field()
		{
			ID = GetType().Name.Replace("`1", "");
			ValueProvider = this;
		}
	}

	public abstract class EntityField<TEntity> : Field, IEntityField<TEntity>
		where TEntity : class
	{
		public override string Caption => Resources.Get(TypeHelper.GetDeclaredType(ViewData), ID);
		public override string Description => Resources.Get(TypeHelper.GetDeclaredType(ViewData), ID, "description");

		public TEntity ViewData { get; set; }
		public Type EntityType => typeof(TEntity);

		public virtual string[] Properties => new string[] { ID };

		public EntityField()
		{
			ID = GetType().Name.Replace("`1", "");
		}

        public void SetViewData<TViewData>(TViewData viewData)
        {
			if (viewData == null) return;

			if (viewData is TEntity obj2)
				ViewData = obj2;
			else if (viewData is ICastable<TViewData, TEntity> obj)
				ViewData = obj.Cast(viewData);
		}
    }

	public abstract class EntityServerField<TEntity, TValue> : EntityField<TEntity>, IServerField
		where TEntity : class
	{
		public abstract void SubmitProperty();
	}


	public abstract class EntityField<TEntity, TValue, TFormValue> : Field<TValue, TFormValue>, IEntityField<TEntity>, IEditableField
		where TEntity : class
	{
		public override string Caption => Resources.Get(TypeHelper.GetDeclaredType(ViewData), ID);
		public override string Description => Resources.Get(TypeHelper.GetDeclaredType(ViewData), ID, "description");

		public TEntity ViewData { get; set; }
		public Type EntityType => typeof(TEntity);

		public virtual string[] Properties
		{
			get {
				var propName = ID;
				var prop = typeof(TEntity).GetProperty(propName);
				if (prop == null || GetFromID(prop.PropertyType))
					propName = ConvertToID(propName);
				return new string[] { propName };
			}
		}

        public void SetViewData<TViewData>(TViewData viewData)
        {
			if (viewData == null) return;

			if (viewData is TEntity obj2)
				ViewData = obj2;
			else if (viewData is ICastable<TViewData, TEntity> obj)
				ViewData = obj.Cast(viewData);
		}

        protected virtual string IDSuffix => DBConventions.IDSuffix;
		protected virtual string GUIDSuffix => DBConventions.GUIDSuffix;

		public override TValue Value => Context.RequestMethod == "POST" ? ProceedFormValue : PropertyValue;

		bool SubmitToID(Type t)
		{
			return !t.IsValueType && t != typeof(string) &&
			(typeof(TFormValue).IsValueType || typeof(TFormValue) == typeof(string));
		}

		bool GetFromID(Type t)
		{
			return SubmitToID(t) && typeof(TValue) == typeof(TFormValue);
		}

		string ConvertToID(string propertyName)
		{
			if (typeof(TFormValue) == typeof(int)) propertyName += IDSuffix;
			else if (typeof(TFormValue) == typeof(int?)) propertyName += IDSuffix;
			else if (typeof(TFormValue) == typeof(string)) propertyName += IDSuffix;
			else if (typeof(TFormValue) == typeof(Guid)) propertyName += GUIDSuffix;
			else if (typeof(TFormValue) == typeof(Guid?)) propertyName += GUIDSuffix;
			return propertyName;
		}

		public virtual TValue PropertyValue
		{
			get
			{
				if (ViewData == null) return DefaultValue;

				var propName = ID;
				var prop = ViewData.GetType().GetProperty(propName);
				if (prop == null || GetFromID(prop.PropertyType))
				{
					propName = ConvertToID(propName);
					prop = ViewData.GetType().GetProperty(propName);
				}

				if (prop == null)
					throw new Exception($"Class {ViewData.GetType().Name} does not have any property named {propName}");

				var v = prop.GetValue(ViewData);

				if (v == null)
					return DefaultValue;
				else if (typeof(TValue).IsEnum && prop.PropertyType == typeof(bool))
					return (TValue)(object)((bool)v ? 1 : 0);
				else
					return (TValue)v;
			}
		}

		public virtual void SubmitProperty(ValidationMessageCollection val)
		{
			if (ViewData == null) return;

			var propName = ID;
			var prop = ViewData.GetType().GetProperty(propName);
			if (prop == null || SubmitToID(prop.PropertyType))
			{
				propName = ConvertToID(propName);
				prop = ViewData.GetType().GetProperty(propName);
			}
			if (prop == null)
				throw new Exception($"Class {ViewData.GetType().Name} does not have any property named {propName}");

			if (typeof(TFormValue) != typeof(TValue))
				prop.SetValue(ViewData, FormValue);
			else if (typeof(TValue).IsEnum && prop.PropertyType == typeof(bool))
				prop.SetValue(ViewData, Convert.ToInt32(Value) == 1);
			else if (Nullable.GetUnderlyingType(prop.PropertyType) != null && 
				Nullable.GetUnderlyingType(prop.PropertyType).IsEnum &&
				typeof(TValue) == typeof(int))
				prop.SetValue(ViewData, Enum.ToObject(Nullable.GetUnderlyingType(prop.PropertyType), Value));
			else
				prop.SetValue(ViewData, Value);
		}
	}

	public abstract class Field<TValue> : Field<TValue, TValue>
	{
	}

	public abstract class EntityField<TEntity, TValue> : EntityField<TEntity, TValue, TValue>
		where TEntity : class
	{
    }

	public abstract class EntityDateTimeField<TEntity> : EntityField<TEntity, DateTime>, IFormattedField
		where TEntity : class
	{
		public override DateTime DefaultValue => DateTime.Now;
		public virtual string Format => "dd.MM.yyyy HH:mm";
		public override string StringValue => Value.ToString(Format);
		public override DateTime GetFormValue() => Context.GetDateTimeArg(ID, Format, DefaultValue);
		public override Func<ValidationBuilder<DateTime>, ValidationBuilder<DateTime>> ValidationFunc =>
			v => base.ValidationFunc(v).ValidateDateInterval();

	}

	public abstract class EntityNullableDateTimeField<TEntity> : EntityField<TEntity, DateTime?>, IFormattedField
		where TEntity : class
	{
		public virtual string Format => "dd.MM.yyyy HH:mm";
		public override string StringValue => Value?.ToString(Format);
		public override DateTime? GetFormValue() => Context.GetDateTimeArg(ID, Format);
		public override Func<ValidationBuilder<DateTime?>, ValidationBuilder<DateTime?>> ValidationFunc =>
			v => base.ValidationFunc(v).ValidateDateInterval();
	}

	public abstract class EntityDateField<TEntity> : EntityField<TEntity, DateTime>, IFormattedField
		where TEntity : class
	{
        public override DateTime DefaultValue => DateTime.Today;
		public virtual string Format => "dd.MM.yyyy";
		public override string StringValue => Value.ToString(Format);
		public override DateTime GetFormValue() => Context.GetDateTimeArg(ID, Format, DefaultValue);

		public override Func<ValidationBuilder<DateTime>, ValidationBuilder<DateTime>> ValidationFunc => 
			v => base.ValidationFunc(v).ValidateDateInterval();

    }

	public abstract class EntityNullableDateField<TEntity> : EntityField<TEntity, DateTime?>, IFormattedField
		where TEntity : class
	{
		public virtual string Format => "dd.MM.yyyy";
		public override string StringValue => Value?.ToString(Format);
		public override DateTime? GetFormValue() => Context.GetDateTimeArg(ID, Format);
		public override Func<ValidationBuilder<DateTime?>, ValidationBuilder<DateTime?>> ValidationFunc =>
			v => base.ValidationFunc(v).ValidateDateInterval();
	}

	public abstract class EntityReferenceManyField<TEntity, TRefClass, TRefKey> : EntityField<TEntity, IQueryable<TRefClass>, List<TRefKey>>
		where TEntity : class
		where TRefClass : class, IWithTitle, IWithKey<TRefKey>
	{
		public abstract IEnumerable<string> StringValueCollection { get; }
		public override List<TRefKey> GetFormValue() => Context.GetListArg<TRefKey>(ID);
	}

	public abstract class EntityReferenceManyField<TEntity, TRefKey> : EntityField<TEntity, List<TRefKey>>
		where TEntity : class
	{
		public abstract IEnumerable<string> StringValueCollection { get; }
		public override List<TRefKey> GetFormValue() => Context.GetListArg<TRefKey>(ID);
	}



    public static class FieldExtensions
    {
        //public static void ValidateDateValue(DateTime? entityDate, string entityName, ValidationMessageCollection val)
        //{
        //    if (Data.ConnectionManager.DBType == Data.DBType.MSSQL && (entityDate < new DateTime(1753, 1, 1) || entityDate > new DateTime(9999, 12, 31)))
        //        val.Add(new ValidationMessage("entitycheck", entityName, "Введена дата вне пределов разрешенного диапазона.", ValidationMessageSeverity.Error));
        //}

        //public static T GetField<T>(this IField field)
        //    where T : class, IField
        //{
        //    if (!field.AllFields.TryGetValue(typeof(T).Name, out var f))
        //        return null;
        //    else
        //        return f as T;
        //}
    }
}
