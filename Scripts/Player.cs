using System;
using System.Collections.Generic;
using Godot;
using MazeRunner.Scripts.Data;
using MazeRunner.Scripts.Logic;

namespace MazeRunner.Scripts;

public partial class Player : CharacterBody2D
{
    [Export] public string Leftkey;
    [Export] public string RightKey;
    [Export] public string UpKey;
    [Export] public string DownKey;
    [Export] public string ShiftCameraKey;
    [Export] public string SkillKey;
    [Export] public Board Board;

    [Export] private float _speed = 20;
    [Export] private Label _nameLabel;
    [Export] private int _currentPlayerNum;
    [Export] private Player _oppositePlayer;
    [Export] private PlayerCamera _playerCamera;
    [Export] private Sprite2D _blindnessSprite;

    public string PlayerName { get; set; } = string.Empty;

    public bool[] PlayerSkillsBools { get; set; }
    public bool IsSkillReady { get; private set; } = false;
    public bool IsShieldOn { get; private set; } = false;
    public Shield Shield { get; private set; } = new Shield();
    public bool IsPortalGunOn { get; private set; } = false;
    public PortalGun PortalGun { get; private set; } = new PortalGun();
    public bool IsBlindOn = false;
    public Blindness Blindness { get; private set; } = new Blindness();
    public bool IsMutedOn = false;
    public Muter Muter { get; private set; } = new Muter();

    public enum State { Spawning, Idle, Moving, Winning }
    public State CurrentState { get; set; } = State.Spawning;

    public enum Condition { None, Spikes, Portal, Sticky }
    private Condition? _previusCondition;
    public Condition CurrentCondition { get; private set; }

    public string CurrentFloor { get; private set; }

    private Global _global;
    private MazeGenerator _mazeGenerator;

    private Vector2 _input;
    private Vector2I _tokenCoord;
    private float _minPosition;
    private float _maxPosition;

    private float _currentSpeed;
    private float _defaultSpeed;
    private float _paralizedSpeed;

    private System.Timers.Timer _cooldownTimer = new(10000);
    private System.Timers.Timer _mutedTimer = new(10000);
    private System.Timers.Timer _blindnessTimer = new(10000);
    private System.Timers.Timer _spikesTimer = new(10000);

    private int _directionalKeysPressCount;

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
        _paralizedSpeed = _defaultSpeed * 0.1f;
        _currentSpeed = _defaultSpeed;

        _mutedTimer.Elapsed += OnMutedEvent;
        _mutedTimer.AutoReset = false;
        
        _blindnessTimer.Elapsed += OnBlindEvent;
        _blindnessTimer.AutoReset = false;

        _cooldownTimer.AutoReset = false;
        _cooldownTimer.Enabled = true;

        _spikesTimer.Elapsed += OnSpikedEvent;
        _spikesTimer.AutoReset = false;

        switch (_currentPlayerNum)
        {
            case 1:
                PlayerName = _global.PlayerOneName;
                PlayerOneSetSkills();
                break;
            case 2:
                PlayerName = _global.PlayerTwoName;
                PlayerTwoSetSkills();
                break;
            default:
                throw new Exception();
        }

