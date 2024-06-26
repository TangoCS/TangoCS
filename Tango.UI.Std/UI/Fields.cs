﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Tango.Data;
using Tango.Html;
using Tango.Localization;
using Tango.Logic;
using static Dapper.SqlMapper;

namespace Tango.UI
{
	public interface IField : IInteractionFlowElement, IWithPropertyInjection
	{
		ValueSource ValueSource { get; set; }
		bool WithCheckBox { get; set; }
		bool DisableCheckBox { get; set; }
		string Caption { get; set; }
		string Description { get; set; }
		string Hint { get; set; }
		bool IsRequired { get; set; }
		bool IsVisible { get; set; }
		bool Disabled { get; set; }
		bool ReadOnly { get; set; }
		bool ShowDescription { get; set; }
		string StringValue { get; set; }

		bool FireOnChangeEvent { get; set; }
		string EventReceiver { get; set; }

		IReadOnlyDictionary<string, object> Args { get; set; }

		void Init();
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
	public enum ValueSource
	{
		[Description("Значение из модели")]
		Model,
		[Description("Значение с формы")]
		Form
	}
	public abstract class Field : InteractionFlowElement, IField
	{
		public ValueSource ValueSource { get; set; }
		public virtual bool WithCheckBox { get; set; } = false;
		public virtual bool DisableCheckBox { get; set; } = false;
		public virtual string Caption { get; set; } = "";
		public virtual string Hint { get; set; }
		public virtual string Description { get; set; } = "";
		public virtual bool IsRequired { get; set; } = false;
		public virtual bool IsVisible { get; set; } = true;
		public virtual bool Disabled { get; set; } = false;
		public virtual bool ReadOnly { get; set; } = false;
		public virtual bool ShowDescription { get; set; } = false;
		public virtual string StringValue { get; set; } = "";

		public IReadOnlyDictionary<string, object> Args { get; set; }

		public virtual bool FireOnChangeEvent { get; set; } = false;
		public string EventReceiver { get; set; }

		public virtual void Init() { }
	}

	public class FieldSnapshot
	{
		public string ID { get; set; }
		public string Caption { get; set; }
		public string StringValue { get; set; }
		public string Value { get; set; }

		public override string ToString()
		{
			return $"{ID} {Caption}: StringValue={StringValue} Value={Value}";
		}
	}

	public abstract class Field<TValue, TFormValue> : Field, IField<TValue>, IFormField<TFormValue>, IFieldValueProvider<TFormValue>
	{
		protected IFieldValueProvider<TFormValue> ValueProvider;

		public override bool IsRequired {
			get {
				if (!(IsVisible)) return false;
				if (ReadOnly || Disabled) return false;
				return base.IsRequired;
			}
			set { base.IsRequired = value; }
		}

		public virtual TValue DefaultValue => default;
		public virtual TValue Value => ValueSource == ValueSource.Form ? ProceedFormValue(FormValue) : DefaultValue;
		public override string StringValue =>
			Value is IWithTitle ? (Value as IWithTitle)?.Title :
			typeof(TValue).IsEnum ? Enumerations.GetEnumDescription((Enum)(object)Value) :
			typeof(TValue) == typeof(bool) ? ((bool)(object)Value).Icon() :
			typeof(TValue) == typeof(bool?) ? ((bool?)(object)Value)?.Icon() :
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
		public virtual TFormValue GetFormValue() => Context.GetArg<TFormValue>(ID);

		public void SetValueProvider(IFieldValueProvider<TFormValue> provider)
		{
			ValueProvider = provider;
		}

		public virtual void ValidateFormValue(ValidationMessageCollection val)
		{
			if (IsRequired && ValidationFunc != null) ValidationFunc(val.Check(Resources, ID, Caption, FormValue));
		}		
		public virtual Func<ValidationBuilder<TFormValue>, ValidationBuilder<TFormValue>> ValidationFunc => vb => vb.NotEmpty();

