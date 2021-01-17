using System;
using System.Collections;

namespace ParseLib
{
    public record Line : IComparable<Line>, IComparable
    {
        public Line(int value)
            => Value = Guard.Against.Range(value, 1, int.MaxValue, nameof(value));

        public int Value { get; init; }

        public int CompareTo(Line? other) => other == null ? 1 : Value.CompareTo(other.Value);

        public int CompareTo(object? obj)
        {
            if (obj is null) return 1;
            if (obj is Line other) return CompareTo(other);
            throw new ArgumentException("Cannot compare types.");
        }

        public static bool operator <(Line? left, Line? right)
            => Comparer.Default.Compare(left, right) < 0;

        public static bool operator <=(Line? left, Line? right)
            => Comparer.Default.Compare(left, right) <= 0;

        public static bool operator >(Line? left, Line? right)
            => Comparer.Default.Compare(left, right) > 0;

        public static bool operator >=(Line? left, Line? right)
            => Comparer.Default.Compare(left, right) >= 0;
    }
}
