using Framework.UI;

namespace Poker.UI;

class PokerTable : GameTable
{
    public PokerTable(Game game) : base(Constants.GameArea, Constants.CommunityCardsPosition,
        Constants.MaxPlayers, UIUtility.GetPlayerPosition, game)
    {
        Hide();
    }
}