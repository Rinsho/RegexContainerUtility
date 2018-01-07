using System;

namespace RegularExpression.Utility
{
    public class InvalidRegexDataException : Exception
    {
        public InvalidRegexDataException(string message) : base(message)
        { }
    }
}
