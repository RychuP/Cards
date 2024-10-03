using Poker.UI.AnimatedHands;

namespace Poker.Gameplay.Players;

class PokerBettingPlayer : PokerCardsHolder
{
    public float Balance { get; private set; }
    public float BetAmount { get; private set; }
    public Gender Gender { get; }
    public Vector2 NamePosition { get; }

    public PokerBettingPlayer(string name, Gender gender, int place, GameManager gm)
        : base(name, gm)
    {
        AnimatedHand = new AnimatedPlayerHand(place, Hand, gm);
        Gender = gender;

        // measure string
        var stringSize = gm.Font.MeasureString(name);

        int verticalOffset = (place == 0 || place == Constants.MaxPlayers - 1) ?
            -Constants.PlayerNameOffset - (int)stringSize.Y :
            Constants.CardSize.Y + 5;

        int playerAreaWidth = Constants.CardSize.Y * 2;
        int horizontalOffset = (playerAreaWidth - (int)stringSize.X - 38) / 2;

        NamePosition = gm.GameTable[place] + new Vector2(horizontalOffset, verticalOffset);
    }
}