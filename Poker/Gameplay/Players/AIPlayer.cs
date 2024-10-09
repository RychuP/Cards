namespace Poker.Gameplay.Players;

class AIPlayer : PokerBettingPlayer
{
    public AIPlayer(string name, Gender gender, int place, GameManager gm) : base(name, gender, place, gm)
    { }

    /// <summary>
    /// Called when there was a raise during turn.
    /// </summary>
    /// <param name="currentBetAmount"></param>
    public void TakeTurn(int currentBetAmount)
    {
        if (currentBetAmount > BetAmount)
        {
            BetAmount = currentBetAmount;
            State = PlayerState.Called;
        }
    }

    /// <summary>
    /// Called when there was no raise during turn.
    /// </summary>
    public void TakeTurn()
    {
        State = PlayerState.Checked;
    }
}