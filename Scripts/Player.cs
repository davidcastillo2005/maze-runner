using System;
using System.Collections.Generic;
using Godot;
using MazeRunner.Scripts.Data;

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
    [Export] public Player _opponent;
    [Export] private PlayerCamera _playerCamera;
    [Export] private Sprite2D _blindnessSprite;

    public System.Timers.Timer CoolDownTimer = new(1000);
    public enum State { Spawning, Idle, Moving, Winning }
    public State CurrentState { get; set; } = State.Spawning;
    public enum Condition { None, Spikes, Portal, Sticky }
    public Condition CurrentCondition { get; private set; }
    public int Energy = 0;
    public int BatteryLife = 0;
    public int SkillNum { get; set; } = new int();
    public string StrName;

    private Random _random = new();
    private Condition? _previusCondition;
    private string PlayerName { get; set; } = string.Empty;
    private bool IsShieldOn { get; set; } = false;
    private Shield _shield = new();
    private bool IsPortalOn { get; set; } = false;
    private Shield _portal = new();
    private bool IsBlind = false;
    private Blind _blind = new();
    private bool IsMuted = false;
    private Muter _muter = new();
    private bool IsParalized = false;
    private Glare _glare = new();
    private Global _global;
    private Vector2 _input;
    private Vector2I _PlayerCoord;
    private float _minPosition;
    private float _maxPosition;
    private float _currentSpeed;
    private float _defaultSpeed;
    private float _paralizedSpeed;
    private int _directionalKeysPressedCount;

    public override void _Ready()
    {
        _global = GetNode<Global>("/root/Global");
        _input = Vector2.Zero;
        _minPosition = Board.GetConvertedPos(0) - Board.TileSize * (float)Math.Pow(4, -1);
        _maxPosition = Board.GetConvertedPos(_global.MazeGenerator.Size - 1) + Board.TileSize * (float)Math.Pow(4, -1);
        _defaultSpeed = _speed * Board.TileSize;
        _paralizedSpeed = _defaultSpeed * 0.1f;
        _currentSpeed = _defaultSpeed;

        CurrentState = State.Spawning;
        CurrentCondition = Condition.None;

        _muter.Timer.Elapsed += OnMutedEvent;
        _muter.Timer.AutoReset = false;

        _blind.Timer.Elapsed += OnBlindEvent;
        _blind.Timer.AutoReset = false;

        Spikes.Timer.Elapsed += OnSpikedEvent;
        Spikes.Timer.AutoReset = false;

        _glare.Timer.Elapsed += OnPredatorEvent;
        _glare.Timer.AutoReset = false;

        CoolDownTimer.Elapsed += OnCooldownEvent;
        CoolDownTimer.AutoReset = false;
        CoolDownTimer.Enabled = true;

        switch (_currentPlayerNum)
        {
            case 1:
                if (_global.PlayerOneName == string.Empty) StrName = "Player One";
                else StrName = _global.PlayerOneName;
                SkillNum = _global.PlayerOneSkill;
                break;
            case 2:
                if (_global.PlayerOneName == string.Empty)
                    StrName = "Player Two";
                else StrName = _global.PlayerTwoName; 
                SkillNum = _global.PlayerTwoSkill;
                break;
            default:
                throw new Exception();
        }
        _nameLabel.Text = StrName;

        BatteryLife = SkillNum switch
        {
            1 => Shield.BatteryLife,
            2 => PortalGun.BatteryLife,
            3 => Blind.BatteryLife,
            4 => Muter.BatteryLife,
            5 => Glare.BatteryLife,
            _ => 0,
        };
        GD.Print(SkillNum);
        Position = new Vector2(Board.GetConvertedPos(_global.MazeGenerator.SpawnerCoord.x), Board.GetConvertedPos(_global.MazeGenerator.SpawnerCoord.y));
    }
    public override void _Input(InputEvent @event)
    {
        if (_playerCamera.CurrentState != PlayerCamera.State.Free)
        {
            _input = Input.GetVector(Leftkey, RightKey, UpKey, DownKey);

            if (CurrentState == State.Spawning) { }
            else if (_input == Vector2.Zero) CurrentState = State.Idle;
            else CurrentState = State.Moving;

            if (!_muter.Timer.Enabled && SkillNum != 0)
            {
                if (SkillNum == 1
                    && Input.IsActionPressed(SkillKey)
                    && Energy == BatteryLife) IsShieldOn = true;
                else IsShieldOn = false;

                if (SkillNum == 2
                    && _input != Vector2.Zero
                    && Input.IsActionJustPressed(SkillKey)
                    && Energy == BatteryLife)
                {
                    IsPortalOn = true;
                    Energy = 0;
                }
                else IsPortalOn = false;

                if (SkillNum == 3
                    && Input.IsActionJustPressed(SkillKey)
                    && !_opponent._blind.Timer.Enabled
                    && !_opponent.IsBlind
                    && Energy == BatteryLife)
                {
                    _opponent.IsBlind = true;
                    GD.Print("IsBlind");
                }

                if (SkillNum == 4 && Input.IsActionJustPressed(SkillKey) && Energy == BatteryLife)
                {
                    _opponent.IsMuted = true;
                    Energy = 0;
                }

                if (SkillNum == 5 && Input.IsActionJustPressed(SkillKey) && IsInsideRadius(Position, _opponent.Position, _glare.Radius * Board.TileSize) && Energy == BatteryLife)
                {
                    _opponent.IsParalized = true;
                    Energy = 0;
                }
            }
        }
        else _input = Vector2.Zero;
    }
    public override void _Process(double delta)
    {
        _PlayerCoord = Board.LocalToMap(Position);

        if (_PlayerCoord.X == _global.MazeGenerator.ExitCoord.x && _PlayerCoord.Y == _global.MazeGenerator.ExitCoord.y) CurrentState = State.Winning;

        if (!CoolDownTimer.Enabled && Energy < BatteryLife)
        {
            CoolDownTimer.Enabled = true;
        }
        if (Energy == BatteryLife)
        {
            CoolDownTimer.Enabled = false;
        }

        if (IsParalized)
        {
            _input = Vector2.Zero;
            _glare.Timer.Enabled = true;
        }

        if (IsMuted) _muter.Timer.Enabled = true;

        if (IsBlind)
        {
            _blindnessSprite.Scale = new Vector2(5.625f, 5.625f);
            _blind.Timer.Enabled = true;
        }
        else { _blindnessSprite.Scale = new Vector2(0, 0); }

        if (IsPortalOn)
        {
            Vector2I nTokenCoord = (Vector2I)(_input * 2 + _PlayerCoord);
            if (nTokenCoord.X >= 0
                && nTokenCoord.Y >= 0
                && nTokenCoord.X < _global.MazeGenerator.Size
                && nTokenCoord.Y < _global.MazeGenerator.Size
                && _global.MazeGenerator.Maze[nTokenCoord.X, nTokenCoord.Y] is not Wall)
            {
                Position = new Vector2(Board.GetConvertedPos(nTokenCoord.X), Board.GetConvertedPos(nTokenCoord.Y));
                IsPortalOn = false;
            }
        }

        if (_global.MazeGenerator.Maze[_PlayerCoord.X, _PlayerCoord.Y] is Spikes spikes && spikes.IsActive)
        {
            spikes.Deactivate();
            if (IsShieldOn)
            {
                Energy = 0;
                goto EscapeTrap;
            }
            else if (CurrentCondition != Condition.Spikes)
            {
                CurrentCondition = Condition.Spikes;
                Spikes.Timer.Enabled = true;
            }
        }

        if (_global.MazeGenerator.Maze[_PlayerCoord.X, _PlayerCoord.Y] is Portal portal && portal.IsActive)
        {
            portal.Deactivate();
            if (IsShieldOn)
            {
                Energy = 0;
                goto EscapeTrap;
            }
            else if (CurrentCondition != Condition.Portal)
            {
                _previusCondition = CurrentCondition;
                CurrentCondition = Condition.Portal;
            }
        }

        if (_global.MazeGenerator.Maze[_PlayerCoord.X, _PlayerCoord.Y] is Shock sticky && sticky.IsActive)
        {
            sticky.Deactivate();
            if (IsShieldOn)
            {
                Energy = 0;
                goto EscapeTrap;
            }
            else if (CurrentCondition != Condition.Sticky) CurrentCondition = Condition.Sticky;
        }
    EscapeTrap:

        if (CurrentCondition == Condition.Spikes && !Spikes.Timer.Enabled) CurrentCondition = Condition.None;

        if (CurrentCondition == Condition.Sticky && _directionalKeysPressedCount == Shock.Struggle)
        {
            _directionalKeysPressedCount = 0;
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
                break;
            default:
                throw new Exception();
        }
    }

    private bool IsInsideRadius(Vector2 pos, Vector2 targetPos, int radius)
    {
        int distance = (int)Math.Sqrt(Math.Pow(pos.X - targetPos.X, 2) + Math.Pow(pos.Y - targetPos.Y, 2));
        return distance <= radius;
    }
    private void OnPredatorEvent(object source, System.Timers.ElapsedEventArgs e) { IsParalized = false; }
    private void OnCooldownEvent(object source, System.Timers.ElapsedEventArgs e) { Energy++; }
    private void OnMutedEvent(object source, System.Timers.ElapsedEventArgs e) { IsMuted = false; }
    private void OnBlindEvent(object source, System.Timers.ElapsedEventArgs e)
    {
        IsBlind = false;
        _opponent.Energy = 0;
    }
    private void OnSpikedEvent(object source, System.Timers.ElapsedEventArgs e) { CurrentCondition = Condition.None; }
    private void ResetStats() { _currentSpeed = _defaultSpeed; }
    private void GetStuckBySticky()
    {
        if (_input != Vector2.Zero) _input = Vector2.Zero;
        if (Input.IsActionJustPressed(Leftkey)
            || Input.IsActionJustPressed(RightKey)
            || Input.IsActionJustPressed(UpKey)
            || Input.IsActionJustPressed(DownKey)) _directionalKeysPressedCount++;
    }
    private void HurtBySpikes() { _currentSpeed = _paralizedSpeed; }
    private void MoveThroughPortal()
    {
        List<Vector2I> possibleTargetPosition = new();
        foreach (var (x, y) in _global.MazeGenerator.Directions)
        {
            Vector2I nTokenCoord = new Vector2I(x + _PlayerCoord.X, y + _PlayerCoord.Y);
            if (!_global.MazeGenerator.IsInsideBounds(nTokenCoord.X, nTokenCoord.Y)
                || _global.MazeGenerator.Maze[nTokenCoord.X, nTokenCoord.Y] is Portal
                || _global.MazeGenerator.Maze[nTokenCoord.X, nTokenCoord.Y] is not Empty and not Spikes) continue;

            Vector2I inBetweenCoord = new Vector2I((int)((_PlayerCoord.X + nTokenCoord.X) * 0.5f), (int)((_PlayerCoord.Y + nTokenCoord.Y) * 0.5f));
            if (_global.MazeGenerator.Maze[inBetweenCoord.X, inBetweenCoord.Y] is not Wall) continue;
            possibleTargetPosition.Add(nTokenCoord);
        }

        if (possibleTargetPosition.Count > 0)
        {
            int index = _random.Next(possibleTargetPosition.Count);
            Position = new Vector2(Board.GetConvertedPos(possibleTargetPosition[index].X), Board.GetConvertedPos(possibleTargetPosition[index].Y));
        }
        CurrentCondition = (Condition)_previusCondition;
        _previusCondition = null;
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
        Position = new Vector2(Board.GetConvertedPos(_global.MazeGenerator.SpawnerCoord.x), Board.GetConvertedPos(_global.MazeGenerator.SpawnerCoord.y));
        CurrentState = State.Idle;
    }
}