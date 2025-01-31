namespace MazeRunner.Scripts.Data;

public abstract class Skill
{
    static public int BatteryLife { get; private set; }
    public Skill(int batteryLife)
    {
        BatteryLife = batteryLife;
    }
}

public class Shield : Skill
{
    public Shield() : base(20) { }
}

public class PortalGun : Skill
{
    public PortalGun() : base(20) { }
}

public class Blind : Skill
{
    public System.Timers.Timer Timer { get; private set; } = new(10000);

    public Blind() : base(20) { }
}

public class Muter : Skill
{
    public System.Timers.Timer Timer { get; private set; } = new(10000);

    public Muter() : base(20) { }
}

public class Glare : Skill
{
    public int Radius { get; set; } = 20;
    public System.Timers.Timer Timer { get; private set; } = new(10000);

    public Glare() : base(20) { }
}