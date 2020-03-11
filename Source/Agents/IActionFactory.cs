using System.Collections.Generic;

using EGoap.Source.Planning;

namespace EGoap.Source.Agents
{
    //Factory for actions
    public interface IActionFactory
    {
        HashSet<PlanningAction> SupportedPlanningActions { get; }

        IAction FromPlanningAction( string planningActionName);
    }
}