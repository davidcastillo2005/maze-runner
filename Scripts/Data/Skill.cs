using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace MazeRunner.Scripts.Data;

/// <summary>
/// Player skill.
/// </summary>
public class Skill
{

    public Skill()
    {
    }
}

public class Shield : Skill
{
    public int Health { get; set; } = 2;
    public Shield() : base()
    {
    }
}

public class PortalGun : Skill
{
    public int Battery { get; set; } = 2;
    public PortalGun() : base()
    {
    }
}

public class Boost : Skill
{
    public int Battery { get; set; } = 2;
    public int Multiplier { get; private set; } = 4;
    public Boost() : base()
    {
    }
}

public class Blindness : Skill
{
    public int Battery { get; set; } = 2;
    public Blindness() : base()
    {
    }
}

public class Muter : Skill
{
    public int Battery { get; set; } = 2;
    public int Radius {get; set; } = 10;
    public Muter() : base()
    {
    }
}