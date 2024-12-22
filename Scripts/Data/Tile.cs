namespace MazeRunner.Scripts.Data
{
    public class Tile
    {
        public int X { get; }
        public int Y { get; }
        public Tile(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class Empty : Tile
    {
        public Empty(int x, int y) : base(x, y)
        {
        }
    }

    public class Wall : Tile
    {
        public Wall(int x, int y) : base(x, y)
        {
        }
    }
}