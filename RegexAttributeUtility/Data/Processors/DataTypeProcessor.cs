using System;

namespace RegularExpression.Utility.Data
{
    internal class DataTypeProcessor : IDataProcessor
    {
        private Type _dataType;

        public DataTypeProcessor(Type dataType) =>
            _dataType = dataType;

        public object Process(string data) =>
            Convert.ChangeType(data, Nullable.GetUnderlyingType(_dataType) ?? _dataType);
    }
}
