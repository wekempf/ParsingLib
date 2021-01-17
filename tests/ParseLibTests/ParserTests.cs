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
    public class ParserTests
    {
        [Fact]
        public void TestCtor()
        {
            var parser = new Parser<int>(input => Result.Success(1, input));

            Assert(parser).IsNotNull();
        }

        [Fact]
        public void ParseShouldReturnResult()
        {
            var parser = new Parser<int>(input => Result.Success(1, input));
            var input = Input.From("");

            var result = parser.Parse(input);

            Assert(result).IsSuccess(1, line: 1, column: 1);
        }
    }
}
