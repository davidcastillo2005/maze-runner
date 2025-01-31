using Godot;
using System;
using MazeRunner.Scripts.Logic;

namespace MazeRunner.Scripts;

public partial class PlayerCamera : Camera2D
{
    [Export] private Player _player;
    [Export] private Board _board;

    public State CurrentState { get; private set; }
    public enum State { Player, Free, Extensive }

    private Global _global;
    private float _minPosition;
    private float _maxPosition;
    private Vector2 _cameraOffset;
    private Vector2 _input;

    public override void _Ready()
    {
        _global = GetNode<Global>("/root/Global");
        _input = Vector2.Zero;
        _cameraOffset = Vector2.Zero;
        _minPosition = 0 + _global.Viewport * 0.5f * (float)Math.Pow(Zoom.X, -1);
        _maxPosition = _board.PixelSize - _global.Viewport * 0.5f * (float)Math.Pow(Zoom.X, -1);

        CurrentState = State.Player;
        Position = _player.Position;

        if (_global.Size < 12) CurrentState = State.Extensive;
    }
    public override void _Input(InputEvent @event) { _input = Input.GetVector(_player.Leftkey, _player.RightKey, _player.UpKey, _player.DownKey); }
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
        if (temp.X < _minPosition)
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
        Zoom = new Vector2((float)(Math.Pow(_player.Board.TileSize, -1) * Math.Pow(_global.MazeGenerator.Size, -1) * _global.Viewport), (float)(Math.Pow(_player.Board.TileSize, -1) * Math.Pow(_global.MazeGenerator.Size, -1) * _global.Viewport));
        Position = new Vector2(_global.MazeGenerator.Size * _player.Board.TileSize * 0.5f, _global.MazeGenerator.Size * _player.Board.TileSize * 0.5f);
    }
}