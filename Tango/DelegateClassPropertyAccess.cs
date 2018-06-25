using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Tango
{
	public static class ClassPropertyAccessActivator
	{
		public static Activator CreateCtor(Type targetType, Type propertyType)
		{
			var type = typeof(ClassPropertyAccess<,>).MakeGenericType(targetType, propertyType);
			var p = new Type[] { typeof(PropertyInfo) };
			ConstructorInfo defConstructor = type.GetConstructor(p);
			var dynamicMethod = new DynamicMethod("CreateInstance", type, p, true);
			ILGenerator g = dynamicMethod.GetILGenerator();
			g.Emit(OpCodes.Nop);
			g.Emit(OpCodes.Newobj, defConstructor);
			g.Emit(OpCodes.Ret);
			return (Activator)dynamicMethod.CreateDelegate(typeof(Activator));
		}

		public delegate object Activator(PropertyInfo propertyInfo);
	}

	public interface IClassPropertyAccess
	{
		object GetValue(object target);
		void SetValue(object target, object value);
	}

	public class ClassPropertyAccess<TTarget, TProperty> : IClassPropertyAccess
		where TTarget : class
	{
		PropertyValueGetter _getter;
		PropertyValueSetter _setter;
		PropertyInfo _propertyInfo;

		delegate TProperty PropertyValueGetter(TTarget target);
		delegate TProperty StaticPropertyValueGetter();

		delegate void PropertyValueSetter(TTarget target, TProperty value);
		delegate void StaticPropertyValueSetter(TProperty value);

		public ClassPropertyAccess(PropertyInfo propertyInfo)
		{
			_propertyInfo = propertyInfo;
		}

		static PropertyValueGetter CreateGetter(PropertyInfo propertyInfo)
		{
			var name = propertyInfo.Name;

			if (propertyInfo.CanRead && propertyInfo.GetIndexParameters().Length == 0)
			{
				var getMethod = propertyInfo.GetGetMethod();
				if (getMethod.IsStatic)
				{
					var staticGetter = (StaticPropertyValueGetter)Delegate.CreateDelegate(typeof(StaticPropertyValueGetter), getMethod);
					return target => staticGetter.Invoke();
				}
				else
				{
					return (PropertyValueGetter)Delegate.CreateDelegate(typeof(PropertyValueGetter), getMethod);
				}
			}
			else
			{
				return target => {
					throw new NotImplementedException("No getter implemented for property " + name);
				};
			}
		}

		static PropertyValueSetter CreateSetter(PropertyInfo propertyInfo)
		{
			var name = propertyInfo.Name;
			if (propertyInfo.CanWrite && propertyInfo.GetIndexParameters().Length == 0)
			{
				var setMethod = propertyInfo.GetSetMethod();
				if (setMethod.IsStatic)
				{
					var staticSetter = (StaticPropertyValueSetter)Delegate.CreateDelegate(typeof(StaticPropertyValueSetter), setMethod);
					return (target, value) => staticSetter.Invoke(value);
				}
				else
				{
					return (PropertyValueSetter)Delegate.CreateDelegate(typeof(PropertyValueSetter), setMethod);
				}
			}
			else
			{
				return (target, value) => {
					throw new NotImplementedException("No setter implemented for property " + name);
				};
			}
		}

		public TProperty GetValue(TTarget target)
		{
			if (_getter == null) _getter = CreateGetter(_propertyInfo);
			return _getter.Invoke(target);
		}

		public void SetValue(TTarget target, TProperty value)
		{
			if (_setter == null) _setter = CreateSetter(_propertyInfo);
			_setter.Invoke(target, value);
		}

		public object GetValue(object target)
		{
			return GetValue((TTarget)target);
		}

		public void SetValue(object target, object value)
		{
			SetValue((TTarget)target, (TProperty)value);
		}
	}
}
