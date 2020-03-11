namespace EGoap.Source.Planning.Internal
{
    internal class RegressiveStatePopulator : IRegressiveStatePopulator
    {
        #region IRegressiveStatePopulator implementation

        public RegressiveState Populate(Goal goal, RegressiveState initialState = default)
        {
            var builder = initialState.BuildUpon();
            foreach (var precondition in goal.Preconditions)
                // IntersectRange() has the effect of merging preconditions on the same symbol
                builder.IntersectRange(precondition.SymbolId, precondition.AsValueRange);
            return builder.Build();
        }

        #endregion
    }
}