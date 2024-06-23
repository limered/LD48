using System;
using Godot;

namespace IsThisATiger2.Empty.Distraction;

public partial class DistractedTourist : Node2D
{
    public float DistractionWaitTime;
    private Action _onDistractionTimeUp;
    private Color _targetColor;
    private Sprite2D _image;
    private double _timeLeft;
    private bool _isRunning;
    
    public override void _Ready()
    {
        _image = GetNode<Sprite2D>("Sprite2D");
    }

    public void SetImage(Texture2D texture)
    {
        _image = GetNode<Sprite2D>("Sprite2D");
        _image.Texture = texture;
    }
    
    public void Start(Action onDone, Color targetColor)
    {
        if (_isRunning) return;
        _timeLeft = DistractionWaitTime;
        _onDistractionTimeUp = onDone;
        _targetColor = targetColor;
        _isRunning = true;
    }

    public void Abort()
    {
        _isRunning = false;
    }

    public override void _Process(double delta)
    {
        if(!_isRunning) return;
        _timeLeft -= delta;
        if (_timeLeft <= 0)
        {
            _onDistractionTimeUp();
            _isRunning = false;
            return;
        }
        var redShift = (float)(_timeLeft / DistractionWaitTime);
        _image.Modulate = Colors.White.Lerp(_targetColor, 1f - redShift);
    }
}
