using Godot;
using MazeRunner.Scripts.Logic;

namespace MazeRunner.Scripts;

public partial class Global : Node
{
    public int Viewport = 720;
    public int? Size { get; set; } = 0;
    public int? Seed { get; set; } = 1805123040;
    public bool IsRandom { get; set; } = true;
    public int PlayerOneSkill { get; set; } = 0;
    public int PlayerTwoSkill { get; set; } = 0;
    public string PlayerOneName { get; set; } = string.Empty;
    public string PlayerTwoName { get; set; } = string.Empty;
    public MazeGenerator MazeGenerator;

    public void SetMaze()
    {
        MazeGenerator = new((int)Size, (int)Seed, IsRandom);
    }
}