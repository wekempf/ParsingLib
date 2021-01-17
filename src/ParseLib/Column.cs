using System;
using System.Collections;

namespace ParseLib
{
    public record Column : IComparable<Column>, IComparable
    {
        public Column(int value)
            => Value = Guard.Against.Range(value, 1, int.MaxValue, nameof(value));

        public int Value { get; init; }

        public int CompareTo(Column? other) => other == null ? 1 : Value.CompareTo(other.Value);

        public int CompareTo(object? obj)
        {
            if (obj is null) return 1;
            if (obj is Column other) return CompareTo(other);
            throw new ArgumentException("Cannot compare types.");
        }

        public static bool operator <(Column? left, Column? right)
            => Comparer.Default.Compare(left, right) < 0;

        public static bool operator <=(Column? left, Column? right)
            => Comparer.Default.Compare(left, right) <= 0;

        public static bool operator >(Column? left, Column? right)
            => Comparer.Default.Compare(left, right) > 0;

        public static bool operator >=(Column? left, Column? right)
            => Comparer.Default.Compare(left, right) >= 0;
    }
}
