using System;

using EGoap.Source.Planning.Preconditions;

namespace EGoap.Source.Planning.Effects
{
    // Effect, that sets a value to the "true" state
    [Serializable]
    public class SetTrue : SingleSymbolEffect
    {
        public SetTrue(SymbolId symbolId)
            : base(symbolId, initialValue => IsTrue.True)
        {
        }

        public override int? ValueAssigned => IsTrue.True;
    }
}