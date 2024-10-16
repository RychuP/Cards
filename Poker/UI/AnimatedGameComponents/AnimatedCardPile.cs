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
    public AnimatedCardPile(GameManager gm) : base(gm, GetThemeTexture(gm.Theme))
    {
        Reset();
        gm.ThemeChanged += GameManager_OnThemeChanged;
    }

    public void Reset(bool resetPosition = true)
    {
        Enabled = true;
        Visible = true;
        if (resetPosition)
            Position = Constants.CardPileHiddenPosition;
        CurrentSegment = new Rectangle(0, 0, Constants.ShuffleFrameSize.X, Constants.ShuffleFrameSize.Y);
        RemoveAnimations();
    }

    /// <summary>
    /// Called when the game moves from screen to the gameplay screen.<br></br>
    /// Slides down and plays shuffle animation.
    /// </summary>
    /// <param name="action"></param>
    /// <param name="arg"></param>
    public void StartPlaying()
    {
        SlideDownAndShuffle();
    }

    public void StartNewGame()
    {
        SlideDownAndShuffle();
    }

    void SlideDownAndShuffle()
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

    public void Show()
    {
        Reset(false);
        RemoveAnimations();
        SlideDown();
    }

    public void Hide()
    {
        Reset(false);
        RemoveAnimations();
        SlideUp();
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
        };
    }

    static Texture2D GetThemeTexture(string theme) =>
        theme == Strings.Red ? CardAssets.ShuffleSpriteSheets[Strings.Red]
            : CardAssets.ShuffleSpriteSheets[Strings.Blue];

    void GameManager_OnThemeChanged(object o, ThemeChangedEventArgs e)
    {
        Texture = GetThemeTexture(e.Theme);
    }
}