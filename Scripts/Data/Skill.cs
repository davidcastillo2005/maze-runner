namespace MazeRunner.Scripts.Data;

public class Skill
{
    public Skill() { }
}

public class Shield : Skill
{
    static public int BatteryLife = 20;
    public Shield() : base() { }
}

public class PortalGun : Skill
{
    static public int BatteryLife = 20;

    public PortalGun() : base() { }
}

public class Blind : Skill
{
    static public int BatteryLife = 20;
    public System.Timers.Timer Timer { get; private set; } = new(10000);

    public Blind() : base() { }
}

public class Muter : Skill
{
    static public int BatteryLife = 20;
    public System.Timers.Timer Timer { get; private set; } = new(10000);

    public Muter() : base() { }
}

public class Glare : Skill
{
    static public int BatteryLife = 20;
    public int Radius { get; set; } = 20;
    public System.Timers.Timer Timer { get; private set; } = new(10000);

    public Glare() : base() { }
}