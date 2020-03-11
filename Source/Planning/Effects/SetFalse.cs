using System;
using EGoap.Source.Planning.Preconditions;

namespace EGoap.Source.Planning.Effects
{
    // Effect, that sets a value to the "false" state
    [Serializable]
    public class SetFalse : SingleSymbolEffect
    {
        public SetFalse(SymbolId symbolId)
            : base(symbolId, initialValue => IsFalse.False)
        {
        }

        public override int? ValueAssigned => IsFalse.False;
    }
}