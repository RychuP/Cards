using Framework.Assets;
using Framework.Engine;
using Framework.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Framework.UI;

/// <summary>
/// An <see cref="AnimatedGameComponent"/> implemented for a card.
/// </summary>
public class AnimatedCardGameComponent : AnimatedGameComponent
{
    public TraditionalCard Card { get; private set; }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="card">The card associated with the animation component.</param>
    /// <param name="cardGame">The associated game.</param>
    public AnimatedCardGameComponent(TraditionalCard card, CardGame cardGame) : base(cardGame, null)
    {
        Card = card;
    }

    /// <summary>
    /// Updates the component.
    /// </summary>
    /// <param name="gameTime">The game time.</param>
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (CardGame is not null)
        {
            Texture = IsFaceDown ? CardAssets.CardBacks[CardGame.Theme] :
                CardAssets.CardFaces[UIUtilty.GetCardAssetName(Card)];
        }
    }

    /// <summary>
    /// Draws the component.
    /// </summary>
    /// <param name="gameTime">The game time.</param>
    public override void Draw(GameTime gameTime)
    {
        var sb = Game.Services.GetService<SpriteBatch>();
        sb.Begin();

        // Draw the current frame at the designated destination, or at the initial 
        // position if a destination has not been set
        if (Texture != null)
        {
            if (Destination.HasValue)
                sb.Draw(Texture, Destination.Value, Color.White);
            else
                sb.Draw(Texture, Position, Color.White);
        }

        sb.End();
    }
}