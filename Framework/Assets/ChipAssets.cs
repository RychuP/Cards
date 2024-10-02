using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Framework.Assets;

public static class ChipAssets
{
    public static readonly int[] Values = { 5, 25, 100, 500 };
    public static readonly string[] Colors = { "Red", "White", "Yellow", "Black" };
    public static readonly Dictionary<int, Texture2D> ValueChips = new();
    public static readonly Dictionary<string, Texture2D> BlankChips = new();

    public static void Initialize(Game game)
    {
        string fileName = "chip";

        // load value chip textures
        foreach (var chipValue in Values)
            ValueChips.Add(chipValue, LoadTexture(fileName + chipValue));

        // load blank chip textures
        foreach (var color in Colors)
            BlankChips.Add(color, LoadTexture(fileName + color));

        Texture2D LoadTexture(string textureName) =>
            game.Content.Load<Texture2D>(Path.Combine("Images", "Chips", textureName));
    }
}