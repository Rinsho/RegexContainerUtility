using System;

namespace RegularExpression.Utility.Data
{
    internal class RegexContainerProcessor<T> : IDataProcessor where T : new()
    {
        private RegexContainer<T> _container = new RegexContainer<T>();

        public object Process(string data)
        {
            ContainerResult<T> result = _container.Parse(data);
            if (!result.Success)
                throw new InvalidRegexDataException($"Data '{data}' does not match regex for container type {nameof(T)}");
            return result.Value;
        }
    }
}
