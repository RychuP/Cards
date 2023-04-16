#region File Description
//-----------------------------------------------------------------------------
// InsuranceRule.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using CardsFramework;

namespace Blackjack;

/// <summary>
/// Represents a rule which checks if the human player can use insurance
/// </summary>
class InsuranceRule : GameRule
{
    readonly Hand _dealerHand;
    bool _done = false;

    /// <summary>
    /// Creates a new instance of the <see cref="InsuranceRule"/> class.
    /// </summary>
    /// <param name="dealerHand">The dealer's hand.</param>
    public InsuranceRule(Hand dealerHand) =>
        _dealerHand = dealerHand;

    /// <summary>
    /// Checks whether or not the dealer's revealed card is an ace.
    /// </summary>
    public override void Check()
    {
        if (!_done && _dealerHand.Count > 0)
        {
            if (_dealerHand[0].Value == CardValue.Ace)
                FireRuleMatch(EventArgs.Empty);

            _done = true;
        }
    }
}