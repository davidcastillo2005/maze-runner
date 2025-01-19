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

        public Tile(int x, int y)
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
        public Empty(int x, int y) : base(x, y) { }
    }

    /// <summary>
    /// Wall tile (obstacule).
    /// </summary>
    public class Wall : Tile
    {
        public Wall(int x, int y) : base(x, y) { }
    }

    /// <summary>
    /// Exit tile.
    /// </summary>
    public class Exit : Empty
    {
        public Exit(int x, int y) : base(x, y) { }
    }

    /// <summary>
    /// Player spawner tile.
    /// </summary>
    public class Spawner : Empty
    {
        public Spawner(int x, int y) : base(x, y) { }
    }

    /// <summary>
    /// Base trap class.
    /// </summary>
    public abstract class Trap : Empty
    {
        public bool IsActivated { get; set; }
        public Trap(int x, int y, bool isActivated) : base(x, y)
        {
            IsActivated = isActivated;
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

    /// <summary>
    /// Spikes Trap.
    /// </summary>
    public class Spikes : Trap
    {
        public Spikes(int x, int y, bool isActivated) : base(x, y, isActivated) { }
    }

    /// <summary>
    /// Portal Trap. 
    /// </summary>
    public class Portal : Trap
    {
        public Portal(int x, int y, bool isActivated) : base(x, y, isActivated) { }
    }

    /// <summary>
    /// Sticky Trap.
    /// </summary>
    public class Sticky : Trap
    {
        public Sticky(int x, int y, bool isActivated) : base(x, y, isActivated) { }
    }
}