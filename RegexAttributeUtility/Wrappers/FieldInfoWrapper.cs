using System;
using System.Reflection;

namespace RegularExpression.Utility.Data
{
    internal class FieldInfoWrapper : IDataAccessor
    {
        private FieldInfo _field;
        public Type DataType { get; }

        public FieldInfoWrapper(FieldInfo field)
        {
            _field = field;
            DataType = _field.FieldType;
        }

        public object GetValue(object container) =>
            _field.GetValue(container);

        public void SetValue(object container, object value) =>
            _field.SetValue(container, value);
    }
}
