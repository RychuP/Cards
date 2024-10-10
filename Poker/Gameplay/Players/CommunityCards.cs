using Framework.Assets;
using Framework.Engine;
using Framework.UI;
using Poker.UI.AnimatedGameComponents;
using System;

namespace Poker.Gameplay.Players;

class CommunityCards : PokerCardsHolder
{
    public CommunityCards(GameManager gm) : base(Constants.CommunityCardsName, gm)
    {
        AnimatedHand = new AnimatedDealerHand(-1, Hand, gm);
    }
}