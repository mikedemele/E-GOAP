using System;

namespace EGoap.Source.Planning.Preconditions
{
    // Precondition, that checks, if a value is "false"
    [Serializable]
    public class IsFalse : IsInRange
    {
        internal const int False = 0;
        public IsFalse(SymbolId symbolId)
            
            : base(symbolId, ValueRange.Exactly(False))
        {
        }
    }
}