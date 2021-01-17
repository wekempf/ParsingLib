using System;


using Testify;
using Xunit;
using static Testify.Assertions;

namespace ParseLib.Tests
{
    public class ColumnTests
    {
        private readonly AnonymousData anon = new();

        [Fact(DisplayName = "Constructor should set value")]
        public void CtorSetsValue()
        {
            var expectedColumn = anon.AnyInt32(1, int.MaxValue);

            var result = new Line(expectedColumn);

            Assert(result.Value).IsEqualTo(expectedColumn, "Column isn't correct");
        }

        [Fact(DisplayName = "Column should be comparable")]
        public void Comparable()
        {
            var verifier = new ComparableVerifier<Column>
            {
                OrderedItemsFactory = () => new[]
                {
                    new Column(1),
                    new Column(2),
                    new Column(3),
                },
                SkipImmutabilityTests = true
            };

            verifier.Verify();
        }

        [Fact(DisplayName = "With should set value")]
        public void WithTests()
        {
            Column originalColumn = new(1);
            var expectedColumn = 42;

            var result = originalColumn with { Value = expectedColumn };

            Assert(result.Value).IsEqualTo(expectedColumn, "Column was not correct");
        }

        [Fact(DisplayName = "Value cannot be less than 1")]
        public void CtorValueValidation()
        {
            Assert(() => new Column(0)).Throws<ArgumentOutOfRangeException>();
        }
    }
}
