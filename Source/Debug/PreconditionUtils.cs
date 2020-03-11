using System;

namespace EGoap.Source.Debug
{
    // Utilities for precondition handling
    public static class PreconditionUtils
    {
        private const string EnsureNotNullFormat = "{0} must not be null";
        private const string EnsureNotBlankFormat = "{0} must not be null or empty";

        public static T EnsureNotNull<T>(
            T value,
            string valueName,
            string format = EnsureNotNullFormat,
            params object[] args
        )
        {
            return value == null
                ? throw new ArgumentNullException(valueName, string.Format(format, valueName, args))
                : value;
        }

        public static string EnsureNotBlank(
            string value,
            string valueName,
            string format = EnsureNotBlankFormat,
            params object[] args
        )
        {
            if (value == null) throw new ArgumentNullException(valueName, string.Format(format, valueName, args));
            if (value.Length == 0) throw new ArgumentException(valueName, string.Format(format, valueName, args));
            return value;
        }
    }
}