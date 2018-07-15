using System;
using System.Reflection;

namespace RegularExpression.Utility.Data
{
    internal class FieldInfoWrapper<TContainer> : IDataAccessor<TContainer>
    {
        private FieldInfo _field;

        public FieldInfoWrapper(FieldInfo field) =>
            _field = field;

        public void SetValue(in TContainer container, object value) =>
            _field.SetValue(container, value);
    }
}
