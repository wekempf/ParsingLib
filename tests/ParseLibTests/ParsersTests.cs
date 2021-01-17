using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

using Testify;
using static Testify.Assertions;

namespace ParseLib.Tests
{
    public class ParsersTests
    {
        // Return (a -> M a) (Func<T, Parser<T>>)
        // Bind (M a -> (a -> M b) -> M b) (Func<Parser<A>, Func<A, Parser<B>>, Parser<B>>)

        [Fact]
        public void ReturnTest()
        {
            Parser<int> parser = Parsers.Return(1);
            var input = Input.From("");

            var result = parser.Parse(input);

            Assert(result).IsSuccess(1, line: 1, column: 1);
        }

        [Fact]
        public void BindTest()
        {
            Parser<int> p1 = Parsers.Return(1);
            var parser = Parsers.Bind(p1, v => Parsers.Return(v + 1));
            var input = Input.From("");

            var result = parser.Parse(input);

            Assert(result).IsSuccess(2, line: 1, column: 1);
        }

        [Fact]
        public void EndOfInputSuccessTest()
        {
            var input = Input.From("");

            var result = Parsers.EndOfInput.Parse(input);

            Assert(result).IsSuccess(default, line: 1, column: 1);
        }

        [Fact]
        public void EndOfInputErrorTest()
        {
            var input = Input.From("xyzzy");

            var result = Parsers.EndOfInput.Parse(input);

            Assert(result).IsError(
                line: 1,
                column: 1,
                "Unexpected character 'x'. Expected end of input.");
        }

        [Fact]
        public void SatSuccessTest()
        {
            Parser<char> parser = Parsers.Sat(c => true, "any character");
            var input = Input.From("x");

            var result = parser.Parse(input);

            Assert(result).IsSuccess('x', line: 1, column: 2);
        }

        [Fact]
        public void SatErrorTest()
        {
            Parser<char> parser = Parsers.Sat(c => false, "not any character");
            var input = Input.From("x");

            var result = parser.Parse(input);

            Assert(result).IsError(
                line: input.Position.Line.Value,
                column: input.Position.Column.Value,
                "Unexpected character in input. Expected not any character.");
        }

        [Fact]
        public void AnyChSuccessTest()
        {
            Parser<char> parser = Parsers.AnyCh;
            var input = Input.From("x");

            var result = parser.Parse(input);

            Assert(result).IsSuccess('x', line: 1, column: 2);
        }

        [Fact]
        public void AnyChErrorTest()
        {
            Parser<char> parser = Parsers.AnyCh;
            var input = Input.From("");

            var result = parser.Parse(input);

            Assert(result).IsError(
                line: input.Position.Line.Value,
                column: input.Position.Column.Value,
                "Unexpected end of input. Expected any character.");
        }

        [Fact]
        public void ChSuccessTest()
        {
            var parser = Parsers.Ch('a');
            var input = Input.From("a");

            var result = parser.Parse(input);

            Assert(result).IsSuccess('a', line: 1, column: 2);
        }

        [Fact]
        public void ChErrorTest()
        {
            var parser = Parsers.Ch('a');
            var input = Input.From("b");

            var result = parser.Parse(input);

            Assert(result).IsError(
                line: 1,
                column: 1,
                "Unexpected character in input. Expected 'a'.");
        }
    }
}
