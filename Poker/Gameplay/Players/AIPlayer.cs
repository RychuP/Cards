using Framework.Engine;

namespace Poker.Gameplay.Players;

class AIPlayer : PokerBettingPlayer
{
    public AIPlayer(string name, Gender gender, int place, GameManager gm) : base(name, gender, place, gm)
    { }

    /// <inheritdoc/>
    public override void TakeTurn(int currentBetAmount, Hand communityCards, bool checkPossible)
    {
        base.TakeTurn(currentBetAmount, communityCards, checkPossible);

        if (checkPossible)
        {
            Check();
        }
        // equals sign for the case of big blind rule
        else if (currentBetAmount >= BetAmount && BetAmount < Balance)
        {
            Call(currentBetAmount);
        }
        else if (currentBetAmount > Balance)
        {
            AllIn();
        }
    }
}