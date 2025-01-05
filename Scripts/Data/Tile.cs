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
        public Tile(int x, int y)
        {
            //Assign values.
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
        bool isActivated;
        public Trap(int x, int y, bool isActivated) : base(x, y)
        {
            this.isActivated = isActivated;
        }

        public void Activate()
        {
            isActivated = true;
        }

        public void Deactivate()
        {
            isActivated = false;
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