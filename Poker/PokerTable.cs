using CardsFramework;
using Microsoft.Xna.Framework;

namespace Poker;

internal class PokerTable : GameTable
{
    public PokerTable(Game game) : base(game.GraphicsDevice.Viewport.TitleSafeArea, Vector2.Zero,
        4, (x) => Vector2.Zero, "Red", game)
    {
        Hide();
    }
}