using Framework.Engine;

namespace Poker.GameElements;

abstract class PokerPlayer : Player
{
    public float Balance { get; private set; }
    public float BetAmount { get; private set; }
    public Gender Gender { get; set; }

    public PokerPlayer(string name, CardGame game) : base(name, game)
    {

    }
}