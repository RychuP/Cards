using Framework.Engine;
using Poker.Gameplay.Players;
using System.Collections.Generic;
using System;
using Framework.Misc;

namespace Poker.Gameplay;

class Evaluator
{
    public GameManager GameManager { get; }
    public Game Game => GameManager.Game;

    public Evaluator(GameManager gameManager)
    {
        GameManager = gameManager;
    }

    public PokerBettingPlayer GetWinner()
    {
        throw new NotImplementedException();
    }

    public bool HasRoyalFlush(Hand hand, Dealer dealer)
    {
        throw new NotImplementedException();
    }

    

    public bool HasPair(Hand hand, Hand dealer, out List<TraditionalCard> pair)
    {
        throw new NotImplementedException();
    }

    public static bool IsFullHouse(TraditionalCard c1, TraditionalCard c2, TraditionalCard c3,
        TraditionalCard c4, TraditionalCard c5) =>
        (IsPair(c1, c2) && IsThreeOfKind(c3, c4, c5)) ||
        (IsPair(c1, c3) && IsThreeOfKind(c2, c4, c5)) ||
        (IsPair(c1, c4) && IsThreeOfKind(c3, c2, c5)) ||
        (IsPair(c1, c5) && IsThreeOfKind(c3, c4, c2)) ||

        (IsPair(c2, c3) && IsThreeOfKind(c1, c4, c5)) ||
        (IsPair(c2, c4) && IsThreeOfKind(c1, c3, c5)) ||
        (IsPair(c2, c5) && IsThreeOfKind(c1, c3, c4)) ||

        (IsPair(c3, c4) && IsThreeOfKind(c1, c2, c5)) ||
        (IsPair(c3, c5) && IsThreeOfKind(c1, c2, c4)) ||

        (IsPair(c4, c5) && IsThreeOfKind(c1, c2, c3));

    public static bool IsRoyalFlush(TraditionalCard c1, TraditionalCard c2, TraditionalCard c3,
        TraditionalCard c4, TraditionalCard c5) =>
        (c1.Value == CardValues.Ace || c2.Value == CardValues.Ace || c3.Value == CardValues.Ace ||
        c4.Value == CardValues.Ace || c5.Value == CardValues.Ace) &&
        IsStraightFlush(c1, c2, c3, c4, c5);

    public static bool IsStraightFlush(TraditionalCard c1, TraditionalCard c2, TraditionalCard c3,
        TraditionalCard c4, TraditionalCard c5) =>
        IsFlush(c1, c2, c3, c4, c5) && IsStraight(c1, c2, c3, c4, c5);


    public static bool IsStraight(TraditionalCard c1, TraditionalCard c2, TraditionalCard c3,
        TraditionalCard c4, TraditionalCard c5)
    {
        TraditionalCard[] cards = {c1, c2, c3, c4, c5};
        Array.Sort(cards);
        return
            GetNextValue(cards[0].Value) == cards[1].Value &&
            GetNextValue(cards[1].Value) == cards[2].Value &&
            GetNextValue(cards[2].Value) == cards[3].Value &&
            GetNextValue(cards[3].Value) == cards[4].Value;
    }

    public bool IsTwoPair(TraditionalCard c1, TraditionalCard c2, TraditionalCard c3, TraditionalCard c4) =>
        (c1.Value == c2.Value && c3.Value == c4.Value) ||
        (c1.Value == c3.Value && c2.Value == c4.Value) ||
        (c1.Value == c4.Value && c2.Value == c3.Value) ||
        (c2.Value == c3.Value && c1.Value == c4.Value);

    public static bool IsFlush(TraditionalCard c1, TraditionalCard c2, TraditionalCard c3, 
        TraditionalCard c4, TraditionalCard c5) => 
        c1.Type == c2.Type && 
        c2.Type == c3.Type &&
        c3.Type == c4.Type &&
        c4.Type == c5.Type;

    public static bool IsFourOfKind(TraditionalCard c1, TraditionalCard c2, 
        TraditionalCard c3, TraditionalCard c4) =>
        c1.Value == c2.Value &&
        c2.Value == c3.Value &&
        c3.Value == c4.Value;

    public static bool IsThreeOfKind(TraditionalCard c1, TraditionalCard c2, TraditionalCard c3) =>
        c1.Value == c2.Value &&
        c2.Value == c3.Value;

    public static bool IsPair(TraditionalCard c1, TraditionalCard c2) =>
        c1.Value == c2.Value;

    bool ContainsCard(Hand hand, TraditionalCard card)
    {
        for (int i = 0; i < hand.Count; i++)
        {
            if (hand[i] == card)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Considers Ace as the highest card and returns a joker as the card following Two.
    /// </summary>
    static CardValues GetNextValue(CardValues cardValue)
    {
        if (cardValue == CardValues.Ace)
            return CardValues.King;
        else if (cardValue == CardValues.Two)
            return CardValues.FirstJoker;
        else
            return (CardValues)((int)cardValue >> 1);
    }
}