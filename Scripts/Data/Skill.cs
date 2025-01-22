using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace MazeRunner.Scripts.Data;

/// <summary>
/// Player skill.
/// </summary>
public class Skill
{
    public string Name { get; set; }

    public string Description { get; set; }

    public Skill(string name, string description)
    {
        Name = name;
        Description = description;
    }
}

public class Shield : Skill
{
    public int Health { get; set; } = 2;
    public Shield() : base("Escudo", "Protección contra trampas.")
    {
    }
}

public class PortalGun : Skill
{
    public int Battery { get; set; } = 2;
    public PortalGun() : base("Pistola de portales", "Atravieza portales.")
    {
    }
}

public class Boost : Skill
{
    public int Battery { get; set; } = 2;
    public int Multiplier {get; private set; } = 2;
    public Boost() : base("Velocidad", "Mayor velocidad hasta llegar a una trampa.")
    {
    }
}