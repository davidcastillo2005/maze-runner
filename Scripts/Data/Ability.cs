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

    public Skill(string name, string description)
    {
        Name = name;
        Description = description;
    }
}