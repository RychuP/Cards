using Microsoft.Xna.Framework;

namespace Poker;

static class Constants
{
    // texture names
    public static readonly string ButtonSpriteSheetTextureName = "buttons";
    public static readonly string BackgroundScreenTextureName = "background";
    public static readonly string TableCardOutlinesTextureName = "card_outlines";
    public static readonly string PokerTitleTextureName = "title";

    // font texture names
    public static readonly string RegularFontTextureName = "regular";
    public static readonly string BoldFontTextureName = "bold";
    public static readonly string MenuFontTextureName = "menufont";

    // button texts
    public static readonly string ButtonRaiseText = "Raise";
    public static readonly string ButtonCheckText = "Check";
    public static readonly string ButtonFoldText = "Fold";
    public static readonly string ButtonCallText = "Call";
    public static readonly string ButtonAllInText = "All In";
    public static readonly string ButtonPlayText = "Play";
    public static readonly string ButtonThemeText = "Theme";
    public static readonly string ButtonExitText = "Exit";

    // dimensions
    public static readonly int GameWidth = 1280;
    public static readonly int GameHeight = 720;
    public static readonly Rectangle GameArea = new(0, 0, GameWidth, GameHeight);
    public static readonly int SpaceBetweenButtons = 80;
    public static readonly int ButtonWidth = 175;
    public static readonly int ButtonWidthWithMargin = ButtonWidth + SpaceBetweenButtons;
    public static readonly int ButtonSpriteHeight = 64;
    public static readonly int ButtonSpriteWidth = 291;

    // sprite source rectangles
    public static readonly Rectangle ButtonSpriteRegularSource =
        new(0, 0, ButtonSpriteWidth, ButtonSpriteHeight);
    public static readonly Rectangle ButtonSpriteHoverSource =
        ButtonSpriteRegularSource.Move(0, ButtonSpriteHeight);
    public static readonly Rectangle ButtonSpritePressedSource =
        ButtonSpriteRegularSource.Move(0, ButtonSpriteHeight * 2);

    // coordinates
    public static readonly int ButtonPositionY = 632;
}
