using System.Collections.Generic;

using EGoap.Source.Debug;
using EGoap.Source.Planning;

namespace EGoap.Source.Graphs
{
    // Configuration for A* 
    public class AStarPathfinderConfiguration<TGraphNode> where TGraphNode : IGraphNode<TGraphNode>
    {
        private const int UnlimitedSearchDepth = -1;
        private const float UnlimitedSecondsPerSearch = float.PositiveInfinity;

        private const bool DefaultAssumeNonNegativeCosts = true;

        private AStarPathfinderConfiguration(
            PathCostHeuristic<TGraphNode> heuristic,
            int maxSearchDepth = UnlimitedSearchDepth,
            float maxSecondsPerSearch = UnlimitedSecondsPerSearch,
            bool assumeNonNegativeCosts = DefaultAssumeNonNegativeCosts
        )
        {
            Heuristic = PreconditionUtils.EnsureNotNull(heuristic, "heuristic must not be null");
            MaxSearchDepth = maxSearchDepth;
            MaxSecondsPerSearch = maxSecondsPerSearch;
            AssumeNonNegativeCosts = assumeNonNegativeCosts;
        }

        public PathCostHeuristic<TGraphNode> Heuristic { get; }
        public int MaxSearchDepth { get; }
        public float MaxSecondsPerSearch { get; }
        public bool AssumeNonNegativeCosts { get; }

        //Heuristic, which returns 0 for any two nodes.
        //Using this heuristic for A* makes it equivalent to Dijkstra's algorithm.
        private static double ZeroPathCostHeuristic(TGraphNode sourceNode, TGraphNode targetNode)
        {
            return 0;
        }

        public class Builder
        {
            private bool assumeNonNegativeCosts;
            private HashSet<ExperienceAction> experienceActions;
            private PathCostHeuristic<TGraphNode> heuristic;
            private int maxSearchDepth;
            private float maxSecondsPerSearch;

            public Builder()
            {
                heuristic = ZeroPathCostHeuristic;
                maxSearchDepth = UnlimitedSearchDepth;
                maxSecondsPerSearch = UnlimitedSecondsPerSearch;
                assumeNonNegativeCosts = DefaultAssumeNonNegativeCosts;
            }

            // Specifies the heuristic to be used by A*.
            public Builder UseHeuristic(PathCostHeuristic<TGraphNode> pathCostHeuristic)
            {
                heuristic = PreconditionUtils.EnsureNotNull(pathCostHeuristic, "heuristic");
                return this;
            }

            // Limits search depth.
            // A negative value indicates no depth limit.
            public Builder LimitSearchDepth(int searchDepthLimit)
            {
                maxSearchDepth = searchDepthLimit;
                return this;
            }

            // Limits search time.
            // Once the time limit is exceeded, it will return the best known (but not necessarily the best overall)
            // path to target or, if no paths to target are found by that time, report a failure.
            public Builder LimitSearchTime(float seconds)
            {
                maxSecondsPerSearch = seconds;
                return this;
            }

            // Limits search time.
            // Once the limit is exceeded, it will return the best known (but not necessarily the best overall)
            // path to target or, if no paths to target are found by that time, report a failure.
            public Builder LimitSearchTime(double seconds)
            {
                maxSecondsPerSearch = (float) seconds;
                return this;
            }

            // Allows A* to assume that all edges have non-negative costs.
            // This allows it to discard certain potential paths quicker.
            public Builder AssumeNonNegativeCosts()
            {
                assumeNonNegativeCosts = true;
                return this;
            }

            public AStarPathfinderConfiguration<TGraphNode> Build()
            {
                return new AStarPathfinderConfiguration<TGraphNode>(
                    heuristic,
                    maxSearchDepth,
                    maxSecondsPerSearch,
                    assumeNonNegativeCosts
                );
            }
        }
    }
}