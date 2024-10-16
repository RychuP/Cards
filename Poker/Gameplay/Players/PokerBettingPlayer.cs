using Framework.Assets;
using Framework.Engine;
using Framework.Misc;
using Framework.UI;
using Poker.Gameplay.Chips;
using Poker.UI.AnimatedGameComponents;
using System;
using System.Collections.Generic;
using System.Transactions;

namespace Poker.Gameplay.Players;

class PokerBettingPlayer : PokerCardsHolder
{
    public event EventHandler<PlayerStateChangedEventArgs> StateChanged;

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
    /// Chips equivalent to the value of <see cref="BetAmount"/>.
    /// </summary>
    readonly List<ValueChip> _chips = new();

    /// <summary>
    /// Component that displays player states.
    /// </summary>
    readonly Label _label;

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
    /// The best achieved poker hand determined during showdown. 
    /// </summary>
    public Result Result { get; set; }

    /// <summary>
    /// Index of the player's location on the table.
    /// </summary>
    public int Place {  get; }

    /// <summary>
    /// Convenience cached position normally provided by the game table.
    /// </summary>
    public Vector2 Position { get; }

    public AnimatedPlayerHand AnimatedPlayerHand =>
        AnimatedHand as AnimatedPlayerHand;

    /// <summary>
    /// Not bankrupt, folded or all-in.
    /// </summary>
    public bool CanParticipateInBettingRound =>
        !IsOut && !IsAllIn;

    /// <summary>
    /// Bankrupt or folded.
    /// </summary>
    public bool IsOut =>
        IsBankrupt || IsFolded;

    public bool IsBankrupt =>
        State == PlayerState.Bankrupt;

    public bool IsFolded =>
        State == PlayerState.Folded;

    public bool IsWaiting =>
        State == PlayerState.Waiting;

    public bool IsAllIn =>
        State == PlayerState.AllIn;

    /// <summary>
    /// Returns the better card out of the two the player holds.<br></br>
    /// If both cards are equal, the first card is returned.
    /// </summary>
    public TraditionalCard BetterCard
    {
        get
        {
            int result = Hand[0].CompareTo(Hand[1]);
            // card 0 is better or equal to card 1
            if (result <= 0)
                return Hand[0];
            else 
                return Hand[1];
        }
    }

    /// <summary>
    /// Returns the worse card value out of the two the player holds.<br></br>
    /// If both cards are equal, the second card's value is returned.
    /// </summary>
    public TraditionalCard WorseCard
    {
        get
        {
            int result = Hand[1].CompareTo(Hand[0]);
            // card 1 is weaker or equal to card 0
            if (result >= 0)
                return Hand[1];
            else
                return Hand[0];
        }
    }

    // backing field
    PlayerState _state;
    /// <summary>
    /// Current state of the player in the game.
    /// </summary>
    public PlayerState State
    {
        get => _state;
        private set
        {
            if (_state == value) return;
            var prevState = _state;
            _state = value;
            OnStateChanged(prevState, value);
        }
    }

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

            if (value < 0)
                throw new Exception("Bets cannot be negative.");

            if (value > (Balance + _betAmount))
                throw new Exception("Insufficient funds to cover the bet");

