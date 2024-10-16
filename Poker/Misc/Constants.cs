using System;

namespace Poker.Misc;

static class Constants
{
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
    /// Vertical spacing between the lines of text.
    /// </summary>
    public static readonly int TextVerticalSpacing = 10;
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
    public static readonly TimeSpan CardFlipAnimationDuration = TimeSpan.FromMilliseconds(500); // 500
}