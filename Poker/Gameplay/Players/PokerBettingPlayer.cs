using Poker.UI.AnimatedHands;

namespace Poker.Gameplay.Players;

class PokerBettingPlayer : PokerCardsHolder
{
    readonly Vector2 _textPosition;
    public int Balance { get; set; }
    public int BetAmount { get; private set; }
    public Gender Gender { get; }
    public Vector2 NamePosition { get; }

    /// <summary>
    /// Index of the player's location on the table.
    /// </summary>
    public int Place {  get; }

    public PokerBettingPlayer(string name, Gender gender, int place, GameManager gm)
        : base(name, gm)
    {
        AnimatedHand = new AnimatedPlayerHand(place, Hand, gm);
        Gender = gender;
        Place = place;

        // calculate name position in the player area
        var stringSize = gm.Font.MeasureString(Name);
        int verticalOffset = IsAtTheBottom ?
            -Constants.PlayerTextVerticalPadding - (int)stringSize.Y :
            Constants.CardSize.Y + Constants.PlayerTextVerticalPadding;
        _textPosition = gm.GameTable[Place] + new Vector2(0, verticalOffset);
        NamePosition = GetCenteredTextPosition(Name, 0);
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
        Balance = 500;
        BetAmount = 0;
    }
}