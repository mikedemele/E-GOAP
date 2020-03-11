﻿using System.Collections.Generic;
using System.Linq;
using EGoap.Source.Debug;

namespace EGoap.Source.Planning.Internal
{
    internal class RelevantSymbolsPopulator : IWorldStatePopulator
    {
        private readonly IEnumerable<SymbolId> relevantSymbols;

        public RelevantSymbolsPopulator(IEnumerable<PlanningAction> availableActions, Goal goal)
        {
            relevantSymbols = PreconditionUtils.EnsureNotNull(availableActions, "availableActions").Aggregate(
                new List<SymbolId>(),
                (soFar, action) =>
                {
                    soFar.AddRange(action.GetRelevantSymbols());
                    return soFar;
                }
            ).Concat(PreconditionUtils.EnsureNotNull(goal, "goal").PreconditionSymbols);
        }

        #region IWorldStatePopulator implementation

        public WorldState PopulateWorldState(IKnowledgeProvider knowledgeProvider,
            WorldState initialWorldState = default)
        {
            PreconditionUtils.EnsureNotNull(knowledgeProvider, "knowledgeProvider");

            var builder = initialWorldState.BuildUpon();
            foreach (var symbolId in relevantSymbols)
                if (!initialWorldState.Contains(symbolId))
                    builder.SetSymbol(symbolId, knowledgeProvider.GetSymbolValue(symbolId));
            return builder.Build();
        }

        #endregion
    }
}