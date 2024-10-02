using Framework.UI;

namespace Poker.UI;

class PokerTable : GameTable
{
    public PokerTable(Game game) : base(Constants.GameArea, Vector2.Zero,
        Constants.MaxPlayers, (x) => Vector2.Zero, Constants.DefaultTheme, game)
    {
        Hide();
    }
}