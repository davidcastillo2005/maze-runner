using Godot;
using System;
using MazeRunner.Scripts.Logic;

namespace MazeRunner.Scripts;

public partial class PlayerCamera : Camera2D
{
    [Export] private Player _player;

    private Global _global;
    private MazeGenerator _mazeGenerator;

    public enum State
    {
        Player,
        Free,
        Extensive
    }

    public State CurrentState { get; private set; }
    private Vector2 _cameraOffset;
    private Vector2 _input;

    public override void _Ready()
    {
        _global = GetNode<Global>("/root/Global");
        _mazeGenerator = _global.Setting.MazeGenerator;

        CurrentState = State.Player;
        _input = Vector2.Zero;
        _cameraOffset = Vector2.Zero;

        Position = new Vector2(_player.GetConvertedPos(_mazeGenerator.SpawnerCoord.x), _player.GetConvertedPos(_mazeGenerator.SpawnerCoord.y));

        if (_global.LevelDifficulty < 5) CurrentState = State.Extensive;
    }

    public override void _Input(InputEvent @event)
    {
        _input = Input.GetVector(_player.Left, _player.Right, _player.Up, _player.Down);
    }


    public override void _Process(double delta)
    {
        switch (CurrentState)
        {
            case State.Player:
                if (Input.IsActionJustPressed(_player.ShiftCamera)) CurrentState = State.Free;
                else OnPlayer();
                break;
            case State.Free:
                if (Input.IsActionJustPressed(_player.ShiftCamera)) CurrentState = State.Player;
                else OnFree();
                break;
            case State.Extensive:
                OnExtensive();
                break;
            default: throw new Exception();
        }
    }

    private void OnPlayer()
    {
        _cameraOffset = Vector2.Zero;
        Position = _player.Position;
    }

    private void OnFree()
    {
        _cameraOffset += _player.Board.TileSize * _input;
        Position = _player.Position + _cameraOffset;
    }

    private void OnExtensive()
    {
        Zoom = new Vector2((float)(Math.Pow(_player.Board.TileSize, -1) * Math.Pow(_mazeGenerator.Size, -1) * 720), (float)(Math.Pow(_player.Board.TileSize, -1) * Math.Pow(_mazeGenerator.Size, -1) * 720));
        Position = new Vector2(_mazeGenerator.Size * _player.Board.TileSize * 0.5f, _mazeGenerator.Size * _player.Board.TileSize * 0.5f);
    }
}