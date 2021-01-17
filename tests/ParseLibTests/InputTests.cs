using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Testify;
using Xunit;
using static Testify.Assertions;

namespace ParseLib.Tests
{
    public class InputTests
    {
        private readonly AnonymousData anon = new();

        [Fact]
        public void FromStringTest()
        {
            var input = Input.From(anon.Any<string>());

            Assert(input).IsNotNull();
        }

        [Fact]
        public void FromTextReader()
        {
            var input = Input.From(new StringReader(anon.Any<string>()));

            Assert(input).IsNotNull();
        }

        [Theory]
        [InlineData("xyzzy", 'x')]
        [InlineData("plough", 'p')]
        public void CurrentShouldReturnCurrentChar(string inputText, char expectedChar)
        {
            var input = Input.From(inputText);

            Assert(input.Current).IsEqualTo(expectedChar);
        }
    }
}
