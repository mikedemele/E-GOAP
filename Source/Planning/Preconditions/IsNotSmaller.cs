using System;

namespace EGoap.Source.Planning.Preconditions
{
    
    // Precondition, that checks, if a value is not smaller, than the precondition value
    [Serializable]
    public class IsNotSmaller : IsInRange
    {
        public IsNotSmaller(SymbolId symbolId, int targetValue)
            : base(symbolId, ValueRange.GreaterThanOrEqual(targetValue))
        {
        }
    }
}