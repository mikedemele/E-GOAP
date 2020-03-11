using System.Collections.Generic;
using EGoap.Source.Graphs;

namespace EGoap.Source.Planning.Internal
{
    internal interface INodeExpander<TNode> where TNode : IGraphNode<TNode>
    {
        IEnumerable<IGraphEdge<TNode>> ExpandNode(TNode node);
    }
}