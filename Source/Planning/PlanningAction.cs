using System;
using System.Collections.Generic;
using System.Linq;

using EGoap.Source.Debug;
using EGoap.Source.Planning.Effects;
using EGoap.Source.Planning.Preconditions;

namespace EGoap.Source.Planning
{
    // Action for a planner
    [Serializable]
    public class PlanningAction
    {
        protected PlanningAction(string name, bool isExperienceAction)
        {
            IsExperience = isExperienceAction;
            Name = PreconditionUtils.EnsureNotBlank(name, "name");
        }

        public PlanningAction(string name, IEnumerable<IPrecondition> preconditions, IEnumerable<IEffect> effects,
            double cost)
        {
            IsExperience = false;
            Name = PreconditionUtils.EnsureNotBlank(name, "name");
            Preconditions = new List<IPrecondition>(PreconditionUtils.EnsureNotNull(preconditions, "preconditions"));
            Effects = new List<IEffect>(PreconditionUtils.EnsureNotNull(effects, "effects"));
            Cost = cost;
        }

        public string Name { get; }

        public IEnumerable<IPrecondition> Preconditions { get; protected set; }

        public IEnumerable<IEffect> Effects { get; protected set; }

        public IEnumerable<SymbolId> PreconditionSymbols
        {
            get { return Preconditions.Select(precondition => precondition.SymbolId); }
        }

        public IEnumerable<SymbolId> AffectedSymbols
        {
            get { return Effects.Select(effect => effect.SymbolId); }
        }

        public double Cost { get; protected set; }

        public bool IsExperience { get; }

        public bool IsAvailableIn(WorldState worldState)
        {
            return Preconditions.Aggregate(true,
                (soFar, precondition) => soFar && precondition.IsSatisfiedBy(worldState));
        }

        public WorldState Apply(WorldState initialState)
        {
            return Effects.Aggregate(initialState, (soFar, effect) => effect.ApplyTo(soFar));
        }

        public override string ToString()
        {
            return Name + (IsExperience ? "(EX)" : "");
        }
    }

    [Serializable]
    public static class ActionExtensions
    {
        public static IEnumerable<SymbolId> GetRelevantSymbols(this PlanningAction action)
        {
            return action.PreconditionSymbols.Concat(action.AffectedSymbols);
        }
    }
}