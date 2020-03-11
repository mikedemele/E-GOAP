using System;

namespace EGoap.Source.Planning.Effects
{
    // Effect, that adds to a value
    [Serializable]
    public class Add : SingleSymbolEffect
    {
        private readonly int delta;

        public Add(SymbolId symbolId, int delta)
            : base(symbolId, initialValue => initialValue + delta )
        {
            this.delta = delta;
        }

        public override int? ValueDelta => delta;
    }
}