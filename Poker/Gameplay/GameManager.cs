using Framework.Engine;
using Framework.Misc;
using Framework.UI;
using Poker.Gameplay.Players;
using Poker.UI;
using System;

namespace Poker.Gameplay;

class GameManager : CardGame
{
    readonly CardPile _cardPile;
    readonly CommunityCards _communityCards;
    readonly Hand _usedCards = new();

    public PokerGameState State { get; set; }

    public GameManager(Game game) : base(1, 0, CardSuits.AllSuits, CardValues.NonJokers,
        Constants.MinPlayers, Constants.MaxPlayers, new PokerTable(game), Constants.DefaultTheme, game)
    {
        // create card pile
        _cardPile = new CardPile(this);
        game.Components.Add(_cardPile);

        // create players
        AddPlayer(new HumanPlayer(GetRandomName(), Gender.Male, this));
        for (int place = 1; place < MaximumPlayers; place++)
            AddPlayer(new AIPlayer(GetRandomName(), (Gender)(place % 2), place, this));

        // create community cards cards holder
        _communityCards = new CommunityCards(this);
    }

    public void Update()
    {
        switch (State)
        {
            case PokerGameState.Shuffling:
                break;

            case PokerGameState.Dealing:
                if (!CheckForRunningAnimations<AnimatedCardGameComponent>())
                {
                    State = PokerGameState.FirstBet;
                    _cardPile.Hide();
                }
                break;

            case PokerGameState.FirstBet:
                if (!CheckForRunningAnimations<CardPile>())
                {
                    
                }
                    
                break;
        }
    }

    public override void StartPlaying()
    {
        State = PokerGameState.Shuffling;

        Dealer.Shuffle();
        _cardPile.ShowAndShuffle();
    }

    public override void Deal()
    {
        State = PokerGameState.Dealing;
        
        DateTime startTime = DateTime.Now;

        // deal player cards
        for (int dealIndex = 0; dealIndex < 2; dealIndex++)
        {
            //TimeSpan dealAnimationOffset = TimeSpan.FromSeconds(
            //    Constants.DealAnimationDuration.TotalSeconds * (dealIndex * Players.Count));
            for (int playerIndex = 0; playerIndex < Players.Count; playerIndex++)
            {
                var player = Players[playerIndex] as PokerCardsHolder;
                TraditionalCard card = Dealer.DealCardToHand(player.Hand);
                bool flip = player is HumanPlayer;

                // calculate start time and add deal animation
                player.AddDealAnimation(card, flip, startTime);
                startTime += Constants.DealAnimationDuration;
            }
        }

        // deal community cards
        for (int i = 0; i < 3; i++)
        {
            TraditionalCard card = Dealer.DealCardToHand(_communityCards.Hand);
            _communityCards.AddDealAnimation(card, true, startTime);
            startTime += Constants.DealAnimationDuration;
        }
    }

    /// <summary>
    /// Checks for running animations.
    /// </summary>
    /// <typeparam name="T">The type of animation to look for.</typeparam>
    /// <returns>True if a running animation of the desired type is found and
    /// false otherwise.</returns>
    internal bool CheckForRunningAnimations<T>() where T : AnimatedGameComponent
    {
        for (int componentIndex = 0; componentIndex < Game.Components.Count; componentIndex++)
            if (Game.Components[componentIndex] is T animComponent && animComponent.IsAnimating)
                return true;
        return false;
    }

    /// <summary>
    /// Selects a random, unique name of an alternating gender.
    /// </summary>
    /// <returns></returns>
    string GetRandomName()
    {
        string name;

        // alternate gender
        Gender gender = (Gender)(Players.Count % 2);

        // get the count of 50% of the names (first half is male, second female)
        int nameCount = Constants.Names.Length / 2;
        do
        {
            // get random index taking into consideration the appropriate half of collection
            int offset = gender == Gender.Male ? 0 : nameCount;
            int index = ((PokerGame)Game).Random.Next(0, nameCount);

            // retrieve the name and cap index (just in case)
            name = Constants.Names[Math.Min(index + offset, Constants.Names.Length - 1)];

            // repeat until a unique name is selected
        } while (Players.Find(p => p.Name == name) is PokerCardsHolder);

        return name;
    }

    /// <summary>
    /// Changes card theme for the game.
    /// </summary>
    /// <param name="theme">New card theme.</param>
    public void SetTheme(string theme)
    {
        if (Theme == theme) return;
        else if (theme != Constants.RedThemeText && theme != Constants.BlueThemeText) return;
        else
        {
            Theme = theme;
            OnThemeChanged(theme);
        }
    }

    /// <summary>
    /// Adds a player to the game.
    /// </summary>
    /// <param name="player">The player to add.</param>
    public override void AddPlayer(Player player)
    {
        if (player is PokerCardsHolder && Players.Count < MaximumPlayers)
            Players.Add(player);
    }

    public override void CheckRules()
    {
        base.CheckRules();
    }

    public override Player GetCurrentPlayer()
    {
        throw new NotImplementedException();
    }

    void OnThemeChanged(string theme)
    {
        ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(theme));
    }

    public event EventHandler<ThemeChangedEventArgs> ThemeChanged;
}