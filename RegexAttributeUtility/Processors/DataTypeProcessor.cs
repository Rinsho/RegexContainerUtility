using System;
using System.Reflection;

namespace RegularExpression.Utility.Data
{
    internal class DataTypeProcessor : IDataProcessor
    {
        private Func<string, object> ProcessImpl;

        public DataTypeProcessor(Type dataType)
        {
            if (dataType.GetTypeInfo().IsEnum)
            {
                ProcessImpl = (string data) =>
                {
                    object newEnum = Enum.Parse(dataType, data, true);
                    if (!Enum.IsDefined(dataType, newEnum))
                        throw new InvalidRegexDataException($"Data '{data}' is not valid for enum {dataType}.");
                    return newEnum;
                };
            }
            else
            {
                ProcessImpl = (string data) =>
                    Convert.ChangeType(data, Nullable.GetUnderlyingType(dataType) ?? dataType);
            }
        }

        public object Process(string data) =>
            ProcessImpl(data);
    }
}
