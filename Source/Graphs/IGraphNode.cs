using System.Collections.Generic;
using System.Linq;

namespace EGoap.Source.Graphs
{
	public interface IGraphNode<TGraphNode> where TGraphNode : IGraphNode<TGraphNode>
    {
        IEnumerable<IGraphEdge<TGraphNode>> OutgoingEdges { get; }
    }

    public static class GraphNodeExtensions
    {
        public static IEnumerable<TGraphNode> GetNeighbours<TGraphNode>(
            this IGraphNode<TGraphNode> node
        ) where TGraphNode : IGraphNode<TGraphNode>
        {
            var neighbours = new List<TGraphNode>();

            neighbours.AddRange(
                (from edge in node.OutgoingEdges select edge.TargetNode).Distinct()
            );

            return neighbours;
        }
    }
}