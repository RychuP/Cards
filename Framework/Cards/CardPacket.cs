#region File Description
//-----------------------------------------------------------------------------
// CardsCollection.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

namespace CardsFramework;

/// <summary>
/// Card related <see cref="EventArgs"/> holding event information of a <see cref="TraditionalCard"/> 
/// </summary>
public class CardEventArgs : EventArgs
{
    public TraditionalCard Card { get; set; }
    public CardEventArgs(TraditionalCard card) =>
        Card = card;
}

/// <summary>
/// A packet of cards
/// </summary>
/// <remarks>
/// A card packet may be initialized with a collection of cards. 
/// It may lose cards or deal them to <see cref="Hand"/>, but may
/// not receive new cards unless derived and overridden.
/// </remarks>
public class CardPacket
{
    #region Field Property Indexer
    protected List<TraditionalCard> Cards { get; set; } = new();

    /// <summary>
    /// An event which triggers when a card is removed from the collection.
    /// </summary>
    public event EventHandler<CardEventArgs> CardRemoved;

    public int Count =>
        Cards.Count;

    /// <summary>
    /// Returns a card at a specified index in the collection.
    /// </summary>
    /// <param name="index">The card's index.</param>
    /// <returns>The card at the specified index.</returns>
    public TraditionalCard this[int index] =>
        Cards[index];

    readonly Random _random = new();
    #endregion

