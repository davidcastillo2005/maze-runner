namespace MazeRunner.Scripts.Data;

public abstract class Skill
{
    public Skill()
    {
    }
}

public class Shield : Skill
{
    public Shield() : base() { }
}

public class PortalGun : Skill
{
    public PortalGun() : base() { }
}

public class Blind : Skill
{
    public System.Timers.Timer Timer { get; private set; } = new(10000);

    public Blind() : base() { }
}

public class Mute : Skill
{
    public System.Timers.Timer Timer { get; private set; } = new(10000);

    public Mute() : base() { }
}

public class Glare : Skill
{
    public int Radius { get; set; } = 20;
    public System.Timers.Timer Timer { get; private set; } = new(10000);

    public Glare() : base() { }
}