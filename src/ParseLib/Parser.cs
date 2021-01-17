using System;
using System.Collections.Generic;
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
    }
}
