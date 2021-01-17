using System;

using Testify;
using Xunit;
using static Testify.Assertions;

namespace ParseLib.Tests
{
    public class LineTests
    {
        private readonly AnonymousData anon = new();

        [Fact(DisplayName = "Constructor should set value")]
        public void CtorSetsValue()
        {
            var expectedLine = anon.AnyInt32(1, int.MaxValue);

            var result = new Line(expectedLine);

            Assert(result.Value).IsEqualTo(expectedLine, "Line isn't correct");
        }

        [Fact(DisplayName = "Line should be comparable")]
        public void Orderable()
        {
            var verifier = new ComparableVerifier<Line>
            {
                OrderedItemsFactory = () => new[]
                {
                    new Line(1),
                    new Line(2),
                    new Line(3),
                },
                SkipImmutabilityTests = true
            };

            verifier.Verify();
        }

        [Fact(DisplayName = "With should set value")]
        public void WithTests()
        {
            Line originalLine = new(1);
            var expectedLine = 42;

            var result = originalLine with { Value = expectedLine };

            Assert(result.Value).IsEqualTo(expectedLine, "Line was not correct");
        }

        [Fact(DisplayName = "Value cannot be less than 1")]
        public void CtorValueValidation()
        {
            Assert(() => new Line(0)).Throws<ArgumentOutOfRangeException>();
        }
    }
}
