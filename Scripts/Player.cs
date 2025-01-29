using System;
using System.Collections.Generic;
using System.Net;
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

    public System.Timers.Timer CoolDownTimer = new(1000);
    public enum State { Spawning, Idle, Moving, Winning }
    public State CurrentState { get; set; } = State.Spawning;
    public enum Condition { None, Spikes, Portal, Sticky }
    public Condition CurrentCondition { get; private set; }
    public string CurrentFloor { get; private set; }

    public int CoolDownCounter = 0;
    private int _batteryLife = 0;
    private Condition? _previusCondition;
    private string PlayerName { get; set; } = string.Empty;
    private int PlayerSkill { get; set; } = new int();
    private bool IsShieldOn { get; set; } = false;
    private Shield _shield = new();
    private bool IsPortalGunOn { get; set; } = false;
    private Shield _portalGun = new();
    private bool IsBlind = false;
    private Blindness _blindness = new();
    private bool IsMuted = false;
    private Muter _muter = new();
    private bool IsParalized = false;
    private Predator _predator = new();
    private Global _global;
    private Vector2 _input;
    private Vector2I _tokenCoord;
    private float _minPosition;
    private float _maxPosition;
    private float _currentSpeed;
    private float _defaultSpeed;
    private float _paralizedSpeed;
    private int _directionalKeysPressCount;

    public override void _Ready()
    {
        _global = GetNode<Global>("/root/Global");
        _input = Vector2.Zero;
        _minPosition = Board.GetConvertedPos(0) - Board.TileSize * (float)Math.Pow(4, -1);
        _maxPosition = Board.GetConvertedPos(_global.Setting.MazeGenerator.Size - 1) + Board.TileSize * (float)Math.Pow(4, -1);
        _defaultSpeed = _speed * Board.TileSize;
        _paralizedSpeed = _defaultSpeed * 0.1f;
        _currentSpeed = _defaultSpeed;

        CurrentState = State.Spawning;
        CurrentCondition = Condition.None;

        _muter.Timer.Elapsed += OnMutedEvent;
        _muter.Timer.AutoReset = false;

        _blindness.Timer.Elapsed += OnBlindEvent;
        _blindness.Timer.AutoReset = false;

        Spikes.Timer.Elapsed += OnSpikedEvent;
        Spikes.Timer.AutoReset = false;

        _predator.Timer.Elapsed += OnPredatorEvent;

        CoolDownTimer.Elapsed += OnCooldownEvent;
        CoolDownTimer.AutoReset = false;
        CoolDownTimer.Enabled = true;

        switch (_currentPlayerNum)
        {
            case 1:
                PlayerName = _global.PlayerOneName;
                PlayerSkill = _global.PlayerOneSkill;
                break;
            case 2:
                PlayerName = _global.PlayerTwoName;
                PlayerSkill = _global.PlayerTwoSkill;
                break;
            default:
                throw new Exception();
        }
        _nameLabel.Text = PlayerName;

        _batteryLife = PlayerSkill switch
        {
            1 => _shield.BatteryLife,
            2 => _portalGun.BatteryLife,
            3 => _blindness.BatteryLife,
            4 => _muter.BatteryLife,
            5 => _predator.BatteryLife,
            _ => 0,
        };


        Position = new Vector2(Board.GetConvertedPos(_global.Setting.MazeGenerator.SpawnerCoord.x), Board.GetConvertedPos(_global.Setting.MazeGenerator.SpawnerCoord.y));
    }
    public override void _Input(InputEvent @event)
    {
        if (_playerCamera.CurrentState != PlayerCamera.State.Free)
        {
            _input = Input.GetVector(Leftkey, RightKey, UpKey, DownKey);

            if (CurrentState == State.Spawning) { }
            else if (_input == Vector2.Zero) CurrentState = State.Idle;
            else CurrentState = State.Moving;

            if (!_muter.Timer.Enabled && PlayerSkill != 0)
            {
                if (PlayerSkill == 1
                    && Input.IsActionPressed(SkillKey)
                    && CoolDownCounter == _batteryLife) IsShieldOn = true;
                else IsShieldOn = false;

                if (PlayerSkill == 2
                    && _input != Vector2.Zero
                    && Input.IsActionJustPressed(SkillKey)
                    && CoolDownCounter == _batteryLife)
                {
                    IsPortalGunOn = true;
                    CoolDownCounter = 0;
                }
                else IsPortalGunOn = false;

                if (PlayerSkill == 3
                    && Input.IsActionJustPressed(SkillKey)
                    && !_oppositePlayer._blindness.Timer.Enabled
                    && !_oppositePlayer.IsBlind
                    && CoolDownCounter == _batteryLife) _oppositePlayer.IsBlind = true;

                if (PlayerSkill == 4 && Input.IsActionJustPressed(SkillKey) && CoolDownCounter == _batteryLife)
                {
                    _oppositePlayer.IsMuted = true;
                    CoolDownCounter = 0;
                }

                if (PlayerSkill == 5 && Input.IsActionJustPressed(SkillKey) && CoolDownCounter == _batteryLife)
                {
                    _oppositePlayer.IsParalized = true;
                    CoolDownCounter = 0;
                }
            }
        }
        else _input = Vector2.Zero;
    }
    public override void _Process(double delta)
    {
        GD.Print(PlayerSkill);
        _tokenCoord = Board.LocalToMap(Position);

        if (!CoolDownTimer.Enabled && CoolDownCounter < _batteryLife)
        {
            CoolDownTimer.Enabled = true;
        }
        if (CoolDownCounter == _batteryLife)
        {
            CoolDownTimer.Enabled = false;
        }

        if (_tokenCoord.X == _global.Setting.MazeGenerator.ExitCoord.x && _tokenCoord.Y == _global.Setting.MazeGenerator.ExitCoord.y) CurrentState = State.Winning;

        if (IsParalized)
        {
            _input = Vector2.Zero;
            _predator.Timer.Enabled = true;
        }

        if (IsMuted) _muter.Timer.Enabled = true;

        if (IsBlind)
        {
            _blindness.Timer.Enabled = true;
            _blindnessSprite.Scale = new Vector2(5.625f, 5.625f);
        }
        else { _blindnessSprite.Scale = new Vector2(0, 0); }

        if (IsPortalGunOn)
        {
            Vector2I nTokenCoord = (Vector2I)(_input * 2 + _tokenCoord);
            if (nTokenCoord.X >= 0
                && nTokenCoord.Y >= 0
                && nTokenCoord.X < _global.Setting.MazeGenerator.Size
                && nTokenCoord.Y < _global.Setting.MazeGenerator.Size
                && _global.Setting.MazeGenerator.Maze[nTokenCoord.X, nTokenCoord.Y] is not Wall)
            {
                Position = new Vector2(Board.GetConvertedPos(nTokenCoord.X), Board.GetConvertedPos(nTokenCoord.Y));
                IsPortalGunOn = false;
            }
        }

        if (_global.Setting.MazeGenerator.Maze[_tokenCoord.X, _tokenCoord.Y] is Spikes spikes && spikes.IsActivated)
        {
            spikes.Deactivate();
            if (IsShieldOn)
            {
                CoolDownCounter = 0;
                goto EscapeTrap;
            }
            else if (CurrentCondition != Condition.Spikes)
            {
                CurrentCondition = Condition.Spikes;
                Spikes.Timer.Enabled = true;
            }
        }

        if (_global.Setting.MazeGenerator.Maze[_tokenCoord.X, _tokenCoord.Y] is Portal portal && portal.IsActivated)
        {
            portal.Deactivate();
            if (IsShieldOn)
            {
                CoolDownCounter = 0;
                goto EscapeTrap;
            }
            else if (CurrentCondition != Condition.Portal)
            {
                _previusCondition = CurrentCondition;
                CurrentCondition = Condition.Portal;
            }
        }

        if (_global.Setting.MazeGenerator.Maze[_tokenCoord.X, _tokenCoord.Y] is Sticky sticky && sticky.IsActivated)
        {
            sticky.Deactivate();
            if (IsShieldOn)
            {
                CoolDownCounter = 0;
                goto EscapeTrap;
            }
            else if (CurrentCondition != Condition.Sticky) CurrentCondition = Condition.Sticky;
        }
    EscapeTrap:

        if (CurrentCondition == Condition.Spikes && !Spikes.Timer.Enabled) CurrentCondition = Condition.None;

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

        CurrentFloor = _global.Setting.MazeGenerator.Maze[_tokenCoord.X, _tokenCoord.Y] switch
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

    private void OnPredatorEvent(object source, System.Timers.ElapsedEventArgs e) { IsParalized = false; }
    private void OnCooldownEvent(object source, System.Timers.ElapsedEventArgs e) { CoolDownCounter++; }
    private void OnMutedEvent(object source, System.Timers.ElapsedEventArgs e) { IsMuted = false; }
    private void OnBlindEvent(object source, System.Timers.ElapsedEventArgs e)
    {
        IsBlind = false;
        CoolDownTimer.Enabled = true;
    }
    private void OnSpikedEvent(object source, System.Timers.ElapsedEventArgs e) { CurrentCondition = Condition.None; }
    private void ResetStats() { _currentSpeed = _defaultSpeed; }
    private void GetStuckBySticky()
    {
        if (_input != Vector2.Zero) _input = Vector2.Zero;
        if (Input.IsActionJustPressed(Leftkey)
            || Input.IsActionJustPressed(RightKey)
            || Input.IsActionJustPressed(UpKey)
            || Input.IsActionJustPressed(DownKey)) _directionalKeysPressCount++;
    }
    private void HurtBySpikes() { _currentSpeed = _paralizedSpeed; }
    private void MoveThroughPortal()
    {
        List<Vector2I> possibleTargetPosition = new();
        foreach (var (x, y) in _global.Setting.MazeGenerator.Directions)
        {
            Vector2I nTokenCoord = new Vector2I(x + _tokenCoord.X, y + _tokenCoord.Y);
            if (!_global.Setting.MazeGenerator.IsInsideBounds(nTokenCoord.X, nTokenCoord.Y)
                || _global.Setting.MazeGenerator.Maze[nTokenCoord.X, nTokenCoord.Y] is Portal
                || _global.Setting.MazeGenerator.Maze[nTokenCoord.X, nTokenCoord.Y] is not Empty and not Spikes) continue;

            Vector2I inBetweenCoord = new Vector2I((int)((_tokenCoord.X + nTokenCoord.X) * 0.5f), (int)((_tokenCoord.Y + nTokenCoord.Y) * 0.5f));
            if (_global.Setting.MazeGenerator.Maze[inBetweenCoord.X, inBetweenCoord.Y] is not Wall) continue;
            possibleTargetPosition.Add(nTokenCoord);
        }

        if (possibleTargetPosition.Count > 0)
        {
            int index = _global.Setting.MazeGenerator.Random.Next(possibleTargetPosition.Count);
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
        Position = new Vector2(Board.GetConvertedPos(_global.Setting.MazeGenerator.SpawnerCoord.x), Board.GetConvertedPos(_global.Setting.MazeGenerator.SpawnerCoord.y));
        CurrentState = State.Idle;
    }
    private void Win() { GetTree().ChangeSceneToFile("res://Scenes/main_menu.tscn"); }
}