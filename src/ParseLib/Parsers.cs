using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static ParseLib.Result;

namespace ParseLib
{
    public static class Parsers
    {
        private static Parser<T> P<T>(Func<Input, Result<T>> func) => new Parser<T>(func);

        public static Parser<T> Return<T>(T value) => P(input => Success(value, input, false));

        public static Parser<TResult> Bind<TSource, TResult>(
            this Parser<TSource> source,
            Func<TSource, Parser<TResult>> binder)
        {
            Guard.Against.Null(source, nameof(source));
            Guard.Against.Null(binder, nameof(binder));
            return P(input =>
                source.Parse(input) switch
                {
                    Result<TSource>.Success s => binder.Invoke(s.Value).Parse(s.RemainingInput),
                    Result<TSource>.Error e => e.ChangeType<TResult>()
                });
        }

        public static Parser<TResult> Select<TSource, TResult>(
            this Parser<TSource> source, Func<TSource, TResult> selector)
        {
            Guard.Against.Null(source, nameof(source));
            Guard.Against.Null(selector, nameof(selector));
            return source.SelectMany(s => Return(selector(s)));
        }

        public static Parser<TResult> SelectMany<TSource, TResult>(
            this Parser<TSource> source,
            Func<TSource, Parser<TResult>> selector)
        {
            Guard.Against.Null(source, nameof(source));
            Guard.Against.Null(selector, nameof(selector));
            return Bind(source, selector);
        }

        public static Parser<TResult> SelectMany<TSource, TParser, TResult>(
            this Parser<TSource> source,
            Func<TSource, Parser<TParser>> parserSelector,
            Func<TSource, TParser, TResult> resultSelector)
        {
            Guard.Against.Null(source, nameof(source));
            Guard.Against.Null(parserSelector, nameof(parserSelector));
            Guard.Against.Null(resultSelector, nameof(resultSelector));

            return P(input =>
                source.Parse(input) switch
                {
                    Result<TSource>.Success s =>
                        parserSelector.Invoke(s.Value).Parse(s.RemainingInput) switch
                        {
                            Result<TParser>.Success p =>
                                Return(resultSelector(s.Value, p.Value))
                                    .Parse(p.RemainingInput).SetConsumed(s.IsConsumed),
                            Result<TParser>.Error p => p.ChangeType<TResult>().SetConsumed(s.IsConsumed)
                        },
                    Result<TSource>.Error e => e.ChangeType<TResult>()
                });
        }

        public static Parser<Unit> EndOfInput
            => P(input =>
                input.IsAtEnd
                    ? Success(Unit.Value, input, false)
                    : Error(input.Position, $"Unexpected character '{input.Current}'.", "end of input", false));

        public static Parser<char> Sat(Func<char, bool> predicate, string expected)
            => P(input =>
                input switch
                {
                    Input(_, var p, true) => Error(p, "Unexpected end of input.", expected, false),
                    Input(var c, _, _, var r) when predicate(c) => Success(c, r, true),
                    _ => Error(input.Position, "Unexpected character in input.", expected, false)
                });

        public static Parser<char> AnyCh => Sat(_ => true, "any character");

        public static Parser<char> Ch(char ch) => Sat(c => c == ch, $"'{ch}'");

        public static Parser<char> NewLine => Sat(c => c == '\n', "new line");

        public static Parser<TResult> Or<TResult>(this Parser<TResult> parser, Parser<TResult> other)
        {
            return P<TResult>(input =>
                parser.Parse(input) switch
                {
                    Result<TResult>.Success s => s,
                    Result<TResult>.Error failure1 when failure1.IsConsumed => failure1,
                    Result<TResult>.Error failure1 =>
                        other.Parse(input) switch
                        {
                            Result<TResult>.Success s => s,
                            Result<TResult>.Error failure2 when failure2.IsConsumed => failure2,
                            Result<TResult>.Error failure2 => failure1.Merge(failure2)
                        }
                });
        }

        public static Parser<TResult> Try<TResult>(this Parser<TResult> parser)
            => P(input => parser.Parse(input).SetConsumed(false));

        public static Parser<TResult> Tag<TResult>(this Parser<TResult> parser, string expected)
            => P(input =>
                parser.Parse(input) switch
                {
                    Result<TResult>.Success s => s,
                    Result<TResult>.Error e => Error<TResult>(
                        e.Position,
                        e.Message,
                        new[] { expected },
                        e.IsConsumed),
                });

