using System;
using System.Runtime.Serialization;

namespace EGoap.Source.Graphs
{
	// Thrown by IPathfinder implementers, when they fail to find a path on a graph.
	[Serializable]
    public class PathNotFoundException : Exception
    {
        private const string DefaultMessage = "{0} failed to find a path: {1}";
        private const string DefaultDetails = "unknown reason";

        public PathNotFoundException(Type pathfinderType, Exception inner)
            : this(pathfinderType, inner.Message)
        {
        }

        public PathNotFoundException(Type pathfinderType, string details = DefaultDetails)
            : this(pathfinderType, details, null)
        {
        }

        public PathNotFoundException(Type pathfinderType, string details, Exception inner)
            : base(string.Format(DefaultMessage, pathfinderType.Name, details), inner)
        {
            PathfinderType = pathfinderType;
        }

        protected PathNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public Type PathfinderType { get; }
    }
}