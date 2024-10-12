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
    public static Texture2D Label { get; private set; }
    public static Texture2D ButtonSpriteSheet { get; private set; }

    public static void Initialize(Game game)
    {
        TableCardOutlines = LoadTexture("card_outlines");
        ButtonSpriteSheet = LoadTexture("buttons");
        SmallBlindChip = LoadTexture("smallblind");
        ChipOutline = LoadTexture("chip_outline");
        BigBlindChip = LoadTexture("bigblind");
        PokerTitle = LoadTexture("title");
        ThemeTitle = LoadTexture("theme");
        PauseTitle = LoadTexture("pause");
        Table = LoadTexture("background");
        Label = LoadTexture("label");

        Texture2D LoadTexture(string textureName) =>
            game.Content.Load<Texture2D>(Path.Combine("Images", textureName));
    }
}