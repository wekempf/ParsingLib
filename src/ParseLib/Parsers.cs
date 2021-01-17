using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ParseLib.Result;

namespace ParseLib
{
    public static class Parsers
    {
        private static Parser<T> P<T>(Func<Input, Result<T>> func) => new Parser<T>(func);

        public static Parser<T> Return<T>(T value) => P(input => Success(value, input));

        public static Parser<TResult> Bind<TSource, TResult>(
            Parser<TSource> source,
            Func<TSource, Parser<TResult>> binder)
        {
            return P(input =>
                source.Parse(input) switch
                {
                    Result<TSource>.Success s => binder.Invoke(s.Value).Parse(s.RemainingInput),
                    Result<TSource>.Error e => e.ChangeType<TResult>()
                });
        }

        public static Parser<Unit> EndOfInput =>
            P(input =>
            {
                if (input.IsAtEnd)
                {
                    return Success(default(Unit), input);
                }
                else
                {
                    return Error<Unit>(input.Position, $"Unexpected character '{input.Current}'.", "end of input");
                }
            });

        public static Parser<char> Sat(Func<char, bool> predicate, string expected)
        {
            return P(input =>
                {
                    if (input.IsAtEnd)
                    {
                        return Error<char>(input.Position, "Unexpected end of input.", expected);
                    }
                    else if (predicate(input.Current))
                    {
                        return Success(input.Current, input.RemainingInput);
                    }
                    else
                    {
                        return Error<char>(input.Position, "Unexpected character in input.", expected);
                    }
                });
        }

        public static Parser<char> AnyCh => Sat(c => true, "any character");

        public static Parser<char> Ch(char ch) => Sat(c => c == ch, $"'{ch}'");
    }
}
