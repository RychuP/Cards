using Framework.UI;
using Poker.Gameplay.Players;

namespace Poker.UI;

class PokerTable : GameTable
{
    public PokerTable(Game game) : base(Constants.GameArea, Constants.CommunityCardsPosition,
        Constants.MaxPlayers, PokerCardsHolder.GetPlayerPosition, game)
    {
        Hide();
    }
}