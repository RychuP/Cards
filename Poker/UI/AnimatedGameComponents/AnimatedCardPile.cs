using Framework.Assets;
using Framework.UI;
using Microsoft.Xna.Framework.Graphics;
using Poker.Gameplay;

namespace Poker.UI.AnimatedGameComponents;

/// <summary>
/// Pile of cards on the table. Manages shuffling and card dealing animations.
/// </summary>
class AnimatedCardPile : AnimatedGameComponent
{
    public AnimatedCardPile(GameManager gm) : base(gm, GetThemeTexture(Constants.DefaultTheme))
    {
        Reset();
        gm.ThemeChanged += GameManager_OnThemeChanged;
    }

    public void Reset()
    {
        Enabled = true;
        Visible = true;
        Position = Constants.CardPileHiddenPosition;
        CurrentSegment = new Rectangle(0, 0, Constants.ShuffleFrameSize.X, Constants.ShuffleFrameSize.Y);
        RemoveAnimations();
    }

    /// <summary>
    /// Shows the card pile, plays shuffle animation and calls <see cref="GameManager.DealCardsToPlayers"/> when finished.
    /// </summary>
    /// <param name="action"></param>
    /// <param name="arg"></param>
    public void ShowAndShuffle()
    {
        FramesetGameComponentAnimation shuffle = new(Texture, 32, 11, Constants.ShuffleFrameSize.ToVector2())
        {
            PerformBeforeStart = (o) => CardSounds.Shuffle.Play(),
            Duration = Constants.ShuffleDuration,
            PerformWhenDone = (o) => CardSounds.Shuffle.Play()
        };
        var slideAnim = SlideDown();
        slideAnim.PerformWhenDone = (o) => AddAnimation(shuffle);
    }

    /// <summary>
    /// Moves the card pile to a visible, onscreen position.
    /// </summary>
    public TransitionGameComponentAnimation SlideDown()
    {
        var slideDown = GetTransitionAnimation(Position, Constants.CardPileVisiblePosition);
        AddAnimation(slideDown);
        return slideDown;
    }

    /// <summary>
    /// Moves the card pile to a hidden, offscreen position.
    /// </summary>
    public void SlideUp()
    {
        AddAnimation(GetTransitionAnimation(Position, Constants.CardPileHiddenPosition));
    }

    static TransitionGameComponentAnimation GetTransitionAnimation(Vector2 start, Vector2 finish)
    {
        return new TransitionGameComponentAnimation(start, finish)
        {
            Duration = Constants.CardPileTransitionDuration,
            //PerformBeforeStart = (o) => CardSounds.Flip.Play()
        };
    }

    static Texture2D GetThemeTexture(string theme) =>
        theme == Constants.RedThemeText ? CardAssets.ShuffleSpriteSheets[Constants.RedThemeText]
            : CardAssets.ShuffleSpriteSheets[Constants.BlueThemeText];

    void GameManager_OnThemeChanged(object o, ThemeChangedEventArgs e)
    {
        Texture = GetThemeTexture(e.Theme);
    }
}