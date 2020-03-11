using System;
using System.Collections.Generic;
using System.Linq;

using EGoap.Source.Debug;
using EGoap.Source.Planning;

namespace EGoap.Source.Agents
{
    //Base executor for plans
    public class PlanExecutor : IPlanExecutor, IPlanExecution
    {
        private readonly IActionFactory actionFactory;

        // currently performed action
        private IAction currentAction;

        public PlanExecutor(IActionFactory actionFactory)
        {
            this.actionFactory = PreconditionUtils.EnsureNotNull(actionFactory, "actionFactory");

            Status = ExecutionStatus.None;
            Plan = null;
            CurrentActionIndex = -1;

            currentAction = null;
        }

        #region IPlanExecutor implementation

        public void SubmitForExecution(Plan plan)
        {
            if (!Status.IsFinal() && Status != ExecutionStatus.None)
                throw new InvalidOperationException(
                    $"SubmitForExecution() was called before current plan was fully completed, failed or interrupted (plan status: {Status})"
                );

            Plan = plan;

            if (plan.Length > 0)
            {
                Status = ExecutionStatus.InProgress;
                CurrentActionIndex = 0;

                currentAction = actionFactory.FromPlanningAction(plan.Actions.First().Name);
            }
            else
            {
                Status = ExecutionStatus.Complete;
                CurrentActionIndex = -1;

                currentAction = null;
            }
        }

        public void InterruptExecution()
        {
            if (Plan == null)
                throw new InvalidOperationException(
                    $"InterruptExecution() cannot be called when {GetType()} has no current plan");

            if (Status == ExecutionStatus.InProgress)
            {
                Status = ExecutionStatus.InInterruption;

                if (currentAction.Status == ExecutionStatus.InProgress) currentAction.StartInterruption();
            }
        }

        public void Update()
        {
            if (Status != ExecutionStatus.None && !Status.IsFinal())
            {
                DebugUtils.Assert(currentAction != null, "currentAction must not be null at this point");
                if (currentAction.Status == ExecutionStatus.None) currentAction.StartExecution();

                currentAction.Update();

                switch (currentAction.Status)
                {
                    case ExecutionStatus.Interrupted:
                    case ExecutionStatus.Failed:
                        Status = currentAction.Status;
                        break;
                    case ExecutionStatus.Complete:
                    {
                        CurrentActionIndex++;

                        if (CurrentActionIndex >= Plan.Length)
                        {
                            currentAction = null;
                            Status = ExecutionStatus.Complete;
                        }
                        else
                        {
                            currentAction =
                                actionFactory.FromPlanningAction(Plan.Actions.ElementAt(CurrentActionIndex).Name);
                        }

                        break;
                    }
                    case ExecutionStatus.None:
                    case ExecutionStatus.InProgress:
                    case ExecutionStatus.InInterruption:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public IPlanExecution CurrentExecution => this;

        #endregion

        #region IPlanExecution implementation

        public ExecutionStatus Status { get; private set; }

        public Plan Plan { get; private set; }

        public int CurrentActionIndex { get; private set; }

        public HashSet<PlanningAction> SupportedPlanningActions => actionFactory.SupportedPlanningActions;

        #endregion
    }
}