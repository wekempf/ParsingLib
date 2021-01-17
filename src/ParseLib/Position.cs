using System;
using System.Collections;

namespace ParseLib
{
    public record Position(Line Line, Column Column) : IComparable<Position>, IComparable
    {
        public int CompareTo(Position? other)
            => other is null ? 1 : (Line, Column).CompareTo((other.Line, other.Column));

        public int CompareTo(object? obj)
        {
            if (obj is null) return 1;
            if (obj is Position other) return CompareTo(other);
            throw new ArgumentException("Cannot compare types.");
        }

        public override string ToString() => $"({Line.Value}, {Column.Value})";

        public Position Next() => new Position(Line, new Column(Column.Value + 1));

        public static bool operator <(Position? left, Position? right)
            => Comparer.Default.Compare(left, right) < 0;

        public static bool operator <=(Position? left, Position? right)
            => Comparer.Default.Compare(left, right) <= 0;

        public static bool operator >(Position? left, Position? right)
            => Comparer.Default.Compare(left, right) > 0;

        public static bool operator >=(Position? left, Position? right)
            => Comparer.Default.Compare(left, right) >= 0;
    }
}
