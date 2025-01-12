using Godot;
using MazeRunner.Scripts.Logic;

public partial class Global : Node
{
    public Setting Setting { get; set; }
    public int LevelDifficulty {get; set;} = 6;

    private int _seed = -831256837;
    private bool _isRandomSeed = true;

    public override void _Ready()
    {
        Setting = new(LevelDifficulty, _seed, _isRandomSeed);
        Setting.MazeGenerator.GenerateMaze();
    }

    public override void _Process(double delta)
    {
    }
}
