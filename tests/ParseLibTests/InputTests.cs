using System.IO;
using System.Linq;


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

        [Fact]
        public void PositionShouldStartAtLine1Column1()
        {
            var expectedPos = new Position(new Line(1), new Column(1));

            var input = Input.From("ab");

            Assert(input.Position).IsEqualTo(expectedPos);
        }

        [Fact]
        public void RemainingInputShouldBeAtNextPosition()
        {
            var expectedPos = new Position(new Line(1), new Column(2));

            var input = Input.From("ab");

            Assert(input.RemainingInput.Position).IsEqualTo(expectedPos);
        }

        [Fact]
        public void RemainingInputShouldBeAtNextCharacter()
        {
            var input = Input.From("ab");

            Assert(input.RemainingInput.Current).IsEqualTo('b');
        }

        [Fact]
        public void IsAtEndShouldBeTrueWithEmptyInput()
        {
            var input = Input.From("");

            Assert(input.IsAtEnd).IsTrue();
        }


        [Fact]
        public void IsAtEndShouldBeFalseWithNonEmptyInput()
        {
            var input = Input.From("ab");

            Assert(input.IsAtEnd).IsFalse();
        }

        [Fact]
        public void CarriageReturnNewLineShouldBeTreatedAsOneChar()
        {
            var expectedPos = new Position(new Line(2), new Column(1));

            var input = Input.From("\r\na");

            AssertAll(
                "RemainingInput not expected state.",
                () => Assert(input.RemainingInput.Position).IsEqualTo(expectedPos),
                () => Assert(input.RemainingInput.Current).IsEqualTo('a'));
        }

        [Fact]
        public void NewLineShouldBeTreatedAsOneChar()
        {
            var expectedPos = new Position(new Line(2), new Column(1));

            var input = Input.From("\na");

            AssertAll(
                "RemainingInput not expected state.",
                () => Assert(input.RemainingInput.Position).IsEqualTo(expectedPos),
                () => Assert(input.RemainingInput.Current).IsEqualTo('a'));
        }

        [Fact]
        public void CarriageReturnNewLineShouldSkipCarriageReturn()
        {
            var expectedPos = new Position(new Line(1), new Column(1));

            var input = Input.From("\r\na");

            AssertAll(
                "RemainingInput not expected state.",
                () => Assert(input.Position).IsEqualTo(expectedPos),
                () => Assert(input.Current).IsEqualTo('\n'));
        }

        [Fact]
        public void CarriageReturnShouldNotSkipCarriageReturn()
        {
            var expectedPos = new Position(new Line(1), new Column(1));

            var input = Input.From("\ra");

            AssertAll(
                "RemainingInput not expected state.",
                () => Assert(input.Position).IsEqualTo(expectedPos),
                () => Assert(input.Current).IsEqualTo('\r'));
        }

        [Fact]
        public void CrNlCrNlRemainingShouldBeOnSecondNl()
        {
            var expectedPos = new Position(new Line(2), new Column(1));

            var input = Input.From("\r\n\r\na");

            AssertAll(
                "RemainingInput not expected state.",
                () => Assert(input.RemainingInput.Position).IsEqualTo(expectedPos),
                () => Assert(input.RemainingInput.Current).IsEqualTo('\n'));
        }

        [Fact]
        public void NlNlRemainingShouldBeOnSecondNl()
        {
            var expectedPos = new Position(new Line(2), new Column(1));

            var input = Input.From("\n\na");

            AssertAll(
                "RemainingInput not expected state.",
                () => Assert(input.RemainingInput.Position).IsEqualTo(expectedPos),
                () => Assert(input.RemainingInput.Current).IsEqualTo('\n'));
        }
    }
}
