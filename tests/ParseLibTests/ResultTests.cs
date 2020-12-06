using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;


using Testify;
using static Testify.Assertions;

using static ParseLib.Result;

namespace ParseLib.Tests
{
    public class ResultTests
    {
        private readonly AnonymousData anon = new AnonymousData();

        [Fact(DisplayName = "Constructing a Success gives expected value")]
        public void SuccessTest()
        {
            var result = Success(10);

            int? value = result switch
            {
                Result<int>.Success(var v) => v,
                Result<int>.Error e => null
            };

            Assert(value).IsEqualTo(10);
        }

        [Fact(DisplayName = "Constructing an Error gives expected values")]
        public void ErrorTest()
        {
            var expectedPosition = anon.AnyPosition();
            var expectedMessage = anon.Any<string>();

            Result<int> result = Error(expectedPosition, expectedMessage);

            var error = (Result<int>.Error)result;

            AssertAll(
                "Not all properties are correct",
                () => Assert(error.Message).IsEqualTo(expectedMessage, "Incorrect Message"),
                () => Assert(error.Position).IsEqualTo(expectedPosition, "Incorrect Position"),
                () => Assert(error.Expected).IsEmpty("Incorrect Expected"));
            
        }
    }
}
