using System;

using EGoap.Source.Debug;

namespace EGoap.Source.Planning.Preconditions
{
    // A sample custom precondition
    [Serializable]
    public class SamplePrecondition : IPrecondition
    {
        private readonly Func<int, double> distanceMetric;
        private readonly Predicate<int> predicate;

        public SamplePrecondition(SymbolId symbolId, Predicate<int> predicate, Func<int, double> distanceMetric = null)
        {
            SymbolId = symbolId;
            this.predicate = PreconditionUtils.EnsureNotNull(predicate, nameof(predicate));
            this.distanceMetric = distanceMetric;
        }

        #region IPrecondition implementation

        public bool IsSatisfiedBy(WorldState worldState)
        {
            return predicate(worldState[SymbolId]);
        }

        public double GetDistanceFrom(WorldState worldState)
        {
            return distanceMetric?.Invoke(worldState[SymbolId]) ?? (IsSatisfiedBy(worldState)
                       ? 0.0
                       : 1.0);
        }

        public SymbolId SymbolId { get; }

        public ValueRange AsValueRange => throw new NotImplementedException();

        public bool IsSatisfiable => true;

        #endregion
    }
}