using Godot;
using MazeRunner.Scripts.Logic;

namespace MazeRunner.Scripts;
public partial class Global : Node
{
    public Setting Setting { get; set; }
    public int? Difficulty { get; set; } = 0;
    public int? Seed { get; set; } = 1805123040;
    public bool IsRandom { get; set; } = true;
    public int PlayerOneSkill { get; set; } = 0;
    public int PlayerTwoSkill { get; set; } = 0;
    public string PlayerOneName { get; set; } = string.Empty;
    public string PlayerTwoName { get; set; } = string.Empty;
    
    public override void _Ready() { }
    public override void _Process(double delta) { }
    public void SetMaze() { Setting = new((int)Difficulty, (int)Seed, IsRandom); }
}