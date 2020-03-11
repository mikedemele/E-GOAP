namespace EGoap.Source.Graphs
{
	// This is the required signature for path cost heuristic functions,
	// utilized by pathfinders.
	public delegate double PathCostHeuristic<in TGraphNode>(
        TGraphNode sourceNode,
        TGraphNode targetNode
    ) where TGraphNode : IGraphNode<TGraphNode>;
}