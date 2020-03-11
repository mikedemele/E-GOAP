using System;
using System.Collections.Generic;
using System.Linq;
using EGoap.Source.Debug;
using EGoap.Source.Graphs;

namespace EGoap.Source.Planning.Internal
{
    internal sealed class RegressiveNode : IGraphNode<RegressiveNode>, IEquatable<RegressiveNode>
    {
        private readonly RegressiveState currentConstraints;

        private readonly INodeExpander<RegressiveNode> nodeExpander;

        private IEnumerable<IGraphEdge<RegressiveNode>> outgoingEdges;

        private RegressiveNode(
            RegressiveState currentConstraints,
            WorldState initialState,
            INodeExpander<RegressiveNode> nodeExpander,
            bool isTarget
        )
        {
            DebugUtils.Assert(
                isTarget || nodeExpander != null,
                "Unless isTarget is true, nodeExpander must not be null"
            );

            this.currentConstraints = currentConstraints;
            InitialState = initialState;
            this.nodeExpander = nodeExpander;
            IsTarget = isTarget;
        }

        public RegressiveState CurrentConstraints
        {
            get
            {
                if (IsTarget)
                    throw new InvalidOperationException(
                        $"CurrentConstraints cannot be retrieved from a target {GetType()}"
                    );
                return currentConstraints;
            }
        }
        
        public WorldState InitialState { get; }

        public bool IsTarget { get; }

        #region IEquatable implementation

        public bool Equals(RegressiveNode other)
        {
            if (other != null && IsTarget && other.IsTarget)
                throw new InvalidOperationException(
                    $"{GetType()}.Equals() cannot compare two target nodes"
                );

            if (!IsTarget && !other.IsTarget) return CurrentConstraints.Equals(other.CurrentConstraints);

            // If one is a target node, and another is a regular node
            var regularNode = IsTarget ? other : this;
            var targetNode = IsTarget ? this : other;

            return targetNode.InitialState.All(kvp => regularNode.CurrentConstraints[kvp.Key].Contains(kvp.Value));
        }

        #endregion

        #region IGraphNode implementation

        public IEnumerable<IGraphEdge<RegressiveNode>> OutgoingEdges
        {
            get
            {
                if (outgoingEdges == null) outgoingEdges = nodeExpander.ExpandNode(this);

                DebugUtils.Assert(
                    outgoingEdges.All(edge => edge.SourceNode.Equals(this)),
                    "this edge must be the source node of every outgoing edge"
                );

                return outgoingEdges;
            }
        }

        #endregion

        public static RegressiveNode MakeRegular(
            RegressiveState currentConstraints,
            WorldState initialState,
            INodeExpander<RegressiveNode> nodeExpander
        )
        {
            return new RegressiveNode(
                currentConstraints,
                initialState,
                PreconditionUtils.EnsureNotNull(nodeExpander, "nodeExpander"),
                false
            );
        }

        public static RegressiveNode MakeTarget(WorldState initialWorldState)
        {
            return new RegressiveNode(default, initialWorldState, null, true);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof(RegressiveNode))
                return false;
            var other = (RegressiveNode) obj;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            return CurrentConstraints.GetHashCode();
        }
        
        public override string ToString()
        {
            //return string.Format("[RegressiveNode: currentConstraints={0}, initialState={1}, IsTarget={2}]", currentConstraints, initialState, IsTarget);
            return $"{currentConstraints}";
        }
    }
}