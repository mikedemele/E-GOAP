using System;
using System.Collections.Generic;
using System.Linq;

using EGoap.Source.Debug;
using EGoap.Source.Planning;

namespace EGoap.Source.Agents
{
    // Simple implementation of action factory
    public class SimpleActionFactory : IActionFactory
    {
        private readonly IDictionary<string, Func<IAction>> actionFactories;

        public SimpleActionFactory(
            IDictionary<PlanningAction, Func<IAction>> actionFactories
        )
        {
            PreconditionUtils.EnsureNotNull(actionFactories, "actionFactories");

            this.actionFactories = actionFactories.ToDictionary(kvp => kvp.Key.Name, kvp => kvp.Value);
            SupportedPlanningActions = new HashSet<PlanningAction>(actionFactories.Keys);
        }

        #region IActionFactory implementation

        public IAction FromPlanningAction(string planningActionName)
        {
            return actionFactories[planningActionName]();
        }

        public HashSet<PlanningAction> SupportedPlanningActions { get; }

        #endregion
    }
}