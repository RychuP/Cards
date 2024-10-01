#region File Description
//-----------------------------------------------------------------------------
// TraditionalCard.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

namespace CardsFramework;

/// <summary>
/// Enum defining the various types of cards for a traditional-western card-set
/// </summary>
[Flags]
public enum CardSuits
{
    Heart = 0x01,
    Diamond = 0x02,
    Club = 0x04,
    Spade = 0x08,

    // Sets:
    AllSuits = Heart | Diamond | Club | Spade
}

/// <summary>
/// Enum defining the various types of card values for a traditional-western card-set
/// </summary>
[Flags]
public enum CardValues
{
    Ace = 0x01,
    Two = 0x02,
    Three = 0x04,
    Four = 0x08,
    Five = 0x10,
    Six = 0x20,
    Seven = 0x40,
    Eight = 0x80,
    Nine = 0x100,
    Ten = 0x200,
    Jack = 0x400,
    Queen = 0x800,
    King = 0x1000,
    FirstJoker = 0x2000,
    SecondJoker = 0x4000,

    // Sets:
    AllNumbers = 0x3FF,
    NonJokers = 0x1FFF,
    Jokers = FirstJoker | SecondJoker,
    AllFigures = Jack | Queen | King,
}

/// <summary>
/// Traditional-western card
/// </summary>
/// <remarks>
/// Each card has a defined <see cref="CardSuits">Type</see> and <see cref="CardValues">Value</see>
/// as well as the <see cref="CardsFramework.CardPacket"/> in which it is being held.
/// A card may not be held in more than one <see cref="CardsFramework.CardPacket"/>. This is achived by enforcing any card transfer
/// operation between <see cref="CarkPacket"/>s and <see cref="Hand"/>s to be performed only from within the card's 
/// <see cref="MoveToHand"/> method only. This method accesses <c>internal</c> <see cref="Hand.Add"/> method and 
/// <see cref="CardPacket.Remove"/> method accordingly to complete the card transfer operation.
/// </remarks>
public class TraditionalCard
{
    #region Properties
    public CardSuits Type { get; set; }
    public CardValues Value { get; set; }
    public CardPacket CardPacket { get; private set; }
    #endregion

    #region Initializations
    /// <summary>
    /// Initializes a new instance of the <see cref="TraditionalCard"/> class.
    /// </summary>
    /// <param name="type">The card suit. Supports only a single value.</param>
    /// <param name="value">The card's value. Only single values are 
    /// supported.</param>
    /// <param name="holdingCardCollection">The holding card collection.</param>
    internal TraditionalCard(CardSuits type, CardValues value, CardPacket holdingCardCollection)
    {
        // Check for single type
        switch (type)
        {
            case CardSuits.Club:
            case CardSuits.Diamond:
            case CardSuits.Heart:
            case CardSuits.Spade:
                break;
            default:
                throw new ArgumentException("type must be a single value", nameof(type));
        }

        // Check for single value
        switch (value)
        {
            case CardValues.Ace:
            case CardValues.Two:
            case CardValues.Three:
            case CardValues.Four:
            case CardValues.Five:
            case CardValues.Six:
            case CardValues.Seven:
            case CardValues.Eight:
            case CardValues.Nine:
            case CardValues.Ten:
            case CardValues.Jack:
            case CardValues.Queen:
            case CardValues.King:
            case CardValues.FirstJoker:
            case CardValues.SecondJoker:
                break;
            default:
                throw new ArgumentException("value must be single value", nameof(value));
        }

        Type = type;
        Value = value;
        CardPacket = holdingCardCollection;
    }
    #endregion

    /// <summary>
    /// Moves the card from its current <see cref="CardsFramework.CardPacket"/> 
    /// to the specified <paramref name="hand"/>. 
    /// This method of operation prevents any one card instance from being held by more than one
    /// <see cref="CardsFramework.CardPacket"/> at the same time.
    /// </summary>
    /// <param name="hand">The receiving hand.</param>
    public void MoveToHand(Hand hand)
    {
        CardPacket.Remove(this);
        CardPacket = hand;
        hand.Add(this);
    } 
}