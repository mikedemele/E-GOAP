using EGoap.Source.Debug;
using EGoap.Source.Graphs;

namespace EGoap.Source.Planning.Internal
{
    internal class ForwardEdge : IGraphEdge<ForwardNode>
    {
        public readonly PlanningAction Action;

        public ForwardEdge(PlanningAction action, ForwardNode sourceNode, ForwardNode targetNode)
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

        public ForwardNode SourceNode { get; }

        public ForwardNode TargetNode { get; }

        #endregion
    }
}