            int prevBetAmount = _betAmount;
            _betAmount = value;
            OnBetAmountChanged(prevBetAmount, value);
        }
    }

    // backing field
    int _balance;
    /// <summary>
    /// How much money the player owns.
    /// </summary>
    public int Balance
    {
        get => _balance;
        set
        {
            if (_balance == value) return;
            if (value < 0)
                throw new ArgumentException("Balance cannot be negative.");
            int prevBalance = _balance;
            _balance = value;
            OnBalanceChanged(prevBalance, value);
        }
    }

    public PokerBettingPlayer(string name, Gender gender, int place, GameManager gm)
        : base(name, gm)
    {
        AnimatedHand = new AnimatedPlayerHand(this);
        Gender = gender;
        Place = place;
        Position = gm.GameTable[Place];
        AnimatedHand.Position = Position;

        // calculate the first line of text position
        var stringSize = gm.Font.MeasureString(Name);
        int verticalOffset = IsAtTheBottom ?
            -Constants.TextVerticalSpacing - (int)stringSize.Y :
            Constants.CardSize.Y + Constants.TextVerticalSpacing;
        _textPosition = Position + new Vector2(0, verticalOffset);

        // calculate name position in the player area
        NamePosition = GetCenteredTextPosition(Name, 0);

        // calculate chip position
        int offsetY = (Constants.CardSize.Y - Chip.Size.Y) / 2;
        int offsetX = IsOnTheLeft ?
            Constants.CardSize.X * 2 + Constants.PlayerCardPadding + Chip.PlayerAreaPadding :
            -Chip.Size.X - Chip.PlayerAreaPadding;
        ChipPosition = Position + new Vector2(offsetX, offsetY);

        // calculate blind chip position
        offsetY = IsAtTheBottom ? -BlindChip.OffsetY - Chip.Size.Y : BlindChip.OffsetY + Chip.Size.Y;
        BlindChipPosition = ChipPosition + new Vector2(0, offsetY);

        // create a label
        _label = new(this, GameManager);
        Game.Components.Add(_label);

        // assign event handler
        gm.StateChanged += GameManager_OnStateChanged;
    }

    /// <inheritdoc/>
    public override void Reset()
    {
        Balance = StartBalance;
        BlindChip = null;
        BetAmount = 0;
        Result = null;
        State = PlayerState.Waiting;
        _label.Hide();

        if (_chips.Count > 0)
            RemoveAllChips();

        base.Reset();
    }

    public virtual void StartPlaying()
    {
        _label.Show();
    }

    /// <summary>
    /// Called when the new poker game starts before the dealer reshuffles cards.
    /// </summary>
    public virtual void StartNewGame()
    {
        ReturnCardsToDealer();
        Result = null;
        if (!IsBankrupt)
            State = PlayerState.Waiting;
    }

    /// <summary>
    /// Called after each flop, turn and river stages before betting begins.
    /// </summary>
    public virtual void StartNewBettingRound()
    {
        if (CanParticipateInBettingRound)
            State = PlayerState.Waiting;
    }

    public void CheckBankrupt()
    {
        if (Balance == 0)
            State = PlayerState.Bankrupt;
    }

    /// <summary>
    /// Removes all animated chip components and clears Chips.
    /// </summary>
    void RemoveAllChips()
    {
        foreach (var chip in _chips)
            chip.RemoveAnimatedComponent();
        _chips.Clear();
    }

    /// <summary>
    /// Removes chips from the top of the stack equivalent to the value.
    /// </summary>
    /// <param name="value"></param>
    void RemoveChips(int value)
    {
        int start = _chips.Count - 1;
        for (int i = start; i >= 0; i--)
        {
            var chip = _chips[i];
            RemoveChip(chip);

            // reduce the value by the removed chip's value
            if (chip.Value <= value)
            {
                value -= chip.Value;
                if (value == 0)
                    return;
            }
            // add change to replace reduced value of the chip
            else
            {
                AddChips(chip.Value - value);
            }
        }
    }

    /// <summary>
    /// Removes a single chip from the top of the stack.
    /// </summary>
    /// <param name="chip"></param>
    void RemoveChip(ValueChip chip)
    {
        _chips.Remove(chip);
        chip.RemoveAnimatedComponent();
    }

    /// <summary>
    /// Adds or replaces (if too many) value chips in the player area.
    /// </summary>
    /// <param name="value"></param>
    void AddChips(int value)
    {
        // check if the bet amount doesn't exceed the max amount of chips that can be displayed
        int biggestChipValue = ValueChip.Values[ValueChip.Count - 1];
        int maxValue = biggestChipValue * ChipMaxAmount;
        if (BetAmount > maxValue)
            value = maxValue;
        
        // get the list of values that chips will created from
        var chipValues = GetChipValues(value);
        if (_chips.Count + chipValues.Count > ChipMaxAmount)
        {
            RemoveAllChips();
            AddChips(BetAmount);
        }
        else
        {
            var startTime = DateTime.Now;
            foreach (var chipValue in chipValues)
            {
                // create the chip and place it in the player area
                var valueChip = new ValueChip(Game, Vector2.Zero, chipValue);
                // start position
                valueChip.Position = valueChip.GetTablePosition();
                int offsetX = ValueChip.HorizontalOffset * _chips.Count;
                Vector2 offset = new(IsOnTheLeft ? offsetX : -offsetX, 0);
                valueChip.AnimationStartTime = startTime;
                // end position for the animation
                valueChip.Position = ChipPosition + offset;
                _chips.Add(valueChip);
                startTime += Chip.Delay;
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

    /// <summary>
    /// Sets the State to Waiting.
    /// </summary>
    public void ResetState() =>
        State = PlayerState.Waiting;

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
            -Constants.TextVerticalSpacing - (int)stringSize.Y :
            Constants.TextVerticalSpacing + (int)stringSize.Y;
        var textPos = _textPosition + new Vector2(horizontalOffset, verticalOffset * lineNumber);
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

    public virtual void Call(int newBetAmount)
    {
        if (newBetAmount < 0)
            throw new ArgumentException("Bet amount cannot be negative.");

        if (newBetAmount < BetAmount)
            throw new ArgumentException("Cannot call bets lower than the current bet.");

        if (newBetAmount > (Balance + BetAmount))
            throw new ArgumentException("Insufficient balance to call.");

        if (newBetAmount >= (Balance + BetAmount))
            AllIn();
        else
        {
            int diff = newBetAmount - BetAmount;
            BetAmount += diff;
            Balance -= diff;
            State = PlayerState.Called;
        }
    }

    public virtual void Check()
    {
        State = PlayerState.Checked;
    }

    public virtual void Raise(int newBetAmount)
    {
        if (newBetAmount < 0)
            throw new ArgumentException("Bet amount cannot be negative.");

        if (newBetAmount > (Balance + BetAmount))
            throw new ArgumentException("Insufficient balance to raise.");

        if (newBetAmount >= (Balance + BetAmount))
        {
            AllIn();
        }
        else
        {
            var diff = newBetAmount - BetAmount;
            BetAmount += diff;
            Balance -= diff;
            State = PlayerState.Raised;
        }
    }

    public virtual void AllIn()
    {
        BetAmount += Balance;
        Balance = 0;
        State = PlayerState.AllIn;
    }

    public virtual void Fold()
    {
        State = PlayerState.Folded;
    }

    /// <summary>
    /// Called each betting round unless folded or bankrupt.
    /// </summary>
    public virtual void TakeTurn(int currentBetAmount, Hand communityCards, bool checkPossible)
    {
        if (!CanParticipateInBettingRound)
            throw new InvalidOperationException("Bet component should not call this player " +
                "to take their turn.");

        if (currentBetAmount < 0)
            throw new ArgumentException("Negative bets are not possible.");
    }

    /// <summary>
    /// Extendes the label with the text: Winner.
    /// </summary>
    public void SetWinnerState()
    {
        State = PlayerState.Winner;
    }

    public int SendChipsToWinner(PokerBettingPlayer winner, DateTime startTime)
    {
        int chipCount = _chips.Count;
        for (int i = _chips.Count - 1; i >= 0; i--)
        {
            var chip = _chips[i];
            chip.AnimationStartTime = startTime;
            chip.Arrived += (o, e) =>
            {
                winner.Balance += chip.Value;
                BetAmount -= chip.Value;
            };
            chip.Position = winner.Position;
            startTime += Chip.WinningDelay;
        }
        return chipCount;
    }

    void OnBlindChipChanged(BlindChip prevBlindChip, BlindChip newBlindChip)
    {
        if (newBlindChip is not null)
        {
            newBlindChip.Position = BlindChipPosition;

            int betAmount = newBlindChip is SmallBlindChip ? BetComponent.SmallBlind : BetComponent.BigBlind;
            if (Balance >= betAmount)
                Raise(betAmount);
            else if (Balance > 0)
                AllIn();
            else
                throw new Exception("This player should not have been issued the blind chip.");
        }
    }

    protected void OnBetAmountChanged(int prevBetAmount, int newBetAmount)
    {
        // remove all chips if new balance is zero
        if (newBetAmount == 0)
            RemoveAllChips();

        // 
        else if (newBetAmount < prevBetAmount)
            RemoveChips(prevBetAmount - newBetAmount);

        else
            AddChips(newBetAmount - prevBetAmount);
    }

    void OnBalanceChanged(int prevBalance, int newBalance)
    {
        
    }

    void OnStateChanged(PlayerState prevState, PlayerState newState)
    {
        switch (newState)
        {
            case PlayerState.Folded:
                AnimatedPlayerHand.Fold();
                break;
        }

        var args = new PlayerStateChangedEventArgs(prevState, newState);
        StateChanged?.Invoke(this, args);
    }

    void GameManager_OnStateChanged(object o, GameStateEventArgs e)
    {
        if (IsFolded && e.NewState > GameState.Preflop && e.NewState < GameState.RiverBet)
        {
            _label.Retract();
        } 
    }
}