using Framework.Assets;
using Framework.UI;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Poker.Gameplay;

/// <summary>
/// Pile of cards on the table. Manages shuffling and card dealing animations.
/// </summary>
class CardPile : AnimatedGameComponent
{
    public CardPile(GameManager gm) : base(gm, GetThemeTexture(Constants.DefaultTheme))
    {
        CurrentSegment = new Rectangle(0, 0, (int)Constants.ShuffleFrameSize.X, (int)Constants.ShuffleFrameSize.Y); // Constants.CardPileFrameSource; // new Rectangle(0, 0, Constants.ShuffleFrameSize, Constants.ShuffleFrameSize);//
        Position = new((Constants.GameWidth - Constants.CardPileFrameSource.Width) / 2, 00);
        gm.ThemeChanged += GameManager_OnThemeChanged;
    }

    public void PlayShuffleAnimation()
    {
        AddAnimation(
            new FramesetGameComponentAnimation(Texture, 32, 11, Constants.ShuffleFrameSize)
            {
                PerformBeforeStart = PlayShuffleSound,
                Duration = TimeSpan.FromSeconds(1.5f),
                PerformWhenDone = PlayShuffleSound
            });
    }

    /// <summary>
    /// Helper method to play shuffle sound and remove component
    /// </summary>
    /// <param name="obj"></param>
    void PlayShuffleSound(object obj)
    {
        CardSounds.Shuffle.Play();
    }

    static Texture2D GetThemeTexture(string theme) =>
        theme == Constants.RedThemeText ? CardAssets.ShuffleSpriteSheets[Constants.RedThemeText]
            : CardAssets.ShuffleSpriteSheets[Constants.BlueThemeText];

    void GameManager_OnThemeChanged(object o, ThemeChangedEventArgs e)
    {
        Texture = GetThemeTexture(e.Theme);
    }
}