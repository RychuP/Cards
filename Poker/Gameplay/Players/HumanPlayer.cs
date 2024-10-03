namespace Poker.Gameplay.Players;

class HumanPlayer : PokerBettingPlayer
{
    public HumanPlayer(Gender gender, GameManager gm) : base("You", gender, 0, gm)
    {

    }
}