using Poker.UI.AnimatedGameComponents;

namespace Poker.Gameplay.Players;

class CommunityCards : PokerCardsHolder
{
    public CommunityCards(GameManager gm) : base(Constants.CommunityCardsName, gm)
    {
        AnimatedHand = new AnimatedDealerHand(-1, Hand, gm);
    }

    public void Reset(Dealer dealer)
    {
        Hand.DealCardsToHand(dealer, Hand.Count);
    }
}