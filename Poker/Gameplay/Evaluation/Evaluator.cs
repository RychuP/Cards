using Framework.Engine;
using Poker.Gameplay.Players;
using System.Collections.Generic;
using System;
using Framework.Misc;

namespace Poker.Gameplay.Evaluation;
delegate bool EvaluateCards(TraditionalCard[] cards, Hand hand, out TraditionalCard[] result);

class Evaluator
{
    #region Predefined lookup tables
    readonly static int[,] s_twoOutOfFiveCombinations =
    {
        {0, 1},
        {0, 2},
        {0, 3},
        {0, 4},
        {1, 2},
        {1, 3},
        {1, 4},
        {2, 3},
        {2, 4},
        {3, 4}
    };

    readonly static int[,] s_threeOutOfFiveCombinations = {
        {0, 1, 2},
        {0, 1, 3},
        {0, 1, 4},
        {0, 2, 3},
        {0, 2, 4},
        {0, 3, 4},
        {1, 2, 3},
        {1, 2, 4},
        {1, 3, 4},
        {2, 3, 4}
    };
    readonly static int[,] s_fourOutOfFiveCombinations =
    {
        {0, 1, 2, 3},
        {0, 1, 2, 4},
        {0, 1, 3, 4},
        {0, 2, 3, 4},
        {1, 2, 3, 4}
    };
    #endregion

    public static Dictionary<PokerHand, string> PokerHands = new()
    {
        { PokerHand.HighCard, "High Card" },
        { PokerHand.OnePair, "One Pair" },
        { PokerHand.TwoPair, "Two Pairs" },
        { PokerHand.ThreeOfKind, "Three-of-a-kind" },
        { PokerHand.Straight, "Straight" },
        { PokerHand.Flush, "Flush" },
        { PokerHand.FullHouse, "Full House" },
        { PokerHand.FourOfKind, "Four-of-a-kind" },
        { PokerHand.StraightFlush, "Straight Flush" },
        { PokerHand.RoyalFlush, "Royal Flush" }
    };

    /// <summary>
    /// Looks for the strongest combination of the player cards and the community cards.
    /// </summary>
    /// <param name="player">Player to check the cards of.</param>
    /// <param name="communityCards">Community cards.</param>
    /// <param name="result">Array of cards that hold the strongest combination of cards.</param>
    /// <returns></returns>
    public static PokerHand CheckHand(PokerBettingPlayer player, CommunityCards communityCards,
        out TraditionalCard[] result)
    {
        // 1. first check if the hand is a flush
        List<TraditionalCard[]> flushMatches;
        if (CheckAllCardCombinations(IsFlush, player.Hand, communityCards.Hand, out flushMatches))
        {
            // 2. check flush matches to see if any of those is a straight
            List<TraditionalCard[]> straightFlushMatches = new();
            foreach (var flush in flushMatches)
            {
                if (IsStraight(flush))
                {
                    straightFlushMatches.Add(flush);
                }
            }

            if (straightFlushMatches.Count > 0)
            {
                // 3. check straight flush matches if any of those is a royal flush
                foreach (var straightFlush in straightFlushMatches)
                {
                    // if the first card is an ace, we have a royal flush
                    if (straightFlush[0].Value == CardValues.Ace)
                    {
                        result = straightFlush;
                        return PokerHand.RoyalFlush;
                    }
                }

                // since straight flush is the second highest hand,
                // there is no need for any further checks
                result = GetArrayWithHighestValueFirstCard(straightFlushMatches);
                return PokerHand.StraightFlush;
            }
        }

        List<TraditionalCard[]> matches;

        // 4. check for the four of a kind
        if (CheckAllCardCombinations(IsFourOfKind, player.Hand, communityCards.Hand, out matches))
        {
            result = GetArrayWithHighestValueFirstCard(matches);
            return PokerHand.FourOfKind;
        }

        // 5. check for full house
        if (CheckAllCardCombinations(IsFullHouse, player.Hand, communityCards.Hand, out matches))
        {
            result = matches[0];    // needs sorter!
            return PokerHand.FullHouse;
        }

        // 6. next hand is flush
        if (flushMatches.Count > 0)
        {
            result = flushMatches[0];
            return PokerHand.Flush;
        }

        // 7. check for straight
        if (CheckAllCardCombinations(IsStraight, player.Hand, communityCards.Hand, out matches))
        {
            result = GetArrayWithHighestValueFirstCard(matches);
            return PokerHand.Straight;
        }

        // 8. check for three of a kind
        if (CheckAllCardCombinations(IsThreeOfKind, player.Hand, communityCards.Hand, out matches))
        {
            result = GetArrayWithHighestValueFirstCard(matches);
            return PokerHand.ThreeOfKind;
        }

        // 9. check for two pairs
        if (CheckAllCardCombinations(IsTwoPair, player.Hand, communityCards.Hand, out matches))
        {
            result = matches[0]; // needs sorter!
            return PokerHand.TwoPair;
        }

        // 10. check for single pairs
        if (CheckAllCardCombinations(IsPair, player.Hand, communityCards.Hand, out matches))
        {
            result = GetArrayWithHighestValueFirstCard(matches);
            return PokerHand.OnePair;
        }

        result = Array.Empty<TraditionalCard>();
        return PokerHand.HighCard;
    }

