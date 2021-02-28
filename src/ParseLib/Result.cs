using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseLib
{
    public abstract record Result<T>
    {
        private Result(bool IsConsumed) { }

        public Result<T> SetConsumed(bool isConsumed) =>
            this switch
            {
                Success s => new Success(s.Value, s.RemainingInput, isConsumed),
                Error e => new Error(e.Position, e.Message, e.Expected, isConsumed)
            };

        public TResult Match<TResult>(Func<Success, TResult> success, Func<Error, TResult> error)
            => this switch
            {
                Success s => success(s),
                Error e => error(e)
            };

        public sealed record Success(T Value, Input RemainingInput, bool IsConsumed) : Result<T>(IsConsumed);

        public sealed record Error(
            Position Position,
            string Message,
            IEnumerable<string> Expected, bool IsConsumed)
            : Result<T>(IsConsumed)
        {
            public Error(Position position, string message, bool isConsumed)
                : this(position, message, Enumerable.Empty<string>(), isConsumed)
            {
            }

            public Result<TResult> ChangeType<TResult>()
                => new Result<TResult>.Error(Position, Message, Expected, IsConsumed);

            public Error Merge(Error other) => new Error(Position, Message, Expected.Concat(other.Expected), IsConsumed);

            public override string ToString()
            {
                var expectedString = string.Join(", ", Expected.SkipLast(1));
                expectedString = expectedString.Length > 0 ? expectedString + " or " : expectedString;
                expectedString += Expected.TakeLast(1).First();
                return $"{Message} Expected {expectedString}.";
            }

        }

        public static implicit operator Result<T>(Result.ErrorResult e) => new Error(e.Position, e.Message, e.Expected, e.IsConsumed);
    }

    public static class Result
    {
        public sealed record ErrorResult(
            Position Position,
            string Message,
            IEnumerable<string> Expected,
            bool IsConsumed);

        public static Result<T> Success<T>(T value, Input remainingInput, bool isConsumed)
            => new Result<T>.Success(value, remainingInput, isConsumed);

        public static ErrorResult Error(Position position, string message, IEnumerable<string> expected, bool isConsumed)
            => new ErrorResult(position, message, expected, isConsumed);

        public static ErrorResult Error(Position position, string message, string expected, bool isConsumed)
            => new ErrorResult(position, message, new[] { expected }, isConsumed);
        public static Result<T> Error<T>(Position position, string message, IEnumerable<string> expected, bool isConsumed)
            => new ErrorResult(position, message, expected, isConsumed);

        public static Result<T> Error<T>(Position position, string message, string expected, bool isConsumed)
            => new ErrorResult(position, message, new[] { expected }, isConsumed);
    }
}
