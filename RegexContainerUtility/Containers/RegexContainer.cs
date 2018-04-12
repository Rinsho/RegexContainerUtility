using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using RegularExpression.Utility.Data;

[assembly: CLSCompliant(true)]

namespace RegularExpression.Utility
{
    [CLSCompliant(true)]
    public class RegexContainer<T> where T: new()
    {
        private static Regex _expression;
        private static List<DataContainer> _dataMembers;

        static RegexContainer()
        {
            _dataMembers = new List<DataContainer>();
            LoadMetadata();
        }

        public static void LoadMetadata()
        {
            ClearMetadata();
            TypeInfo containerTypeInfo = typeof(T).GetTypeInfo();
            ExtractContainerMetadata(containerTypeInfo);
        }

        public static void ClearMetadata()
        {
            _expression = null;
            _dataMembers.Clear();
        }

        private static void ExtractContainerMetadata(TypeInfo containerTypeInfo)
        {
            RegexContainerAttribute containerMetadata = containerTypeInfo.GetCustomAttribute<RegexContainerAttribute>();
            _expression = new Regex(containerMetadata.Pattern, containerMetadata.Options);
            ExtractFields(containerTypeInfo);
            ExtractProperties(containerTypeInfo);
        }

        private static void ExtractFields(TypeInfo containerTypeInfo)
        {
            IEnumerable<FieldInfo> fields = containerTypeInfo.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (FieldInfo field in fields)
                if (field.IsDefined(typeof(RegexDataAttribute)))
                    _dataMembers.Add(new DataContainer(field));
        }

        private static void ExtractProperties(TypeInfo containerTypeInfo)
        {
            IEnumerable<PropertyInfo> properties = containerTypeInfo.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo property in properties)
            {
                if (property.IsDefined(typeof(RegexDataAttribute)))
                {
                    if (!property.CanWrite)
                        throw new InvalidRegexDataException($"Property { property.Name } on type { typeof(T) } is not writeable.");
                    _dataMembers.Add(new DataContainer(property));
                }
            }
        }

        private ContainerResult<T> CreateContainer(Match match)
        {
            object container = new T();
            bool success = match.Success;
            if (success)
            {
                try
                {
                    foreach (DataContainer member in _dataMembers)
                        member.ProcessMatch(container, match);
                }
                catch (Exception ex) when (
                    ex is InvalidRegexDataException || 
                    ex is InvalidCastException || 
                    ex is FormatException ||
                    ex is OverflowException)
                {
                    success = false;
                }
            }
            return new ContainerResult<T>(container, success);
        }

        private ContainerResultCollection<T> CreateContainers(MatchCollection matches)
        {
            List<T> containers = new List<T>();
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    ContainerResult<T> container = CreateContainer(match);
                    if (container.Success)
                        containers.Add(container.Value);
                }
            }
            return new ContainerResultCollection<T>(containers);
        }

        public ContainerResult<T> Parse(string text)
        {
            Match match = _expression.Match(text);
            return CreateContainer(match);
        }

        public ContainerResultCollection<T> ParseAll(string text)
        {
            MatchCollection matches = _expression.Matches(text);
            return CreateContainers(matches);
        }
    }
}
