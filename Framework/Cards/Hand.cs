#region File Description
//-----------------------------------------------------------------------------
// Hand.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

namespace CardsFramework;

/// <summary>
/// Represents a hand of cards held by a player, dealer or the game table
/// </summary>
/// <remarks>
/// A <see cref="Hand"/> is a type of <see cref="CardPacket"/> that may also
/// receive <see cref="Card"/> items, as well as loose them.
/// Therefore, it may receive <see cref="Card"/> items from any 
/// <see cref="CardPacket"/> or from another <see cref="Hand"/>. 
/// </remarks>
public class Hand : CardPacket
{
    #region Internal Methods
    /// <summary>
    /// Adds the specified card to the hand
    /// </summary>
    /// <param name="card">The card to add to the hand. The card will be added
    /// as the last card of the hand.</param>
    internal void Add(TraditionalCard card)
    {
        Cards.Add(card);
        CardReceived?.Invoke(this, new CardEventArgs(card));
    }

    /// <summary>
    /// Adds the specified cards to the hand
    /// </summary>
    /// <param name="cards">The cards to add to the hand. The cards are added
    /// as the last cards of the hand.</param>
    internal void Add(IEnumerable<TraditionalCard> cards)
    {
        Cards.AddRange(cards);
    }
    #endregion

    /// <summary>
    /// An event which triggers when a card is added to the hand.
    /// </summary>
    public event EventHandler<CardEventArgs> CardReceived;
}