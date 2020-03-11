using System;
using System.Collections.Generic;

using EGoap.Source.Debug;
using EGoap.Source.Planning;

namespace EGoap.Source.Agents
{
    // Configuration for agent, with builder
    public sealed class AgentConfiguration
    {
        private AgentConfiguration(
            IGoalSelector goalSelector,
            IPlanner planner,
            IKnowledgeProvider knowledgeProvider,
            IPlanExecutor planExecutor,
            IReevaluationSensor reevaluationSensor = null
        )
        {
            GoalSelector = PreconditionUtils.EnsureNotNull(goalSelector, "goalSelector");
            Planner = PreconditionUtils.EnsureNotNull(planner, "planner");
            KnowledgeProvider = PreconditionUtils.EnsureNotNull(knowledgeProvider, "knowledgeProvider");
            PlanExecutor = PreconditionUtils.EnsureNotNull(planExecutor, "planExecutor");
            ReevaluationSensor = reevaluationSensor ?? new NullReevaluationSensor();
        }

        public IGoalSelector GoalSelector { get; }
        public IPlanner Planner { get; }
        public IKnowledgeProvider KnowledgeProvider { get; }
        public IPlanExecutor PlanExecutor { get; }
        public IReevaluationSensor ReevaluationSensor { get; }

        public IPlanExecution CurrentPlanExecution => PlanExecutor.CurrentExecution;

        public HashSet<PlanningAction> SupportedPlanningActions => PlanExecutor.SupportedPlanningActions;

        public class Builder
        {
            private IGoalSelector goalSelector;
            private IKnowledgeProvider knowledgeProvider;
            private IPlanExecutor planExecutor;
            private IPlanner planner;
            private IReevaluationSensor reevaluationSensor;

            public Builder WithGoalSelector(IGoalSelector selector)
            {
                goalSelector = PreconditionUtils.EnsureNotNull(selector, "goalSelector");
                return this;
            }

            public Builder WithPlanner(IPlanner givenPlanner)
            {
                planner = PreconditionUtils.EnsureNotNull(givenPlanner, "planner");
                return this;
            }

            public Builder WithKnowledgeProvider(IKnowledgeProvider provider)
            {
                knowledgeProvider = PreconditionUtils.EnsureNotNull(provider, "knowledgeProvider");
                return this;
            }

            public Builder WithPlanExecutor(IPlanExecutor executor)
            {
                planExecutor = PreconditionUtils.EnsureNotNull(executor, "planExecutor");
                return this;
            }

            public Builder WithReevaluationSensor(IReevaluationSensor sensor)
            {
                reevaluationSensor = sensor;
                return this;
            }

            public Builder WithLearnFromExperience(bool doLearn)
            {
                return this;
            }

            public AgentConfiguration Build()
            {
                try
                {
                    return new AgentConfiguration(goalSelector, planner, knowledgeProvider, planExecutor,
                        reevaluationSensor);
                }
                catch (ArgumentNullException)
                {
                    throw new InvalidOperationException(
                        $"{GetType()} hasn't been properly configured before the call to Build()"
                    );
                }
            }
        }
    }
}