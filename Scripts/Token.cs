using System;
using System.Collections.Generic;
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
    private SpikesTimer _spikesTimer;
    private Timer _timer;

    public bool[] TokenSkillsBools { get; set; } = new bool[1];
    public Shield Shield { get; private set; } = new Shield();

    public enum State
    {
        Spawning,
        Idle,
        Moving,
        Winning
    }
    public State CurrentState { get; set; } = State.Spawning;

    public enum Condition
    {
        None,
        Spikes,
        Trampoline,
        Sticky
    }
    private Condition? _previusCondition;
    public Condition CurrentCondition { get; private set; }

    public string CurrentFloor { get; private set; }

    private Vector2 _input;
    private Vector2I _tokenCoord;
    private float _speed;
    private float _minPosition;
    private float _maxPosition;
    public int _directionalKeysPressCount;
    public override void _Ready() // Called when the node enters the scene tree for the first time.
    {
        _playerCamera = GetNode<PlayerCamera>("/root/Game/MainGame/PlayerCamera");
        _global = GetNode<Global>("/root/Global");
        _mazeGenerator = _global.Setting.MazeGenerator;
        _board = GetNode<Board>("/root/Game/MainGame/Board");
        _spikesTimer = GetNode<SpikesTimer>("/root/Game/MainGame/Token/SpikesTimer");
        _timer = GetNode<SpikesTimer>("/root/Game/MainGame/Token/SpikesTimer");

        _speed = _defaultSpeed * _board.TileSize;
        CurrentState = State.Spawning;
        CurrentCondition = Condition.None;
        _input = Vector2.Zero;
        _minPosition = GetConvertedPos(0) - _board.TileSize * (float)Math.Pow(4, -1);
        _maxPosition = GetConvertedPos(_mazeGenerator.Size - 1) + _board.TileSize * (float)Math.Pow(4, -1);

        for (int i = 0; i < _global.Setting.SkillBools.Length; i++)
        {
            if (_global.Setting.SkillBools[i])
            {
                TokenSkillsBools[i] = true;
            }
        }
    }


    public override void _Input(InputEvent @event)
    {
        if (_playerCamera.CurrentState != PlayerCamera.State.Free)
        {
            _input = Input.GetVector("UILeft", "UIRight", "UIUp", "UIDown");
            if (CurrentState != State.Spawning)
            {
                if (_input != Vector2.Zero) CurrentState = State.Moving;
                else CurrentState = State.Idle;
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

        if (_tokenCoord.X == _mazeGenerator.ExitCoord.x && _tokenCoord.Y == _mazeGenerator.ExitCoord.y) CurrentState = State.Winning;

        if (_mazeGenerator.Maze[_tokenCoord.X, _tokenCoord.Y] is Spikes spikes && spikes.IsActivated)
        {
            spikes.Deactivate();
            if (TokenSkillsBools[0] && Shield.Health > 0)
            {
                Shield.Health--;
                goto EscapeTrap;
            }
            else
            {
                if (CurrentCondition != Condition.Spikes)
                {
                    CurrentCondition = Condition.Spikes;
                    _timer.Start();
                }

            }
            if (Shield.Health < 0)
            {
                Shield.Health = 0;
            }
        }

        if (_mazeGenerator.Maze[_tokenCoord.X, _tokenCoord.Y] is Portal trampoline && trampoline.IsActivated)
        {
            trampoline.Deactivate();
            if (TokenSkillsBools[0] && Shield.Health > 0)
            {
                Shield.Health--;
                goto EscapeTrap;
            }
            else
            {
                if (CurrentCondition != Condition.Trampoline)
                {
                    _previusCondition = CurrentCondition;
                    CurrentCondition = Condition.Trampoline;
                }
            }

            if (Shield.Health < 0)
            {
                Shield.Health = 0;
            }

        }

        if (_mazeGenerator.Maze[_tokenCoord.X, _tokenCoord.Y] is Sticky sticky && sticky.IsActivated)
        {
            sticky.Deactivate();
            if (TokenSkillsBools[0] && Shield.Health > 0)
            {
                Shield.Health--;
                goto EscapeTrap;
            }
            else
            {
                if (CurrentCondition != Condition.Sticky)
                {
                    CurrentCondition = Condition.Sticky;
                    _timer.Start();
                }
            }

            if (Shield.Health < 0)
            {
                Shield.Health = 0;
            }
        }

    EscapeTrap:



        if (CurrentCondition == Condition.Spikes && _timer.IsStopped()) CurrentCondition = Condition.None;

        if (CurrentCondition == Condition.Sticky && _directionalKeysPressCount == 10)
        {
            _directionalKeysPressCount = 0;
            CurrentCondition = Condition.None;
        }

        switch (CurrentCondition)
        {
            case Condition.None:
                ResetStats();
                break;
            case Condition.Spikes:
                HurtBySpikes();
                break;
            case Condition.Trampoline:
                BounceOnTrampoline();
                break;
            case Condition.Sticky:
                GetStuckBySticky();
                break;
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

        switch (_mazeGenerator.Maze[_tokenCoord.X, _tokenCoord.Y])
        {
            case Spawner:
                CurrentFloor = "Spawner";
                break;
            case Exit:
                CurrentFloor = "Exit";
                break;
            case Spikes:
                CurrentFloor = "Spikes";
                break;
            case Portal:
                CurrentFloor = "Trampoline";
                break;
            case Sticky:
                CurrentFloor = "Sticky";
                break;
            case Wall:
                CurrentFloor = "Wall";
                break;
            case Empty:
                CurrentFloor = "Empty";
                break;
            default:
                throw new Exception();

        }
    }

    void OnSpikeTimerTimeout()
    {
        CurrentCondition = Condition.None;
    }

    private void ResetStats()
    {
        _speed = _defaultSpeed * _board.TileSize;
    }

    private void GetStuckBySticky()
    {
        if (_input != Vector2.Zero) _input = Vector2.Zero;
        if (Input.IsActionJustPressed("UILeft") || Input.IsActionJustPressed("UIRight") || Input.IsActionJustPressed("UIUp") || Input.IsActionJustPressed("UIDown"))
        {
            _directionalKeysPressCount++;
        }
    }

    private void HurtBySpikes()
    {
        _speed = _board.TileSize;
    }

    private void BounceOnTrampoline()
    {
        List<Vector2I> possibleNPosition = new();
        foreach (var (x, y) in _mazeGenerator.Directions)
        {
            Vector2I nTokenCoord = new Vector2I(x + _tokenCoord.X, y + _tokenCoord.Y);
            Vector2I inBetweenCoord;
            if (!_mazeGenerator.IsInsideBounds(nTokenCoord.X, nTokenCoord.Y) || _mazeGenerator.Maze[nTokenCoord.X, nTokenCoord.Y] is Portal || _mazeGenerator.Maze[nTokenCoord.X, nTokenCoord.Y] is not Empty and not Spikes) continue;
            inBetweenCoord = new Vector2I((int)((_tokenCoord.X + nTokenCoord.X) * 0.5f), (int)((_tokenCoord.Y + nTokenCoord.Y) * 0.5f));
            if (_mazeGenerator.Maze[inBetweenCoord.X, inBetweenCoord.Y] is not Wall) continue;
            possibleNPosition.Add(nTokenCoord);
        }

        if (possibleNPosition.Count > 0)
        {
            int index = _mazeGenerator.Random.Next(possibleNPosition.Count);
            Position = new Vector2(GetConvertedPos(possibleNPosition[index].X), GetConvertedPos(possibleNPosition[index].Y));
        }
        CurrentCondition = (Condition)_previusCondition;
        _previusCondition = null;
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

        Position = new Vector2(Math.Clamp(Position.X, _minPosition, _maxPosition), Math.Clamp(Position.Y, _minPosition, _maxPosition));
    }

    private void Spawn()
    {
        Position = new Vector2(GetConvertedPos(_mazeGenerator.SpawnerCoord.x), GetConvertedPos(_mazeGenerator.SpawnerCoord.y));
        CurrentState = State.Idle;
    }

    private void Win()
    {
        GetTree().Quit();
    }

}