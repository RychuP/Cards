using Framework.Engine;
using Poker.Gameplay.Players;
using System;

namespace Poker.Gameplay.Rules;

/// <summary>
/// Checks if the current game has ended.
/// </summary>
class GameEndRule : GameRule
{
    readonly GameManager _gameManager;

    public GameEndRule(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    public override void Check()
    {
        // check in the first three rounds of betting if everybode folded but one player
        if (_gameManager.State == GameState.Preflop ||
            _gameManager.State == GameState.TurnBet ||
            _gameManager.State == GameState.RiverBet)
        {
            int foldedCount = 0;
            PokerBettingPlayer winner = null;

            // count the players who folded
            for (int i = 0; i < _gameManager.PlayerCount; i++)
            {
                if (_gameManager[i].State == PlayerState.Folded)
                    foldedCount++;
                else 
                    winner = _gameManager[i];
            }

            // everybody folded... this should not happen
            if (winner is null)
            {
                throw new Exception("All players folded. This state should not be reached.");
            }
            // all folded but one... we have a winner
            else if (foldedCount == _gameManager.PlayerCount - 1)
            {
                OnRuleMatch(new GameEndEventArgs(winner));
            }
        }
    }
}