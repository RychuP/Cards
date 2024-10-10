using Framework.Engine;
using System.Collections.Generic;
using System;
using Framework.Misc;
using System.Collections;
using System.Net.NetworkInformation;
using System.Threading;

namespace Poker.Gameplay.Players;

/// <summary>
/// Card dealer at the poker table.
/// </summary>
/// <remarks>Intended to replace the <see cref="CardGame.Dealer"/>, 
/// to allow getting the cards back from players.</remarks>
class Dealer : Hand
{
    PokerHand _pokerHand = PokerHand.RoyalFlush;

    // backing field
    ShufflingType _shufflingType = ShufflingType.Random;
    public ShufflingType ShufflingType
    {
        get => _shufflingType;
        set
        {
            if (_shufflingType == value) return;
            var prevShufflingType = _shufflingType;
            _shufflingType = value;
            OnShufflingTypeChange(prevShufflingType, value);
        }
    }

    public Dealer(CardPacket cardPacket)
    {
        cardPacket.DealCardsToHand(this, cardPacket.Count);
    }

    // test shuffle
    public override void Shuffle()
    {
        base.Shuffle();

        if (ShufflingType == ShufflingType.Random)
            return;

        // ordered shuffling for debugging purposes
        List<TraditionalCard> deck = new();

        switch (_pokerHand)
        {
            case PokerHand.RoyalFlush:
                InsertRoyalFlush(deck);
                break;

            case PokerHand.StraightFlush:
                InsertStraightFlush(deck);
                break;

            case PokerHand.FourOfKind:
                InsertFourOfKind(deck);
                break;

            case PokerHand.FullHouse:
                InsertFullHouse(deck);
                break;

            default:
                ShufflingType = ShufflingType.Random;
                break;
        }

        while (Cards.Count > 0)
        {
            var card = Cards[0];
            Cards.Remove(Cards[0]);
            deck.Add(card);
        }

        Cards = deck;
    }

    void InsertRoyalFlush(List<TraditionalCard> deck)
    {
        var suit = GetRandomSuit();

        AddCardToDeck(FindCard(suit, CardValues.Ace), deck);
        AddCardToDeck(FindCard(suit, CardValues.King), deck);
        AddCardToDeck(FindCard(suit, CardValues.Queen), deck);
        AddCardToDeck(FindCard(suit, CardValues.Jack), deck);
        AddCardToDeck(FindCard(suit, CardValues.Ten), deck);

        _pokerHand = PokerHand.StraightFlush;
    }

    void InsertStraightFlush(List<TraditionalCard> deck)
    {
        var suit = GetRandomSuit();

        AddCardToDeck(FindCard(suit, CardValues.Queen), deck);
        AddCardToDeck(FindCard(suit, CardValues.Jack), deck);
        AddCardToDeck(FindCard(suit, CardValues.Ten), deck);
        AddCardToDeck(FindCard(suit, CardValues.Nine), deck);
        AddCardToDeck(FindCard(suit, CardValues.Eight), deck);

        _pokerHand = PokerHand.FourOfKind;
    }

    void InsertFourOfKind(List<TraditionalCard> deck)
    {
        var value = GetRandomValue();

        AddCardToDeck(FindCard(CardSuits.Heart, value), deck);
        AddCardToDeck(FindCard(CardSuits.Club, value), deck);
        AddCardToDeck(FindCard(CardSuits.Diamond, value), deck);
        AddCardToDeck(FindCard(CardSuits.Spade, value), deck);

        _pokerHand = PokerHand.FullHouse;
    }

    void InsertFullHouse(List<TraditionalCard> deck)
    {
        AddCardToDeck(FindCard(CardSuits.Club, CardValues.Eight), deck);
        AddCardToDeck(FindCard(CardSuits.Spade, CardValues.Eight), deck);
        AddCardToDeck(FindCard(CardSuits.Diamond, CardValues.Jack), deck);
        AddCardToDeck(FindCard(CardSuits.Heart, CardValues.Jack), deck);
        AddCardToDeck(FindCard(CardSuits.Club, CardValues.Jack), deck);

        _pokerHand = PokerHand.Flush;
    }

    void AddCardToDeck(TraditionalCard card, List<TraditionalCard> deck)
    {
        Cards.Remove(card);
        deck.Add(card);
    }

    static CardSuits GetRandomSuit()
    {
        var rand = new Random();
        return rand.Next(4) switch
        {
            0 => CardSuits.Heart,
            1 => CardSuits.Club,
            2 => CardSuits.Diamond,
            _ => CardSuits.Spade
        };
    }

    static CardValues GetRandomValue()
    {
        var rand = new Random();
        return rand.Next(13) switch
        {
            0 => CardValues.Ace,
            1 => CardValues.Two,
            2 => CardValues.Three,
            3 => CardValues.Four,
            4 => CardValues.Five,
            5 => CardValues.Six,
            6 => CardValues.Seven,
            7 => CardValues.Eight,
            8 => CardValues.Nine,
            9 => CardValues.Ten,
            10 => CardValues.Jack,
            11 => CardValues.Queen,
            _ => CardValues.King
        };
    }

    TraditionalCard FindCard(CardSuits suit, CardValues value)
    {
        foreach (TraditionalCard card in Cards)
            if (card.Type == suit && card.Value == value)
                return card;
        return null;
    }

    void OnShufflingTypeChange(ShufflingType prevShufflingType, ShufflingType newShufflingType)
    {
        if (newShufflingType == ShufflingType.Ordered)
        {
            _pokerHand = PokerHand.RoyalFlush;
        }
    }
}