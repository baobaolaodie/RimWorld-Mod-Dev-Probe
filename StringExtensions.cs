using System;

namespace RimWorldModDevProbe
{
    public static class StringExtensions
    {
        public static bool Contains(this string source, string value, StringComparison comparison)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(value))
                return false;
            return source.IndexOf(value, comparison) >= 0;
        }
    }
}
