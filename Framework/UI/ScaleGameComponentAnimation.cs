using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Framework.UI;

/// <summary>
/// An animation which scales a component.
/// </summary>
public class ScaleGameComponentAnimation : AnimatedGameComponentAnimation
{
    float _percent = 0;
    readonly float _beginFactor;
    readonly float _factorDelta;

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="beginFactor">The initial scale factor.</param>
    /// <param name="endFactor">The eventual scale factor.</param>
    public ScaleGameComponentAnimation(float beginFactor, float endFactor)
    {
        _beginFactor = beginFactor;
        _factorDelta = endFactor - beginFactor;
    }

    /// <summary>
    /// Runs the scaling animation.
    /// </summary>
    /// <param name="gameTime">Game time information.</param>
    public override void Run(GameTime gameTime)
    {
        if (IsStarted())
        {
            Texture2D texture = Component.Texture;
            if (texture != null)
            {
                // Calculate the completion percent of animation
                _percent += (float)(gameTime.ElapsedGameTime.TotalSeconds /
                    Duration.TotalSeconds);

                // Inflate the component with an increasing delta. The eventual
                // delta will have the component scale to the specified target
                // scaling factor.
                Rectangle bounds = texture.Bounds;
                bounds.X = (int)Component.Position.X;
                bounds.Y = (int)Component.Position.Y;
                float currentFactor = _beginFactor + _factorDelta * _percent - 1;
                bounds.Inflate((int)(bounds.Width * currentFactor),
                    (int)(bounds.Height * currentFactor));
                Component.Destination = bounds;
            }
        }
    }
}