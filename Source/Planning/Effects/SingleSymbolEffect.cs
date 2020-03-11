using System;

using EGoap.Source.Debug;

namespace EGoap.Source.Planning.Effects
{
    // Effect, that affects a single value 
    [Serializable]
    public abstract class SingleSymbolEffect : IEffect
    {
        private readonly Func<int, int> effectApplication;

        protected SingleSymbolEffect(SymbolId symbolId,
            Func<int, int> effectApplication)
        {
            SymbolId = symbolId;
            this.effectApplication = PreconditionUtils.EnsureNotNull(effectApplication, "effectApplication");
        }

        public WorldState ApplyTo(WorldState initialState)
        {
            return initialState.BuildUpon()
                .SetSymbol(SymbolId, effectApplication(initialState[SymbolId]))
                .Build();
        }

        public SymbolId SymbolId { get; }

        public virtual int? ValueAssigned => null;

        public virtual int? ValueDelta => null;
    }
}