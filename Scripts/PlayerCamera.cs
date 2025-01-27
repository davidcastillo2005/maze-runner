using Godot;
using System;
using MazeRunner.Scripts.Logic;

namespace MazeRunner.Scripts;

public partial class PlayerCamera : Camera2D
{
    [Export] private Player _player;
    [Export] private Board _board;

    private Global _global;
    private MazeGenerator _mazeGenerator;

    private float _minPosition;
    private float _maxPosition;

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

        _minPosition = 0 + 720 * 0.5f * (float)Math.Pow(Zoom.X, -1);
        _maxPosition = _board.PixelSize - 720 * 0.5f * (float)Math.Pow(Zoom.X, -1);

        Position = _player.Position;

        if (_global.LevelDifficulty < 5) CurrentState = State.Extensive;
    }

    public override void _Input(InputEvent @event)
    {
        _input = Input.GetVector(_player.Leftkey, _player.RightKey, _player.UpKey, _player.DownKey);
    }


    public override void _Process(double delta)
    {
        switch (CurrentState)
        {
            case State.Player:
                if (Input.IsActionJustPressed(_player.ShiftCameraKey)) CurrentState = State.Free;
                else OnPlayer();
                break;
            case State.Free:
                if (Input.IsActionJustPressed(_player.ShiftCameraKey)) CurrentState = State.Player;
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
        Position = new Vector2(Math.Clamp(_player.Position.X, _minPosition, _maxPosition), Math.Clamp(_player.Position.Y, _minPosition, _maxPosition));
    }

    private void OnFree()
    {
        _cameraOffset += _player.Board.TileSize * _input * 0.5f;
        Position = new Vector2(Math.Clamp(_player.Position.X + _cameraOffset.X, _minPosition, _maxPosition), Math.Clamp(_player.Position.Y + _cameraOffset.Y, _minPosition, _maxPosition));

        Vector2 temp = new Vector2(_player.Position.X + _cameraOffset.X, _player.Position.Y + _cameraOffset.Y);
        if (temp.X < _minPosition)// || temp.Y < _minPosition || temp.X > _maxPosition || temp.Y > _maxPosition)
        {
            temp = new Vector2(_minPosition, temp.Y);
        }
        else if (temp.X > _maxPosition)
        {
            temp = new Vector2(_maxPosition, temp.Y);
        }
        if (temp.Y < _minPosition)
        {
            temp = new Vector2(temp.X, _minPosition);
        }
        else if (temp.Y > _maxPosition)
        {
            temp = new Vector2(temp.X, _maxPosition);
        }

        _cameraOffset = temp - _player.Position;
    }

    private void OnExtensive()
    {
        Zoom = new Vector2((float)(Math.Pow(_player.Board.TileSize, -1) * Math.Pow(_mazeGenerator.Size, -1) * 720), (float)(Math.Pow(_player.Board.TileSize, -1) * Math.Pow(_mazeGenerator.Size, -1) * 720));
        Position = new Vector2(_mazeGenerator.Size * _player.Board.TileSize * 0.5f, _mazeGenerator.Size * _player.Board.TileSize * 0.5f);
    }
}