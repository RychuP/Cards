using Microsoft.Xna.Framework;

namespace Framework.UI;

/// <summary>
/// An animation which moves a component from one point to another.
/// </summary>
public class TransitionGameComponentAnimation : AnimatedGameComponentAnimation
{
    Vector2 _sourcePosition;
    Vector2 _positionDelta;
    float _percent = 0;
    Vector2 _destinationPosition;

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="sourcePosition">The source position.</param>
    /// <param name="destinationPosition">The destination position.</param>
    public TransitionGameComponentAnimation(Vector2 sourcePosition, Vector2 destinationPosition)
    {
        _sourcePosition = sourcePosition;
        _destinationPosition = destinationPosition;
        _positionDelta = destinationPosition - sourcePosition;
    }

    /// <summary>
    /// Runs the transition animation.
    /// </summary>
    /// <param name="gameTime">Game time information.</param>
    public override void Run(GameTime gameTime)
    {
        if (IsStarted())
        {
            // Calculate the animation's completion percentage.
            _percent += (float)(gameTime.ElapsedGameTime.TotalSeconds / Duration.TotalSeconds);

            // Move the component towards the destination as the animation progresses
            Component.Position = _sourcePosition + _positionDelta * _percent;

            if (IsDone())
                Component.Position = _destinationPosition;
        }
    }
}