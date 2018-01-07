using System;
using System.Reflection;

namespace RegularExpression.Utility.Data
{
    internal class PropertyInfoWrapper : IDataAccessor
    {
        private PropertyInfo _property;
        public Type DataType { get; }

        public PropertyInfoWrapper(PropertyInfo property)
        {
            _property = property;
            DataType = _property.PropertyType;
        }

        public object GetValue(object container) =>
            _property.GetValue(container);

        public void SetValue(object container, object value) =>
            _property.SetValue(container, value);
    }
}
