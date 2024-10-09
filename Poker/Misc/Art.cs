using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Poker.Misc;

static class Art
{
    public static Texture2D Table { get; private set; }
    public static Texture2D TableCardOutlines { get; private set; }
    public static Texture2D ChipOutline { get; private set; }
    public static Texture2D PokerTitle { get; private set; }
    public static Texture2D ThemeTitle { get; private set; }
    public static Texture2D PauseTitle { get; private set; }
    public static Texture2D SmallBlindChip { get; private set; }
    public static Texture2D BigBlindChip { get; private set; }
    public static Texture2D ButtonSpriteSheet { get; private set; }

    public static void Initialize(Game game)
    {
        Table = LoadTexture(Constants.BackgroundScreenTextureName);
        TableCardOutlines = LoadTexture(Constants.TableCardOutlinesTextureName);
        ChipOutline = LoadTexture(Constants.ChipOutlineTextureName);
        PokerTitle = LoadTexture(Constants.PokerTitleTextureName);
        ThemeTitle = LoadTexture(Constants.ThemeScreenTextureName);
        PauseTitle = LoadTexture(Constants.PauseScreenTextureName);
        SmallBlindChip = LoadTexture(Constants.SmallBlindTextureName);
        BigBlindChip = LoadTexture(Constants.BigBlindTextureName);
        ButtonSpriteSheet = LoadTexture(Constants.ButtonSpriteSheetName);

        Texture2D LoadTexture(string textureName) =>
            game.Content.Load<Texture2D>(Path.Combine("Images", textureName));
    }
}