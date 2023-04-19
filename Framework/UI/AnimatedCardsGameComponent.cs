#region File Description
//-----------------------------------------------------------------------------
// AnimatedCardsGameComponent.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using Microsoft.Xna.Framework;

namespace CardsFramework;

/// <summary>
/// An <see cref="AnimatedGameComponent"/> implemented for a card game
/// </summary>
public class AnimatedCardsGameComponent : AnimatedGameComponent
{
    public TraditionalCard Card { get; private set; }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="card">The card associated with the animation component.</param>
    /// <param name="cardGame">The associated game.</param>
    public AnimatedCardsGameComponent(TraditionalCard card, CardsGame cardGame) : base(cardGame, null)
    {
        Card = card;
    }

    #region Update and Render
    /// <summary>
    /// Updates the component.
    /// </summary>
    /// <param name="gameTime">The game time.</param>
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (CardGame is not null)
        {
            CurrentFrame = IsFaceDown ? CardGame.CardAssets["CardBack_" + CardGame.Theme] :
                CardGame.CardAssets[UIUtilty.GetCardAssetName(Card)];
        }
    }

    /// <summary>
    /// Draws the component.
    /// </summary>
    /// <param name="gameTime">The game time.</param>
    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);

        SpriteBatch.Begin();

        // Draw the current frame at the designated destination, or at the initial 
        // position if a destination has not been set
        if (CurrentFrame != null)
        {
            if (CurrentDestination.HasValue)
                SpriteBatch.Draw(CurrentFrame, CurrentDestination.Value, Color.White);
            else
                SpriteBatch.Draw(CurrentFrame, CurrentPosition, Color.White);
        }

        SpriteBatch.End();
    }
    #endregion
}