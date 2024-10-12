using System;

namespace Poker.Misc;

static class Constants
{
    

    // button texts
    public static readonly string ButtonRaiseText = "Raise";
    public static readonly string ButtonCheckText = "Check";
    public static readonly string ButtonFoldText = "Fold";
    public static readonly string ButtonCallText = "Call";
    public static readonly string ButtonAllInText = "All In";
    public static readonly string ButtonPlayText = "Play";
    public static readonly string ButtonThemeText = "Theme";
    public static readonly string ButtonExitText = "Exit";
    public static readonly string ButtonReturnText = "Return";
    public static readonly string ButtonClearText = "Clear";
    public static readonly string ButtonMatchBetText = "Match Bet";
    public static readonly string ButtonEndTurnText = "End Turn";

    // errors
    public static readonly string OnlyHumanPlayerExceptionText = 
        "Button clicks should occur on human player's turn only.";
    public static readonly string EvaluatorCardArrayLengthException =
        "Cards array must have the length of 5.";

    // npc names
    public static readonly string[] Names = new[] {
        "Liam", "Noah", "Oliver", "James", "Elijah", "Theodore", "Henry", "Lucas", "William",
        "Olivia", "Emily", "Charlotte", "Amelia", "Sophia", "Mia", "Isabella", "Ava", "Evelyn"};
    public static readonly string CommunityCardsName = "Community Cards";

    // themes
    public static readonly string RedThemeText = "Red";
    public static readonly string BlueThemeText = "Blue";
    public static readonly string DefaultTheme = RedThemeText;

    // dimensions
    public static readonly int GameWidth = 1280;
    public static readonly int GameHeight = 720;
    /// <summary>
    /// Space between buttons.
    /// </summary>
    public static readonly int ButtonPadding = 80;
    public static readonly int ButtonWidth = 175;
    /// <summary>
    /// Basically the x coordinate for the next button.
    /// </summary>
    public static readonly int ButtonWidthWithPadding = ButtonWidth + ButtonPadding;
    public static readonly int ButtonSpriteHeight = 64;
    public static readonly int ButtonSpriteWidth = 291;
    public static readonly Point CardSize = new(80, 106);
    public static readonly Rectangle GameArea = new(0, 0, GameWidth, GameHeight);
    /// <summary>
    /// Distance between player cards.
    /// </summary>
    public static readonly int PlayerCardPadding = 11;
    /// <summary>
    /// Distance between community cards.
    /// </summary>
    public static readonly int CommunityCardPadding = 32;
    /// <summary>
    /// Distance between the card and an edge of the screen.
    /// </summary>
    public static readonly int PlayerCardMargin = 31;
    /// <summary>
    /// Vertical space between the lines of text in the player area.
    /// </summary>
    public static readonly int PlayerTextVerticalPadding = 10;
    public static readonly int PlayerAreaWidth = CardSize.X * 2 + PlayerCardPadding;

    // quantities
    public static readonly int MaxPlayers = 4;
    public static readonly int MinPlayers = 4;

    // sprite source rectangles
    public static readonly Point ShuffleFrameSize = new(180, 180);
    /// <summary>
    /// Card pile sprite in shuffle sprite sheet.
    /// </summary>
    public static readonly Rectangle CardPileFrameSource = new(0, 60, 95, 120);
    public static readonly Rectangle ButtonSpriteRegularSource = new(0, 0, ButtonSpriteWidth, ButtonSpriteHeight);
    public static readonly Rectangle ButtonSpriteHoverSource = ButtonSpriteRegularSource.Move(0, ButtonSpriteHeight);
    public static readonly Rectangle ButtonSpritePressedSource = ButtonSpriteRegularSource.Move(0, ButtonSpriteHeight * 2);

    // coordinates
    public static readonly Vector2 CardPileHiddenPosition = 
        new((GameWidth - CardPileFrameSource.Width) / 2, -ShuffleFrameSize.Y);
    public static readonly Vector2 CardPileVisiblePosition = 
        new((GameWidth - CardPileFrameSource.Width) / 2, 0);
    public static readonly Vector2 CommunityCardsPosition = new(375, 301);
    public static readonly Vector2 DealerPosition = new(585, 68);
    public static readonly int ButtonPositionY = 632;

    // timers
    public static TimeSpan CardPileTransitionDuration = TimeSpan.FromSeconds(0.2f);
    public static TimeSpan ShuffleDuration = TimeSpan.FromSeconds(1.5f);
    public static readonly TimeSpan DealAnimationDuration = TimeSpan.FromMilliseconds(500); // 500
    public static readonly TimeSpan FlipAnimationDuration = TimeSpan.FromMilliseconds(500); // 500
}