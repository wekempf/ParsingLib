using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseLib
{
    public sealed class Parser<T>
    {
        private readonly Func<Input, Result<T>> func;

        public Parser(Func<Input, Result<T>> func)
            => this.func = Guard.Against.Null(func, nameof(func));

        public Result<T> Parse(Input input) => func.Invoke(input);

        public Result<T> Parse(string text) => Parse(Input.From(text));

        public Result<T> Parse(TextReader reader) => Parse(Input.From(reader));

        public static Parser<T> operator |(Parser<T> left, Parser<T> right)
            => left.Or(right);
    }
}
