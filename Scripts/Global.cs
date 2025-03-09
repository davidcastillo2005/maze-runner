using Godot;
using MazeRunner.Scripts.Logic;

namespace MazeRunner.Scripts;

public partial class Global : Node
{
    public int Viewport = 720;
    public int Size { get; set; } = 10;
    public int Seed { get; set; } = 0;
    public bool IsRandom { get; set; } = true;
    public int PlayerOneSkill { get; set; } = 0;
    public int PlayerTwoSkill { get; set; } = 0;
    public string PlayerOneName { get; set; } = string.Empty;
    public string PlayerTwoName { get; set; } = string.Empty;
    public string PlayerNameWon { get; set; } = string.Empty;
    public string PlayerNameLost { get; set; } = string.Empty;
    public MazeGenerator MazeGenerator;

    public void SetMaze()
    {
        MazeGenerator = new(Size, Seed, IsRandom);
    }

    public void ResetGame()
    {
        PlayerOneSkill = 0;
        IsRandom = true;
        PlayerOneName = string.Empty;
        PlayerTwoName = string.Empty;
        PlayerNameWon = string.Empty;
    }
}