using Poker.UI.AnimatedHands;

namespace Poker.Gameplay.Players;

class CommunityCards : PokerCardsHolder
{
    public CommunityCards(GameManager cardGame) : base(Constants.CommunityCardsName, -1, cardGame)
    {
        AnimatedHand = new AnimatedDealerHand(-1, Hand, cardGame);
    }
}