    /// <summary>
    /// Checks all possible combinations of player cards with the community cards.
    /// </summary>
    /// <param name="evaluator"><see cref="EvaluateCards"/> delegate to check the cards with.</param>
    /// <param name="player">Player hand.</param>
    /// <param name="communityCards">Community cards hand.</param>
    /// <param name="result">Multidimensional array of card combinations. 
    /// Second dimension is the 5 cards that match the evaluator.</param>
    /// <returns>True if matches found.</returns>
    /// <remarks>It's down to the evaluator delegate to eliminate results
    /// that don't involve player cards.</remarks>
    static bool CheckAllCardCombinations(EvaluateCards evaluator, Hand player, Hand communityCards,
        out List<TraditionalCard[]> results)
    {
        bool foundMatches = false;
        results = new List<TraditionalCard[]>();

        // check combinations of both player cards and three community cards
        for (int i = 0; i < s_threeOutOfFiveCombinations.GetLength(0); i++)
        {
            int x = s_threeOutOfFiveCombinations[i, 0];
            int y = s_threeOutOfFiveCombinations[i, 1];
            int z = s_threeOutOfFiveCombinations[i, 2];
            TraditionalCard[] cards = { player[0], player[1], communityCards[x],
                communityCards[y], communityCards[z] };

            if (evaluator(cards, player, out TraditionalCard[] match))
            {
                foundMatches = true;
                results.Add(match);
            }
        }

        // check combinations of one player card and four community cards
        for (int p = 0; p < 2; p++)
        {
            for (int i = 0; i < s_fourOutOfFiveCombinations.GetLength(0); i++)
            {
                int x = s_fourOutOfFiveCombinations[i, 0];
                int y = s_fourOutOfFiveCombinations[i, 1];
                int z = s_fourOutOfFiveCombinations[i, 2];
                int t = s_fourOutOfFiveCombinations[i, 3];
                TraditionalCard[] cards = { player[p], communityCards[x], communityCards[y],
                    communityCards[z], communityCards[t] };

                if (evaluator(cards, player, out TraditionalCard[] match))
                {
                    foundMatches = true;
                    results.Add(match);
                }
            }
        }

        return foundMatches;
    }

    #region Full house
    static bool IsFullHouse(TraditionalCard[] cards, Hand player, out TraditionalCard[] result)
    {
        if (cards.Length != 5)
            throw new ArgumentException(Constants.EvaluatorCardArrayLengthException);

        if (IsFullHouse(cards))
        {
            result = cards;
            return true;
        }

        result = Array.Empty<TraditionalCard>();
        return false;
    }

    static bool IsFullHouse(TraditionalCard c1, TraditionalCard c2, TraditionalCard c3,
        TraditionalCard c4, TraditionalCard c5) =>
        IsPair(c1, c2) && IsThreeOfKind(c3, c4, c5) ||
        IsPair(c1, c3) && IsThreeOfKind(c2, c4, c5) ||
        IsPair(c1, c4) && IsThreeOfKind(c3, c2, c5) ||
        IsPair(c1, c5) && IsThreeOfKind(c3, c4, c2) ||

        IsPair(c2, c3) && IsThreeOfKind(c1, c4, c5) ||
        IsPair(c2, c4) && IsThreeOfKind(c1, c3, c5) ||
        IsPair(c2, c5) && IsThreeOfKind(c1, c3, c4) ||

        IsPair(c3, c4) && IsThreeOfKind(c1, c2, c5) ||
        IsPair(c3, c5) && IsThreeOfKind(c1, c2, c4) ||

        IsPair(c4, c5) && IsThreeOfKind(c1, c2, c3);

