using Microsoft.Xna.Framework.Graphics;

namespace Solitaire.UI;

internal class Art
{
    public static Texture2D CardOutline { get; private set; }
    public static Texture2D Buttons { get; private set; }
    public static Texture2D BackgroundTile { get; private set; }
    public static Texture2D CardsLogo { get; private set; }
    public static Texture2D SolitaireTitle { get; private set; }
    public static Texture2D OptionsTitle { get; private set; }
    public static Texture2D PauseTitle { get; private set; }
    public static Texture2D WinTitle { get; private set; }

    public static void Initialize(Game game)
    {
        CardOutline = LoadTexture("card_outline");
        Buttons = LoadTexture("buttons");
        BackgroundTile = LoadTexture("background_tile");
        CardsLogo = LoadTexture("cards_logo");
        SolitaireTitle = LoadTexture("solitaire_title");
        OptionsTitle = LoadTexture("options_title");
        PauseTitle = LoadTexture("pause_title");
        WinTitle = LoadTexture("solitaire_title");

        Texture2D LoadTexture(string textureName) =>
            game.Content.Load<Texture2D>(textureName);
    }
}