    #region Initializations
    public CardPacket() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CardPacket"/> class.
    /// </summary>
    /// <param name="numberOfDecks">The number of decks to add to 
    /// the collection.</param>
    /// <param name="jokersInDeck">The amount of jokers in each deck.</param>
    /// <param name="suits">The suits to add to each decks. Suits are specified 
    /// as flags and several can be added.</param>
    /// <param name="cardValues">The card values which will appear in each deck.
    /// values are specified as flags and several can be added.</param>
    public CardPacket(int numberOfDecks, int jokersInDeck, CardSuits suits, CardValues cardValues)
    {
        for (int deckIndex = 0; deckIndex < numberOfDecks; deckIndex++)
        {
            AddSuit(suits, cardValues);

            for (int j = 0; j < jokersInDeck / 2; j++)
            {
                Cards.Add(new TraditionalCard(CardSuits.Club, CardValues.FirstJoker, this));
                Cards.Add(new TraditionalCard(CardSuits.Club, CardValues.SecondJoker, this));
            }

            if (jokersInDeck % 2 == 1)
            {
                Cards.Add(new TraditionalCard(CardSuits.Club, CardValues.FirstJoker, this));
            }
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Adds suits of cards to the collection.
    /// </summary>
    /// <param name="suits">The suits to add to each decks. Suits are specified 
    /// as flags and several can be added.</param>
    /// <param name="cardValues">The card values which will appear in each deck.
    /// values are specified as flags and several can be added.</param>
    private void AddSuit(CardSuits suits, CardValues cardValues)
    {
        if ((suits & CardSuits.Club) == CardSuits.Club)
        {
            AddCards(CardSuits.Club, cardValues);
        }

        if ((suits & CardSuits.Diamond) == CardSuits.Diamond)
        {
            AddCards(CardSuits.Diamond, cardValues);
        }

        if ((suits & CardSuits.Heart) == CardSuits.Heart)
        {
            AddCards(CardSuits.Heart, cardValues);
        }

        if ((suits & CardSuits.Spade) == CardSuits.Spade)
        {
            AddCards(CardSuits.Spade, cardValues);
        }
    }

    /// <summary>
    /// Adds cards to the collection.
    /// </summary>
    /// <param name="suit">The suit of the added cards.</param>
    /// <param name="cardValues">The card values which will appear in each deck.
    /// values are specified as flags and several can be added.</param>
    private void AddCards(CardSuits suit, CardValues cardValues)
    {
        if ((cardValues & CardValues.Ace) == CardValues.Ace)
        {
            Cards.Add(new TraditionalCard(suit, CardValues.Ace, this));
        }

        if ((cardValues & CardValues.Two) == CardValues.Two)
        {
            Cards.Add(new TraditionalCard(suit, CardValues.Two, this));
        }

        if ((cardValues & CardValues.Three) == CardValues.Three)
        {
            Cards.Add(new TraditionalCard(suit, CardValues.Three, this));
        }

        if ((cardValues & CardValues.Four) == CardValues.Four)
        {
            Cards.Add(new TraditionalCard(suit, CardValues.Four, this));
        }

        if ((cardValues & CardValues.Five) == CardValues.Five)
        {
            Cards.Add(new TraditionalCard(suit, CardValues.Five, this));
        }

        if ((cardValues & CardValues.Six) == CardValues.Six)
        {
            Cards.Add(new TraditionalCard(suit, CardValues.Six, this));
        }

        if ((cardValues & CardValues.Seven) == CardValues.Seven)
        {
            Cards.Add(new TraditionalCard(suit, CardValues.Seven, this));
        }

        if ((cardValues & CardValues.Eight) == CardValues.Eight)
        {
            Cards.Add(new TraditionalCard(suit, CardValues.Eight, this));
        }

        if ((cardValues & CardValues.Nine) == CardValues.Nine)
        {
            Cards.Add(new TraditionalCard(suit, CardValues.Nine, this));
        }

        if ((cardValues & CardValues.Ten) == CardValues.Ten)
        {
            Cards.Add(new TraditionalCard(suit, CardValues.Ten, this));
        }

        if ((cardValues & CardValues.Jack) == CardValues.Jack)
        {
            Cards.Add(new TraditionalCard(suit, CardValues.Jack, this));
        }

        if ((cardValues & CardValues.Queen) == CardValues.Queen)
        {
            Cards.Add(new TraditionalCard(suit, CardValues.Queen, this));
        }

        if ((cardValues & CardValues.King) == CardValues.King)
        {
            Cards.Add(new TraditionalCard(suit, CardValues.King, this));
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Shuffles the cards in the packet by randomly changing card placement.
    /// </summary>
    public void Shuffle()
    {
        List<TraditionalCard> shuffledDeck = new();

        while (Cards.Count > 0)
        {
            TraditionalCard card = Cards[_random.Next(0, Cards.Count)];
            Cards.Remove(card);
            shuffledDeck.Add(card);
        }

        Cards = shuffledDeck;
    }

    /// <summary>
    /// Removes the specified card from the packet. The first matching card
    /// will be removed.
    /// </summary>
    /// <param name="card">The card to remove.</param>
    /// <returns>The card that was removed from the collection.</returns>
    /// <remarks>
    /// Please note that removing a card from a packet may only be performed internally by
    /// other card-framework classes to maintain the principle that a card may only be held
    /// by one <see cref="CardPacket"/> only at any given time.
    /// </remarks>
    internal TraditionalCard Remove(TraditionalCard card)
    {
        if (Cards.Contains(card))
        {
            Cards.Remove(card);
            CardRemoved?.Invoke(this, new CardEventArgs(card));
            return card;
        }
        return null;
    }


    /// <summary>
    /// Removes all the cards from the collection.
    /// </summary>
    /// <returns>A list of all the cards that were removed.</returns>
    internal List<TraditionalCard> Remove()
    {
        List<TraditionalCard> cards = Cards;
        Cards = new List<TraditionalCard>();
        return cards;
    }

    /// <summary>
    /// Deals the first card from the collection to a specified hand.
    /// </summary>
    /// <param name="destinationHand">The destination hand.</param>
    /// <returns>The card that was moved to the hand.</returns>
    public TraditionalCard DealCardToHand(Hand destinationHand)
    {
        TraditionalCard firstCard = Cards[0];
        firstCard.MoveToHand(destinationHand);
        return firstCard;
    }

    /// <summary>
    /// Deals several cards to a specified hand.
    /// </summary>
    /// <param name="destinationHand">The destination hand.</param>
    /// <param name="count">The amount of cards to deal.</param>
    /// <returns>A list of the cards that were moved to the hand.</returns>
    public List<TraditionalCard> DealCardsToHand(Hand destinationHand, int count)
    {
        List<TraditionalCard> dealtCards = new();
        for (int cardIndex = 0; cardIndex < count; cardIndex++)
            dealtCards.Add(DealCardToHand(destinationHand));
        return dealtCards;
    }
    #endregion
}