using Testify;

namespace ParseLib.Tests
{
    public static class TestAnonymousData
    {
        public static Line AnyLine(this IAnonymousData anon) => new Line(anon.AnyInt32(1, int.MaxValue));

        public static Column AnyColumn(this IAnonymousData anon) => new Column(anon.AnyInt32(1, int.MaxValue));

        public static Position AnyPosition(this IAnonymousData anon) => new Position(anon.AnyLine(), anon.AnyColumn());
    }
}