    static bool IsFullHouse(TraditionalCard[] cards) =>
        IsPair(cards[0], cards[1]) && IsThreeOfKind(cards[2], cards[3], cards[4]) ||
        IsPair(cards[0], cards[2]) && IsThreeOfKind(cards[1], cards[3], cards[4]) ||
        IsPair(cards[0], cards[3]) && IsThreeOfKind(cards[2], cards[1], cards[4]) ||
        IsPair(cards[0], cards[4]) && IsThreeOfKind(cards[2], cards[3], cards[1]) ||

        IsPair(cards[1], cards[2]) && IsThreeOfKind(cards[0], cards[3], cards[4]) ||
        IsPair(cards[1], cards[3]) && IsThreeOfKind(cards[0], cards[2], cards[4]) ||
        IsPair(cards[1], cards[4]) && IsThreeOfKind(cards[0], cards[2], cards[3]) ||

        IsPair(cards[2], cards[3]) && IsThreeOfKind(cards[0], cards[1], cards[4]) ||
        IsPair(cards[2], cards[4]) && IsThreeOfKind(cards[0], cards[1], cards[3]) ||

        IsPair(cards[3], cards[4]) && IsThreeOfKind(cards[0], cards[1], cards[2]);
    #endregion

    #region Straight
    static bool IsStraight(TraditionalCard[] cards, Hand player, out TraditionalCard[] result)
    {
        if (cards.Length != 5)
            throw new ArgumentException(Constants.EvaluatorCardArrayLengthException);

        if (IsStraight(cards))
        {
            result = cards;
            return true;
        }

        result = Array.Empty<TraditionalCard>();
        return false;
    }

    static bool IsStraight(TraditionalCard[] cards)
    {
        Array.Sort(cards);
        return
            GetNextValue(cards[0].Value) == cards[1].Value &&
            GetNextValue(cards[1].Value) == cards[2].Value &&
            GetNextValue(cards[2].Value) == cards[3].Value &&
            GetNextValue(cards[3].Value) == cards[4].Value;
    }
    #endregion

    #region Flush
    static bool IsFlush(TraditionalCard[] cards, Hand player, out TraditionalCard[] result)
    {
        if (cards.Length != 5)
            throw new ArgumentException(Constants.EvaluatorCardArrayLengthException);

        if (IsFlush(cards))
        {
            result = cards;
            return true;
        }

        result = Array.Empty<TraditionalCard>();
        return false;
    }

    static bool IsFlush(TraditionalCard c1, TraditionalCard c2,
        TraditionalCard c3, TraditionalCard c4, TraditionalCard c5) =>
        c1.Type == c2.Type &&
        c2.Type == c3.Type &&
        c3.Type == c4.Type &&
        c4.Type == c5.Type;

    static bool IsFlush(TraditionalCard[] cards) =>
        cards[0].Type == cards[1].Type &&
        cards[1].Type == cards[2].Type &&
        cards[2].Type == cards[3].Type &&
        cards[3].Type == cards[4].Type;
    #endregion

    #region Four of a kind
    static bool IsFourOfKind(TraditionalCard[] cards, Hand player, out TraditionalCard[] result)
    {
        if (cards.Length != 5)
            throw new ArgumentException(Constants.EvaluatorCardArrayLengthException);

        var indices = s_fourOutOfFiveCombinations;
        for (int i = 0; i < indices.GetLength(0); i++)
        {
            // create a temporary array with the cards to check
            var temp = new TraditionalCard[] { cards[indices[i, 0]], cards[indices[i, 1]],
                cards[indices[i, 2]], cards[indices[i, 3]] };

            // eliminate temps that don't include player cards
            if (!HasCards(temp, player))
            {
                continue;
            }
            else if (IsFourOfKind(temp))
            {
                result = temp;
                return true;
            }
        }

        result = Array.Empty<TraditionalCard>();
        return false;
    }

    static bool IsFourOfKind(TraditionalCard[] cards) =>
        cards[0].Value == cards[1].Value &&
        cards[1].Value == cards[2].Value &&
        cards[2].Value == cards[3].Value;
    #endregion

    #region Three of a kind
    static bool IsThreeOfKind(TraditionalCard[] cards, Hand player, out TraditionalCard[] result)
    {
        if (cards.Length != 5)
            throw new ArgumentException(Constants.EvaluatorCardArrayLengthException);

        var indices = s_threeOutOfFiveCombinations;
        for (int i = 0; i < indices.GetLength(0); i++)
        {
            // create a temporary array with the cards to check
            var temp = new TraditionalCard[] { cards[indices[i, 0]], cards[indices[i, 1]],
                cards[indices[i, 2]] };

            // eliminate temps that don't include player cards
            if (!HasCards(temp, player))
            {
                continue;
            }
            else if (IsThreeOfKind(temp))
            {
                result = temp;
                return true;
            }
        }

        result = Array.Empty<TraditionalCard>();
        return false;
    }

