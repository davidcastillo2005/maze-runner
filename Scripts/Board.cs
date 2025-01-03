using Godot;
using MazeRunner.Scripts.Data;
using MazeRunner.Scripts.Logic;

public partial class Board : TileMapLayer
{
	private MazeGenerator _mazeGenerator;
	private Global _global;
	public int PixelSize => _pixelsize;
	private int _pixelsize;
	public override void _Ready()
	{
		_global = GetNode<Global>("/root/Global");
		_mazeGenerator = _global.Setting.MazeGenerator;
		_mazeGenerator.GenerateMaze();
		_pixelsize = TileSet.TileSize.X;
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
				if (_mazeGenerator.Maze[x, y] is Empty) SetCell(new Vector2I(x, y), 2, new Vector2I(2, 0), 0);
				else if (_mazeGenerator.Maze[x, y] is Wall) SetCell(new Vector2I(x, y), 2, new Vector2I(1, 0), 0);
				else if (_mazeGenerator.Maze[x, y] is Spawner) SetCell(new Vector2I(x, y), 2, new Vector2I(0, 0), 0);
				else if (_mazeGenerator.Maze[x, y] is Exit) SetCell(new Vector2I(x, y), 2, new Vector2I(0, 0), 0);
			}
		}
	}
}
