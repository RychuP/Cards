#region File Description
//-----------------------------------------------------------------------------
// BlackjackAIPlayer.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using CardsFramework;

namespace Blackjack;

class BlackjackAIPlayer : BlackjackPlayer
{
    #region Fields
    readonly static Random _random = new();

    public event EventHandler Hit;
    public event EventHandler Stand;
    #endregion

    /// <summary>
    /// Creates a new instance of the <see cref="BlackjackAIPlayer"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="game">The game.</param>
    public BlackjackAIPlayer(string name, CardGame game) : base(name, game) { }

    #region Pulic Methods
    /// <summary>
    /// Performs a move during a round.
    /// </summary>
    public void AIPlay()
    {
        int value = FirstValue;
        if (FirstValueConsiderAce && value + 10 <= 21)
        {
            value += 10;
        }

        if (value < 17)
        {
            Hit?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Stand?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Returns the amount which the AI player decides to bet.
    /// </summary>
    /// <returns>The AI player's bet.</returns>
    public int AIBet()
    {
        int[] chips = { 0, 5, 25, 100, 500 };
        int bet = chips[_random.Next(0, chips.Length)];

        if (bet < Balance)
        {
            return bet;
        }

        return 0;
    }
    #endregion
}