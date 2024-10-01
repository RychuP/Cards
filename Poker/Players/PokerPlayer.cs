using CardsFramework;

namespace Poker.Players;

internal class PokerPlayer : Player
{
    public float Balance { get; private set; }
    public float BetAmount { get; private set; }

    public PokerPlayer(string name, CardGame game) : base(name, game)
    {

    }
}