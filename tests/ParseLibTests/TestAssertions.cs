using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testify;

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
    }
}
