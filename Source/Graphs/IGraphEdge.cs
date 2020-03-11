using EGoap.Source.Planning;

namespace EGoap.Source.Graphs
{
	// Interface for graph edges
	public interface IGraphEdge<out TGraphNode> where TGraphNode : IGraphNode<TGraphNode>
    {
        double Cost { get; }
        
        TGraphNode SourceNode { get; }
        TGraphNode TargetNode { get; }

        PlanningAction GetAction();
    }
}