#region File Description
//-----------------------------------------------------------------------------
// Cardsgame.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CardsFramework;

/// <summary>
/// A cards-game handler.
/// </summary>
/// <remarks>
/// Use a singleton of a class that derives from class to empower a cards-game, while making sure
/// to call the various methods in order to allow the implementing instance to run the game.
/// </remarks>
public abstract class CardGame
{
    #region Fields and Properties
    protected List<GameRule> Rules = new();
    protected List<Player> Players = new();
    protected CardPacket Dealer;

    public int MinimumPlayers { get; protected set; }
    public int MaximumPlayers { get; protected set; }

    public string Theme { get; protected set; }
    protected internal Dictionary<string, Texture2D> CardAssets = new();
    public GameTable GameTable { get; protected set; }
    public SpriteFont Font { get; set; }
    public SpriteBatch SpriteBatch { get; set; }
    public Game Game { get; set; }
    #endregion

    #region Initializations
    /// <summary>
    /// Initializes a new instance of the <see cref="CardGame"/> class.
    /// </summary>
    /// <param name="decks">The amount of decks in the game.</param>
    /// <param name="jokersInDeck">The amount of jokers in each deck.</param>
    /// <param name="suits">The suits which will appear in each deck. Multiple 
    /// values can be supplied using flags.</param>
    /// <param name="cardValues">The card values which will appear in each deck. 
    /// Multiple values can be supplied using flags.</param>
    /// <param name="minimumPlayers">The minimal amount of players 
    /// for the game.</param>
    /// <param name="maximumPlayers">The maximal amount of players 
    /// for the game.</param>
    /// <param name="theme">The name of the theme to use for the 
    /// game's assets.</param>
    /// <param name="game">The associated game object.</param>
    public CardGame(int decks, int jokersInDeck, CardSuit suits, CardValue cardValues,
        int minimumPlayers, int maximumPlayers, GameTable gameTable, string theme, Game game)
    {
        Dealer = new CardPacket(decks, jokersInDeck, suits, cardValues);

        Game = game;
        MinimumPlayers = minimumPlayers;
        MaximumPlayers = maximumPlayers;

        Theme = theme;
        GameTable = gameTable;
        GameTable.DrawOrder = -10000;
        game.Components.Add(GameTable);
    }
    #endregion

    #region Virtual Methods
    /// <summary>
    /// Checks which of the game's rules need to be fired.
    /// </summary>
    public virtual void CheckRules()
    {
        for (int ruleIndex = 0; ruleIndex < Rules.Count; ruleIndex++)
            Rules[ruleIndex].Check();
    }

    /// <summary>
    /// Returns a card's value in the scope of the game.
    /// </summary>
    /// <param name="card">The card for which to return the value.</param>
    /// <returns>The card's value.</returns>        
    public virtual int GetCardValue(TraditionalCard card)
    {
        return card.Value switch
        {
            CardValue.Ace => 1,
            CardValue.Two => 2,
            CardValue.Three => 3,
            CardValue.Four => 4,
            CardValue.Five => 5,
            CardValue.Six => 6,
            CardValue.Seven => 7,
            CardValue.Eight => 8,
            CardValue.Nine => 9,
            CardValue.Ten => 10,
            CardValue.Jack => 11,
            CardValue.Queen => 12,
            CardValue.King => 13,
            _ => throw new ArgumentException("Ambigous card value"),
        };
    }
    #endregion

    #region Abstract Methods
    /// <summary>
    /// Adds a player to the game.
    /// </summary>
    /// <param name="player">The player to add to the game.</param>
    public abstract void AddPlayer(Player player);

    /// <summary>
    /// Gets the player who is currently taking his turn.
    /// </summary>
    /// <returns>The currently active player.</returns>
    public abstract Player GetCurrentPlayer();

    /// <summary>
    /// Deals cards to the participating players.
    /// </summary>
    public abstract void Deal();

    /// <summary>
    /// Initializes the game lets the players start playing.
    /// </summary>
    public abstract void StartPlaying();
    #endregion

    #region Loading
    /// <summary>
    /// Load the basic contents for a card game (the card assets)
    /// </summary>
    public void LoadContent()
    {
        SpriteBatch = new SpriteBatch(Game.GraphicsDevice);

        // Initialize a full deck
        CardPacket fullDeck = new(1, 2, CardSuit.AllSuits, CardValue.NonJokers | CardValue.Jokers);
        string assetName;

        // Load all card assets
        for (int cardIndex = 0; cardIndex < 54; cardIndex++)
        {
            assetName = UIUtilty.GetCardAssetName(fullDeck[cardIndex]);
            LoadUITexture("Cards", assetName);
        }
        // Load card back picture
        LoadUITexture("Cards", "CardBack_" + Theme);

        // Load the game's font
        Font = Game.Content.Load<SpriteFont>("Fonts/Regular");

        GameTable.Initialize();
    }

    /// <summary>
    /// Loads the UI textures for the game, taking the theme into account.
    /// </summary>
    /// <param name="folder">The asset's folder under the theme folder. For example,
    /// for an asset belonging to the "Fish" theme and which sits in 
    /// "Images\Fish\Beverages\Soda" folder under the content project, use
    /// "Beverages\Soda" as this argument's value.</param>
    /// <param name="assetName">The name of the asset.</param>
    public void LoadUITexture(string folder, string assetName)
    {
        var texture = Game.Content.Load<Texture2D>($"Images/{folder}/{assetName}");
        CardAssets.Add(assetName, texture);
    }
    #endregion
}