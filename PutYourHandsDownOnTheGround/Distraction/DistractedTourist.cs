using System;
using Godot;

namespace IsThisATiger2.Empty.Distraction;

public partial class DistractedTourist : Node2D
{
    public float DistractionWaitTime;
    public Action OnDistractionTimeUp;

    private Sprite2D _image;

    private double _timeLeft;
    private bool _isRunning;
    
    public override void _Ready()
    {
        _image = GetNode<Sprite2D>("Sprite2D");
    }

    public void Start()
    {
        if (_isRunning) return;
        _timeLeft = DistractionWaitTime;
        _isRunning = true;
    }

    public override void _Process(double delta)
    {
        if(!_isRunning) return;
        _timeLeft -= delta;
        if (_timeLeft <= 0) return;
        var redShift = (float)(_timeLeft / DistractionWaitTime);
        _image.Modulate = new Color(1f, redShift, redShift);
    }
}

