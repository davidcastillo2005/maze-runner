namespace MazeRunner.Scripts.Data;

public class Tile
{
    public int X { get; set; }
    public int Y { get; set; }

    public Tile(int x, int y)
    {
        X = x;
        Y = y;
    }
}

public class Empty : Tile
{
    public Empty(int x, int y) : base(x, y) { }
}

public class Exit : Empty
{
    public Exit(int x, int y) : base(x, y) { }
}

public class Spawner : Empty
{
    public Spawner(int x, int y) : base(x, y) { }
}

public abstract class Trap : Empty
{
    public bool IsActive { get; set; }

    public Trap(int x, int y, bool isActive) : base(x, y) { IsActive = isActive; }

    public void Activate() { IsActive = true; }
    public void Deactivate() { IsActive = false; }
}

public class Spikes : Trap
{
    static public System.Timers.Timer Timer { get; private set; } = new(10000);

    public Spikes(int x, int y, bool isActivated) : base(x, y, isActivated) { }
}

public class Portal : Trap
{
    public Portal(int x, int y, bool isActivated) : base(x, y, isActivated) { }
}

public class Shock : Trap
{
    static public int Struggle { get; private set; } = 10;
    public Shock(int x, int y, bool isActivated) : base(x, y, isActivated) { }
}

public class Wall : Tile
{
    public Wall(int x, int y) : base(x, y) { }
}
