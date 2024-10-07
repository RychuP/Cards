using Framework.Assets;
using Framework.UI;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Poker.Gameplay.Chips;

abstract class Chip
{
    /// <summary>
    /// Size of any chip used in the game.
    /// </summary>
    public static readonly Point Size = Art.SmallBlindChip.Bounds.Size;

    /// <summary>
    /// Distance between chips placed below the community cards.
    /// </summary>
    public static readonly int Padding = 25;

    /// <summary>
    /// Gap between the player cards and the first chip.
    /// </summary>
    public static readonly int PlayerAreaPadding = 11;

    /// <summary>
    /// Off the screen, hidden position for the community chips.
    /// </summary>
    public static readonly Vector2 HiddenPosition 
        = new((Constants.GameWidth - Size.X) / 2, Constants.GameHeight + Size.Y);

    /// <summary>
    /// Graphical representation of the chip.
    /// </summary>
    readonly AnimatedGameComponent _animatedChipComponent;

    protected readonly Game Game;

    Vector2 _position;
    /// <summary>
    /// Position of the chip on the table.
    /// </summary>
    public Vector2 Position
    {
        get => _position;
        set
        {
            if (value == _position) return;
            var prevPos = _position;
            _position = value;
            OnPositionChanged(prevPos, value);
        }
    }

    public Chip(Game game, Vector2 position, Texture2D texture)
    {
        _animatedChipComponent = new(game, texture)
        {
            Position = position,
            Visible = true
        };
        game.Components.Add(_animatedChipComponent);
        Position = position;
        Game = game;
    }

    void OnPositionChanged(Vector2 prevPosition, Vector2 newPosition)
    {
        if (newPosition == HiddenPosition)
        {
            _animatedChipComponent.Position = newPosition;
            _animatedChipComponent.Enabled = true;
            _animatedChipComponent.Visible = true;
            return;
        }
        
        _animatedChipComponent.RemoveAnimations();

        // Add transition animation
        _animatedChipComponent.AddAnimation(new TransitionGameComponentAnimation(prevPosition, newPosition)
        {
            Duration = TimeSpan.FromSeconds(1f),
            PerformWhenDone = (o) => CardSounds.Bet.Play()
        });

        // Add flip animation
        _animatedChipComponent.AddAnimation(new FlipGameComponentAnimation()
        {
            Duration = TimeSpan.FromSeconds(1f),
            AnimationCycles = 3,
        });
    }

    public virtual Vector2 GetTablePosition() =>
        Vector2.Zero;
}