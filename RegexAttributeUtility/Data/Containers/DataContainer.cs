using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace RegularExpression.Utility.Data
{
    internal class DataContainer
    {       
        private string _matchID;        
        private IDataAccessor _dataAccessor;
        private IDataProcessor _dataProcessor;

        private DataContainer(MemberInfo member, Type dataType)
        {
            SetMatchID(member);
            SetDataProcessor(member, dataType);
        }

        public DataContainer(PropertyInfo property) : this(property, property.PropertyType) =>
            _dataAccessor = new PropertyInfoWrapper(property);

        public DataContainer(FieldInfo field) : this(field, field.FieldType) =>
            _dataAccessor = new FieldInfoWrapper(field);           

        private void SetMatchID(MemberInfo member)
        {
            RegexDataAttribute data = member.GetCustomAttribute<RegexDataAttribute>();
            _matchID = string.IsNullOrWhiteSpace(data.MatchID) ? member.Name : data.MatchID;
        }

        private void SetDataProcessor(MemberInfo member, Type dataType)
        {
            if (member.IsDefined(typeof(RegexDataListAttribute)))
            {
                RegexDataListAttribute listData = member.GetCustomAttribute<RegexDataListAttribute>();
                Type elementType = dataType.GetElementType() ?? dataType.GetTypeInfo().GetGenericArguments()[0];
                Type listProcessorType = typeof(DataListProcessor<,>).MakeGenericType(dataType, elementType);
                _dataProcessor = (IDataProcessor)Activator.CreateInstance(listProcessorType, listData.Delimiter);
            }
            else if (dataType.GetTypeInfo().IsDefined(typeof(RegexContainerAttribute)))
            {
                Type containerType = typeof(RegexContainerProcessor<>).MakeGenericType(dataType);
                _dataProcessor = (IDataProcessor)Activator.CreateInstance(containerType);
            }
            else
                _dataProcessor = new DataTypeProcessor(dataType);
        }

        public void ProcessMatch(object container, Match match)
        {
            Group result = match.Groups[_matchID];
            if (result.Success)
            {
                object data = _dataProcessor.Process(result.Value);
                _dataAccessor.SetValue(container, data);
            }
        }
    }
}
