using System;

namespace RegularExpression.Utility
{
    [CLSCompliant(true)]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class RegexDataListAttribute : Attribute
    {
        public char Delimiter { get; }
        public bool TrimWhitespace { get; set; } = true;

        public RegexDataListAttribute(char delimiter) =>
            Delimiter = delimiter;
    }
}
