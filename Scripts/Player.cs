using System;
using System.Collections.Generic;
using Godot;
using MazeRunner.Scripts.Data;
using MazeRunner.Scripts.Logic;

namespace MazeRunner.Scripts;

public partial class Player : CharacterBody2D
{
    [Export] float _speed = 20;
    [Export] public string Left;
    [Export] public string Right;
    [Export] public string Up;
    [Export] public string Down;
    [Export] public string ShiftCamera;
    [Export] public string Skill;
    [Export] public PlayerCamera PlayerCamera;
    [Export] public Board Board;
    [Export] public Timer Timer;
    [Export] public int CurrentPlayer;

    private Global _global;
    private MazeGenerator _mazeGenerator;

    public bool[] PlayerSkillsBools { get; set; }

    public bool _isShieldOn = false;
    public Shield Shield { get; private set; } = new Shield();

    public bool _isPortalGunOn = false;
    public PortalGun PortalGun { get; private set; } = new PortalGun();

    public Boost Boost { get; private set; } = new Boost();
    public bool _isBoostOn { get; private set; } = false;
    public bool _isBoolStillOn { get; private set; } = false;

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
        Portal,
        Sticky
    }
    private Condition? _previusCondition;
    public Condition CurrentCondition { get; private set; }

    public string CurrentFloor { get; private set; }

    private Vector2 _input;
    private Vector2I _tokenCoord;
    private float _minPosition;
    private float _maxPosition;
    private int _directionalKeysPressCount;

    private float _currentSpeed;
    private float _defaultSpeed;
    private float _paralizedSpeed;


    public override void _Ready()
    {
        _global = GetNode<Global>("/root/Global");
        _mazeGenerator = _global.Setting.MazeGenerator;
        CurrentState = State.Spawning;
        CurrentCondition = Condition.None;
        _input = Vector2.Zero;
        _minPosition = GetConvertedPos(0) - Board.TileSize * (float)Math.Pow(4, -1);
        _maxPosition = GetConvertedPos(_mazeGenerator.Size - 1) + Board.TileSize * (float)Math.Pow(4, -1);

        _defaultSpeed = _speed * Board.TileSize;
        _paralizedSpeed = _defaultSpeed / 10;
        _currentSpeed = _defaultSpeed;

        switch (CurrentPlayer)
        {
            case 1:
                PlayerOneSetSkills();
                break;
            case 2:
                PlayerTwoSetSkills();
                break;
            default:
                throw new Exception();
        }

        Position = new Vector2(GetConvertedPos(_mazeGenerator.SpawnerCoord.x), GetConvertedPos(_mazeGenerator.SpawnerCoord.y));
    }

    private void PlayerOneSetSkills()
    {
        for (int i = 0; i < _global.Setting.PlayerOneSkillBools.Length; i++)
        {
            PlayerSkillsBools = _global.Setting.PlayerOneSkillBools;
        }
    }

    private void PlayerTwoSetSkills()
    {
        for (int i = 0; i < _global.Setting.PlayerTwoSkillBools.Length; i++)
        {
            PlayerSkillsBools = _global.Setting.PlayerTwoSkillBools;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (PlayerCamera.CurrentState != PlayerCamera.State.Free)
        {
            _input = Input.GetVector(Left, Right, Up, Down);
            if (CurrentState != State.Spawning)
            {
                if (_input != Vector2.Zero) CurrentState = State.Moving;
                else CurrentState = State.Idle;
            }

            if (PlayerSkillsBools[0] && Input.IsActionPressed(Skill) && Shield.Health != 0)
            {
                _isShieldOn = true;
            }
            else
            {
                _isShieldOn = false;
            }

            if (PlayerSkillsBools[1] && _input != Vector2.Zero && Input.IsActionJustPressed(Skill) && PortalGun.Battery > 0)
            {
                _isPortalGunOn = true;
            }
            else
            {
                _isPortalGunOn = false;
            }

            // if (TokenSkillsBools[2] && Input.IsActionPressed(Skill) && Boost.Battery >= 1 && CurrentCondition == Condition.None)
            // {
            //     _isBoostOn = true;
            // }
            // else
            // {
            //     _isBoostOn = false;
            // }
        }
        else
        {
            _input = Vector2.Zero;
        }
    }

    public override void _Process(double delta)
    {
        _tokenCoord = Board.LocalToMap(Position);

        if (_tokenCoord.X == _mazeGenerator.ExitCoord.x && _tokenCoord.Y == _mazeGenerator.ExitCoord.y) CurrentState = State.Winning;

        // if (_isBoostOn && !_isBoolStillOn)
        // {
        //     Boost.Battery--;
        // }
        // if (Boost.Battery < 1) Boost.Battery = 0;
        // if (_isBoostOn)
        // {
        //     _isBoolStillOn = true;
        // }
        // else
        // {
        //     _isBoolStillOn = false;
        // }

        if (_isPortalGunOn)
        {
            Vector2I nTokenCoord = (Vector2I)(_input * 2 + _tokenCoord);
            if (nTokenCoord.X >= 0 && nTokenCoord.Y >= 0 && nTokenCoord.X < _mazeGenerator.Size && nTokenCoord.Y < _mazeGenerator.Size && _mazeGenerator.Maze[nTokenCoord.X, nTokenCoord.Y] is not Wall)
            {
                Position = new Vector2(GetConvertedPos(nTokenCoord.X), GetConvertedPos(nTokenCoord.Y));
                PortalGun.Battery--;
                _isPortalGunOn = false;
            }
        }
        if (PortalGun.Battery < 0) PortalGun.Battery = 0;

        if (_mazeGenerator.Maze[_tokenCoord.X, _tokenCoord.Y] is Spikes spikes && spikes.IsActivated)
        {
            spikes.Deactivate();
            if (PlayerSkillsBools[0] && Shield.Health > 0 && _isShieldOn)
            {
                Shield.Health--;
                goto EscapeTrap;
            }
            else
            {
                if (CurrentCondition != Condition.Spikes)
                {
                    CurrentCondition = Condition.Spikes;
                    Timer.Start();
                }
            }
        }

        if (_mazeGenerator.Maze[_tokenCoord.X, _tokenCoord.Y] is Portal portal && portal.IsActivated)
        {
            portal.Deactivate();
            if (PlayerSkillsBools[0] && Shield.Health > 0 && _isShieldOn)
            {
                Shield.Health--;
                goto EscapeTrap;
            }
            else
            {
                if (CurrentCondition != Condition.Portal)
                {
                    _previusCondition = CurrentCondition;
                    CurrentCondition = Condition.Portal;
                }
            }
        }

        if (_mazeGenerator.Maze[_tokenCoord.X, _tokenCoord.Y] is Sticky sticky && sticky.IsActivated)
        {
            sticky.Deactivate();
            if (PlayerSkillsBools[0] && Shield.Health > 0 && _isShieldOn)
            {
                Shield.Health--;
                goto EscapeTrap;
            }
            else
            {
                if (CurrentCondition != Condition.Sticky)
                {
                    CurrentCondition = Condition.Sticky;
                    Timer.Start();
                }
            }
        }

        if (Shield.Health < 0)
        {
            Shield.Health = 0;
        }

    EscapeTrap:

        if (CurrentCondition == Condition.Spikes && Timer.IsStopped()) CurrentCondition = Condition.None;

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
            case Condition.Portal:
                MoveThroughPortal();
                break;
            case Condition.Sticky:
                GetStuckBySticky();
                break;
            default:
                throw new Exception();

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
            default:
                throw new Exception();
        }

        CurrentFloor = _mazeGenerator.Maze[_tokenCoord.X, _tokenCoord.Y] switch
        {
            Spawner => "Spawner",
            Exit => "Exit",
            Spikes => "Spikes",
            Portal => "Trampoline",
            Sticky => "Sticky",
            Wall => "Wall",
            Empty => "Empty",
            _ => throw new Exception(),
        };

    }

    void OnSpikeTimerTimeout()
    {
        CurrentCondition = Condition.None;
    }

    private void ResetStats()
    {
        _currentSpeed = _defaultSpeed;
    }

    private void GetStuckBySticky()
    {
        if (_input != Vector2.Zero) _input = Vector2.Zero;
        if (Input.IsActionJustPressed(Left) || Input.IsActionJustPressed(Right) || Input.IsActionJustPressed(Up) || Input.IsActionJustPressed(Down)) _directionalKeysPressCount++;
    }

    private void HurtBySpikes()
    {
        _currentSpeed = _paralizedSpeed;
    }

    private void MoveThroughPortal()
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

    public float GetConvertedPos(int i)
    {
        return (i + 0.5f) * Board.TileSize;
    }

    private void Idle()
    {
    }

    private void Move()
    {
        Velocity = _input * _currentSpeed;
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
        GetTree().ChangeSceneToFile("res://Scenes/main_menu.tscn");
    }
}