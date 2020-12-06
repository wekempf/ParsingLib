namespace ParseLib
{
    public record Line(int Value);

    public record Column(int Value);

    public record Position(Line Line, Column Column)
    {
        public override string ToString() => $"({Line.Value}, {Column.Value})";
    }
}
