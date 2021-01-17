using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Testify;
using Xunit;
using static Testify.Assertions;

namespace ParseLib.Tests
{
    public class PositionTests
    {
        private readonly AnonymousData anon = new();

        [Fact(DisplayName = "Constructor should set properties")]
        public void CtorSetsProperties()
        {
            var expectedLine = anon.AnyLine();
            var expectedColumn = anon.AnyColumn();

            var result = new Position(expectedLine, expectedColumn);


            AssertAll(
                "Not all properties were set correctly.",
                () => Assert(result.Line).IsEqualTo(expectedLine, "Line isn't correct"),
                () => Assert(result.Column).IsEqualTo(expectedColumn, "Column isn't correct"));
        }

        [Fact(DisplayName = "Position should be comparable")]
        public void Comparable()
        {
            var verifier = new ComparableVerifier<Position>
            {
                OrderedItemsFactory = () => new[]
                {
                    new Position(new Line(1), new Column(1)),
                    new Position(new Line(2), new Column(1)),
                    new Position(new Line(2), new Column(2))
                },
                SkipImmutabilityTests = true
            };

            verifier.Verify();
        }

        [Fact(DisplayName = "With should set properties")]
        public void WithTests()
        {
            Position original = new(new Line(1), new Column(1));
            Line expectedLine = new(42);

            var result = original with { Line = expectedLine };

            AssertAll(
                "Not all properties are correct.",
                () => Assert(result.Line).IsEqualTo(expectedLine, "Line was not correct"),
                () => Assert(result.Column).IsEqualTo(original.Column, "Column was not correct"));
        }

        [Fact(DisplayName = "Deconstruction should work")]
        public void DeconstructionTests()
        {
            var position = anon.AnyPosition();

            var (line, column) = position;

            AssertAll(
                "Not all values were deconstructed correctly",
                () => Assert(line).IsEqualTo(position.Line, "Line is not correct"),
                () => Assert(column).IsEqualTo(position.Column, "Column is not correct"));
        }

        [Fact(DisplayName = "ToString should produce formatted text")]
        public void ToStringTest()
        {
            var position = anon.AnyPosition();

            var result = position.ToString();

            Assert(result).IsEqualTo($"({position.Line.Value}, {position.Column.Value})");
        }
    }
}
