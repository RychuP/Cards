using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Poker.Misc;

static class Art
{
    public static Texture2D Table { get; private set; }
    public static Texture2D TableCardOutlines { get; private set; }
    public static Texture2D PokerTitle { get; private set; }
    public static Texture2D ThemeTitle { get; private set; }
    public static Texture2D ButtonSpriteSheet { get; private set; }

    public static void Initialize(Game game)
    {
        Table = LoadTexture(Constants.BackgroundScreenTextureName);
        TableCardOutlines = LoadTexture(Constants.TableCardOutlinesTextureName);
        PokerTitle = LoadTexture(Constants.PokerTitleTextureName);
        ThemeTitle = LoadTexture(Constants.ThemeScreenTextureName);
        ButtonSpriteSheet = LoadTexture(Constants.ButtonSpriteSheetName);

        Texture2D LoadTexture(string textureName) =>
            game.Content.Load<Texture2D>(Path.Combine("Images", textureName));
    }
}