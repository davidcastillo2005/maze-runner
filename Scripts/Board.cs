using Godot;
using MazeRunner.Scripts.Data;
using MazeRunner.Scripts.Logic;

namespace MazeRunner.Scripts;

public partial class Board : TileMapLayer
{
    public int TileSize { get; private set; }

    private MazeGenerator _mazeGenerator;
    private Global _global;

    public override void _Ready()
    {
        _global = GetNode<Global>("/root/Global");
        _mazeGenerator = _global.Setting.MazeGenerator;
        _mazeGenerator.GenerateMaze();
        TileSize = TileSet.TileSize.X;
    }

    public override void _Process(double delta)
    {
        PaintBoardTileMapLayer();
    }

    void PaintBoardTileMapLayer()
    {
        Clear();
        for (int x = 0; x < _mazeGenerator.Size; x++)
        {
            for (int y = 0; y < _mazeGenerator.Size; y++)
            {
                if (_mazeGenerator.Maze[x, y] is Spawner)
                    SetCell(new Vector2I(x, y), 2, new Vector2I(0, 0));
                else if (_mazeGenerator.Maze[x, y] is Exit)
                    SetCell(new Vector2I(x, y), 2, new Vector2I(0, 0));
                else if (_mazeGenerator.Maze[x, y] is Empty and not Exit and not Spawner and not Spikes)
                    SetCell(new Vector2I(x, y), 2, new Vector2I(0, 0));
                else if (_mazeGenerator.Maze[x, y] is Wall)
                    SetCell(new Vector2I(x, y), 2, new Vector2I(3, 0));
                else if (_mazeGenerator.Maze[x, y] is Spikes spikes)
                {
                    if (spikes.IsActivated)
                    {
                        SetCell(new Vector2I(x, y), -1, new Vector2I(0, 0));
                    }
                    else
                    {
                        SetCell(new Vector2I(x, y), 2, new Vector2I(0, 0));
                    }
                }
            }
        }
    }
}