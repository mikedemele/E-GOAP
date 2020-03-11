using System;
using System.Runtime.Serialization;

namespace EGoap.Source.Graphs
{
    [Serializable]
    public class NoPathExistsException : PathNotFoundException
    {
        private const string DefaultMessage = "No path exists{0}";
        private const string DetailsTemplate = " (given max search depth {0})";
        
        public NoPathExistsException(Type pathfinderType, Exception inner)
            : this(pathfinderType, null, inner)
        {
        }

        public NoPathExistsException(Type pathfinderType, int? maxSearchDepth, Exception inner = null)
            : base(
                pathfinderType,
                string.Format(
                    DefaultMessage,
                    maxSearchDepth != null
                        ? string.Format(DetailsTemplate, maxSearchDepth)
                        : ""
                ),
                inner
            )
        {
            MaxSearchDepth = maxSearchDepth;
        }

        protected NoPathExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public int? MaxSearchDepth { get; }
    }
}