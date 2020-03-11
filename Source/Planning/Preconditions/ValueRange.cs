using System;

namespace EGoap.Source.Planning.Preconditions
{
    // Representation of a value range
    [Serializable]
    public struct ValueRange : IEquatable<ValueRange>
    {
        public static readonly ValueRange AnyValue = new ValueRange(int.MinValue, int.MaxValue);
        public static readonly ValueRange Empty = new ValueRange(1, 0);

        public static ValueRange Exactly(int value)
        {
            return new ValueRange(value, value);
        }

        public static ValueRange LessThan(int value)
        {
            return new ValueRange(int.MinValue, value - 1);
        }

        public static ValueRange LessThanOrEqual(int value)
        {
            return new ValueRange(int.MinValue, value);
        }

        public static ValueRange GreaterThan(int value)
        {
            return new ValueRange(value + 1, int.MaxValue);
        }

        public static ValueRange GreaterThanOrEqual(int value)
        {
            return new ValueRange(value, int.MaxValue);
        }

        public static ValueRange Between(int minValue, int maxValue)
        {
            return new ValueRange(minValue, maxValue);
        }

        public int MinValue { get; }
        public int MaxValue { get; }

        private ValueRange(int minValue, int maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public bool Contains(int value)
        {
            return value >= MinValue && value <= MaxValue;
        }

        public bool IsEmpty => MaxValue < MinValue;

        public bool IsSingleValue => MinValue == MaxValue;

        public int Size =>
            IsEmpty
                ? 0
                : MaxValue - MinValue + 1;

        public int AbsDistanceFrom(int value)
        {
            return Contains(value)
                ? 0
                : value < MinValue
                    ? MinValue - value
                    : value - MaxValue;
        }

        public ValueRange Intersect(ValueRange other)
        {
            return new ValueRange(
                Math.Max(MinValue, other.MinValue),
                Math.Min(MaxValue, other.MaxValue)
            );
        }

        public ValueRange ShiftBy(int shift)
        {
            return new ValueRange(
                SafeAdd(MinValue, shift),
                SafeAdd(MaxValue, shift)
            );
        }

        private static int SafeAdd(int initialValue, int addedValue)
        {
            return initialValue == int.MinValue || initialValue == int.MaxValue
                ? initialValue
                : initialValue + (initialValue >= 0
                      ? Math.Min(addedValue, int.MaxValue - initialValue)
                      : Math.Max(addedValue, -(initialValue - int.MinValue)));
        }

        #region IEquatable implementation

        public bool Equals(ValueRange other)
        {
            return MinValue == other.MinValue && MaxValue == other.MaxValue;
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != typeof(ValueRange))
                return false;
            var other = (ValueRange) obj;
            return Equals(other);
        }


        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 23;
                hash = hash * 37 + MinValue.GetHashCode();
                hash = hash * 37 + MaxValue.GetHashCode();
                return hash;
            }
        }


        public override string ToString()
        {
            return IsEmpty
                ? "{}"
                : MinValue == MaxValue
                    ? $"{{{MinValue}}}"
                    : $"[{MinValue}, {MaxValue}]";
        }
    }
}