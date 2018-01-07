using System;

namespace RegularExpression.Utility
{
    public class ContainerResult<T>
    {
        public bool Success { get; }
        public T Value { get; }

        public ContainerResult(T container, bool success)
        {
            Success = success;
            Value = container;
        }
    }
}
