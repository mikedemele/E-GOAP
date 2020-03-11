using System;
using System.Runtime.Serialization;

namespace EGoap.Source.Planning
{
    [Serializable]
    public class PlanNotFoundException : Exception
    {
        private const string MessageTemplate =
            "{0} failed to find a plan, containing at most {1} actions, to satisfy the goal \"{2}\"{3}";
        private const string DetailsTemplate = ": {0}";
        
        public PlanNotFoundException(IPlanner planner, int maxPlanLength, Goal goal, Exception inner = null)
            : this(planner, maxPlanLength, goal, null, inner)
        {
        }
        
        public PlanNotFoundException(IPlanner planner, int maxPlanLength, Goal goal, string details,
            Exception inner = null)
            : base(
                string.Format(
                    MessageTemplate,
                    planner.GetType(),
                    maxPlanLength,
                    goal.Name,
                    details != null ? string.Format(DetailsTemplate, details) : ""
                ),
                inner
            )
        {
            Planner = planner;
            MaxPlanLength = maxPlanLength;
            Goal = goal;
        }

        protected PlanNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public IPlanner Planner { get; }
        public int MaxPlanLength { get; }
        public Goal Goal { get; }
    }
}