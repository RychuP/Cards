using Poker.UI.AnimatedGameComponents;

namespace Poker.Gameplay.Players;

class CommunityCards : PokerCardsHolder
{
    public CommunityCards(GameManager gm) : base(string.Empty, gm)
    {
        AnimatedHand = new AnimatedDealerHand(-1, Hand, gm);
    }
}