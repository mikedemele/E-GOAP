using System;
using System.Linq;

using EGoap.Source.Debug;
using EGoap.Source.Planning;
using EGoap.Source.Utils.Time;

namespace EGoap.Source.Agents
{
    // Base planning agent
    public class Agent : IAgent
    {
        private readonly AgentConfiguration env;

        public Agent(AgentConfiguration configuration)
        {
            env = PreconditionUtils.EnsureNotNull(configuration, nameof(configuration));
            CurrentGoal = configuration.GoalSelector.FallbackGoal;
        }

        public void Update()
        {
            switch (env.CurrentPlanExecution.Status)
            {
                // If the plan has been completed, interrupted or there is no plan yet,
                // select a new goal, plan for it and execute the plan.
                case ExecutionStatus.None:
                case ExecutionStatus.Complete:
                // If the execution of the current plan has failed, attempt to re-plan for the same goal,
                // if re-planning fails, select a new goal and proceed with it.
                case ExecutionStatus.Interrupted:
                    // If the plan was interrupted, force goal reevaluation and plan for the most relevant goal,
                    // otherwise just plan for the most relevant goal.
                    AchieveRelevantGoal(env.CurrentPlanExecution.Status == ExecutionStatus.Interrupted);
                    break;
                case ExecutionStatus.Failed:
                    try
                    {
                        // Attempt to re-plan for the current goal
                        env.PlanExecutor.SubmitForExecution(
                            env.Planner.FormulatePlan(env.KnowledgeProvider, env.SupportedPlanningActions, CurrentGoal)
                        );
                    }
                    catch (PlanNotFoundException)
                    {
                        // Find the most relevant goal and plan for it
                        AchieveRelevantGoal(false);
                    }

                    break;
                case ExecutionStatus.InProgress:
                {
                    // If reevaluation sensor fires, force goal reevaluation and,
                    // if a different goal is selected than the one currently pursued,
                    // interrupt the current plan execution.
                    if (env.ReevaluationSensor.IsReevaluationNeeded)
                    {
                        env.GoalSelector.ForceReevaluation();
                        var mostRelevantGoal = env.GoalSelector.RelevantGoals.FirstOrDefault();
                        if (mostRelevantGoal != null
                            && !env.PlanExecutor.CurrentExecution.Plan.Goal.Equals(mostRelevantGoal))
                            env.PlanExecutor.InterruptExecution();
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            env.PlanExecutor.Update();
        }

        public Goal CurrentGoal { get; private set; }

        //Find a plan for the provided goal
        private Plan GetPlanFor(Goal goal)
        {
            var timer = new StopwatchExecutionTimer();
            var plan = env.Planner.FormulatePlan(env.KnowledgeProvider, env.SupportedPlanningActions, goal);
            UnityEngine.Debug.Log(
                $"{env.Planner.GetType()} has found a plan of length {plan.Length} and cost {plan.Cost} to satisfy \"{goal.Name}\" in {timer.ElapsedSeconds} seconds"
            );
            return plan;
        }

        // Find plan for the most relevant goal, where a plan, can be found and start execution
        private void AchieveRelevantGoal(bool forceReevaluation)
        {
            if (forceReevaluation) env.GoalSelector.ForceReevaluation();

            var relevantGoals = env.GoalSelector.RelevantGoals;
            var planSubmitted = false;
            foreach (var goal in relevantGoals)
                try
                {
                    env.PlanExecutor.SubmitForExecution(GetPlanFor(goal));
                    CurrentGoal = goal;
                    planSubmitted = true;
                    break;
                }
                catch (PlanNotFoundException)
                {
                    // UnityEngine.Debug.Log($"No plan not found for {CurrentGoal}");
                }

            // If no relevant goal can be achieved, use the default goal.
            if (!planSubmitted) env.PlanExecutor.SubmitForExecution(GetPlanFor(env.GoalSelector.FallbackGoal));
        }
    }
}