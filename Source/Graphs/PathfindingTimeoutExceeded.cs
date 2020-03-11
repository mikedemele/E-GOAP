using System;
using System.Runtime.Serialization;

namespace EGoap.Source.Graphs
{
    [Serializable]
    public class PathfindingTimeoutException : PathNotFoundException
    {
        private const string DefaultMessage = "time limit exceeded ({0}s)";

        public PathfindingTimeoutException(Type pathfinderType, float timeLimitSeconds, Exception inner = null)
            : base(
                pathfinderType,
                string.Format(
                    DefaultMessage,
                    timeLimitSeconds
                ),
                inner
            )
        {
            TimeLimitSeconds = timeLimitSeconds;
        }

        protected PathfindingTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public float TimeLimitSeconds { get; }
    }
}