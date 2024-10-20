using Framework.Assets;
using Framework.Engine;
using Framework.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics.Metrics;

namespace Framework.UI;

/// <summary>
/// Game component with variable display and a set of <see cref="AnimatedGameComponentAnimation">Animations</see>
/// </summary>
public class AnimatedGameComponent : DrawableGameComponent
{
    public event EventHandler<TextChangedEventArgs> TextChanged;
    public event EventHandler<PositionChangedEventArgs> PositionChanged;
    public Texture2D Texture { get; set; }
    public Rectangle? CurrentSegment { get; set; }
    public Color TextColor { get; set; } = Color.Black;
    public bool IsFaceDown { get; set; } = true;
    public Rectangle? Destination { get; set; }

    readonly List<AnimatedGameComponentAnimation> _runningAnimations = new();

    /// <summary>
    /// Whether or not an animation belonging to the component is running.
    /// </summary>
    public virtual bool IsAnimating =>
        _runningAnimations.Count > 0;

    public CardGame CardGame { get; private set; }

    // backing field
    Vector2 _position;
    public Vector2 Position
    {
        get => _position;
        set
        {
            if (_position == value) return;
            var prevPosition = _position;
            _position = value;

        }
    }

    // backing field
    string _text;
    public string Text
    {
        get => _text;
        set
        {
            if (_text == value) return;
            var prevText = _text;
            _text = value;
            OnTextChanged(prevText, value);
        }
    }

    public AnimatedGameComponent(Game game) : base(game)
    { }

    /// <summary>
    /// Initializes a new instance of the class, using black text color.
    /// </summary>
    /// <param name="game">The associated game class.</param>
    /// <param name="currentFrame">The texture serving as the current frame
    /// to display as the component.</param>
    public AnimatedGameComponent(Game game, Texture2D currentFrame) : this(game)
    {
        Texture = currentFrame;
    }

    /// <summary>
    /// Initializes a new instance of the class, using black text color.
    /// </summary>
    /// <param name="cardGame">The associated card game.</param>
    /// <param name="currentFrame">The texture serving as the current frame
    /// to display as the component.</param>
    public AnimatedGameComponent(CardGame cardGame, Texture2D currentFrame) : this(cardGame.Game)
    {
        CardGame = cardGame;
        Texture = currentFrame;
    }

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

        var sb = Game.Services.GetService<SpriteBatch>();
        sb.Begin();

        // Draw at the destination if one is set
        if (Destination is Rectangle destination && Texture != null)
        {
            sb.Draw(Texture, destination, CurrentSegment, Color.White);
            if (CardGame != null && Text != null)
                DrawText(Text, CardGame.Font, destination, destination.Location.ToVector2());
        }
        // Draw at the component's position if there is no destination
        else if (Texture != null)
        {
            sb.Draw(Texture, Position, CurrentSegment, Color.White);
            if (CardGame != null && Text != null)
                DrawText(Text, CardGame.Font, Texture.Bounds, Position);
        }

        sb.End();
    }

    protected virtual void DrawText(string text, SpriteFont font, Rectangle destination, Vector2 position)
    {
        var sb = Game.Services.GetService<SpriteBatch>();
        Vector2 size = font.MeasureString(Text);
        float x = position.X + (destination.Width - size.X) / 2;
        float y = position.Y + (destination.Height - size.Y) / 2;
        Vector2 textPosition = new(x, y);
        sb.DrawString(font, text, textPosition, TextColor);
    }

    public void RemoveAnimations() =>
        _runningAnimations.Clear();

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

    protected virtual void OnTextChanged(string prevText, string newText)
    { 
        var args = new TextChangedEventArgs(prevText, newText);
        TextChanged?.Invoke(this, args);
    }

    protected virtual void OnPositionChanged(Vector2 prevPosition, Vector2 newPosition)
    {
        var args = new PositionChangedEventArgs(prevPosition, newPosition);
        PositionChanged?.Invoke(this, args);
    }
}