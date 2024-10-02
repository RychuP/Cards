using Framework.Engine;
using Microsoft.Xna.Framework;
using System;

namespace Blackjack.Misc;

/// <summary>
/// Custom event argument which includes the index of the player who
/// triggered the event. This is used by the MenuEntry.Selected event.
/// </summary>
class PlayerIndexEventArgs : EventArgs
{
    /// <summary>
    /// Gets the index of the player who triggered this event.
    /// </summary>
    public PlayerIndex PlayerIndex { get; init; }

    /// <summary>
    /// Constructor.
    /// </summary>
    public PlayerIndexEventArgs(PlayerIndex playerIndex) =>
        PlayerIndex = playerIndex;
}

public class BlackjackGameEventArgs : EventArgs
{
    public Player Player { get; set; }
    public HandTypes Hand { get; set; }
}