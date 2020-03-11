using System.Collections.Generic;

namespace EGoap.Source.Graphs
{
	//This interface describes classes, capable of finding paths on generalized graphs.
	public interface IPathfinder<TGraphNode> where TGraphNode : IGraphNode<TGraphNode>
    {
        //Finds a path between the given source and target nodes on a graph.
        Path<TGraphNode> FindPath(
            TGraphNode sourceNode,
            TGraphNode targetNode,
            IEqualityComparer<TGraphNode> targetEqualityComparer,
            IEqualityComparer<TGraphNode> nodeEqualityComparer
        );
    }

    public static class PathfinderExtensions
    {
        private static Path<TGraphNode> FindPath<TGraphNode>(
            this IPathfinder<TGraphNode> pathfinder,
            TGraphNode sourceNode,
            TGraphNode targetNode,
            IEqualityComparer<TGraphNode> targetEqualityComparer
        ) where TGraphNode : IGraphNode<TGraphNode>
        {
            return pathfinder.FindPath(
                sourceNode,
                targetNode,
                targetEqualityComparer,
                EqualityComparer<TGraphNode>.Default
            );
        }

        public static Path<TGraphNode> FindPath<TGraphNode>(
            this IPathfinder<TGraphNode> pathfinder,
            TGraphNode sourceNode,
            TGraphNode targetNode
        ) where TGraphNode : IGraphNode<TGraphNode>
        {
            return pathfinder.FindPath(
                sourceNode,
                targetNode,
                EqualityComparer<TGraphNode>.Default
            );
        }
    }
}