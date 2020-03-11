using System;

namespace EGoap.Source.Planning.Preconditions
{
    // Precondition, that checks for a range of values
    [Serializable]
    public class IsInRange : IPrecondition
    {
        public IsInRange(SymbolId symbolId, ValueRange range)
        {
            SymbolId = symbolId;
            AsValueRange = range;
        }

        public override string ToString()
        {
            return $"{SymbolId} in {AsValueRange}";
        }

        #region IPrecondition implementation

        public bool IsSatisfiedBy(WorldState worldState)
        {
            return AsValueRange.Contains(worldState[SymbolId]);
        }

        public double GetDistanceFrom(WorldState worldState)
        {
            var value = worldState[SymbolId];
            return AsValueRange.AbsDistanceFrom(value);
        }

        public ValueRange AsValueRange { get; }

        public SymbolId SymbolId { get; }

        public bool IsSatisfiable => !AsValueRange.IsEmpty;

        #endregion
    }
}