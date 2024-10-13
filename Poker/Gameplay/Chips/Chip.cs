using Framework.Assets;
using Framework.UI;
using Microsoft.Xna.Framework.Graphics;
using Poker.UI.AnimatedGameComponents;
using System;

namespace Poker.Gameplay.Chips;

abstract class Chip
{
    public static readonly TimeSpan AnimationDuration = TimeSpan.FromMilliseconds(800);

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
    AnimatedChipComponent _animatedChipComponent;

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

    protected bool IsAnimating => _animatedChipComponent.IsAnimating;

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

    public virtual Vector2 GetTablePosition() => Vector2.Zero;

    /// <summary>
    /// Removes <see cref="AnimatedChipComponent"/> from <see cref="Game.Components"/>.
    /// </summary>
    /// <remarks>Local field is also marked internally as null.</remarks>
    public void RemoveAnimatedComponent()
    {
        if (Game.Components.Contains(_animatedChipComponent))
            Game.Components.Remove(_animatedChipComponent);
        _animatedChipComponent = null;
    }

    /// <summary>
    /// Adds transition animations that follow the position change of the chip.
    /// </summary>
    void OnPositionChanged(Vector2 prevPosition, Vector2 newPosition)
    {
        // two special cases for not creating animations:
        // 1. when the chip is created by bet component and starts in hidden position
        // 2. when the chip is created by a player and starts at the zero vector
        //    and is then subsequently moved to the community chip position
        //    from where it's going to start its actual animation
        if (newPosition == HiddenPosition || prevPosition == Vector2.Zero)
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
            Duration = AnimationDuration,
            PerformWhenDone = (o) => CardSounds.Bet.Play()
        });

        // Add flip animation
        _animatedChipComponent.AddAnimation(new FlipGameComponentAnimation()
        {
            Duration = AnimationDuration,
            AnimationCycles = 3,
        });
    }
}