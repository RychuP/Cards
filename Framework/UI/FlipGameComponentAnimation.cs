using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Framework.UI;

public class FlipGameComponentAnimation : AnimatedGameComponentAnimation
{
    protected int _percent = 0;
    public bool IsFromFaceDownToFaceUp = true;

    /// <summary>
    /// Runs the flip animation, which makes the component appear as if it has
    /// been flipped.
    /// </summary>
    /// <param name="gameTime">Game time information.</param>
    public override void Run(GameTime gameTime)
    {
        if (IsStarted())
        {
            if (IsDone())
            {
                // Finish tha animation
                Component.IsFaceDown = !IsFromFaceDownToFaceUp;
                Component.Destination = null;
            }
            else
            {
                Texture2D texture = Component.Texture;
                if (texture != null)
                {
                    // Calculate the completion percent of the animation
                    _percent += (int)(gameTime.ElapsedGameTime.TotalMilliseconds /
                        (Duration.TotalMilliseconds / AnimationCycles) * 100);

                    if (_percent >= 100)
                        _percent = 0;

                    int currentPercent;
                    if (_percent < 50)
                    {
                        // On the first half of the animation the component is
                        // on its initial size
                        currentPercent = _percent;
                        Component.IsFaceDown = IsFromFaceDownToFaceUp;
                    }
                    else
                    {
                        // On the second half of the animation the component
                        // is flipped
                        currentPercent = 100 - _percent;
                        Component.IsFaceDown = !IsFromFaceDownToFaceUp;
                    }
                    // Shrink and widen the component to look like it is flipping
                    Component.Destination = new Rectangle(
                        (int)(Component.Position.X + texture.Width * currentPercent / 100),
                        (int)Component.Position.Y,
                        texture.Width - texture.Width * currentPercent / 100 * 2,
                        texture.Height
                    );
                }
            }
        }
    }
}