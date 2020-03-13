using System;
using System.Collections.Generic;
using System.Linq;

using EGoap.Source.Utils.Collections;
using EGoap.Source.Utils.Time;

namespace EGoap.Source.Graphs
{
	//This pathfinder uses A* algorithm to find paths on a graph.
	public class AStarPathfinder<TGraphNode> : IPathfinder<TGraphNode> where TGraphNode : IGraphNode<TGraphNode>
    {
        private readonly bool assumeNonNegativeCosts;

        private readonly PathCostHeuristic<TGraphNode> heuristic;
        private readonly int maxSearchDepth;
        private readonly float maxSecondsPerSearch;

        public AStarPathfinder(AStarPathfinderConfiguration<TGraphNode> configuration = null)
        {
            if (configuration == null) configuration = new AStarPathfinderConfiguration<TGraphNode>.Builder().Build();

            heuristic = configuration.Heuristic;
            maxSearchDepth = configuration.MaxSearchDepth;
            maxSecondsPerSearch = configuration.MaxSecondsPerSearch;
            assumeNonNegativeCosts = configuration.AssumeNonNegativeCosts;
        }

        //Maximum depth defines maximum number of edges, allowed in a path.
        //A negative value indicates no depth limit.
        public AStarPathfinder(
            PathCostHeuristic<TGraphNode> heuristic) : this(new AStarPathfinderConfiguration<TGraphNode>.Builder()
            .UseHeuristic(heuristic)
            .AssumeNonNegativeCosts()
            .Build())
        {
        }

        #region IPathfinder implementation

        public Path<TGraphNode> FindPath(
            TGraphNode sourceNode,
            TGraphNode targetNode,
            IEqualityComparer<TGraphNode> targetEqualityComparer,
            IEqualityComparer<TGraphNode> nodeEqualityComparer
        )
        {
            // If there is a limit on search time, we need to initialize the timer
            var timer = new StopwatchExecutionTimer();

            // Set of already visited nodes
            var closed = new HashSet<TGraphNode>(nodeEqualityComparer);
            // Priority queue of partial and potentially complete paths
            var open = new SmartPriorityQueue<PartialPath, double> {new PartialPath(heuristic(sourceNode, targetNode))};

            // Best known path to target in the open priority queue
            PartialPath bestPathToTarget = null;
            // Cost of the cheapest known path to target
            var minTotalCost = double.PositiveInfinity;

            // While there are unexplored nodes
            while (open.Count > 0)
            {
                // Pop partial path with the lowest estimated cost from the priority queue
                var currentPartialPath = open.PopFront();

                // Node to explore
                // If partial path is empty, we are at the source of our search.
                var currentNode = currentPartialPath.IsEmpty ? sourceNode : currentPartialPath.LastNode;
                if (closed.Contains(currentNode)) continue;

                if (targetEqualityComparer.Equals(currentNode, targetNode))
                {
                    UnityEngine.Debug.Log(
                        $"Path found of cost {currentPartialPath.CostSoFar} : {currentPartialPath.EdgeCount} edges; {closed.Count} nodes expanded"
                    );
                    return currentPartialPath.ToPath();
                }

                // If there is a time limit, check the timer.
                // If the time limit has been exceeded, return the best known path to target or,
                // otherwise break and report a failure.
                if (maxSecondsPerSearch < float.PositiveInfinity && timer.ElapsedSeconds > maxSecondsPerSearch)
                {
                    if (bestPathToTarget != null)
                    {
                        // Time out, best path so far
                        UnityEngine.Debug.Log(
                            $"Time out! Path found of cost {currentPartialPath.CostSoFar} : {currentPartialPath.EdgeCount} edges"
                        );
                        return bestPathToTarget.ToPath();
                    }

                    throw new PathfindingTimeoutException(GetType(), maxSecondsPerSearch);
                }

                // If we have a limit on max search depth, and current path is at max depth limit, 
                // don't add paths to currentNode's neighbours to the open priority queue
                if (maxSearchDepth >= 0 && currentPartialPath.EdgeCount >= maxSearchDepth) continue;
                if (assumeNonNegativeCosts && currentPartialPath.CostSoFar > minTotalCost) continue;

                // Mark currentNode as visited by placing it into closed set.
                closed.Add(currentNode);
                // Insert paths from sourceNode to currentNode's neighbours into the open priority queue
                foreach (var outgoingEdge in currentNode.OutgoingEdges)
                {
                    var neighbour = outgoingEdge.TargetNode;
                    var pathToNeighbour = new PartialPath(currentPartialPath);
                    pathToNeighbour.AppendEdge(outgoingEdge, heuristic(neighbour, targetNode));
                    open.Add(pathToNeighbour);
                    
                    // If there is a time limit, check if the neighbour is the target node
                    // and update bestPathToTarget, if needed.
                    // This allows us to keep track of the best path to target, found so far.
                    if (maxSecondsPerSearch < float.PositiveInfinity
                        && targetEqualityComparer.Equals(neighbour, targetNode)
                        && (bestPathToTarget == null ||
                            pathToNeighbour.EstimatedTotalCost < bestPathToTarget.CostSoFar))
                        bestPathToTarget = pathToNeighbour;

                    // If the assumption of non-negative costs is in effect, check if the neighbour
                    // is the target node and update minTotalCost, if needed.
                    if (assumeNonNegativeCosts && targetEqualityComparer.Equals(neighbour, targetNode))
                        minTotalCost = Math.Min(minTotalCost, pathToNeighbour.CostSoFar);
                }
            }

            UnityEngine.Debug.Log("No plan found");
            throw new NoPathExistsException(
                GetType(),
                maxSearchDepth
            );
        }
        #endregion

        // Partial path with estimated cost, comparable by estimated cost.
        private class PartialPath : IComparable<PartialPath>, IPriorityItem<double>
        {
            private readonly IList<IGraphEdge<TGraphNode>> edges;

            public PartialPath(double estimatedTotalCost)
            {
                edges = new List<IGraphEdge<TGraphNode>>();
                EstimatedTotalCost = estimatedTotalCost;
            }

            public PartialPath(PartialPath other)
            {
                edges = new List<IGraphEdge<TGraphNode>>(other.edges);
                EstimatedTotalCost = other.EstimatedTotalCost;
            }

            public double EstimatedTotalCost { get; private set; }

            public bool IsEmpty => edges.Count == 0;

            public int EdgeCount => edges.Count;

            public TGraphNode LastNode
            {
                get
                {
                    var lastEdge = edges.LastOrDefault();
                    if (lastEdge == null)
                        throw new InvalidOperationException();
                    return lastEdge.TargetNode;
                }
            }

            public double CostSoFar
            {
                get { return edges.Sum(edge => edge.Cost); }
            }

            #region IComparable implementation

            public int CompareTo(PartialPath other)
            {
                return EstimatedTotalCost.CompareTo(other.EstimatedTotalCost);
            }

            #endregion

            public void AppendEdge(IGraphEdge<TGraphNode> edge, double estimatedRemainingCost)
            {
                edges.Add(edge);
                EstimatedTotalCost = CostSoFar + estimatedRemainingCost;
            }

            public Path<TGraphNode> ToPath()
            {
                return new Path<TGraphNode>(edges);
            }

            public double GetPriority()
            {
                return EstimatedTotalCost;
            }
        }
    }
}