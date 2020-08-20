using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;

namespace RegularExpression.Utility.Data
{
    internal class DataListProcessor<TList, TElement> : IDataProcessor where TList : ICollection<TElement>
    {
        private char _delimiter;
        private bool _trimWhitespace;
        private IDataProcessor _elementProcessor;
        private Func<string[], ICollection<TElement>> ProcessList;
        private Func<TList> CreateCollection;
        private Func<int, TElement[]> CreateElementArray;

        public DataListProcessor(char delimiter, bool trimWhitespace)
        {
            _delimiter = delimiter;
            _trimWhitespace = trimWhitespace;
            AssignCollectionProcessor();
            AssignElementProcessor();

            void AssignCollectionProcessor()
            {
                if (typeof(TList).IsArray)
                {
                    ParameterExpression length = Expression.Parameter(typeof(int), nameof(length));
                    CreateElementArray = Expression.Lambda<Func<int, TElement[]>>(Expression.NewArrayBounds(typeof(TElement), length), length).Compile();
                    ProcessList = ProcessArrayType;
                }
                else
                {
                    CreateCollection = Expression.Lambda<Func<TList>>(Expression.New(typeof(TList))).Compile();
                    ProcessList = ProcessCollectionType;
                }
            }

            void AssignElementProcessor()
            {
                Type elementType = typeof(TElement);
                if (elementType.GetTypeInfo().IsDefined(typeof(RegexContainerAttribute)))
                {
                    Type containerType = typeof(RegexContainerProcessor<>).MakeGenericType(elementType);
                    _elementProcessor = (IDataProcessor)Activator.CreateInstance(containerType);
                }
                else
                    _elementProcessor = new DataTypeProcessor(elementType);
            }
        }

        private ICollection<TElement> ProcessArrayType(string[] dataList)
        {
            TElement[] collection = CreateElementArray(dataList.Length);
            for (int i = 0; i < dataList.Length; i++)
            {
                string tempItem = _trimWhitespace ? dataList[i].Trim() : dataList[i];
                collection[i] = (TElement)_elementProcessor.Process(tempItem);
            }
            return collection;
        }

        private ICollection<TElement> ProcessCollectionType(string[] dataList)
        {
            TList collection = CreateCollection();
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
            return ProcessList(dataList);
        }
    }
}
