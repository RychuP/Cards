﻿using CardsFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Poker;

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
    public static Texture2D ButtonSpriteSheet { get; private set; }

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
        ButtonSpriteSheet = LoadTexture("UI", Constants.ButtonSpriteSheetTextureName);
        Table = LoadTexture(Constants.BackgroundScreenTextureName);
        TableCardOutlines = LoadTexture(Constants.TableCardOutlinesTextureName);
        PokerTitle = LoadTexture(Constants.PokerTitleTextureName);
    }

    static Texture2D LoadTexture(string textureName) =>
        s_game.Content.Load<Texture2D>(Path.Combine("Images", textureName));

    static Texture2D LoadTexture(string folder, string textureName) =>
        s_game.Content.Load<Texture2D>(Path.Combine("Images", folder, textureName));

    static SpriteFont LoadFont(string fontName) =>
        s_game.Content.Load<SpriteFont>(Path.Combine("Fonts", fontName));
}
