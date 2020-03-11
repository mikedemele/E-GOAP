using System.Collections.Generic;
using System.Linq;

namespace EGoap.Source.Graphs
{
	// Path between two nodes of an abstract graph.
	public sealed class Path<TGraphNode> where TGraphNode : IGraphNode<TGraphNode>
    {
        public Path(IEnumerable<IGraphEdge<TGraphNode>> edges)
        {
            System.Diagnostics.Debug.Assert(edges != null, nameof(edges) + " != null");
            Edges = new List<IGraphEdge<TGraphNode>>(edges);
            Cost = edges.Sum(edge => edge.Cost);
        }

        // Total path cost, sum of costs of all of the edges.
        public double Cost { get; }

        // Ordered collection of edges, which form the path.
        public IEnumerable<IGraphEdge<TGraphNode>> Edges { get; }
    }
}