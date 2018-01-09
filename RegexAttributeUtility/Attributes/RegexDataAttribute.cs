using System;

namespace RegularExpression.Utility
{
    [CLSCompliant(true)]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class RegexDataAttribute : Attribute
    {
        public string MatchID { get; set; } = string.Empty;
    }
}
