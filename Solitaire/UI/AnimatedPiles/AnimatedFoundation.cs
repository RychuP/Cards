using Framework.Misc;
using Microsoft.Xna.Framework.Graphics;
using Solitaire.Gameplay.Piles;
using Solitaire.Managers;

namespace Solitaire.UI.AnimatedPiles;

internal class AnimatedFoundation : AnimatedPile
{
    public AnimatedFoundation(Foundation foundation) : base(foundation)
    {

    }

    protected override void Hand_OnCardReceived(object o, CardEventArgs e)
    {
        base.Hand_OnCardReceived(o, e);
        var animCard = GetCardGameComponent(e.Card);
        animCard.IsFaceDown = false;
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        //var t = ScreenManager.CreateTexture(Game.GraphicsDevice, Pile.Bounds.Width, Pile.Bounds.Height,
        //    Color.LightBlue);
        //var sb = Game.Services.GetService<SpriteBatch>();
        //sb.Begin();
        //sb.Draw(t, Position, Color.White);
        //sb.End();

    }
}