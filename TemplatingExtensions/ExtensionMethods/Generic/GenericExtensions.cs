using System;

namespace TemplatingExtensions.ExtensionMethods.Generic
{
    public static class GenericExtensions
    {
        // <summary>
        // Extension to default String.Contains(). Allows you to do case insensitive comparisons more easily
        // </summary>
        public static bool Contains(this string source, string value, StringComparison comparisonType)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(value))
            {
                return false;
            }

            return source.IndexOf(value, comparisonType) >= 0;
        }
    }
}