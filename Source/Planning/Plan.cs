using System.Collections.Generic;
using System.Linq;

using EGoap.Source.Debug;

namespace EGoap.Source.Planning
{
    // A completed plan
    public sealed class Plan
    {
        public Plan(IEnumerable<PlanningAction> actions, Goal goal)
        {
            var actionList = actions.ToList();
            PrintRawPlan(actionList);
            for (var i = 0; i < actionList.Count; i++)
            {
                var action = actionList[i];
                if (action.IsExperience)
                {
                    actionList.Remove(action);
                    actionList.InsertRange(i, ((ExperienceAction) action).Actions);
                }
            }

            Actions = new List<PlanningAction>(PreconditionUtils.EnsureNotNull(actionList, "actions")).AsReadOnly();
            Goal = PreconditionUtils.EnsureNotNull(goal, "goal");
        }

        public IEnumerable<PlanningAction> Actions { get; }

        public Goal Goal { get; }

        public int Length => Actions.Count();

        public double Cost
        {
            get { return Actions.Sum(action => action.Cost); }
        }

        public override string ToString()
        {
            return PrintRawPlan(Actions);
        }

        private static string PrintRawPlan(IEnumerable<PlanningAction> list)
        {
            var result = "";
            var first = true;
            foreach (var actionName in list.Select(action => action.Name))
            {
                if (!first) result += " -> ";
                result += actionName;
                first = false;
            }

            return result;
        }
    }
}