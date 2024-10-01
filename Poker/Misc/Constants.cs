namespace Poker.Misc;

static class Constants
{
    // texture names
    public static readonly string TableCardOutlinesTextureName = "card_outlines";
    public static readonly string BackgroundScreenTextureName = "background";
    public static readonly string ButtonSpriteSheetName = "buttons";
    public static readonly string ThemeScreenTextureName = "theme";
    public static readonly string PokerTitleTextureName = "title";
    public static readonly string ShuffleSpriteSheetName = "Shuffle_";

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
    public static readonly string ButtonReturnText = "Return";

    // themes
    public static readonly string RedThemeText = "Red";
    public static readonly string BlueThemeText = "Blue";
    public static readonly string DefaultTheme = RedThemeText;

    // dimensions
    public static readonly int GameWidth = 1280;
    public static readonly int GameHeight = 720;
    public static readonly int SpaceBetweenButtons = 80;
    public static readonly int ButtonWidth = 175;
    public static readonly int ButtonWidthWithMargin = ButtonWidth + SpaceBetweenButtons;
    public static readonly int ButtonSpriteHeight = 64;
    public static readonly int ButtonSpriteWidth = 291;
    //public static readonly int ShuffleFrameSize = 180;   // shuffle frame is square 180 x 180
    public static readonly Rectangle GameArea = new(0, 0, GameWidth, GameHeight);
    public static readonly Rectangle CardPileFrameSource = new(0, 60, 95, 120);  // card pile in shuffle sprite
    public static readonly Vector2 ShuffleFrameSize = new(180, 180);

    // quantities
    public static readonly int MaxPlayers = 4;
    public static readonly int MinPlayers = 4;

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