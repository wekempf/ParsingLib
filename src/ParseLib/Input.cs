using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseLib
{
    public class Input
    {
        private readonly Lazy<Input> remaining;

        private Input(TextReader reader, Position? position = null)
        {
            Position = position ?? new Position(new Line(1), new Column(1));

            var inp = reader.Read();
            IsAtEnd = inp < 0;

            if (!IsAtEnd)
            {
                Current = (char)inp;
            }

            remaining = new Lazy<Input>(() => new Input(reader, Position.Next()));
        }

        public static Input From(string text) => From(new StringReader(text));

        public static Input From(TextReader textReader) => new Input(textReader);

        public char Current { get; init; }

        public Position Position { get; init; }

        public Input RemainingInput => remaining.Value;

        public bool IsAtEnd { get; init; }
    }
}
