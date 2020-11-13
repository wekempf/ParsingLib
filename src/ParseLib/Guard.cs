using System;

namespace ParseLib
{
    internal class Guard
    {
        private Guard()
        {
        }

        public static Guard Against { get; } = new Guard();

        public T Null<T>(T? value, string paramName)
            where T : class
            => value ?? throw new ArgumentNullException(paramName);

        public T Range<T>(T value, T min, T max, string paramName)
            where T : IComparable<T>
            => value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0
                    ? value
                    : throw new ArgumentOutOfRangeException(paramName);
    }
}
