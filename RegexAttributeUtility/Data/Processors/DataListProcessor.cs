using System;
using System.Collections.Generic;
using System.Reflection;

namespace RegularExpression.Utility.Data
{
    internal class DataListProcessor<TList, TElement> : IDataProcessor where TList : ICollection<TElement>
    {
        private char _delimiter;
        private bool _trimWhitespace;
        private IDataProcessor _elementProcessor;

        public DataListProcessor(char delimiter, bool trimWhitespace)
        {
            _delimiter = delimiter;
            _trimWhitespace = trimWhitespace;
            AssignElementProcessor();
        }

        private void AssignElementProcessor()
        {
            Type elementType = typeof(TElement);
            TypeInfo elementTypeInfo = elementType.GetTypeInfo();
            if (elementTypeInfo.IsDefined(typeof(RegexContainerAttribute)))
            {
                Type containerType = typeof(RegexContainerProcessor<>).MakeGenericType(typeof(TElement));
                _elementProcessor = (IDataProcessor)Activator.CreateInstance(containerType);
            }
            else
                _elementProcessor = new DataTypeProcessor(elementType);
        }

        private TElement[] ProcessArrayType(string[] dataList)
        {
            TElement[] collection = new TElement[dataList.Length];
            for (int i = 0; i < dataList.Length; i++)
            {
                string tempItem = _trimWhitespace ? dataList[i].Trim() : dataList[i];
                collection[i] = (TElement)_elementProcessor.Process(tempItem);
            }
            return collection;
        }

        private TList ProcessCollectionType(string[] dataList)
        {
            TList collection = (TList)Activator.CreateInstance(typeof(TList));
            foreach (string item in dataList)
            {
                string tempItem = _trimWhitespace ? item.Trim() : item;
                collection.Add((TElement)_elementProcessor.Process(tempItem));
            }
            return collection;
        }

        public object Process(string data)
        {
            string[] dataList = data.Split(_delimiter);
            ICollection<TElement> processedList;
            if (typeof(TList).IsArray)
                processedList = ProcessArrayType(dataList);
            else
                processedList = ProcessCollectionType(dataList);
            return processedList;
        }
    }
}
