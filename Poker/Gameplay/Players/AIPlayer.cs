using Framework.Engine;

namespace Poker.Gameplay.Players;

class AIPlayer : PokerBettingPlayer
{
    public AIPlayer(string name, Gender gender, int place, CardGame game) : base(name, gender, place, game)
    {

    }
}