        public static Parser<TResult> Then<TIgnore, TResult>(
                    this Parser<TIgnore> parser,
                    Parser<TResult> next)
        {
            return parser.Then(_ => next);
        }

        public static Parser<TResult> Then<TSource, TResult>(
                    this Parser<TSource> parser,
                    Func<TSource, Parser<TResult>> selector)
            => P(inp => parser.Parse(inp).Match(
                s => selector(s.Value).Parse(s.RemainingInput).SetConsumed(s.IsConsumed),
                e => Error(e.Position, e.Message, e.Expected, e.IsConsumed)));

        public static Parser<IEnumerable<TResult>> Many<TResult>(this Parser<TResult> parser)
            => parser.Many1()
                .Or(Return(Enumerable.Empty<TResult>()));

        public static Parser<IEnumerable<TResult>> Many1<TResult>(this Parser<TResult> parser)
            => parser.Then(x =>
                   parser.Many()
                       .Then(xs =>
                           Return(xs.Prepend(x))));

        public static Parser<string> Literal(string text)
        {
            Guard.Against.NullOrEmpty(text, nameof(text));
            return text
                .Select(c => Ch(c))
                .Aggregate((p, ch) => p?.Then(ch) ?? ch)
                .Then(Return(text))
                .Tag($"'{text}'");
        }

        public static Parser<char> Digit => Sat(char.IsDigit, "digit");

        public static Parser<char> OctalDigit = Sat(c => "01234567".Contains(c), "octal digit");

        public static Parser<char> HexDigit = Sat(c => "0123456789abcdefABCDEF".Contains(c), "hex digit");

        public static Parser<int> DigitVal = Digit.Select(c => c - '0');

        public static Parser<char> IsAlpha => Sat(char.IsLetter, "alpha character");

        public static Parser<char> IsAlphaOrDigit => Sat(char.IsLetterOrDigit, "alpha or digit character");

        public static Parser<int> Natural => Digit.Many1()
            .Select(cs => cs.Aggregate(0, (a, c) => a * 10 + c))
            .Tag("natural number");

        public static Parser<IEnumerable<char>> WhiteSpace => Sat(char.IsWhiteSpace, "whitespace").Many();

        public static Parser<char> NoneOf(IEnumerable<char> chars)
        {
            var set = chars.ToHashSet();
            return Sat(c => !set.Contains(c), $"any character except one of [{string.Join(", ", set.Select(c => $"'{c}'"))}]");
        }

        public static Parser<char> OneOf(IEnumerable<char> chars)
        {
            var set = chars.ToHashSet();
            return Sat(set.Contains, $"one of [{string.Join(", ", set.Select(c => $"'{c}'"))}]");
        }

        public static Result<T> Parse<T>(this Parser<T> parser, string text)
            => parser.Parse(Input.From(text));

        public static Parser<TResult> Return<TSource, TResult>(this Parser<TSource> parser, TResult result)
            => parser.Then(Return(result));

        public static Parser<TResult> Return<TSource, TResult>(this Parser<TSource> parser, Func<TSource, TResult> resultSelector)
            => parser.Then(x => Return(resultSelector.Invoke(x)));

        public static Parser<TResult> Ref<TResult>(Func<Parser<TResult>> reference)
        {
            var parser = new Lazy<Parser<TResult>>(reference);
            return P(input => parser.Value.Parse(input));
        }

        public static Parser<TResult> Choice<TResult>(IEnumerable<Parser<TResult>> parsers)
            => parsers.Aggregate((a, p) => a == null ? p : p.Or(a));

        public static Parser<TResult> Choice<TResult>(params Parser<TResult>[] parsers)
            => Choice(parsers.AsEnumerable());

        public static Parser<TResult> Tokenize<TResult>(this Parser<TResult> parser)
            => from leading in WhiteSpace
               from result in parser
               from trailing in WhiteSpace
               select result;

        // Does not handle scientific notation. Also doesn't handle sign.
        public static Parser<double> DecimalValue(CultureInfo? ci = null)
        {
            static Parser<string> FractionalPart(CultureInfo ci)
                => from dot in Literal(ci.NumberFormat.NumberDecimalSeparator)
                   from fraction in Natural
                   select $"{dot}{fraction}";
            static Parser<string> DecimalText(CultureInfo ci)
                => from whole in Natural
                   from fractional in FractionalPart(ci).Try().Or(Return(string.Empty))
                   select $"{whole}{fractional}";
            ci ??= CultureInfo.CurrentCulture;
            return (FractionalPart(ci) | DecimalText(ci))
                .Select(s => double.Parse(s, ci));
        }
    }
}
