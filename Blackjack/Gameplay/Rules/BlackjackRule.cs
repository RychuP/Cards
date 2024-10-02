using System.Collections.Generic;
using Blackjack.Gameplay.Players;
using Blackjack.Misc;
using Framework.Engine;

namespace Blackjack.Gameplay.Rules;

/// <summary>
/// Represents a rule which checks if one of the player has achieved "blackjack".
/// </summary>
public class BlackJackRule : GameRule
{
    readonly List<BlackjackPlayer> _players = new();

    /// <summary>
    /// Creates a new instance of the <see cref="BlackJackRule"/> class.
    /// </summary>
    /// <param name="players">A list of players participating in the game.</param>
    public BlackJackRule(List<Player> players)
    {
        for (int playerIndex = 0; playerIndex < players.Count; playerIndex++)
            _players.Add((BlackjackPlayer)players[playerIndex]);
    }

    /// <summary>
    /// Check if any of the players has a hand value of 21 in any of their hands.
    /// </summary>
    public override void Check()
    {
        for (int playerIndex = 0; playerIndex < _players.Count; playerIndex++)
        {
            _players[playerIndex].CalculateValues();

            if (!_players[playerIndex].BlackJack)
            {
                // Check to see if the hand is eligible for a Black Jack
                if ((_players[playerIndex].FirstValue == 21 ||
                    _players[playerIndex].FirstValueConsiderAce &&
                    _players[playerIndex].FirstValue + 10 == 21) &&
                    _players[playerIndex].Hand.Count == 2)
                {
                    FireRuleMatch(new BlackjackGameEventArgs()
                    {
                        Player = _players[playerIndex],
                        Hand = HandTypes.First
                    });
                }
            }
            if (!_players[playerIndex].SecondBlackJack)
            {
                // Check to see if the hand is eligible for a Black Jack
                // A Black Jack is only eligible with 2 cards in a hand                   
                if (_players[playerIndex].IsSplit && (_players[playerIndex].SecondValue == 21 ||
                    _players[playerIndex].SecondValueConsiderAce &&
                     _players[playerIndex].SecondValue + 10 == 21) &&
                     _players[playerIndex].SecondHand.Count == 2)
                {
                    FireRuleMatch(new BlackjackGameEventArgs()
                    {
                        Player = _players[playerIndex],
                        Hand = HandTypes.Second
                    });
                }
            }
        }
    }
}