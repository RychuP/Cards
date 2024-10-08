namespace Poker.Gameplay.Players;

class AIPlayer : PokerBettingPlayer
{
    public AIPlayer(string name, Gender gender, int place, GameManager gm) : base(name, gender, place, gm)
    { }

    public PlayerState TakeTurn(int currentBetAmount)
    {
        if (currentBetAmount > BetAmount)
        {
            BetAmount = currentBetAmount;
            State = PlayerState.Called;
            return State;
        }

        return State;
    }
}