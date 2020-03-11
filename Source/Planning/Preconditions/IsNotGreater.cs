using System;

namespace EGoap.Source.Planning.Preconditions
{
    // Precondition, that checks, if a value is not greater, than the precondition value
    [Serializable]
    public class IsNotGreater : IsInRange
    {
        public IsNotGreater(SymbolId symbolId, int targetValue)
            : base(symbolId, ValueRange.LessThanOrEqual(targetValue))
        {
        }
    }
}