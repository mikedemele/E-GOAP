using System.Collections.Generic;

using EGoap.Source.Planning;

namespace EGoap.Source.Agents
{
    // Interface for plan executor
    public interface IPlanExecutor
    {
        // Currently active execution
        IPlanExecution CurrentExecution { get; }

        HashSet<PlanningAction> SupportedPlanningActions { get; }
        void SubmitForExecution(Plan plan);
        void InterruptExecution();

        void Update();
    }
}