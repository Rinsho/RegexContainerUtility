using System;

namespace RegularExpression.Utility.Data
{
    internal interface IDataAccessor<T>
    {
        void SetValue(in T container, object value);
    }
}
