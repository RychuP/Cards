#region File Description
//-----------------------------------------------------------------------------
// AnimatedGameComponent.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CardsFramework;

/// <summary>
/// A game component.
/// Enable variable display while managing and displaying a set of
/// <see cref="AnimatedGameComponentAnimation">Animations</see>
/// </summary>
public class AnimatedGameComponent : DrawableGameComponent
{
    #region Fields and Properties
    public Texture2D CurrentFrame { get; set; }
    public Rectangle? CurrentSegment { get; set; }
    public string Text { get; set; }
    public Color TextColor { get; set; } = Color.Black;
    public bool IsFaceDown { get; set; } = true;
    public Vector2 CurrentPosition { get; set; }
    public Rectangle? CurrentDestination { get; set; }
    protected SpriteBatch SpriteBatch { get; init; }

    readonly List<AnimatedGameComponentAnimation> _runningAnimations = new();

    /// <summary>
    /// Whether or not an animation belonging to the component is running.
    /// </summary>
    public virtual bool IsAnimating =>
        _runningAnimations.Count > 0;

    public CardsGame CardGame { get; private set; }
    #endregion

    #region Initializations
    /// <summary>
    /// Initializes a new instance of the class, using black text color.
    /// </summary>
    /// <param name="game">The associated game class.</param>
    public AnimatedGameComponent(Game game): base(game)
    {
        SpriteBatch = CardGame != null ? CardGame.SpriteBatch : new SpriteBatch(game.GraphicsDevice);
    }

    /// <summary>
    /// Initializes a new instance of the class, using black text color.
    /// </summary>
    /// <param name="game">The associated game class.</param>
    /// <param name="currentFrame">The texture serving as the current frame
    /// to display as the component.</param>
    public AnimatedGameComponent(Game game, Texture2D currentFrame) : this(game)
    {
        CurrentFrame = currentFrame;
    }

    /// <summary>
    /// Initializes a new instance of the class, using black text color.
    /// </summary>
    /// <param name="cardGame">The associated card game.</param>
    /// <param name="currentFrame">The texture serving as the current frame
    /// to display as the component.</param>
    public AnimatedGameComponent(CardsGame cardGame, Texture2D currentFrame) : this(cardGame.Game)
    {
        CardGame = cardGame;
        CurrentFrame = currentFrame;
    }
    #endregion

    #region Update and Render
    /// <summary>
    /// Keeps track of the component's animations.
    /// </summary>
    /// <param name="gameTime">The time which as elapsed since the last call
    /// to this method.</param>
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        for (int animationIndex = 0; animationIndex < _runningAnimations.Count; animationIndex++)
        {
            var anim = _runningAnimations[animationIndex];
            anim.AccumulateElapsedTime(gameTime.ElapsedGameTime);
            anim.Run(gameTime);
            if (anim.IsDone())
            {
                _runningAnimations.RemoveAt(animationIndex);
                animationIndex--;
            }
        }
    }

    /// <summary>
    /// Draws the animated component and its associated text, if it exists, at
    /// the object's set destination. If a destination is not set, its initial
    /// position is used.
    /// </summary>
    /// <param name="gameTime">The time which as elapsed since the last call
    /// to this method.</param>
    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);

        SpriteBatch.Begin();

        // Draw at the destination if one is set
        if (CurrentDestination is Rectangle destination && CurrentFrame != null)
        {
            SpriteBatch.Draw(CurrentFrame, destination, CurrentSegment, Color.White);
            if (CardGame != null && Text != null)
                DrawText(Text, CardGame.Font, destination, destination.Location.ToVector2());
        }
        // Draw at the component's position if there is no destination
        else if (CurrentFrame != null)
        {
            SpriteBatch.Draw(CurrentFrame, CurrentPosition, CurrentSegment, Color.White);
            if (CardGame != null && Text != null)
                DrawText(Text, CardGame.Font, CurrentFrame.Bounds, CurrentPosition);
        }

        SpriteBatch.End();
    }

    void DrawText(string text, SpriteFont font, Rectangle destination, Vector2 position)
    {
        Vector2 size = font.MeasureString(Text);
        float x = position.X + (destination.Width - size.X) / 2;
        float y = position.Y + (destination.Height - size.Y) / 2;
        Vector2 textPosition = new(x, y);
        SpriteBatch.DrawString(font, text, textPosition, TextColor);
    }
    #endregion

    /// <summary>
    /// Adds an animation to the animated component.
    /// </summary>
    /// <param name="animation">The animation to add.</param>
    public void AddAnimation(AnimatedGameComponentAnimation animation)
    {
        animation.Component = this;
        _runningAnimations.Add(animation);
    }

    /// <summary>
    /// Calculate the estimated time at which the longest lasting animation currently managed 
    /// will complete.
    /// </summary>
    /// <returns>The estimated time for animation complete </returns>
    public virtual TimeSpan EstimatedTimeForAnimationsCompletion()
    {
        TimeSpan result = TimeSpan.Zero;

        if (IsAnimating)
        {
            for (int animationIndex = 0; animationIndex < _runningAnimations.Count; animationIndex++)
            {
                if (_runningAnimations[animationIndex].EstimatedTimeForAnimationCompletion > result)
                {
                    result = _runningAnimations[animationIndex].EstimatedTimeForAnimationCompletion;
                }
            }
        }

        return result;
    }
}
