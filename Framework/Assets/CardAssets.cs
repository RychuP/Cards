using Framework.Engine;
using Framework.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Framework.Assets;

public static class CardAssets
{
    public static readonly string[] Themes = { "Red", "Blue" };
    public static readonly Dictionary<string, Texture2D> CardFaces = new();
    public static readonly Dictionary<string, Texture2D> CardBacks = new();
    public static readonly Dictionary<string, Texture2D> ShuffleSpriteSheets = new();

    public static void Initialize(Game game)
    {
        // initialize a full deck
        CardPacket fullDeck = new(1, 2, CardSuits.AllSuits, CardValues.NonJokers | CardValues.Jokers);
        string assetName;

        // load card faces
        for (int cardIndex = 0; cardIndex < 54; cardIndex++)
        {
            assetName = UIUtilty.GetCardAssetName(fullDeck[cardIndex]);
            CardFaces.Add(assetName, LoadTexture(assetName));
        }

        // load card backs
        foreach (string themeName in Themes)
            CardBacks.Add(themeName, LoadTexture($"CardBack_{themeName}"));

        // load shuffle sprite sheets
        foreach (string themeName in Themes)
            ShuffleSpriteSheets.Add(themeName, LoadTexture($"Shuffle_{themeName}"));

        Texture2D LoadTexture(string textureName) =>
            game.Content.Load<Texture2D>(Path.Combine("Images", "Cards", textureName));
    }
}