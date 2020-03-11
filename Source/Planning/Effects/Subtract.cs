using System;

namespace EGoap.Source.Planning.Effects
{
    // Effect, that subtracts from a value
    [Serializable]
    public class Subtract : SingleSymbolEffect
    {
        private readonly int delta;

        public Subtract(SymbolId symbolId, int delta)
            : base(symbolId, initialValue => initialValue - delta)
        {
            this.delta = delta;
        }

        public override int? ValueDelta => -delta;
    }
}