using System;
using Godot;

namespace IsThisATiger2.Empty.Distraction;

public partial class DistractedTourist : Node2D
{
    public float DistractionWaitTime;
    public Action OnDistractionTimeUp;

    private Timer _distractedWaitTimer;
    private Sprite2D _image;
    public override void _Ready()
    {
        _distractedWaitTimer = GetNode<Timer>("DistractionWaitTime");
        _distractedWaitTimer.WaitTime = DistractionWaitTime;
        _distractedWaitTimer.Timeout += OnDistractionTimeUp;

        _image = GetNode<Sprite2D>("Sprite2D");
    }

    public void Start()
    {
        _distractedWaitTimer.Start();
    }

    public override void _Process(double delta)
    {
        var redShift = (float)(_distractedWaitTimer.TimeLeft / DistractionWaitTime);
        _image.SelfModulate = new Color(1f, 1f-redShift, 1f-redShift);
    }
}

