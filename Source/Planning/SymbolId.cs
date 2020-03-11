using System;
using EGoap.Source.Debug;

namespace EGoap.Source.Planning
{
    [Serializable]
    public struct SymbolId : IEquatable<SymbolId>
    {
        public string Name { get; }
        
        public SymbolId(string name)
        {
            Name = PreconditionUtils.EnsureNotBlank(name, "name");
        }

        #region IEquatable implementation

        public bool Equals(SymbolId other)
        {
            return Name == other.Name;
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != typeof(SymbolId))
                return false;
            var other = (SymbolId) obj;
            return Equals(other);
        }


        public override int GetHashCode()
        {
            return Name != null ? Name.GetHashCode() : 0;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}