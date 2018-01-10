using System;

namespace RegularExpression.Utility.Data
{
    internal interface IDataAccessor
    {
        object GetValue(object container);
        void SetValue(object container, object value);
        Type DataType { get; }
    }
}
