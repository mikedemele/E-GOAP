using System;
using System.Linq;

using EGoap.Source.Planning;

namespace EGoap.Source.Agents
{
    // Interface for active execution
    public interface IPlanExecution
    {
        ExecutionStatus Status { get; }
        Plan Plan { get; }
        int CurrentActionIndex { get; }
    }

    public static class PlanExecutionExtensions
    {
        public static PlanningAction GetCurrentAction(this IPlanExecution execution)
        {
            if (!execution.Status.IsCurrentActionAvailable())
                throw new InvalidOperationException(
                    $"Current action cannot be retrieved when plan execution status is {execution.Status}"
                );
            return execution.Plan.Actions.ElementAt(execution.CurrentActionIndex);
        }
    }
}