using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace MazeRunner.Scripts.Data;

/// <summary>
/// Player skill.
/// </summary>
public class Skill
{
    /// <summary>
    /// Skill name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Skill description.
    /// </summary>
    public string Description { get; set; }

    public int Percentage { get; set; }

    public Skill(string name, string description, int percentage)
    {
        Name = name;
        Description = description;
        Percentage = percentage;
    }
}

public class Shield : Skill
{
    public int Health { get; set; } = 2;
    public Shield() : base("Escudo", "Protección contra trampas.", 30)
    {
    }
}