using Framework.Engine;

namespace Poker.Gameplay.Players;

class HumanPlayer : PokerBettingPlayer
{
    public HumanPlayer(string name, Gender gender, CardGame game) : base(name, gender, 0, game)
    {

    }
}