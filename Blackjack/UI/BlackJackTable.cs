using System;
using Blackjack.Misc;
using Framework.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blackjack.UI;

class BlackJackTable : GameTable
{
    public Vector2 RingOffset { get; private set; }

    public BlackJackTable(Vector2 ringOffset, Rectangle tableBounds, Vector2 dealerPosition, 
        int places, Func<int, Vector2> placeOrder, Game game)
        : base(tableBounds, dealerPosition, places, placeOrder, game)
    {
        RingOffset = ringOffset;
    }

    /// <summary>
    /// Draw the rings of the chip on the table
    /// </summary>
    /// <param name="gameTime"></param>
    public override void Draw(GameTime gameTime)
    {
        var sb = Game.Services.GetService<SpriteBatch>();
        sb.Begin();
        sb.Draw(Art.Table, TableBounds, Color.White);
        for (int placeIndex = 0; placeIndex < Places; placeIndex++)
            sb.Draw(Art.Ring, PlaceOrder(placeIndex) + RingOffset, Color.White);
        sb.End();
    }
}