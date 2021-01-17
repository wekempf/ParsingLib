using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseLib
{
    public abstract record Result<T>
    {
        private Result() { }

        public sealed record Success(T Value, Input RemainingInput) : Result<T>;

        public sealed record Error(Position Position, string Message, IEnumerable<string> Expected) : Result<T>
        {
            public Error(Position position, string message)
                : this(position, message, Enumerable.Empty<string>())
            {
            }

            public Result<TResult> ChangeType<TResult>()
                => new Result<TResult>.Error(Position, Message, Expected);

            public override string ToString()
            {
                var expectedString = string.Join(", ", Expected.SkipLast(1));
                expectedString = expectedString.Length > 0 ? expectedString + " or " : expectedString;
                expectedString += Expected.TakeLast(1).First();
                return $"{Message} Expected {expectedString}.";
            }
        }
    }

    public static class Result
    {
        public static Result<T> Success<T>(T value, Input remainingInput)
            => new Result<T>.Success(value, remainingInput);

        public static Result<T> Error<T>(Position position, string message, IEnumerable<string> expected)
            => new Result<T>.Error(position, message, expected);

        public static Result<T> Error<T>(Position position, string message, string expected)
            => new Result<T>.Error(position, message, new[] { expected });
    }
}
