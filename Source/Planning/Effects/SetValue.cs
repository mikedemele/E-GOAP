using System;

namespace EGoap.Source.Planning.Effects
{
    // Effect, that sets a value to the specified value
    [Serializable]
    public class SetValue : SingleSymbolEffect
    {
        private readonly int newValue;

        public SetValue(SymbolId symbolId, int newValue)
            : base(symbolId, initialValue => newValue)
        {
            this.newValue = newValue;
        }

        public override int? ValueAssigned => newValue;
    }
}