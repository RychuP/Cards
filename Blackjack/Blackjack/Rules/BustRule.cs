#region File Description
//-----------------------------------------------------------------------------
// BustRule.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System.Collections.Generic;
using CardsFramework;

namespace Blackjack;

/// <summary>
/// Represents a rule which checks if one of the player has gone bust.
/// </summary>
public class BustRule : GameRule
{
    readonly List<BlackjackPlayer> _players = new();

    /// <summary>
    /// Creates a new instance of the <see cref="BustRule"/> class.
    /// </summary>
    /// <param name="players">A list of players participating in the game.</param>
    public BustRule(List<Player> players)
    {
        for (int playerIndex = 0; playerIndex < players.Count; playerIndex++)
            _players.Add((BlackjackPlayer)players[playerIndex]);
    }

    /// <summary>
    /// Check if any of the players has exceeded 21 in any of their hands.
    /// </summary>
    public override void Check()
    {
        for (int playerIndex = 0; playerIndex < _players.Count; playerIndex++)
        {
            _players[playerIndex].CalculateValues();

            if (!_players[playerIndex].Bust)
            {
                if (!_players[playerIndex].FirstValueConsiderAce && _players[playerIndex].FirstValue > 21)
                {
                    FireRuleMatch(new BlackjackGameEventArgs() { 
                        Player = _players[playerIndex], Hand = HandTypes.First });
                }
            }
            if (!_players[playerIndex].SecondBust)
            {
                if ((_players[playerIndex].IsSplit && 
                    !_players[playerIndex].SecondValueConsiderAce && 
                     _players[playerIndex].SecondValue > 21))
                {
                    FireRuleMatch(new BlackjackGameEventArgs() { 
                        Player = _players[playerIndex], Hand = HandTypes.Second});
                }
            }
        }
    }
}