    static bool IsThreeOfKind(TraditionalCard[] cards) =>
        cards[0].Value == cards[1].Value &&
        cards[1].Value == cards[2].Value;

    static bool IsThreeOfKind(TraditionalCard c1, TraditionalCard c2, TraditionalCard c3) =>
        c1.Value == c2.Value &&
        c2.Value == c3.Value;
    #endregion

    #region Two pair
    static bool IsTwoPair(TraditionalCard[] cards, Hand player, out TraditionalCard[] result)
    {
        if (cards.Length != 5)
            throw new ArgumentException(Constants.EvaluatorCardArrayLengthException);

        var indices = s_fourOutOfFiveCombinations;
        for (int i = 0; i < indices.GetLength(0); i++)
        {
            // create a temporary array with the cards to check
            var temp = new TraditionalCard[] { cards[indices[i, 0]], cards[indices[i, 1]],
                cards[indices[i, 2]], cards[indices[i, 3]] };

            // eliminate temps that don't include player cards
            if (!HasCards(temp, player))
            {
                continue;
            }
            else if (IsTwoPair(temp))
            {
                result = temp;
                return true;
            }
        }

        result = Array.Empty<TraditionalCard>();
        return false;
    }

    static bool IsTwoPair(TraditionalCard c1, TraditionalCard c2, TraditionalCard c3,
        TraditionalCard c4) =>
        c1.Value == c2.Value && c3.Value == c4.Value ||
        c1.Value == c3.Value && c2.Value == c4.Value ||
        c1.Value == c4.Value && c2.Value == c3.Value ||
        c2.Value == c3.Value && c1.Value == c4.Value;

    static bool IsTwoPair(TraditionalCard[] cards) =>
        cards[0].Value == cards[1].Value && cards[2].Value == cards[3].Value ||
        cards[0].Value == cards[2].Value && cards[1].Value == cards[3].Value ||
        cards[0].Value == cards[3].Value && cards[1].Value == cards[2].Value ||
        cards[1].Value == cards[2].Value && cards[0].Value == cards[3].Value;
    #endregion

    #region One pair
    static bool IsPair(TraditionalCard[] cards, Hand player, out TraditionalCard[] result)
    {
        if (cards.Length != 5)
            throw new ArgumentException(Constants.EvaluatorCardArrayLengthException);

        var indices = s_twoOutOfFiveCombinations;
        for (int i = 0; i < indices.GetLength(0); i++)
        {
            // pairs to check
            TraditionalCard c1 = cards[indices[i, 0]];
            TraditionalCard c2 = cards[indices[i, 1]];
            var temp = new TraditionalCard[] { c1, c2 };

            // eliminate pairs that don't invole player cards
            if (!HasCards(temp, player))
            {
                continue;
            }
            else if (IsPair(c1, c2))
            {
                result = temp;
                return true;
            }
        }

        result = Array.Empty<TraditionalCard>();
        return false;
    }
    static bool IsPair(TraditionalCard c1, TraditionalCard c2) =>
        c1.Value == c2.Value;

    static bool IsPair(TraditionalCard[] cards) =>
        cards[0].Value == cards[1].Value;
    #endregion

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

    /// <summary>
    /// Checks if the hand owns the card.
    /// </summary>
    /// <param name="card">Card to check the ownership of.</param>
    /// <param name="hand">Hand to compare the card with.</param>
    /// <returns></returns>
    static bool HasCard(TraditionalCard card, Hand hand) =>
        hand[0] == card || hand[1] == card;

    /// <summary>
    /// Checks if the hand owns any of the given cards.
    /// </summary>
    /// <param name="cards">Cards to check the ownership of.</param>
    /// <param name="hand">Hand to compare the cards with.</param>
    /// <returns></returns>
    static bool HasCards(TraditionalCard[] cards, Hand hand)
    {
        foreach (var card in cards)
        {
            if (HasCard(card, hand))
                return true;
        }
        return false;
    }

    static TraditionalCard[] GetArrayWithHighestValueFirstCard(List<TraditionalCard[]> matches)
    {
        if (matches.Count == 0)
            throw new ArgumentException("Can't work with empty list.");

        else if (matches.Count == 1)
            return matches[0];

        else
        {
            matches.Sort(new CardValueArrayComparer());
            return matches[0];
        }
    }
}