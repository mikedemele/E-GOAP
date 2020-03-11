using System;

namespace EGoap.Source.Planning.Preconditions
{
    // Equality precondition
    [Serializable]
    public class IsEqual : IsInRange
    {
        public IsEqual(SymbolId id, int targetValue)
            : base(id, ValueRange.Exactly(targetValue))
        {
        }
    }
}