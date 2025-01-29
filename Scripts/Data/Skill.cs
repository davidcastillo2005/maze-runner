namespace MazeRunner.Scripts.Data;

public class Skill
{
    public Skill() { }
}

public class Shield : Skill
{
    public Shield() : base() { }
}

public class PortalGun : Skill
{
    public PortalGun() : base() { }
}

public class Blindness : Skill
{
    public System.Timers.Timer Timer { get; private set; } = new(1000);
    public Blindness() : base() { }
}

public class Muter : Skill
{
    public System.Timers.Timer Timer { get; private set; } = new(1000);
    public Muter() : base() { }
}

public class Predator : Skill
{
    public int Radius { get; set; } = 10;
    public Predator() : base() { }
}