﻿using System;

namespace Poker.Misc;

static class Constants
{
    // texture names
    public static readonly string TableCardOutlinesTextureName = "card_outlines";
    public static readonly string BackgroundScreenTextureName = "background";
    public static readonly string ButtonSpriteSheetName = "buttons";
    public static readonly string ThemeScreenTextureName = "theme";
    public static readonly string PokerTitleTextureName = "title";

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
    public static readonly int ButtonWidthWithPadding = ButtonWidth + ButtonPadding;
    public static readonly int ButtonSpriteHeight = 64;
    public static readonly int ButtonSpriteWidth = 291;
    public static readonly Point CardSize = new(80, 106);
    public static readonly Rectangle GameArea = new(0, 0, GameWidth, GameHeight);

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
    public static readonly Vector2 CommunityCardsPosition = new(376, 301);
    public static readonly Vector2 DealerPosition = new(585, 68);
    public static readonly int ButtonPositionY = 632;
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
    

    // times
    public static readonly TimeSpan DealAnimationDuration = TimeSpan.FromMilliseconds(500);
    public static readonly TimeSpan FlipAnimationDuration = TimeSpan.FromMilliseconds(500);
}