        _nameLabel.Text = PlayerName;

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
        if (_playerCamera.CurrentState != PlayerCamera.State.Free)
        {
            _input = Input.GetVector(Leftkey, RightKey, UpKey, DownKey);
            if (CurrentState != State.Spawning)
            {
                if (_input != Vector2.Zero) CurrentState = State.Moving;
                else CurrentState = State.Idle;
            }

            if (!_mutedTimer.Enabled && !_cooldownTimer.Enabled)
            {
                if (PlayerSkillsBools[0] && Input.IsActionPressed(SkillKey) && Shield.Battery > 0)
                {
                    IsShieldOn = true;
                }
                else
                {
                    IsShieldOn = false;
                }

                if (PlayerSkillsBools[1] && _input != Vector2.Zero
                    && Input.IsActionJustPressed(SkillKey) && PortalGun.Battery > 0)
                {
                    IsPortalGunOn = true;
                    _cooldownTimer.Enabled = true;
                }
                else
                {
                    IsPortalGunOn = false;
                }

                if (PlayerSkillsBools[2] && Input.IsActionJustPressed(SkillKey)
                    && !_blindnessTimer.Enabled && !IsBlindOn && Blindness.Battery > 0)
                {
                    IsBlindOn = true;
                    Blindness.Battery--;
                    _blindnessTimer.Enabled = true;
                }

                if (PlayerSkillsBools[3] && Input.IsActionJustPressed(SkillKey))
                {
                    _oppositePlayer.IsMutedOn = true;
                    _cooldownTimer.Enabled = true;
                }
            }
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

        if (IsMutedOn)
        {
            _mutedTimer.Enabled = true;
        }

        if (IsBlindOn)
        {
            _blindnessSprite.Scale = new Vector2(5.625f, 5.625f);
        }
        else
        {
            _blindnessSprite.Scale = new Vector2(0, 0);
        }

        if (_cooldownTimer.Enabled)
        {
            IsSkillReady = false;
        }
        else
        {
            IsSkillReady = true;
        }

        if (IsPortalGunOn)
        {
            Vector2I nTokenCoord = (Vector2I)(_input * 2 + _tokenCoord);
            if (nTokenCoord.X >= 0 && nTokenCoord.Y >= 0 && nTokenCoord.X < _mazeGenerator.Size && nTokenCoord.Y < _mazeGenerator.Size && _mazeGenerator.Maze[nTokenCoord.X, nTokenCoord.Y] is not Wall)
            {
                Position = new Vector2(GetConvertedPos(nTokenCoord.X), GetConvertedPos(nTokenCoord.Y));
                PortalGun.Battery--;
                IsPortalGunOn = false;
            }
        }

        if (_mazeGenerator.Maze[_tokenCoord.X, _tokenCoord.Y] is Spikes spikes && spikes.IsActivated)
        {
            spikes.Deactivate();
            if (PlayerSkillsBools[0] && Shield.Battery > 0 && IsShieldOn)
            {
                Shield.Battery--;
                goto EscapeTrap;
            }
            else
            {
                if (CurrentCondition != Condition.Spikes)
                {
                    CurrentCondition = Condition.Spikes;
                    _spikesTimer.Enabled = true;
                    GD.Print("YIKES!");
                }
            }
        }

        if (_mazeGenerator.Maze[_tokenCoord.X, _tokenCoord.Y] is Portal portal && portal.IsActivated)
        {
            portal.Deactivate();
            if (PlayerSkillsBools[0] && Shield.Battery > 0 && IsShieldOn)
            {
                Shield.Battery--;
                goto EscapeTrap;
            }
            else if (CurrentCondition != Condition.Portal)
            {
                _previusCondition = CurrentCondition;
                CurrentCondition = Condition.Portal;
            }
        }

        if (_mazeGenerator.Maze[_tokenCoord.X, _tokenCoord.Y] is Sticky sticky && sticky.IsActivated)
        {
            sticky.Deactivate();
            if (PlayerSkillsBools[0] && Shield.Battery > 0 && IsShieldOn)
            {
                Shield.Battery--;
                goto EscapeTrap;
            }
            else
            {
                if (CurrentCondition != Condition.Sticky)
                {
                    CurrentCondition = Condition.Sticky;
                }
            }
        }
    EscapeTrap:

        if (Shield.Battery < 0) Shield.Battery = 0;
        if (PortalGun.Battery < 0) PortalGun.Battery = 0;
        if (Blindness.Battery < 0) Blindness.Battery = 0;

        if (CurrentCondition == Condition.Spikes && !_spikesTimer.Enabled) CurrentCondition = Condition.None;

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

    private void OnMutedEvent(object source, System.Timers.ElapsedEventArgs e)
    {
        IsMutedOn = false;
    }

    private void OnBlindEvent(object source, System.Timers.ElapsedEventArgs e)
    {
        IsBlindOn = false;
        _cooldownTimer.Enabled = true;
    }

    private void OnSpikedEvent(object source, System.Timers.ElapsedEventArgs e)
    {
        CurrentCondition = Condition.None;
        GD.Print("OFF");
    }

    private void ResetStats()
    {
        _currentSpeed = _defaultSpeed;
    }

    private void GetStuckBySticky()
    {
        if (_input != Vector2.Zero) _input = Vector2.Zero;
        if (Input.IsActionJustPressed(Leftkey) || Input.IsActionJustPressed(RightKey) || Input.IsActionJustPressed(UpKey) || Input.IsActionJustPressed(DownKey)) _directionalKeysPressCount++;
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

    private void Idle() { }

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