using System;
using System.Reflection;

namespace RegularExpression.Utility.Data
{
    internal class PropertyInfoWrapper<TContainer> : IDataAccessor<TContainer>
    {
        private delegate void RefAction<TProperty>(in TContainer obj, TProperty prop);

        private RefAction<object> _setValueDelegate;

        public PropertyInfoWrapper(PropertyInfo property)
        {
            MethodInfo delegateHelper = typeof(PropertyInfoWrapper<TContainer>).GetTypeInfo()
                .GetMethod(nameof(this.CreateDelegate), BindingFlags.Instance | BindingFlags.NonPublic)
                .MakeGenericMethod(property.PropertyType);
            _setValueDelegate = (RefAction<object>)delegateHelper.Invoke(this, new object[] { property.SetMethod });
        }

        private RefAction<object> CreateDelegate<TProperty>(MethodInfo method)
        {
            RefAction<object> del;
            if (typeof(TContainer).GetTypeInfo().IsValueType)
            {
                RefAction<TProperty> temp = (RefAction<TProperty>)method.CreateDelegate(typeof(RefAction<TProperty>));
                del = (in TContainer obj, object val) =>
                    temp(obj, (TProperty)val);
            }
            else
            {
                Action<TContainer, TProperty> temp = (Action<TContainer, TProperty>)method.CreateDelegate(typeof(Action<TContainer, TProperty>));
                del = (in TContainer obj, object val) => 
                    temp(obj, (TProperty)val);
            }
            return del;
        }

        public void SetValue(in TContainer container, object value) =>
            _setValueDelegate(container, value);
    }
}
