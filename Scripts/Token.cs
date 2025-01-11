using System;
using Godot;
using MazeRunner.Scripts.Data;
using MazeRunner.Scripts.Logic;

namespace MazeRunner.Scripts;

public partial class Token : CharacterBody2D
{
    [Export] float _defaultSpeed = 20;

    private Global _global;
    private PlayerCamera _playerCamera;
    private Board _board;
    private MazeGenerator _mazeGenerator;
    private SpikeTrappedTimer _spikeTrappedTimer;
    private Timer _timer;

    public enum State
    {
        Spawning,
        Idle,
        Moving,
        Winning
    }

    public State CurrentState { get; private set; } = State.Spawning;

    public enum Condition
    {
        None,
        SpikeTrapped
    }

    public Condition CurrentCondition { get; private set; }

    public string CurrentFloor { get; private set; }

    private Vector2 _input;
    private Vector2I _tokenCoord;
    private float _speed;
    private float _minPosition;
    private float _maxPosition;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _playerCamera = GetNode<PlayerCamera>("/root/Game/MainGame/PlayerCamera");
        _global = GetNode<Global>("/root/Global");
        _mazeGenerator = _global.Setting.MazeGenerator;
        _board = GetNode<Board>("/root/Game/MainGame/Board");
        _spikeTrappedTimer = GetNode<SpikeTrappedTimer>("/root/Game/MainGame/Token/SpikeTrappedTimer");
        _timer = GetNode<SpikeTrappedTimer>("/root/Game/MainGame/Token/SpikeTrappedTimer");

        _speed = _defaultSpeed * _board.TileSize;
        CurrentState = State.Spawning;
        CurrentCondition = Condition.None;
        _input = Vector2.Zero;
        _minPosition = GetConvertedPos(0) - _board.TileSize * (float)Math.Pow(4, -1);
        _maxPosition = GetConvertedPos(_mazeGenerator.Size - 1) + _board.TileSize * (float)Math.Pow(4, -1);
    }


    public override void _Input(InputEvent @event)
    {
        if (_playerCamera.CurrentState == PlayerCamera.State.Player)
        {
            _input = Input.GetVector("UILeft", "UIRight", "UIUp", "UIDown");
            if (_input == Vector2.Zero && CurrentState != State.Spawning)
            {
                CurrentState = State.Moving;
            }
        }
        else
        {
            _input = Vector2.Zero;
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        _tokenCoord = _board.LocalToMap(Position);

        if (_tokenCoord.X == _mazeGenerator.ExitCoord.x && _tokenCoord.Y == _mazeGenerator.ExitCoord.y)
        {
            CurrentState = State.Winning;
        }

        switch (CurrentState)
        {
            case State.Spawning:
                Spawn();
                break;
            case State.Idle:
                Idle();
                break;
            case State.Moving:
                Move();
                break;
            case State.Winning:
                Win();
                break;
        }

        if (_mazeGenerator.Maze[_tokenCoord.X, _tokenCoord.Y] is Spikes spikes)
        {
            if (spikes.IsActivated)
            {
                spikes.Deactivate();
                if (CurrentCondition != Condition.SpikeTrapped)
                {
                    CurrentCondition = Condition.SpikeTrapped;
                    _timer.Start();
                }
            }
        }

        if (CurrentCondition == Condition.SpikeTrapped && _timer.IsStopped())
        {
            CurrentCondition = Condition.None;
        }

        switch (CurrentCondition)
        {
            case Condition.None:
                ResetStats();
                break;
            case Condition.SpikeTrapped:
                GetHurtBySpikeTrap();
                break;
        }

        if (_mazeGenerator.Maze[_tokenCoord.X, _tokenCoord.Y] is Spawner) CurrentFloor = "Spawner";
        else if (_mazeGenerator.Maze[_tokenCoord.X, _tokenCoord.Y] is Exit) CurrentFloor = "Exit";
        else if (_mazeGenerator.Maze[_tokenCoord.X, _tokenCoord.Y] is Spikes) CurrentFloor = "Spikes";
        else if (_mazeGenerator.Maze[_tokenCoord.X, _tokenCoord.Y] is Wall) CurrentFloor = "Wall";
        else if (_mazeGenerator.Maze[_tokenCoord.X, _tokenCoord.Y] is Empty) CurrentFloor = "Empty";
    }

    private void OnSpikeTrappedTimerTimeout()
    {
        CurrentCondition = Condition.None;
    }

    private void ResetStats()
    {
        _speed = _defaultSpeed * _board.TileSize;
    }

    private void GetHurtBySpikeTrap()
    {
        _speed = _board.TileSize;
    }

    private float GetConvertedPos(int i)
    {
        return (i + 0.5f) * _board.TileSize;
    }

    private void Idle()
    {
    }

    private void Move()
    {
        Velocity = _input * _speed;
        MoveAndSlide();

        Position = new Vector2(Math.Clamp(Position.X, _minPosition,
                _maxPosition),
            Math.Clamp(Position.Y, _minPosition,
                _maxPosition));
    }

    private void Spawn()
    {
        Position = new Vector2(GetConvertedPos(_mazeGenerator.SpawnerCoord.x),
            GetConvertedPos(_mazeGenerator.SpawnerCoord.y));
        CurrentState = State.Idle;
    }

    private void Win()
    {
        GetTree().Quit();
    }
}