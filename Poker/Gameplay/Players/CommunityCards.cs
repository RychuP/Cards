using Poker.UI.AnimatedHands;

namespace Poker.Gameplay.Players;

class CommunityCards : PokerCardsHolder
{
    public CommunityCards(GameManager gm) : base(Constants.CommunityCardsName, gm)
    {
        AnimatedHand = new AnimatedDealerHand(-1, Hand, gm);
    }
}