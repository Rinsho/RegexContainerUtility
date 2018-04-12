using System;
using System.Reflection;

namespace RegularExpression.Utility.Data
{
    internal class FieldInfoWrapper : IDataAccessor
    {
        private FieldInfo _field;

        public FieldInfoWrapper(FieldInfo field) =>
            _field = field;

        public void SetValue(object container, object value) =>
            _field.SetValue(container, value);
    }
}
