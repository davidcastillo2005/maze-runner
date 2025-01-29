using System;
using Godot;
using MazeRunner.Scripts.Data;
using MazeRunner.Scripts.Logic;

namespace MazeRunner.Scripts;

public partial class Board : TileMapLayer
{
    public int TileSize { get; private set; }

    private MazeGenerator _mazeGenerator;
    private Global _global;
    public int PixelSize;

    public override void _Ready()
    {
        _global = GetNode<Global>("/root/Global");
        _mazeGenerator = _global.Setting.MazeGenerator;
        _mazeGenerator.GenerateMaze();
        TileSize = TileSet.TileSize.X;
        PixelSize = TileSize * _mazeGenerator.Size;
    }
    public override void _Process(double delta) { PaintBoardTileMapLayer(); }
    public float GetConvertedPos(int i)
    {
        return (i + 0.5f) * TileSize;
    }

    private void PaintBoardTileMapLayer()
    {
        Clear();
        for (int x = 0; x < _mazeGenerator.Size; x++)
        {
            for (int y = 0; y < _mazeGenerator.Size; y++)
            {
                if (_mazeGenerator.Maze[x, y] is Spawner)
                    SetCell(new Vector2I(x, y), 0, new Vector2I(0, 0));
                else if (_mazeGenerator.Maze[x, y] is Exit)
                    SetCell(new Vector2I(x, y), 0, new Vector2I(0, 0));
                else if (_mazeGenerator.Maze[x, y] is Empty and not Exit and not Spawner and not Spikes and not Portal and not Sticky)
                {
                    SetCell(new Vector2I(x, y), 0, new Vector2I(0, 0));
                }
                else if (_mazeGenerator.Maze[x, y] is Wall)
                    SetCell(new Vector2I(x, y), 3, new Vector2I(0, 0));
                else if (_mazeGenerator.Maze[x, y] is Spikes spikes)
                {
                    if (spikes.IsActivated) SetCell(new Vector2I(x, y), 2, new Vector2I(0, 0));
                    else SetCell(new Vector2I(x, y), 0, new Vector2I(0, 0));
                }
                else if (_mazeGenerator.Maze[x, y] is Portal portal)
                {
                    if (portal.IsActivated) SetCell(new Vector2I(x, y), 2, new Vector2I(0, 0));
                    else SetCell(new Vector2I(x, y), 0, new Vector2I(0, 0));
                }
                else if (_mazeGenerator.Maze[x, y] is Sticky sticky)
                {
                    if (sticky.IsActivated) SetCell(new Vector2I(x, y), 2, new Vector2I(0, 0));
                    else SetCell(new Vector2I(x, y), 0, new Vector2I(0, 0));
                }
            }
        }
    }
}