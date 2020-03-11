using System;
using System.Collections.Generic;
using System.Linq;
using EGoap.Source.Debug;
using EGoap.Source.Graphs;

namespace EGoap.Source.Planning.Internal
{
    internal sealed class ForwardNode : IGraphNode<ForwardNode>, IEquatable<ForwardNode>
    {
        private readonly Goal goal;
        private readonly INodeExpander<ForwardNode> nodeExpander;
        private readonly WorldState worldState;

        private IEnumerable<IGraphEdge<ForwardNode>> outgoingEdges;

        // Preconditions are checked in static factory methods
        private ForwardNode(WorldState worldState, INodeExpander<ForwardNode> nodeExpander, Goal goal, bool isGoal)
        {
            DebugUtils.Assert(
                isGoal || nodeExpander != null,
                "nodeExpander must not be null for regular (non-goal) nodes"
            );

            this.worldState = worldState;
            this.nodeExpander = nodeExpander;
            this.goal = goal;
            IsGoal = isGoal;

            outgoingEdges = null;
        }

        public bool IsGoal { get; }

        public WorldState WorldState
        {
            get
            {
                if (IsGoal)
                    throw new InvalidOperationException(
                        $"WordState property cannot be retrieved from a goal {GetType()}"
                    );
                return worldState;
            }
        }

        public Goal Goal
        {
            get
            {
                if (!IsGoal)
                    throw new InvalidOperationException(
                        $"Goal property can only be retrieved from a goal {GetType()}"
                    );
                return goal;
            }
        }


        #region IEquatable implementation

        public bool Equals(ForwardNode other)
        {
            System.Diagnostics.Debug.Assert(other != null, nameof(other) + " != null");
            if (IsGoal && other.IsGoal)
                throw new InvalidOperationException(
                    $"Equals() cannot be used to compare two goal instances of {GetType()}"
                );

            if (!IsGoal && !other.IsGoal) return WorldState.Equals(other.WorldState);

            // If one is a goal node, and another is a regular node
            var regularNode = IsGoal ? other : this;
            var goalNode = IsGoal ? this : other;

            return goalNode.Goal.IsReachedIn(regularNode.WorldState);
        }

        #endregion

        #region IGraphNode implementation

        public IEnumerable<IGraphEdge<ForwardNode>> OutgoingEdges
        {
            get
            {
                if (IsGoal)
                    throw new InvalidOperationException(
                        $"OutgoingEdges property cannot be retrieved from a goal {GetType()}"
                    );

                if (outgoingEdges == null) outgoingEdges = nodeExpander.ExpandNode(this);

                DebugUtils.Assert(
                    outgoingEdges.All(edge => edge.SourceNode.Equals(this)),
                    "Node expander must make this node the source of every outgoing edge"
                );

                return outgoingEdges;
            }
        }

        #endregion

        public static ForwardNode MakeRegularNode(WorldState worldState, INodeExpander<ForwardNode> nodeExpander)
        {
            return new ForwardNode(
                PreconditionUtils.EnsureNotNull(worldState, "worldState"),
                PreconditionUtils.EnsureNotNull(nodeExpander, "nodeExpander"),
                default,
                false
            );
        }

        public static ForwardNode MakeGoalNode(Goal goal)
        {
            return new ForwardNode(default, null, goal, true);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof(ForwardNode))
                return false;
            var other = (ForwardNode) obj;
            return Equals(other);
        }


        public override int GetHashCode()
        {
            return worldState.GetHashCode();
        }
    }
}