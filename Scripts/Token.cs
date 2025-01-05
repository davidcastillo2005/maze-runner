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
    public State CurrentState { get; private set; }

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

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _playerCamera = GetNode<PlayerCamera>("/root/Game/MainGame/PlayerCamera");
        _global = GetNode<Global>("/root/Global");
        _mazeGenerator = _global.Setting.MazeGenerator;
        _board = GetNode<Board>("/root/Game/MainGame/Board");
        _spikeTrappedTimer = GetNode<SpikeTrappedTimer>("/root/Game/MainGame/Token/SpikeTrappedTimer");
        _timer = GetNode<SpikeTrappedTimer>("/root/Game/MainGame/Token/SpikeTrappedTimer");

        CurrentState = State.Spawning;
        CurrentCondition = Condition.None;
        _defaultSpeed *= _board.TileSize;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("UIShiftCamera"))
        {
            CurrentState = State.Idle;
        }

        _input = Input.GetVector("UILeft", "UIRight", "UIUp", "UIDown");
        CurrentState = _input == Vector2.Zero && _playerCamera.CurrentState != PlayerCamera.State.Free
            ? State.Idle
            : State.Moving;

        _tokenCoord = _board.LocalToMap(Position);

        for (int i = 0; i < _mazeGenerator.SpikesTrapsCoords.Count; i++)
        {
            if (_mazeGenerator.Maze[_tokenCoord.X, _tokenCoord.Y] is Spikes)
            {
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

    private void ResetStats()
    {
        _speed = 20 * _board.TileSize;
    }

    private void GetHurtBySpikeTrap()
    {
        _speed = 1 * _board.TileSize;
    }

    private void OnSpikeTrappedTimerTimeout()
    {
        CurrentCondition = Condition.None;
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
    }

    private void Spawn()
    {
    }

    private void Win()
    {
        GetTree().Quit();
    }
}