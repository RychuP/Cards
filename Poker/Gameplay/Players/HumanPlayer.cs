﻿namespace Poker.Gameplay.Players;

class HumanPlayer : PokerBettingPlayer
{
    /// <summary>
    /// Bet amount that the player started their turn with.
    /// </summary>
    public int PrevBetAmount { get; private set; } = -1;

    // backing field
    bool _startedTurn;
    /// <summary>
    /// Turn marker that shows whether the human player is currently taking their turn.
    /// </summary>
    public bool StartedTurn
    {
        get => _startedTurn;
        set
        {
            if (_startedTurn == value) return;
            _startedTurn = value;

            if (value == true)
                PrevBetAmount = BetAmount;
            else
                PrevBetAmount = -1;
        }
    }

    public HumanPlayer(Gender gender, GameManager gm) : base("You", gender, 0, gm)
    {

    }
}