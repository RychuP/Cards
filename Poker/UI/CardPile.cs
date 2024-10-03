using Framework.Assets;
using Framework.UI;
using Microsoft.Xna.Framework.Graphics;
using Poker.Gameplay;
using System;

namespace Poker.UI;

/// <summary>
/// Pile of cards on the table. Manages shuffling and card dealing animations.
/// </summary>
class CardPile : AnimatedGameComponent
{
    const float TransitionDuration = 0.2f;
    readonly Vector2 _hiddenPosition = new((Constants.GameWidth - Constants.CardPileFrameSource.Width) / 2, -Constants.ShuffleFrameSize.Y);
    readonly Vector2 _visiblePosition = new((Constants.GameWidth - Constants.CardPileFrameSource.Width) / 2, 0);

    public CardPile(GameManager gm) : base(gm, GetThemeTexture(Constants.DefaultTheme))
    {
        CurrentSegment = new Rectangle(0, 0, Constants.ShuffleFrameSize.X, Constants.ShuffleFrameSize.Y);
        Position = _hiddenPosition;
        gm.ThemeChanged += GameManager_OnThemeChanged;
    }

    /// <summary>
    /// Shows the card pile, plays shuffle animation and calls <see cref="GameManager.Deal"/> when finished.
    /// </summary>
    /// <param name="action"></param>
    /// <param name="arg"></param>
    public void ShowAndShuffle()
    {
        Show(Shuffle);
    }

    // part of ShowAndShuffle (stage 2)
    void Shuffle(object o)
    {
        FramesetGameComponentAnimation anim = new(Texture, 32, 11, Constants.ShuffleFrameSize.ToVector2())
        {
            PerformBeforeStart = (o) => CardSounds.Shuffle.Play(),
            Duration = TimeSpan.FromSeconds(1.5f),
            PerformWhenDone = CallDeal
        };
        AddAnimation(anim);
    }

    // part of ShowAndShuffle (stage 3)
    void CallDeal(object o)
    {
        ((GameManager)CardGame).Deal();
    }

    /// <summary>
    /// Moves the card pile to a visible, onscreen position.
    /// </summary>
    /// <param name="action"></param>
    /// <param name="arg"></param>
    public void Show(Action<object> action, object arg = null)
    {
        AddAnimation(GetTransitionAnimation(Position, _visiblePosition, action, arg));
    }

    /// <summary>
    /// Moves the card pile to a hidden, offscreen position.
    /// </summary>
    /// <param name="action"></param>
    /// <param name="arg"></param>
    public void Hide(Action<object> action, object arg = null)
    {
        AddAnimation(GetTransitionAnimation(Position, _hiddenPosition, action, arg));
    }

    /// <summary>
    /// Moves the card pile to a visible, onscreen position.
    /// </summary>
    public void Show()
    {
        Show((o) => { });
    }

    /// <summary>
    /// Moves the card pile to a hidden, offscreen position.
    /// </summary>
    public void Hide()
    {
        Hide((o) => { });
    }

    static TransitionGameComponentAnimation GetTransitionAnimation(Vector2 start, Vector2 finish, 
        Action<object> action, object arg)
    {
        return new TransitionGameComponentAnimation(start, finish)
        {
            Duration = TimeSpan.FromSeconds(TransitionDuration),
            PerformBeforeStart = (o) => CardSounds.Flip.Play(),
            PerformWhenDone = action,
            PerformWhenDoneArgs = arg
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