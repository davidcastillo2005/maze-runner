using Godot;
using System;
using MazeRunner.Scripts.Logic;

namespace MazeRunner.Scripts;

public partial class PlayerCamera : Camera2D
{
    private Token _tokenNode;
    private Global _global;
    private Board _board;
    private MazeGenerator _mazeGenerator;

    public enum State
    {
        Player,
        Free,
        Extensive
    }

    public State CurrentState { get; private set; }

    private int _tileSize;
    private Vector2 _cameraOffset;
    private Vector2 _input;

    public override void _Ready()
    {
        _board = GetNode<Board>("/root/Game/MainGame/Board");
        _tileSize = _board.TileSize;
        _tokenNode = GetNode<Token>("/root/Game/MainGame/Token");
        _global = GetNode<Global>("/root/Global");
        _mazeGenerator = _global.Setting.MazeGenerator;

        CurrentState = State.Player;
        _input = Vector2.Zero;
        _cameraOffset = Vector2.Zero;

        Zoom = new Vector2((float)(Math.Pow(_tileSize, -1) * Math.Pow(_mazeGenerator.Size, -1) * 720), (float)(Math.Pow(_tileSize, -1) * Math.Pow(_mazeGenerator.Size, -1) * 720));
        
        if (_global.LevelDifficulty < 5) CurrentState = State.Extensive;
    }

    public override void _Input(InputEvent @event)
    {
        _input = Input.GetVector("UILeft", "UIRight", "UIUp", "UIDown");
    }

    public override void _Process(double delta)
    {
        switch (CurrentState)
        {
            case State.Player:
                if (Input.IsActionJustPressed("UIShiftCamera")) CurrentState = State.Free;
                else OnPlayer();
                break;
            case State.Free:
                if (Input.IsActionJustPressed("UIShiftCamera")) CurrentState = State.Player;
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
        Position = _tokenNode.Position;
    }

    private void OnFree()
    {
        _cameraOffset += _tileSize * _input;
        Position = _tokenNode.Position + _cameraOffset;
    }

    private void OnExtensive()
    {
        Position = new Vector2(_mazeGenerator.Size * _tileSize * 0.5f, _mazeGenerator.Size * _tileSize * 0.5f);
    }
}