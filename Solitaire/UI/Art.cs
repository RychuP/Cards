using Microsoft.Xna.Framework.Graphics;

namespace Solitaire.UI;

internal class Art
{
    public static Texture2D CardOutline { get; private set; }
    public static Texture2D Buttons { get; private set; }
    public static Texture2D BackgroundTile { get; private set; }
    public static Texture2D CardsLogo { get; private set; }
    public static Texture2D SolitaireText { get; private set; }
    public static Texture2D OptionsText { get; private set; }

    public static void Initialize(Game game)
    {
        CardOutline = LoadTexture("card_outline");
        Buttons = LoadTexture("buttons");
        BackgroundTile = LoadTexture("background_tile");
        CardsLogo = LoadTexture("cards_logo");
        SolitaireText = LoadTexture("solitaire_text");
        OptionsText = LoadTexture("options_text");

        Texture2D LoadTexture(string textureName) =>
            game.Content.Load<Texture2D>(textureName);
    }
}