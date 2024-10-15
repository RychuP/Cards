using Framework.Engine;
using Framework.Misc;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Poker.Gameplay.Players;

internal class Result
{
    /// <summary>
    /// Type of the poker hand.
    /// </summary>
    public PokerHand HandType { get; }

    /// <summary>
    /// Cards that form the poker hand (maybe none if hand type was HighCard)
    /// </summary>
    public TraditionalCard[] Cards { get; }

    /// <summary>
    /// Player that owns the result.
    /// </summary>
    public PokerBettingPlayer Player { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handType">Poker hand type established during evaluation.</param>
    /// <param name="cards">Cards that form the poker hand.</param>
    public Result(PokerHand handType, TraditionalCard[] cards, PokerBettingPlayer player)
    {
        Player = player;
        HandType = handType;
        Cards = cards;
    }

    public static bool operator >(Result firstResult, Result secondResult)
    {
        if (secondResult is null)
            return true;
        else if (firstResult is null)
            return false;
        else if (firstResult ==  secondResult)
            return false;

        if (firstResult.HandType > secondResult.HandType)
            return true;
        else if (firstResult.HandType < secondResult.HandType)
            return false;

        // hands are equal -> compare player cards
        else
        {
            int result = firstResult.Player.BetterCard.CompareTo(secondResult.Player.BetterCard);

            // first result better player card is stronger than the second resul better player card
            if (result < 0)
                return true;
            // is worse
            else if (result > 0)
                return false;
            // is equal
            else
            {
                result = firstResult.Player.WorseCard.CompareTo(secondResult.Player.WorseCard);
                // worse card is better than the other worse card
                if (result < 0)
                    return true;
                // is worse
                else if (result > 0)
                    return false;
                // is equal
                else
                {
                    return false;
                }
            }
        }
    }

    public static bool operator <(Result firstResult, Result secondResult) =>
        secondResult > firstResult;

    public static bool operator ==(Result firstResult, Result secondResult)
    {
        if (firstResult is null && secondResult is null) return true;
        else if (firstResult is null || secondResult is null) return false;

        // compare hand types 
        bool handTypesAreEqual = firstResult.HandType == secondResult.HandType;

        // compare better cards
        int result = firstResult.Player.BetterCard.CompareTo(secondResult.Player.BetterCard);
        bool betterCardsAreEqual = result == 0;

        // compare worse cards
        result = firstResult.Player.WorseCard.CompareTo(secondResult.Player.WorseCard);
        bool worseCardsAreEqual = result == 0;

        if (handTypesAreEqual && betterCardsAreEqual && worseCardsAreEqual)
            return true;
        else
            return false;
    }

    public static bool operator !=(Result firstResult, Result secondResult) =>
        !(firstResult == secondResult);

    public override bool Equals([NotNullWhen(true)] object obj)
    {
        if (obj is null || obj is not Result result)
            return false;
        else
            return result == this;
    }

    public override int GetHashCode() =>
        HandType.GetHashCode() ^ Cards.GetHashCode() ^ Player.GetHashCode();
}