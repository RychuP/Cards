#region File Description
//-----------------------------------------------------------------------------
// FramesetGameComponentAnimation.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CardsFramework;

/// <summary>
/// A "typical" animation that consists of alternating between a set of frames.
/// </summary>
public class FramesetGameComponentAnimation : AnimatedGameComponentAnimation
{
    #region Fields
    readonly Texture2D _framesTexture;
    readonly int _numberOfFrames;
    readonly int _numberOfFramePerRow;
    Vector2 _frameSize;

    private double _percent = 0;
    #endregion

    #region Initializations
    /// <summary>
    /// Creates a new instance of the class.
    /// </summary>
    /// <param name="framesTexture">The frames texture (animation sheet).</param>
    /// <param name="numberOfFrames">The number of frames in the sheet.</param>
    /// <param name="numberOfFramePerRow">The number of frame per row.</param>
    /// <param name="frameSize">Size of the frame.</param>
    public FramesetGameComponentAnimation(Texture2D framesTexture, int numberOfFrames, 
        int numberOfFramePerRow, Vector2 frameSize)
    {
        _framesTexture = framesTexture;
        _numberOfFrames = numberOfFrames;
        _numberOfFramePerRow = numberOfFramePerRow;
        _frameSize = frameSize;
    }
    #endregion

    /// <summary>
    /// Runs the frame set animation.
    /// </summary>
    /// <param name="gameTime">Game time information.</param>
    public override void Run(GameTime gameTime)
    {
        if (IsStarted())
        {
            // Calculate the completion percent of the animation
            _percent += gameTime.ElapsedGameTime.TotalMilliseconds /
                (Duration.TotalMilliseconds / AnimationCycles) * 100;

            if (_percent >= 100)
                _percent = 0;

            // Calculate the current frame index
            int animationIndex = (int)(_numberOfFrames * _percent / 100);
            Component.CurrentSegment = new Rectangle(
                    (int)_frameSize.X * (animationIndex % _numberOfFramePerRow), 
                    (int)_frameSize.Y * (animationIndex / _numberOfFramePerRow),
                    (int)_frameSize.X, (int)_frameSize.Y);
            Component.CurrentFrame = _framesTexture;
        }
        else
        {
            Component.CurrentFrame = null;
            Component.CurrentSegment = null;
        }
        base.Run(gameTime);
    }
}
