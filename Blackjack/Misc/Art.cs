using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Blackjack.Misc;

static class Art
{
    public static Texture2D YouLoose { get; private set; }
    public static Texture2D Blackjack { get; private set; }
    public static Texture2D Bust { get; private set; }
    public static Texture2D Loose { get; private set; }
    public static Texture2D Push { get; private set; }
    public static Texture2D Win { get; private set; }
    public static Texture2D Pass { get; private set; }
    public static Texture2D Ring { get; private set; }
    public static Texture2D Table { get; private set; }
    public static Texture2D ButtonPressed { get; private set; }
    public static Texture2D ButtonRegular { get; private set; }
    public static Texture2D Button { get; private set; }
    public static Texture2D Blank { get; private set; }
    public static Texture2D TitleScreen { get; private set; }

    public static void Initialize(Game game)
    {
        TitleScreen = LoadTexture("titlescreen");
        Blank = LoadTexture("blank");
        YouLoose = LoadTexture("youlose");
        Button = LoadTexture("Button");
        ButtonPressed = LoadTexture("ButtonPressed");
        ButtonRegular = LoadTexture("ButtonRegular");
        Ring = LoadUITexture("ring");
        Table = LoadUITexture("table");
        Blackjack = LoadUITexture("blackjack");
        Bust = LoadUITexture("bust");
        Loose = LoadUITexture("lose");
        Push = LoadUITexture("push");
        Win = LoadUITexture("win");
        Pass = LoadUITexture("pass");

        Texture2D LoadTexture(string textureName) =>
            game.Content.Load<Texture2D>(Path.Combine("Images", textureName));

        Texture2D LoadUITexture(string textureName) =>
            game.Content.Load<Texture2D>(Path.Combine("Images", "UI", textureName));
    }
}