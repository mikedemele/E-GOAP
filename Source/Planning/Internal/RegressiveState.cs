using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EGoap.Source.Debug;
using EGoap.Source.Planning.Preconditions;

namespace EGoap.Source.Planning.Internal
{
    internal struct RegressiveState : IEquatable<RegressiveState>, IEnumerable<KeyValuePair<SymbolId, ValueRange>>
    {
        private readonly IDictionary<SymbolId, ValueRange> ranges;

        private RegressiveState(IDictionary<SymbolId, ValueRange> ranges)
        {
            DebugUtils.Assert(ranges != null, "ranges must not be null");
            this.ranges = ranges;
        }

        public bool Contains(SymbolId symbolId)
        {
            return ranges != null && ranges.ContainsKey(symbolId);
        }

        public ValueRange this[SymbolId key]
        {
            get
            {
                try
                {
                    return ranges[key];
                }
                catch (KeyNotFoundException)
                {
                    return ValueRange.AnyValue;
                }
            }
        }

        #region IEnumerable implementation

        public IEnumerator<KeyValuePair<SymbolId, ValueRange>> GetEnumerator()
        {
            return ranges.GetEnumerator();
        }

        #endregion

        #region IEnumerable implementation

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEquatable implementation

        public bool Equals(RegressiveState other)
        {
            if (ranges == null && other.ranges != null || ranges != null && other.ranges == null)
                return false;
            return ranges == null && other.ranges == null
                   || ranges.Count == other.ranges.Count && !ranges.Except(other.ranges).Any();
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != typeof(RegressiveState))
                return false;
            var other = (RegressiveState) obj;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // 0 for no constraints
                if (ranges == null) return 0;

                // Only non-AnyValue constraints contribute to the hash value, order is not important.
                var sum = 0;
                foreach (var kvp in ranges)
                    sum += ValueRange.AnyValue.Equals(kvp.Value)
                        ? 0
                        : 17 * kvp.Key.GetHashCode() + 23 * kvp.Value.GetHashCode();

                return sum;
            }
        }

        public override string ToString()
        {
            return
                $"[{(ranges == null ? "" : ranges.Aggregate("", (soFar, kvp) => soFar + $"{kvp.Key} in {kvp.Value}; "))}]";
        }

        public Builder BuildUpon()
        {
            return new Builder(this);
        }

        public class Builder
        {
            private readonly IDictionary<SymbolId, ValueRange> ranges;

            public Builder(RegressiveState original = default)
            {
                ranges = original.ranges == null ? new Dictionary<SymbolId, ValueRange>() : new Dictionary<SymbolId, ValueRange>(original.ranges);
            }

            public ValueRange this[SymbolId key]
            {
                get
                {
                    try
                    {
                        return ranges[key];
                    }
                    catch (KeyNotFoundException)
                    {
                        return ValueRange.AnyValue;
                    }
                }
                set => SetRange(key, value);
            }

            public Builder ClearRanges()
            {
                ranges.Clear();
                return this;
            }

            public Builder UnsetRange(SymbolId key)
            {
                ranges.Remove(key);
                return this;
            }

            public Builder SetRange(SymbolId key, ValueRange value)
            {
                ranges[key] = value;
                return this;
            }

            public Builder IntersectRange(SymbolId key, ValueRange intersectedRange)
            {
                ranges[key] = this[key].Intersect(intersectedRange);
                return this;
            }

            public RegressiveState Build()
            {
                return new RegressiveState(ranges);
            }
        }
    }
}