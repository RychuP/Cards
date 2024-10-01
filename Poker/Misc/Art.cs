using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Poker.Misc;

static class Art
{
    static Game s_game;
    public static SpriteBatch SpriteBatch { get; private set; }
    public static SpriteFont MenuFont { get; private set; }
    public static SpriteFont RegularFont { get; private set; }
    public static SpriteFont BoldFont { get; private set; }
    public static Texture2D Pixel { get; private set; }
    public static Texture2D Table { get; private set; }
    public static Texture2D TableCardOutlines { get; private set; }
    public static Texture2D PokerTitle { get; private set; }
    public static Texture2D ThemeTitle { get; private set; }
    public static Texture2D ButtonSpriteSheet { get; private set; }
    public static Texture2D RedShuffleSpriteSheet { get; private set; }
    public static Texture2D BlueShuffleSpriteSheet { get; private set; }

    public static void Initialize(Game game)
    {
        s_game = game;
        LoadContent();
    }

    public static void LoadContent()
    {
        SpriteBatch = new SpriteBatch(s_game.GraphicsDevice);
        Pixel = new Texture2D(s_game.GraphicsDevice, 1, 1);
        Pixel.SetData(new[] { Color.White });

        // fonts
        MenuFont = LoadFont(Constants.MenuFontTextureName);
        RegularFont = LoadFont(Constants.RegularFontTextureName);
        BoldFont = LoadFont(Constants.BoldFontTextureName);

        // textures
        ButtonSpriteSheet = LoadTexture("UI", Constants.ButtonSpriteSheetName);
        Table = LoadTexture(Constants.BackgroundScreenTextureName);
        TableCardOutlines = LoadTexture(Constants.TableCardOutlinesTextureName);
        PokerTitle = LoadTexture(Constants.PokerTitleTextureName);
        ThemeTitle = LoadTexture(Constants.ThemeScreenTextureName);
        RedShuffleSpriteSheet = LoadTexture(Constants.ShuffleSpriteSheetName + Constants.RedThemeText);
        BlueShuffleSpriteSheet = LoadTexture(Constants.ShuffleSpriteSheetName + Constants.BlueThemeText);
    }

    static Texture2D LoadTexture(string textureName) =>
        s_game.Content.Load<Texture2D>(Path.Combine("Images", textureName));

    static Texture2D LoadTexture(string folder, string textureName) =>
        s_game.Content.Load<Texture2D>(Path.Combine("Images", folder, textureName));

    static SpriteFont LoadFont(string fontName) =>
        s_game.Content.Load<SpriteFont>(Path.Combine("Fonts", fontName));
}
