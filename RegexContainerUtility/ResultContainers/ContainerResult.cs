using System;

namespace RegularExpression.Utility
{
    [CLSCompliant(true)]
    public class ContainerResult<T>
    {
        public bool Success { get; }
        public T Value { get; }

        public ContainerResult(object container, bool success)
        {
            Success = success;
            Value = (T)container;
        }
    }
}
