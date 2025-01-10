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
                bool isSpikeTrap = false;
                bool isActivated = false;
                
                foreach (var item in _mazeGenerator.SpikesTrapsCoords)
                {
                    if (x == item.Key.x && y == item.Key.y)
                    {
                        isSpikeTrap = true;
                        if (item.Value.IsActivated)
                        {
                            isActivated = true;
                            break;
                        }
                    }
                }

                if (isSpikeTrap)
                {
                    if (isActivated)
                    {
                        SetCell(new Vector2I(x, y), -1, new Vector2I(0, 0));
                    }
                    else
                    {
                        SetCell(new Vector2I(x, y), 2, new Vector2I(0, 0));
                    }
                }
                else if (_mazeGenerator.Maze[x, y] is Spawner)
                    SetCell(new Vector2I(x, y), 2, new Vector2I(0, 0));
                else if (_mazeGenerator.Maze[x, y] is Exit)
                    SetCell(new Vector2I(x, y), 2, new Vector2I(0, 0));
                else if (_mazeGenerator.Maze[x, y] is Empty)
                    SetCell(new Vector2I(x, y), 2, new Vector2I(0, 0));
                else if (_mazeGenerator.Maze[x, y] is Wall) SetCell(new Vector2I(x, y), 2, new Vector2I(3, 0));
            }
        }
    }
}