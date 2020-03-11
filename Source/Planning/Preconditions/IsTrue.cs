using System;

namespace EGoap.Source.Planning.Preconditions
{
    // Precondition, that checks, if a value is "true"
    [Serializable]
    public sealed class IsTrue : IsInRange
    {
        internal const int True = 10;

        public IsTrue(SymbolId symbolId)
            : base(symbolId, ValueRange.Exactly(True))
        {
        }
    }
}