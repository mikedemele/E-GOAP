using System.Diagnostics;

namespace EGoap.Source.Debug
{
    // Utilities for debugging
    public static class DebugUtils
    {
        private const string AssertionMessage = "DEBUG ASSERTION FAILED: {0}";

        // Check the specified condition.
        // Calls to this method will be omitted from Release builds.
        [Conditional("DEBUG")]
        public static void Assert(bool condition, string format, params object[] args)
        {
            System.Diagnostics.Debug.Assert(
                condition,
                string.Format(
                    AssertionMessage,
                    string.Format(
                        format,
                        args
                    )
                )
            );
        }
    }
}