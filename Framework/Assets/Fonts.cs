using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Framework.Assets;

public class Fonts
{
    public static class Moire
    {
        public static SpriteFont Regular { get; private set; }
        public static SpriteFont Bold { get; private set; }
        public static SpriteFont Menu { get; private set; }

        public static void Initialize(SpriteFont regular, SpriteFont bold, SpriteFont menu) =>
            (Regular, Bold, Menu) = (regular, bold, menu);
    }

    public static void Initialize(Game game)
    {
        Moire.Initialize(LoadFont("regular"), LoadFont("bold"), LoadFont("Menu"));

        SpriteFont LoadFont(string fontName) =>
            game.Content.Load<SpriteFont>(Path.Combine("Fonts", fontName));
    }
}