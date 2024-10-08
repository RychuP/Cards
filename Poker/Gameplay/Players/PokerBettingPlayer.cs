using Framework.Engine;
using Framework.UI;
using Poker.Gameplay.Chips;
using Poker.UI.AnimatedGameComponents;
using System;
using System.Collections.Generic;

namespace Poker.Gameplay.Players;

class PokerBettingPlayer : PokerCardsHolder
{
    public static readonly int StartBalance = 2000;

    /// <summary>
    /// Max amount of value chips that can be added to the player area.
    /// </summary>
    public static readonly int ChipMaxAmount = 20;

    /// <summary>
    /// Position of the first line of text in the player area.
    /// </summary>
    /// <remarks>Normally, it's where the player name is displayed.</remarks>
    readonly Vector2 _textPosition;

    /// <summary>
    /// How much money the player owns.
    /// </summary>
    public int Balance { get; set; }

    /// <summary>
    /// Gender of the player.
    /// </summary>
    public Gender Gender { get; }

    /// <summary>
    /// Center aligned position for the player name.
    /// </summary>
    public Vector2 NamePosition { get; }

    /// <summary>
    /// Position of the first betting chip next to the cards in the player area.
    /// </summary>
    public Vector2 ChipPosition { get; }

    /// <summary>
    /// Position of the blind chip in the player area.
    /// </summary>
    public Vector2 BlindChipPosition { get; }

    /// <summary>
    /// Index of the player's location on the table.
    /// </summary>
    public int Place {  get; }

    /// <summary>
    /// Current state of the player in the game.
    /// </summary>
    public PlayerState State { get; set; }

    /// <summary>
    /// Chips equivalent to the value of <see cref="BetAmount"/>.
    /// </summary>
    readonly List<ValueChip> Chips = new();

    // backing field
    BlindChip _blindChip;
    /// <summary>
    /// Blind chip that signifies whether this player should make a blind bet
    /// during the first bet stage.
    /// </summary>
    /// <remarks>This property is nullable.</remarks>
    public BlindChip BlindChip
    {
        get => _blindChip;
        set
        {
            if (BlindChip == value) return;
            var prevBlindChip = BlindChip;
            _blindChip = value;
            OnBlindChipChanged(prevBlindChip, value);
        }
    }

    // backing field
    int _betAmount;
    /// <summary>
    /// Sum being bet in the current round.
    /// </summary>
    public int BetAmount
    {
        get => _betAmount;
        set
        {
            if (_betAmount == value) return;
            int prevBetAmount = _betAmount;
            _betAmount = value;
            OnBetAmountChanged(prevBetAmount, value);
        }
    }

    public PokerBettingPlayer(string name, Gender gender, int place, GameManager gm)
        : base(name, gm)
    {
        AnimatedHand = new AnimatedPlayerHand(place, Hand, gm);
        Gender = gender;
        Place = place;

        // calculate the first line of text position
        var stringSize = gm.Font.MeasureString(Name);
        int verticalOffset = IsAtTheBottom ?
            -Constants.PlayerTextVerticalPadding - (int)stringSize.Y :
            Constants.CardSize.Y + Constants.PlayerTextVerticalPadding;
        _textPosition = gm.GameTable[Place] + new Vector2(0, verticalOffset);

        // calculate name position in the player area
        NamePosition = GetCenteredTextPosition(Name, 0);

        // calculate chip position
        int offsetY = (Constants.CardSize.Y - Chip.Size.Y) / 2;
        int offsetX = IsOnTheLeft ?
            Constants.CardSize.X * 2 + Constants.PlayerCardPadding + Chip.PlayerAreaPadding :
            -Chip.Size.X - Chip.PlayerAreaPadding;
        ChipPosition = gm.GameTable[Place] + new Vector2(offsetX, offsetY);

        // calculate blind chip position
        offsetY = IsAtTheBottom ? -BlindChip.OffsetY - Chip.Size.Y : BlindChip.OffsetY + Chip.Size.Y;
        BlindChipPosition = ChipPosition + new Vector2(0, offsetY);
    }

    /// <summary>
    /// Returns centered text position at the given line number (0 based index).
    /// </summary>
    /// <param name="text">Text to return the position for.</param>
    /// <param name="lineNumber">Zero based line number.</param>
    /// <returns></returns>
    public Vector2 GetCenteredTextPosition(string text, int lineNumber)
    {
        var stringSize = CardGame.Font.MeasureString(text);
        int horizontalOffset = (Constants.PlayerAreaWidth - (int)stringSize.X) / 2;
        int verticalOffset = IsAtTheBottom ?
            -Constants.PlayerTextVerticalPadding - (int)stringSize.Y :
            Constants.PlayerTextVerticalPadding + (int)stringSize.Y;
        var textPos = _textPosition + new Vector2(horizontalOffset, verticalOffset * lineNumber);
        bool test = textPos.X == NamePosition.X;
        return textPos;
    }

