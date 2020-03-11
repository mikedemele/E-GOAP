using System;
using System.Collections.Generic;
using EGoap.Source.Debug;
using EGoap.Source.Graphs;

namespace EGoap.Source.Planning.Internal
{
    internal class ForwardNodeExpander : INodeExpander<ForwardNode>
    {
        private readonly IEnumerable<PlanningAction> availableActions;

        public ForwardNodeExpander(IEnumerable<PlanningAction> availableActions)
        {
            this.availableActions = PreconditionUtils.EnsureNotNull(availableActions, "availableActions");
        }


        #region INodeExpander implementation

        public IEnumerable<IGraphEdge<ForwardNode>> ExpandNode(ForwardNode node)
        {
            if (node.IsGoal)
                throw new ArgumentException(
                    $"Goal {node.GetType()} cannot be expanded",
                    nameof(node)
                );

            foreach (var action in availableActions)
                if (action.IsAvailableIn(node.WorldState))
                    yield
                        return
                        new ForwardEdge(action, node,
                            ForwardNode.MakeRegularNode(action.Apply(node.WorldState), this)
                        );
        }

        #endregion
    }
}