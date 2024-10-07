using Framework.Engine;
using Framework.UI;
using Poker.Gameplay.Chips;
using Poker.UI.AnimatedGameComponents;
using System;

namespace Poker.Gameplay.Players;

class PokerBettingPlayer : PokerCardsHolder
{
    /// <summary>
    /// Position of the first line of text in the player area.
    /// </summary>
    /// <remarks>Normally, it's where the player name is displayed.</remarks>
    readonly Vector2 _textPosition;

    public int Balance { get; set; }
    public int BetAmount { get; private set; }
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
        int y = (int)gm.GameTable[Place].Y + (Constants.CardSize.Y - Chip.Size.Y) / 2;
        int x = (int)gm.GameTable[Place].X + Constants.CardSize.X * 2
            + Constants.PlayerCardPadding + Chip.PlayerAreaPadding;
        ChipPosition = new Vector2(y, x);

        // calculate blind chip position
        var offsetY = IsAtTheBottom ? -BlindChip.OffsetY : BlindChip.OffsetY;
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

    public void Reset(Dealer dealer)
    {
        // reset stats
        Balance = 500;
        BetAmount = 0;

        // return cards to the dealer
        Hand.DealCardsToHand(dealer, Hand.Count);
    }

    void OnBlindChipChanged(BlindChip prevBlindChip, BlindChip newBlindChip)
    {

    }
}