using System;
using System.Text.RegularExpressions;

namespace RegularExpression.Utility
{
    [CLSCompliant(true)]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public sealed class RegexContainerAttribute : Attribute
    {
        public string Pattern { get; }
        public RegexOptions Options { get; set; } = RegexOptions.None;

        public RegexContainerAttribute(string regexPattern) =>
            Pattern = regexPattern;
    }
}
