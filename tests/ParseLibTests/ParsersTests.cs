using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

using static Testify.Assertions;
using static ParseLib.Parsers;

namespace ParseLib.Tests
{
    public class ParsersTests
    {
        // Return (a -> M a) (Func<T, Parser<T>>)
        // Bind (M a -> (a -> M b) -> M b) (Func<Parser<A>, Func<A, Parser<B>>, Parser<B>>)

        [Fact]
        public void ReturnTest()
        {
            var parser = Return(1);

            var result = parser.Parse("");

            Assert(result).IsSuccess(1, line: 1, column: 1);
        }

        [Fact]
        public void BindTest()
        {
            var p1 = Return(1);
            var parser = p1.Bind(v => Return(v + 1));

            var result = parser.Parse("");

            Assert(result).IsSuccess(2, line: 1, column: 1);
        }

        [Fact]
        public void EndOfInputSuccessTest()
        {
            var result = EndOfInput.Parse("");

            Assert(result).IsSuccess(default, line: 1, column: 1);
        }

        [Fact]
        public void EndOfInputErrorTest()
        {
            var result = EndOfInput.Parse("xyzzy");

            Assert(result).IsError(
                line: 1,
                column: 1,
                "Unexpected character 'x'. Expected end of input.");
        }

        [Fact]
        public void SatSuccessTest()
        {
            var parser = Sat(c => c == 'x', "'x'");

            var result = parser.Parse("x");

            Assert(result).IsSuccess('x', line: 1, column: 2);
        }

        [Fact]
        public void SatErrorTest()
        {
            var parser = Sat(_ => false, "not any character");

            var result = parser.Parse("x");

            Assert(result).IsError(
                line: 1,
                column: 1,
                "Unexpected character in input. Expected not any character.");
        }

        [Fact]
        public void AnyChSuccessTest()
        {
            var parser = AnyCh;

            var result = parser.Parse("x");

            Assert(result).IsSuccess('x', line: 1, column: 2);
        }

        [Fact]
        public void AnyChErrorTest()
        {
            var parser = AnyCh;

            var result = parser.Parse("");

            Assert(result).IsError(
                line: 1,
                column: 1,
                "Unexpected end of input. Expected any character.");
        }

        [Fact]
        public void ChSuccessTest()
        {
            var parser = Ch('a');

            var result = parser.Parse("a");

            Assert(result).IsSuccess('a', line: 1, column: 2);
        }

        [Fact]
        public void ChErrorTest()
        {
            var parser = Ch('a');

            var result = parser.Parse("b");

            Assert(result).IsError(
                line: 1,
                column: 1,
                "Unexpected character in input. Expected 'a'.");
        }

        [Fact]
        public void NewLineShouldParseNewline()
        {
            var parser = NewLine;

            var result = parser.Parse("\na");

            Assert(result).IsSuccess('\n', 2, 1);
        }

        [Fact]
        public void SelectReturnsExpectedValue()
        {
            var parser = AnyCh.Select(c => char.ToUpper(c));

            var result = parser.Parse("a");

            Assert(result).IsSuccess('A', 1, 2);
        }

        [Fact]
        public void LinqWorks()
        {
            var parser =
                from a in Ch('a')
                from b in Ch('b')
                select $"{a} then {b}";

            var result = parser.Parse("ab");

            Assert(result).IsSuccess("a then b", 1, 3);
        }

        [Fact]
        public void OrShouldReturnFirstValueWhenSuccessful()
        {
            var pa = Ch('a');
            var pb = Ch('b');
            var parser = pa.Or(pb);

            var result = parser.Parse("a");

            Assert(result).IsSuccess('a', 1, 2);
        }

        [Fact]
        public void OrShouldReturnSecondValueWhenSuccessful()
        {
            var pa = Ch('a');
            var pb = Ch('b');
            var parser = pa.Or(pb);

            var result = parser.Parse("b");

            Assert(result).IsSuccess('b', 1, 2);
        }

        [Fact]
        public void OrOpShouldReturnFirstValueWhenSuccessful()
        {
            var pa = Ch('a');
            var pb = Ch('b');
            var parser = pa | pb;

            var result = parser.Parse("a");

            Assert(result).IsSuccess('a', 1, 2);
        }

        [Fact]
        public void OrOpShouldReturnSecondValueWhenSuccessful()
        {
            var pa = Ch('a');
            var pb = Ch('b');
            var parser = pa | pb;

            var result = parser.Parse("b");

            Assert(result).IsSuccess('b', 1, 2);
        }

        [Fact]
        public void OrShouldReturnFirstFailureIfConsumed()
        {
            var p1 =
                from a in Ch('a')
                from b in Ch('b')
                select "ab";
            var p2 = Ch('c').Select(c => c.ToString());
            var parser = p1.Or(p2);

            var result = parser.Parse("ac");

            Assert(result).IsError(1, 2, "Unexpected character in input. Expected 'b'.");
        }

        [Fact]
        public void TryOrShouldReturnFirstFailureIfConsumed()
        {
            var p1 =
                (from a in Ch('a')
                from b in Ch('b')
                select "ab").Tag("\"ab\"");
            var p2 = Ch('c').Select(c => c.ToString());
            var parser = p1.Try().Or(p2);

            var result = parser.Parse("ac");

            Assert(result).IsError(1, 2, "Unexpected character in input. Expected \"ab\" or 'c'.");
        }

        [Fact]
        public void TryOrOpShouldReturnFirstFailureIfConsumed()
        {
            var p1 =
                (from a in Ch('a')
                 from b in Ch('b')
                 select "ab").Tag("\"ab\"");
            var p2 = Ch('c').Select(c => c.ToString());
            var parser = p1.Try() | p2;

            var result = parser.Parse("ac");

            Assert(result).IsError(1, 2, "Unexpected character in input. Expected \"ab\" or 'c'.");
        }

        [Fact]
        public void Many1ShouldParseOneOrMoreResults()
        {
            var parser = Ch('a').Many1();
            var input = Input.From("aaab");

            var result = parser.Parse(input);

            Assert(result).IsSuccess("aaa", new SequenceComparer<char>(), 1, 4);
        }


        [Fact]
        public void LiteralShouldParseString()
        {
            var parser = Literal("abc");
            var input = Input.From("abcd");

            var result = parser.Parse(input);

            Assert(result).IsSuccess("abc", 1, 4);
        }

        [Fact]
        public void LiteralShouldFailWithInvalidInput()
        {
            var parser = Literal("abc");
            var input = Input.From("acbd");

            var result = parser.Parse(input);

            Assert(result).IsError(1, 2, "Unexpected character in input. Expected 'abc'.");
        }

        private class SequenceComparer<T> : EqualityComparer<IEnumerable<T>>
        {
            public override bool Equals(IEnumerable<T>? x, IEnumerable<T>? y)
                => x == y || (x != null && y != null &&  x.SequenceEqual(y));

            public override int GetHashCode(IEnumerable<T> obj) => 0;
        }
    }
}
