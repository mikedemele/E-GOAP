using System.Collections.Generic;
using System.Linq;

using EGoap.Source.Debug;
using EGoap.Source.Planning;
using EGoap.Source.Planning.Preconditions;
using EGoap.Source.Utils.Time;

namespace EGoap.Source.Agents
{
    //Evaluates the relevance of goals
    public delegate double GoalEvaluator();

    //Base goal selector
    public class GoalSelector : IGoalSelector
    {
        //Available goals
        private const float DefaultReevaluationPeriod = 0.0f;
        private static readonly Goal IdleGoal = new Goal("Idle", new List<IPrecondition>());
        private readonly IDictionary<Goal, GoalEvaluator> evaluators;
        private IEnumerable<Goal> relevantGoals;

        //Reevaluation timing
        private readonly float reevaluationPeriod;
        private readonly IResettableTimer reevaluationTimer;

        private GoalSelector(IDictionary<Goal, GoalEvaluator> evaluators,
            float reevaluationPeriod = DefaultReevaluationPeriod)
        {
            this.evaluators = PreconditionUtils.EnsureNotNull(evaluators, "evaluators");
            this.reevaluationPeriod = reevaluationPeriod;
            reevaluationTimer = new ResettableStopwatchExecutionTimer(false);

            ForceReevaluation();
        }

        public class Builder
        {
            private readonly IDictionary<Goal, GoalEvaluator> evaluators;
            private float reevaluationPeriod;

            public Builder()
            {
                evaluators = new Dictionary<Goal, GoalEvaluator>();
                reevaluationPeriod = DefaultReevaluationPeriod;
            }

            public Builder WithGoal(Goal goal, GoalEvaluator evaluator)
            {
                evaluators.Add(goal, evaluator);
                return this;
            }

            public Builder WithReevaluationPeriod(float reEvaluationPeriod)
            {
                reevaluationPeriod = reEvaluationPeriod;
                return this;
            }

            public GoalSelector Build()
            {
                return new GoalSelector(
                    evaluators,
                    reevaluationPeriod
                );
            }
        }

        #region IGoalSelector implementation

        public IEnumerable<Goal> RelevantGoals
        {
            get
            {
                if (relevantGoals == null || reevaluationTimer.ElapsedSeconds > reevaluationPeriod) ForceReevaluation();
                return relevantGoals;
            }
        }

        public Goal FallbackGoal => IdleGoal;

        public void ForceReevaluation()
        {
            relevantGoals = from relevanceKvp in from evalKvp in evaluators
                    select new KeyValuePair<Goal, double>(evalKvp.Key, evalKvp.Value())
                where relevanceKvp.Value > 0
                orderby relevanceKvp.Value descending
                select relevanceKvp.Key;

            reevaluationTimer.Reset(false);
        }

        #endregion
    }
}