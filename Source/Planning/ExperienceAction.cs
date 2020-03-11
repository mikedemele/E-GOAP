using System;
using System.Collections.Generic;
using System.Linq;

namespace EGoap.Source.Planning
{
	// A successfully learned sequence of actions with supporting data for use in an experience graph
	[Serializable]
	public sealed class ExperienceAction : PlanningAction
	{
		// The sequence of actions
		public List<PlanningAction> Actions { get; }
		// The state reached by the sequence of actions
		public WorldState TargetState { get; }
		
		// The state, the action can be used in
		public WorldState StartState { get; }
		public ExperienceAction(WorldState worldState, params PlanningAction[] actionList) : base(actionList.FirstOrDefault()?.Name, true)
		{
			StartState = new WorldState(worldState);
			Actions = actionList.ToList();
			Preconditions = worldState.ToPreconditions();
			var affected = new HashSet<SymbolId>();
			var cost = 0.0;
			foreach (var action in actionList)
			{
				worldState = action.Apply(worldState);
				foreach (var symbol in action.AffectedSymbols)
				{
					affected.Add(symbol);
				}

				cost += action.Cost;
			}
			Effects = worldState.ToEffects().Where(effect => affected.Contains(effect.SymbolId));
			TargetState = worldState;
			Cost = cost;
		}

		public override bool Equals(object other)
		{
			if (ReferenceEquals(this, other)) return true;
			return other is ExperienceAction action && Equals(StartState, action.StartState) && Equals(Actions, action.Actions);
		}

		public override int GetHashCode()
		{
			return StartState.GetHashCode() + Actions.GetHashCode();
		}

		public override string ToString()
		{
			var text = "";
			var first = true;
			foreach(var actionName in Actions.Select(action => action.Name))
			{
				if(!first)
				{
					text += " -> ";
				}
				text += actionName;
				first = false;
			}

			return "EX{" + text + "}";
		}
	}
}

