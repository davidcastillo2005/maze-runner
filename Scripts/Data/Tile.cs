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
        public int X { get; set; }
        /// <summary>
        /// Y component
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Tile constructor.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected Tile(int x, int y)
        {
            X = x;
            Y = y;
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

    public class Trap : Empty
    {
        public bool IsActivated { get; set; }
        public Trap(int x, int y, bool isActivated) : base(x, y)
        {
            this.IsActivated = isActivated;
        }

        public void Activate()
        {
            IsActivated = true;
        }

        public void Deactivate()
        {
            IsActivated = false;
        }
    }

    public class Spikes : Trap
    {
        public int SpeedMultiPlier { get; set; } = 1;
        public Spikes(int x, int y, bool isActivated) : base(x, y, isActivated)
        {

        }
    }
}