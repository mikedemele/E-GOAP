using EGoap.Source.Debug;
using EGoap.Source.Graphs;

namespace EGoap.Source.Planning.Internal
{
    internal class RegressiveEdge : IGraphEdge<RegressiveNode>
    {
        public readonly PlanningAction Action;

        public RegressiveEdge(PlanningAction action, RegressiveNode sourceNode, RegressiveNode targetNode)
        {
            Action = PreconditionUtils.EnsureNotNull(action, "action");
            SourceNode = sourceNode;
            TargetNode = targetNode;
        }

        #region IGraphEdge implementation

        public double Cost => Action.Cost;

        public PlanningAction GetAction()
        {
            return Action;
        }

        public RegressiveNode SourceNode { get; }

        public RegressiveNode TargetNode { get; }

        #endregion
    }
}