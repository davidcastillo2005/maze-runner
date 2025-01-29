namespace MazeRunner.Scripts.Data;

public class Skill
{
    public Skill() { }
}

public class Shield : Skill
{
    public int BatteryLife = 20;
    public Shield() : base() { }
}

public class PortalGun : Skill
{
    public int BatteryLife = 20;

    public PortalGun() : base() { }
}

public class Blindness : Skill
{
    public int BatteryLife = 20;
    public System.Timers.Timer Timer { get; private set; } = new(10000);

    public Blindness() : base() { }
}

public class Muter : Skill
{
    public int BatteryLife = 10;
    public System.Timers.Timer Timer { get; private set; } = new(10000);

    public Muter() : base() { }
}

public class Predator : Skill
{
    public int BatteryLife = 20;
    public int Radius { get; set; } = 10;
    public System.Timers.Timer Timer { get; private set; } = new(10000);

    public Predator() : base() { }
}