    /// <summary>
    /// Player's position is on the left of the screen.
    /// </summary>
    public bool IsOnTheLeft => Place == 0 || Place == 1;

    /// <summary>
    /// Player's position is on the right of the screen.
    /// </summary>
    public bool IsOnTheRight => Place == 2 || Place == 3;

    /// <summary>
    /// Player's position is at the top of the screen.
    /// </summary>
    public bool IsAtTheTop => Place == 1 || Place == 2;

    /// <summary>
    /// Player's position is at the bottom of the screen.
    /// </summary>
    public bool IsAtTheBottom => Place == 0 || Place == 3;

    public void Reset()
    {
        State = PlayerState.Waiting;
        ResetBalance();
        ReturnCardsToDealer();

        if (Chips.Count > 0)
            ResetChips();
    }

    public void StartNewRound()
    {
        State = PlayerState.Waiting;
        BetAmount = 0;
        ReturnCardsToDealer();
    }

    public void ResetBalance()
    {
        Balance = StartBalance;
        BetAmount = 0;
    }

    /// <summary>
    /// Removes all animated chip components and clears Chips.
    /// </summary>
    void ResetChips()
    {
        foreach (var chip in Chips)
            chip.RemoveAnimatedComponent();
        Chips.Clear();
    }

    void AddChips(int value)
    {
        // check if the bet amount doesn't exceed the max amount of chips that can be displayed
        int biggestChipValue = ValueChip.Values[ValueChip.Count - 1];
        int maxValue = biggestChipValue * ChipMaxAmount;
        if (BetAmount > maxValue)
            value = maxValue;
        
        // get the list of values that chips will created from
        var chipValues = GetChipValues(value);
        if (Chips.Count + chipValues.Count > ChipMaxAmount)
        {
            ResetChips();
            AddChips(BetAmount);
        }
        else
        {
            foreach (var chipValue in chipValues)
            {
                // create the chip and place it in the player area
                var valueChip = new ValueChip(Game, Vector2.Zero, chipValue);
                valueChip.Position = valueChip.GetTablePosition();
                int offsetX = ValueChip.HorizontalOffset * Chips.Count;
                Vector2 offset = new(IsOnTheLeft ? offsetX : -offsetX, 0);
                valueChip.Position = ChipPosition + offset;
                Chips.Add(valueChip);
            }
        }
    }

    /// <summary>
    /// Devides the value by the chip values to assist the creation of chips.
    /// </summary>
    /// <param name="value">Value to break down.</param>
    /// <returns>The list of individual values that correspond to value chips.</returns>
    /// <exception cref="ArgumentException">Exception thrown in case the value 
    /// cannot be devided by the smallest chip value.</exception>
    static List<int> GetChipValues(int value)
    {
        if (value % 5 != 0)
            throw new ArgumentException("Unable to create value chips equal to the give value.", nameof(value));

        List<int> chipValues = new();
        for (int i = ValueChip.Values.Length - 1; i >= 0; i--)
        {
            int chipValue = ValueChip.Values[i];
            while (value - chipValue >= 0)
            {
                chipValues.Add(chipValue);
                value -= chipValue;
            }
        }
        return chipValues;
    }

    void OnBlindChipChanged(BlindChip prevBlindChip, BlindChip newBlindChip)
    {
        if (newBlindChip is not null)
        {
            newBlindChip.Position = BlindChipPosition;
            BetAmount = newBlindChip is SmallBlindChip ? BetComponent.SmallBlind : BetComponent.BigBlind;
        }
    }

    void OnBetAmountChanged(int prevBetAmount, int newBetAmount)
    {
        if (newBetAmount < 0 || newBetAmount > Balance)
            throw new ArgumentOutOfRangeException(nameof(newBetAmount), "Invalid bet amount.");

        if (newBetAmount != 0 && newBetAmount < prevBetAmount)
            throw new ArgumentException("New bet cannot be lower than previous bet unless it is zero.");

        // remove all chips if new balance is zero
        if (newBetAmount == 0)
        {
            ResetChips();
            return;
        }

        if (newBetAmount == Balance)
            State = PlayerState.AllIn;

        AddChips(newBetAmount - prevBetAmount);
    }
}