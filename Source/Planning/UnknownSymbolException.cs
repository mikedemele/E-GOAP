using System;
using System.Runtime.Serialization;

namespace EGoap.Source.Planning
{
    [Serializable]
    public class UnknownSymbolException : Exception
    {
        private const string MessageTemplate = "Unknown world state symbol {0}{1}";
        private const string DetailsTemplate = " ({0})";

        public UnknownSymbolException(SymbolId symbolId, string details)
            : this(symbolId, details, null)
        {
        }

        public UnknownSymbolException(SymbolId symbolId, string details = null, Exception inner = null)
            : base(
                string.Format(
                    MessageTemplate, symbolId, details != null ? string.Format(DetailsTemplate, details) : ""
                ),
                inner
            )
        {
            SymbolId = symbolId;
        }

        protected UnknownSymbolException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public SymbolId SymbolId { get; }
    }
}