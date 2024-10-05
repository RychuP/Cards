using Framework.UI;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Poker.Gameplay.Chips;

abstract class Chip
{
    Vector2 _position;
    readonly AnimatedGameComponent _animatedGameComponent;

    public Chip(Game game, Vector2 position, Texture2D texture)
    {
        Position = position;
        _animatedGameComponent = new(game, texture);
    }

    public Vector2 Position
    {
        get => _position;
        set
        {
            if (value != _position)
            {
                var prevPos = _position;
                _position = value;
                var eventArgs = new PositionChangedEventArgs(prevPos, value);
                OnPositionChanged(eventArgs);
            }
        }
    }

    void OnPositionChanged(PositionChangedEventArgs e)
    {
        PositionChanged?.Invoke(this, e);
    }

    public event EventHandler<PositionChangedEventArgs> PositionChanged;
}