		internal protected TValue ProceedFormValue(TFormValue val)
		{
			if (val is string s)
				return (TValue)(object)s.Trim();
			else if (val is TValue v)
				return v;
			else if (val == null)
				return (TValue)(object)null;
			else if (val is ICastable<TFormValue, TValue> obj)
				return obj.Cast(val);
			else
				throw new NotImplementedException($"Cast from {typeof(TFormValue).Name} to {typeof(TValue).Name} is not implemented");
		}

		public Field()
		{
			ID = GetType().Name.Replace("`1", "");
			ValueProvider = this;

			var type = typeof(TFormValue);
			IsRequired = type.IsValueType && type != typeof(bool) && (Nullable.GetUnderlyingType(type) == null);
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}

	public abstract class EntityField<TEntity> : Field, IEntityField<TEntity>
		where TEntity : class
	{
		public override string Caption => Resources.Get(TypeHelper.GetDeclaredType(ViewData), ID);
		public override string Description => Resources.Get(TypeHelper.GetDeclaredType(ViewData), ID, "description");
		string hint;
		public override string Hint
		{
			get
			{
				if (hint == null)
				{
					if (Resources.TryGet(TypeHelper.GetDeclaredType(ViewData) + "." + ID + "-hint", out hint))
						return hint;
					Resources.SetNotFound(TypeHelper.GetDeclaredType(ViewData) + "." + ID + "-hint;" + Caption);
					hint = "";
				}
				return hint;
			}
			set
			{
				hint = value;
			}
		}

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
		string hint;
		public override string Hint
		{
			get
			{
				if (hint == null)
				{
					if (Resources.TryGet(TypeHelper.GetDeclaredType(ViewData) + "." + ID + "-hint", out hint))
						return hint;
					Resources.SetNotFound(TypeHelper.GetDeclaredType(ViewData) + "." + ID + "-hint;" + Caption);
					hint = "";
				}
				return hint;
			}
			set
			{
				hint = value;
			}
		}

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

		public sealed override TValue Value => ValueSource == ValueSource.Form ? ProceedFormValue(FormValue) : PropertyValue;

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
					//var baseNaming = ViewData.GetType().GetCustomAttribute(typeof(BaseNamingConventionsAttribute)) as BaseNamingConventionsAttribute;
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
		
		protected string ChangePropertyName(PropertyInfo p)
		{
			var name = p.Name;

			var conventions = p.PropertyType == typeof(Guid) || p.PropertyType == typeof(Guid?)
				? DBConventions.GUIDSuffix
				: DBConventions.IDSuffix;

			if (name.EndsWith(BaseNamingConventions.IDSuffix) && !name.EndsWith(conventions))
			{
				name = name.Substring(0, name.Length - BaseNamingConventions.IDSuffix.Length) + conventions;
			}

			return name;
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

		public override string ToString()
		{
			if (ValueSource == ValueSource.Form)
				return FormValue.ToString();

			var v = PropertyValue;
			if (v is IWithKey<int> intv)
				return intv.ID.ToString();
			else if (v is IWithKey<Guid> guidv)
				return guidv.ID.ToString();
			else if (v is IWithKey<string> strv)
				return strv.ID;
			else
				return v.ToString();
		}
	}

	public abstract class Field<TValue> : Field<TValue, TValue>
	{
	}

	public abstract class EntityField<TEntity, TValue> : EntityField<TEntity, TValue, TValue>
		where TEntity : class
	{
    }

	public abstract class EntityTimeField<TEntity> : EntityField<TEntity, TimeSpan>, IFormattedField
		where TEntity : class
	{
		public override TimeSpan DefaultValue => DateTime.Now.TimeOfDay;
		public string Format => "hh:mm:ss";
		
		public override Func<ValidationBuilder<TimeSpan>, ValidationBuilder<TimeSpan>> ValidationFunc =>
			v => base.ValidationFunc(v).ValidateTimeInterval();
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
			v => {
				var vf = base.ValidationFunc(v);
				if (!vf.Collection.HasItems())
					vf = vf.ValidateDateInterval();
				return vf;
			};

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

	public abstract class EntityFileField<TEntity> : EntityField<TEntity, PostedFileInfo>
		where TEntity : class
	{
		public override PostedFileInfo GetFormValue() => Context.GetArg<PostedFileInfo>(ID);
	}
}
