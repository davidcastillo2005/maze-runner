namespace MazeRunner.Scripts.Data;

public abstract class Skill
{
    public int BatteryLife { get; private set; } = 0;
    public int Radius { get; private set; } = 10;
    public Skill(int batteryLife, int radius)
    {
        BatteryLife = batteryLife;
        Radius = radius;
    }
}

public class Shield : Skill
{
    public Shield() : base(20, 0) { }
}

public class PortalGun : Skill
{
    public PortalGun() : base(40, 0) { }
}

public class Blind : Skill
{
    public System.Timers.Timer Timer { get; private set; } = new(10000);

    public Blind() : base(20, 10) { }
}

public class Mute : Skill
{
    public System.Timers.Timer Timer { get; private set; } = new(10000);

    public Mute() : base(20, 10) { }
}

public class Glare : Skill
{
    public System.Timers.Timer Timer { get; private set; } = new(10000);

    public Glare() : base(20, 10) { }
}