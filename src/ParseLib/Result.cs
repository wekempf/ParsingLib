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

        public sealed record Success(T Value) : Result<T>;

        public sealed record Error(Position Position, string Message, IEnumerable<string> Expected) : Result<T>
        {
            public Error(Position position, string message)
                : this(position, message, Enumerable.Empty<string>())
            {
            }
        }
    }

    public static class Result
    {
        public static Result<T> Success<T>(T value) => new Result<T>.Success(value);
    }
}
