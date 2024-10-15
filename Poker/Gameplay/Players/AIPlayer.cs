using Framework.Engine;
using Framework.Misc;
using System;

namespace Poker.Gameplay.Players;

class AIPlayer : PokerBettingPlayer
{
    static readonly int MaxCardValue = 14;

    /// <summary>
    /// 0 super safe, 1 very risky play style.
    /// </summary>
    readonly double _riskTakingTendency;

    readonly Random _rand;

    public AIPlayer(string name, Gender gender, int place, GameManager gm) : base(name, gender, place, gm)
    {
        _rand = new Random();
        _riskTakingTendency = (double)_rand.NextDouble();
    }

    /// <inheritdoc/>
    public override void TakeTurn(int currentBetAmount, Hand communityCards, bool checkPossible)
    {
        base.TakeTurn(currentBetAmount, communityCards, checkPossible);

        // establish which betting round is on now
        GameState gameState = 
            communityCards.Count == 0 ? GameState.Preflop :
            communityCards.Count == 3 ? GameState.FlopBet :
            communityCards.Count == 4 ? GameState.TurnBet : 
            GameState.RiverBet;

        // grab reference of both cards held
        TraditionalCard c1 = Hand[0];
        int c1Val = GetCardValue(c1);
        double c1ValFactor = c1Val / MaxCardValue;
        TraditionalCard c2 = Hand[1];
        int c2Val = GetCardValue(c2);
        double c2ValFactor = c2Val / MaxCardValue;

        // behavior variables
        bool feelingLucky = _rand.NextDouble() > 0.6d;
        double confidence = 0;
        double riskFactor = 0;

        // calculate confidence of a good outcome
        switch (gameState)
        {
            case GameState.Preflop:
                // pair in hand
                if (c1 == c2)
                {
                    confidence += 0.5f;
                    
                    // card value factor
                    riskFactor = c1ValFactor / 2;

                    // same color factor
                    if (c1.Type != c2.Type)
                        riskFactor *= 0.75f;

                    confidence += riskFactor;
                }
                // high card in hand
                else
                {
                    riskFactor += c1ValFactor / 2 + c2ValFactor / 2;
                    if (c1.Type != c2.Type)
                        riskFactor *= 0.75f;
                    confidence += riskFactor;
                }
                break;

            default:
                confidence = _rand.NextDouble();
                break;
        }
        confidence *= _riskTakingTendency;



            //if (gameState == GameState.FlopBet)
        //if ((confidence + (feelingLucky ? 0.5 : -0.5)) < 0.2 &&
        //    gameState != GameState.RiverBet)
        if (_rand.Next(10) > 7 && gameState != GameState.RiverBet)
        {
            Fold();
        }
        else if (checkPossible)
        {
            if (confidence > 0.6 || feelingLucky)
            {
                int raiseAmount = GetRaiseAmount(confidence, currentBetAmount);
                if (raiseAmount <= currentBetAmount)
                    throw new Exception("Incorrent raise amount generated.");
                Raise(raiseAmount);
            }
            else
                Check();
        }
        // equals sign for the case of the big blind rule
        else if (currentBetAmount <= (BetAmount + Balance))
        {
            if ((confidence > 0.6 || feelingLucky) &&
                (currentBetAmount + 5 <= (BetAmount + Balance)))
            {
                int raiseAmount = GetRaiseAmount(confidence, currentBetAmount);
                if (raiseAmount <= currentBetAmount)
                    throw new Exception("Incorrent raise amount generated.");
                Raise(raiseAmount);
            }
            else
                Call(currentBetAmount);
        }
        else
        {
            if (confidence > 0.6 || feelingLucky)
            {
                AllIn();
            }
            else
                Fold();
        }
    }

    int GetRaiseAmount(double confidence, int currentBet)
    {
        double maxBet = Balance * _rand.NextDouble();
        maxBet *= confidence;
        var diff = maxBet % 5;
        maxBet -= diff;
        int betAmount = (int)maxBet + currentBet;
        betAmount = Math.Max(currentBet + 5, betAmount);
        return betAmount;
    }

    int GetCardValue(TraditionalCard card)
    {
        int value = GameManager.GetCardValue(card);
        return value == 1 ? 14: value;
    }
}