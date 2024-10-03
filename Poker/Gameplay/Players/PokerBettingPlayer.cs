using Framework.Engine;
using Poker.UI.AnimatedHands;

namespace Poker.Gameplay.Players;

class PokerBettingPlayer : PokerCardsHolder
{
    public float Balance { get; private set; }
    public float BetAmount { get; private set; }
    public Gender Gender { get; }

    public PokerBettingPlayer(string name, Gender gender, int place, CardGame cardGame)
        : base(name, place, cardGame)
    {
        AnimatedHand = new AnimatedPlayerHand(place, Hand, cardGame);
        Gender = gender;
    }
}