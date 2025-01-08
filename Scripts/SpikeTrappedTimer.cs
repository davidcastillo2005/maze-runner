using Godot;
using System;

namespace MazeRunner.Scripts;

public partial class SpikeTrappedTimer : Timer
{
    Token _token;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _token = GetNode<Token>("/root/Game/MainGame/Token");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (_token.CurrentCondition == Token.Condition.SpikeTrapped)
        {
            
        }
    }
}