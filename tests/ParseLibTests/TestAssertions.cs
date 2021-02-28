using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testify;
using static Testify.Assertions;

namespace ParseLib.Tests
{
    public static class TestAssertions
    {
        public static void IsEmpty<T>(
            this ActualValue<IEnumerable<T>> actual,
            string message,
            params object[] parameters)
            => actual.IsSequenceEqualTo(Enumerable.Empty<T>(), message, parameters);

        public static void IsEmpty<T>(
            this ActualValue<IEnumerable<T>> actual)
            => actual.IsSequenceEqualTo(Enumerable.Empty<T>());

        public static void IsSuccess<T>(
            this ActualValue<Result<T>> actual, T value, int line, int column)
        {
            if (actual.Value is Result<T>.Success s)
            {
                AssertAll("Success Result does not have expected properties.",
                    () => Assert(value).IsEqualTo(s.Value, "Value is not correct."),
                    () => Assert(s.RemainingInput.Position)
                        .IsEqualTo(new Position(new Line(line), new Column(column)), "Position is not correct.")
                );
            }
            else
            {
                Fail("Result is not a Success.");
            }
        }

        public static void IsSuccess<T>(
            this ActualValue<Result<T>> actual, T value, IEqualityComparer<T> comparer, int line, int column)
        {
            if (actual.Value is Result<T>.Success s)
            {
                AssertAll("Success Result does not have expected properties.",
                    () => Assert(comparer.Equals(value, s.Value)).IsTrue("Value is not correct."),
                    () => Assert(s.RemainingInput.Position)
                        .IsEqualTo(new Position(new Line(line), new Column(column)), "Position is not correct.")
                );
            }
            else
            {
                Fail("Result is not a Success.");
            }
        }

        public static void IsError<T>(
            this ActualValue<Result<T>> actual, int line, int column, string message)
        {
            if (actual.Value is Result<T>.Error e)
            {
                AssertAll("Error Result does not have expected properties.",
                    () => Assert(e.Position)
                        .IsEqualTo(new Position(new Line(line), new Column(column)), "Position is not correct."),
                    () => Assert(e.ToString()).IsEqualTo(message, "Error message is not correct.")
                );
            }
            else
            {
                Fail("Result is not an Error.");
            }
        }
    }
}
