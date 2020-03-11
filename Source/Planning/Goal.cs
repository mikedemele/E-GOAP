using System.Collections.Generic;
using System.Linq;
using EGoap.Source.Debug;
using EGoap.Source.Planning.Preconditions;

namespace EGoap.Source.Planning
{
    // Planning goal
    public sealed class Goal
    {
        public Goal(string name, IEnumerable<IPrecondition> preconditions)
        {
            Name = PreconditionUtils.EnsureNotBlank(name, "name");
            Preconditions = new List<IPrecondition>(PreconditionUtils.EnsureNotNull(preconditions, "preconditions"));
        }

        public string Name { get; }
        
        public IEnumerable<IPrecondition> Preconditions { get; }

        public IEnumerable<SymbolId> PreconditionSymbols
        {
            get { return Preconditions.Select(precondition => precondition.SymbolId); }
        }

        public bool IsReachedIn(WorldState worldState)
        {
            return Preconditions.Aggregate(true,
                (soFar, precondition) => soFar && precondition.IsSatisfiedBy(worldState));
        }

        public double GetDistanceFrom(WorldState worldState)
        {
            return Preconditions.Sum(precondition => precondition.GetDistanceFrom(worldState));
        }
    }
}