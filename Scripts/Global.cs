using Godot;
using MazeRunner.Scripts.Logic;

namespace MazeRunner.Scripts;
public partial class Global : Node
{
    public Setting Setting { get; set; }
    public int LevelDifficulty { get; set; } = 5;

    public int Seed { get; set; } = 1805123040;
    public bool IsRandomSeed { get; set; } = true;
    public int PlayerOneSkill { get; set; } = 0;
    public int PlayerTwoSkill { get; set; } = 0;

    public override void _Ready()
    {
    }

    public override void _Process(double delta)
    {
    }

    public void SetMaze()
    {
        Setting = new(LevelDifficulty, Seed, IsRandomSeed);
    }
}