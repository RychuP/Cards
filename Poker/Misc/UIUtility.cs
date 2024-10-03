using System;

namespace Poker.Misc;

static class UIUtility
{
    /// <summary>
    /// Gets the player hand positions according to the player index.
    /// </summary>
    /// <param name="playerIndex">The player's index.</param>
    /// <returns>The position for the player's hand on the game table.</returns>
    public static Vector2 GetPlayerPosition(int playerIndex)
    {
        return playerIndex switch
        {
            0 => new Vector2(Constants.PlayerCardMargin, Constants.GameHeight - Constants.CardSize.Y - Constants.PlayerCardMargin),
            1 => new Vector2(Constants.PlayerCardMargin, Constants.PlayerCardMargin),
            2 => new Vector2(Constants.GameWidth - Constants.CardSize.X * 2 - Constants.PlayerCardMargin - Constants.PlayerCardPadding,
                Constants.PlayerCardMargin),
            3 => new Vector2(Constants.GameWidth - Constants.CardSize.X * 2 - Constants.PlayerCardMargin - Constants.PlayerCardPadding,
                Constants.GameHeight - Constants.CardSize.Y - Constants.PlayerCardMargin),
            _ => throw new ArgumentException("Player index should be between 0 and 2", nameof(playerIndex)),
        };
    }
}