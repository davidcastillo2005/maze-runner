namespace MazeRunner.Scripts.Data
{
    /// <summary>
    /// Tile.
    /// </summary>
    public class Tile
    {
        /// <summary>
        /// X component.
        /// </summary>
        public int X { get => _x; set => _x = value; }
        private int _x;
        /// <summary>
        /// Y component
        /// </summary>
        public int Y { get => _y; set => _y = value; }
        private int _y;

        /// <summary>
        /// Tile constructor.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Tile(int x, int y)
        {
            //Assign values.
            _x = x;
            _y = y;
        }
    }

    /// <summary>
    /// Empty tile.
    /// </summary>
    public class Empty : Tile
    {
        /// <summary>
        /// Constructor of the Empty class.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Empty(int x, int y) : base(x, y) { }
    }

    /// <summary>
    /// Wall tile (obstacule).
    /// </summary>
    public class Wall : Tile
    {
        /// <summary>
        /// Wall constructor.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Wall(int x, int y) : base(x, y) { }
    }

    /// <summary>
    /// Exit tile.
    /// </summary>
    public class Exit : Empty
    {
        /// <summary>
        /// Exit constructor.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Exit(int x, int y) : base(x, y) { }
    }

    /// <summary>
    /// Player spawner tile.
    /// </summary>
    public class Spawner : Empty
    {
        /// <summary>
        /// Spawner constructor.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Spawner(int x, int y) : base(x, y) { }
    }
}