using System;
using System.Reflection;

namespace RegularExpression.Utility.Data
{
    internal class PropertyInfoWrapper : IDataAccessor
    {
        private Action<object, object> _setValueDelegate;

        public PropertyInfoWrapper(PropertyInfo property)
        {
            MethodInfo delegateHelper = typeof(PropertyInfoWrapper).GetTypeInfo()
                .GetMethod(nameof(this.CreateDelegate), BindingFlags.Instance | BindingFlags.NonPublic)
                .MakeGenericMethod(property.DeclaringType, property.PropertyType);
            _setValueDelegate = (Action<object, object>)delegateHelper.Invoke(this, new object[] { property.SetMethod });
        }

        private Action<object, object> CreateDelegate<TObject, TProp>(MethodInfo method)
        {
            var del = (Action<TObject, TProp>)method.CreateDelegate(typeof(Action<TObject, TProp>));
            return (object obj, object val) => { del((TObject)obj, (TProp)val); };
        }

        public void SetValue(object container, object value) =>
            _setValueDelegate(container, value);
    }
}
