#region File Description
//-----------------------------------------------------------------------------
// BetGameComponent.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CardsFramework;
using GameStateManagement;

namespace Blackjack;

public class BetGameComponent : DrawableGameComponent
{
    #region Fields and Properties
    readonly List<Player> _players;
    readonly int[] _assetNames = { 5, 25, 100, 500 };
    readonly Dictionary<int, Texture2D> _chipsAssets = new();
    readonly CardsGame _cardGame;
    Texture2D _blankChip;
    Vector2[] _positions;
    SpriteBatch _spriteBatch;

    bool isKeyDown = false;

    Button _bet;
    Button _clear;

    static readonly float _insuranceYPosition = 120 * BlackjackGame.HeightScale;
    static Vector2 _secondHandOffset = new (25 * BlackjackGame.WidthScale, 30 * BlackjackGame.HeightScale);

    readonly List<AnimatedGameComponent> _currentChipComponent = new();
    int _currentBet = 0;

    readonly InputState _input;
    InputHelper _inputHelper;
    #endregion

    #region Initializations
    /// <summary>
    /// Creates a new instance of the <see cref="BetGameComponent"/> class.
    /// </summary>
    /// <param name="players">A list of participating players.</param>
    /// <param name="input">An instance of 
    /// <see cref="InputState"/> which can be used to 
    /// check user input.</param>
    /// <param name="theme">The name of the selcted card theme.</param>
    /// <param name="cardGame">An instance of <see cref="CardsGame"/> which
    /// is the current game.</param>
    public BetGameComponent(List<Player> players, InputState input, CardsGame cardGame)
        : base(cardGame.Game) =>
        (_players, _cardGame, _input) = (players, cardGame, input);

    /// <summary>
    /// Initializes the component.
    /// </summary>
    public override void Initialize()
    {
        // Get xbox cursor
        _inputHelper = null;
        for (int componentIndex = 0; componentIndex < Game.Components.Count; componentIndex++)
        {
            if (Game.Components[componentIndex] is InputHelper helper)
            {
                _inputHelper = helper;
                break;
            }
        }

        // Show mouse
        Game.IsMouseVisible = true;
        base.Initialize();

        _spriteBatch = new SpriteBatch(Game.GraphicsDevice);

        // Calculate chips position for the chip buttons which allow placing the bet
        Rectangle size = _chipsAssets[_assetNames[0]].Bounds;

        Rectangle bounds = _spriteBatch.GraphicsDevice.Viewport.TitleSafeArea;

        _positions[_chipsAssets.Count - 1] = new Vector2(bounds.Left + 10,
            bounds.Bottom - size.Height - 80);
        for (int chipIndex = 2; chipIndex <= _chipsAssets.Count; chipIndex++)
        {
            size = _chipsAssets[_assetNames[_chipsAssets.Count - chipIndex]].Bounds;
            _positions[_chipsAssets.Count - chipIndex] = _positions[_chipsAssets.Count - (chipIndex - 1)] -
                new Vector2(0, size.Height + 10);
        }

        // Initialize bet button
        _bet = new Button("ButtonRegular", "ButtonPressed", _input, _cardGame)
        {
            Bounds = new Rectangle(bounds.Left + 10, bounds.Bottom - 60, 100, 50),
            Font = _cardGame.Font,
            Text = "Deal",
        };
        _bet.Click += BetButton_OnClick;
        Game.Components.Add(_bet);

        // Initialize clear button
        _clear = new Button("ButtonRegular", "ButtonPressed", _input, _cardGame)
        {
            Bounds = new Rectangle(bounds.Left + 120, bounds.Bottom - 60, 100, 50),
            Font = _cardGame.Font,
            Text = "Clear",
        };
        _clear.Click += ClearButton_OnClick;
        Game.Components.Add(_clear);
        ShowAndEnableButtons(false);
    }
    #endregion

    #region Loading
    /// <summary>
    /// Load component content.
    /// </summary>
    protected override void LoadContent()
    {
        // Load blank chip texture
        _blankChip = Game.Content.Load<Texture2D>(
            string.Format(@"Images\Chips\chip{0}", "White"));

        // Load chip textures
        int[] assetNames = { 5, 25, 100, 500 };
        for (int chipIndex = 0; chipIndex < assetNames.Length; chipIndex++)
        {
            _chipsAssets.Add(assetNames[chipIndex], Game.Content.Load<Texture2D>(
                string.Format(@"Images\Chips\chip{0}", assetNames[chipIndex])));
        }
        _positions = new Vector2[assetNames.Length];

        base.LoadContent();
    }
    #endregion

    #region Update and Render
    /// <summary>
    /// Perform update logic related to the component.
    /// </summary>
    /// <param name="gameTime">Time elapsed since the last call to 
    /// this method.</param>
    public override void Update(GameTime gameTime)
    {
        if (_players.Count > 0)
        {
            // If betting is possible
            if (((BlackjackCardGame)_cardGame).State == BlackjackGameState.Betting &&
                !((BlackjackPlayer)_players[^1]).IsDoneBetting)
            {
                int playerIndex = GetCurrentPlayer();

                BlackjackPlayer player = (BlackjackPlayer)_players[playerIndex];

                // If the player is an AI player, have it bet
                if (player is BlackjackAIPlayer aiPlayer)
                {
                    ShowAndEnableButtons(false);
                    int bet = aiPlayer.AIBet();
                    if (bet == 0)
                    {
                        BetButton_OnClick(this, EventArgs.Empty);
                    }
                    else
                    {
                        AddChip(playerIndex, bet, false);
                    }
                }
                else
                {
                    // Reveal the input buttons for a human player and handle input
                    // remember that buttons handle their own imput, so we only check
                    // for input on the chip buttons
                    ShowAndEnableButtons(true);

                    HandleInput(Mouse.GetState());
                }
            }

            // Once all players are done betting, advance the game to the dealing stage
            if (((BlackjackPlayer)_players[_players.Count - 1]).IsDoneBetting)
            {
                BlackjackCardGame blackjackGame = ((BlackjackCardGame)_cardGame);

                if (!blackjackGame.CheckForRunningAnimations<AnimatedGameComponent>())
                {
                    ShowAndEnableButtons(false);
                    blackjackGame.State = BlackjackGameState.Dealing;

                    Enabled = false;
                }
            }
        }

        base.Update(gameTime);
    }

    /// <summary>
    /// Gets the player which is currently betting. This is the first player who has
    /// yet to finish betting.
    /// </summary>
    /// <returns>The player which is currently betting.</returns>
    private int GetCurrentPlayer()
    {
        for (int playerIndex = 0; playerIndex < _players.Count; playerIndex++)
        {
            if (!((BlackjackPlayer)_players[playerIndex]).IsDoneBetting)
            {
                return playerIndex;
            }
        }
        return -1;
    }

    /// <summary>
    /// Handle the input of adding chip on all platform
    /// </summary>
    /// <param name="mouseState">Mouse input information.</param>
    private void HandleInput(MouseState mouseState)
    {
        bool isPressed = false;
        Vector2 position = Vector2.Zero;

        if (mouseState.LeftButton == ButtonState.Pressed)
        {
            isPressed = true;
            position = new Vector2(mouseState.X, mouseState.Y);
        }
        else if (_inputHelper.IsPressed)
        {
            isPressed = true;
            position = _inputHelper.PointPosition;
        }

        if (isPressed)
        {
            if (!isKeyDown)
            {
                int chipValue = GetIntersectingChipValue(position);
                if (chipValue != 0)
                {
                    AddChip(GetCurrentPlayer(), chipValue, false);
                }
                isKeyDown = true;
            }
        }
        else
        {
            isKeyDown = false;
        }
    }

    /// <summary>
    /// Get which chip intersects with a given position.
    /// </summary>
    /// <param name="position">The position to check for intersection.</param>
    /// <returns>The value of the chip intersecting with the specified position, or
    /// 0 if no chips intersect with the position.</returns>
    private int GetIntersectingChipValue(Vector2 position)
    {
        Rectangle size;
        // Calculate the bounds of the position
        Rectangle touchTap = new Rectangle((int)position.X - 1,
            (int)position.Y - 1, 2, 2);
        for (int chipIndex = 0; chipIndex < _chipsAssets.Count; chipIndex++)
        {
            // Calculate the bounds of the asset
            size = _chipsAssets[_assetNames[chipIndex]].Bounds;
            size.X = (int)_positions[chipIndex].X;
            size.Y = (int)_positions[chipIndex].Y;
            if (size.Intersects(touchTap))
            {
                return _assetNames[chipIndex];
            }
        }

        return 0;
    }

    /// <summary>
    /// Draws the component
    /// </summary>
    /// <param name="gameTime">Time passed since the last call to 
    /// this method.</param>
    public override void Draw(GameTime gameTime)
    {
        _spriteBatch.Begin();

        // Draws the chips
        for (int chipIndex = 0; chipIndex < _chipsAssets.Count; chipIndex++)
        {
            _spriteBatch.Draw(_chipsAssets[_assetNames[chipIndex]], _positions[chipIndex],
                Color.White);
        }

        BlackjackPlayer player;

        // Draws the player balance and bet amount
        for (int playerIndex = 0; playerIndex < _players.Count; playerIndex++)
        {
            BlackJackTable table = (BlackJackTable)_cardGame.GameTable;
            Vector2 position = table[playerIndex] + table.RingOffset +
                new Vector2(table.RingTexture.Bounds.Width, 0);
            player = (BlackjackPlayer)_players[playerIndex];
            _spriteBatch.DrawString(_cardGame.Font, "$" + player.BetAmount.ToString(),
                position, Color.White);
            _spriteBatch.DrawString(_cardGame.Font, "$" + player.Balance.ToString(),
                position + new Vector2(0, 30), Color.White);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Adds the chip to one of the player betting zones.
    /// </summary>
    /// <param name="playerIndex">Index of the player for whom to add 
    /// a chip.</param>
    /// <param name="chipValue">The value on the chip to add.</param>
    /// <param name="secondHand">True if this chip is added to the chip pile
    /// belonging to the player's second hand.</param>
    public void AddChip(int playerIndex, int chipValue, bool secondHand)
    {
        // Only add the chip if the bet is successfully performed
        if (((BlackjackPlayer)_players[playerIndex]).Bet(chipValue))
        {
            _currentBet += chipValue;
            // Add chip component
            AnimatedGameComponent chipComponent = new(_cardGame, _chipsAssets[chipValue])
            {
                Visible = false
            };

            Game.Components.Add(chipComponent);

            // Get the proper offset according to the platform (pc, phone, xbox)
            Vector2 offset = GetChipOffset(secondHand);

            // Calculate the position for the new chip
            Vector2 position = _cardGame.GameTable[playerIndex] + offset +
                new Vector2(-_currentChipComponent.Count * 2, _currentChipComponent.Count * 1);

            // Find the index of the chip
            int currentChipIndex = 0;
            for (int chipIndex = 0; chipIndex < _chipsAssets.Count; chipIndex++)
            {
                if (_assetNames[chipIndex] == chipValue)
                {
                    currentChipIndex = chipIndex;
                    break;
                }
            }

            // Add transition animation
            chipComponent.AddAnimation(new TransitionGameComponentAnimation(
                _positions[currentChipIndex], position)
            {
                Duration = TimeSpan.FromSeconds(1f),
                PerformBeforeStart = ShowComponent,
                PerformBeforeStartArgs = chipComponent,
                PerformWhenDone = PlayBetSound
            });

            // Add flip animation
            chipComponent.AddAnimation(new FlipGameComponentAnimation()
            {
                Duration = TimeSpan.FromSeconds(1f),
                AnimationCycles = 3,
            });

            _currentChipComponent.Add(chipComponent);
        }
    }

    /// <summary>
    /// Helper method to show component
    /// </summary>
    /// <param name="obj"></param>
    void ShowComponent(object obj)
    {
        ((AnimatedGameComponent)obj).Visible = true;
    }

    /// <summary>
    /// Helper method to play bet sound
    /// </summary>
    /// <param name="obj"></param>
    void PlayBetSound(object obj)
    {
        AudioManager.PlaySound("Bet");
    }

    /// <summary>
    /// Adds chips to a specified player.
    /// </summary>
    /// <param name="playerIndex">Index of the player.</param>
    /// <param name="amount">The total amount to add.</param>
    /// <param name="insurance">If true, an insurance chip is added instead of
    /// regular chips.</param>
    /// <param name="secondHand">True if chips are to be added to the player's
    /// second hand.</param>
    public void AddChips(int playerIndex, float amount, bool insurance, bool secondHand)
    {
        if (insurance)
        {
            AddInsuranceChipAnimation(amount);
        }
        else
        {
            AddChips(playerIndex, amount, secondHand);
        }
    }

    /// <summary>
    /// Resets this instance.
    /// </summary>
    public void Reset()
    {
        ShowAndEnableButtons(true);
        _currentChipComponent.Clear();
    }

    /// <summary>
    /// Updates the balance of all players in light of their bets and the dealer's
    /// hand.
    /// </summary>
    /// <param name="dealerPlayer">Player object representing the dealer.</param>
    public void CalculateBalance(BlackjackPlayer dealerPlayer)
    {
        for (int playerIndex = 0; playerIndex < _players.Count; playerIndex++)
        {
            BlackjackPlayer player = (BlackjackPlayer)_players[playerIndex];

            // Calculate first factor, which represents the amount of the first
            // hand bet which returns to the player
            float factor = CalculateFactorForHand(dealerPlayer, player,
                HandTypes.First);


            if (player.IsSplit)
            {
                // Calculate the return factor for the second hand
                float factor2 = CalculateFactorForHand(dealerPlayer, player,
                    HandTypes.Second);
                // Calculate the initial bet performed by the player
                float initialBet =
                    player.BetAmount /
                    ((player.Double ? 2f : 1f) + (player.SecondDouble ? 2f : 1f));

                float bet1 = initialBet * (player.Double ? 2f : 1f);
                float bet2 = initialBet * (player.SecondDouble ? 2f : 1f);

                // Update the balance in light of the bets and results
                player.Balance += bet1 * factor + bet2 * factor2;

                if (player.IsInsurance && dealerPlayer.BlackJack)
                {
                    player.Balance += initialBet;
                }
            }
            else
            {
                if (player.IsInsurance && dealerPlayer.BlackJack)
                {
                    player.Balance += player.BetAmount;
                }

                // Update the balance in light of the bets and results
                player.Balance += player.BetAmount * factor;
            }

            player.ClearBet();
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Adds chips to a specified player in order to reach a specified bet amount.
    /// </summary>
    /// <param name="playerIndex">Index of the player to whom the chips are to
    /// be added.</param>
    /// <param name="amount">The bet amount to add to the player.</param>
    /// <param name="secondHand">True to add the chips to the player's second
    /// hand, false to add them to the first hand.</param>
    private void AddChips(int playerIndex, float amount, bool secondHand)
    {
        int[] assetNames = { 5, 25, 100, 500 };

        while (amount > 0)
        {
            if (amount >= 5)
            {
                // Add the chip with the highest possible value
                for (int chipIndex = assetNames.Length; chipIndex > 0; chipIndex--)
                {
                    int chip = assetNames[chipIndex];
                    while (chip <= amount)
                    {
                        AddChip(playerIndex, chip, secondHand);
                        amount -= chip;
                    }
                }
            }
            else
                amount = 0;
        }
    }

    /// <summary>
    /// Animates the placement of an insurance chip on the table.
    /// </summary>
    /// <param name="amount">The amount which should appear on the chip.</param>
    private void AddInsuranceChipAnimation(float amount)
    {
        // Add chip component
        AnimatedGameComponent chipComponent = new(_cardGame, _blankChip)
        {
            TextColor = Color.Black,
            Enabled = true,
            Visible = false
        };

        Game.Components.Add(chipComponent);

        // Add transition animation
        chipComponent.AddAnimation(new TransitionGameComponentAnimation(_positions[0],
            new Vector2(GraphicsDevice.Viewport.Width / 2, _insuranceYPosition))
        {
            PerformBeforeStart = ShowComponent,
            PerformBeforeStartArgs = chipComponent,
            PerformWhenDone = ShowChipAmountAndPlayBetSound,
            PerformWhenDoneArgs = new object[] { chipComponent, amount },
            Duration = TimeSpan.FromSeconds(1),
            StartTime = DateTime.Now
        });

        // Add flip animation
        chipComponent.AddAnimation(new FlipGameComponentAnimation()
        {
            Duration = TimeSpan.FromSeconds(1f),
            AnimationCycles = 3,
        });
    }

    /// <summary>
    /// Helper method to show the amount on the chip and play bet sound
    /// </summary>
    /// <param name="obj"></param>
    void ShowChipAmountAndPlayBetSound(object obj)
    {
        object[] arr = (object[])obj;
        ((AnimatedGameComponent)arr[0]).Text = arr[1].ToString();
        AudioManager.PlaySound("Bet");
    }

    /// <summary>
    /// Gets the offset at which newly added chips should be placed.
    /// </summary>
    /// <param name="playerIndex">Index of the player to whom the chip 
    /// is added.</param>
    /// <param name="secondHand">True if the chip is added to the player's second
    /// hand, false otherwise.</param>
    /// <returns>The offset from the player's position where chips should be
    /// placed.</returns>
    private Vector2 GetChipOffset(bool secondHand)
    {
        Vector2 offset;

        BlackJackTable table = ((BlackJackTable)_cardGame.GameTable);
        offset = table.RingOffset +
            new Vector2(table.RingTexture.Bounds.Width - _blankChip.Bounds.Width,
                table.RingTexture.Bounds.Height - _blankChip.Bounds.Height) / 2f;

        if (secondHand == true)
            offset += _secondHandOffset;

        return offset;
    }

    /// <summary>
    /// Show and enable, or hide and disable, the bet related buttons.
    /// </summary>
    /// <param name="visibleEnabled">True to show and enable the buttons, false
    /// to hide and disable them.</param>
    private void ShowAndEnableButtons(bool visibleEnabled)
    {
        _bet.Visible = visibleEnabled;
        _bet.Enabled = visibleEnabled;
        _clear.Visible = visibleEnabled;
        _clear.Enabled = visibleEnabled;
    }

    /// <summary>
    /// Returns a factor which determines how much of a bet a player should get 
    /// back, according to the outcome of the round.
    /// </summary>
    /// <param name="dealerPlayer">The player representing the dealer.</param>
    /// <param name="player">The player for whom we calculate the factor.</param>
    /// <param name="currentHand">The hand to calculate the factor for.</param>
    /// <returns></returns>
    static float CalculateFactorForHand(BlackjackPlayer dealerPlayer,
        BlackjackPlayer player, HandTypes currentHand)
    {
        float factor;

        bool blackjack, bust, considerAce;
        int playerValue;
        player.CalculateValues();

        // Get some player status information according to the desired hand
        switch (currentHand)
        {
            case HandTypes.First:
                blackjack = player.BlackJack;
                bust = player.Bust;
                playerValue = player.FirstValue;
                considerAce = player.FirstValueConsiderAce;
                break;
            case HandTypes.Second:
                blackjack = player.SecondBlackJack;
                bust = player.SecondBust;
                playerValue = player.SecondValue;
                considerAce = player.SecondValueConsiderAce;
                break;
            default:
                throw new Exception(
                    "Player has an unsupported hand type.");
        }

        if (considerAce)
        {
            playerValue += 10;
        }


        if (bust)
        {
            factor = -1; // Bust
        }
        else if (dealerPlayer.Bust)
        {
            if (blackjack)
            {
                factor = 1.5f; // Win BlackJack
            }
            else
            {
                factor = 1; // Win
            }
        }
        else if (dealerPlayer.BlackJack)
        {
            if (blackjack)
            {
                factor = 0; // Push BlackJack
            }
            else
            {
                factor = -1; // Lose BlackJack
            }
        }
        else if (blackjack)
        {
            factor = 1.5f;
        }
        else
        {
            int dealerValue = dealerPlayer.FirstValue;

            if (dealerPlayer.FirstValueConsiderAce)
            {
                dealerValue += 10;
            }

            if (playerValue > dealerValue)
            {
                factor = 1; // Win
            }
            else if (playerValue < dealerValue)
            {
                factor = -1; // Lose
            }
            else
            {
                factor = 0; // Push
            }
        }
        return factor;
    }
    #endregion

    #region Event Handlers
    /// <summary>
    /// Handles the Click event of the Clear button.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The 
    /// <see cref="System.EventArgs"/> instance containing the event data.</param>
    void ClearButton_OnClick(object sender, EventArgs e)
    {
        // Clear current player chips from screen and resets his bet
        _currentBet = 0;
        ((BlackjackPlayer)_players[GetCurrentPlayer()]).ClearBet();
        for (int chipComponentIndex = 0; chipComponentIndex < _currentChipComponent.Count; chipComponentIndex++)
        {
            Game.Components.Remove(_currentChipComponent[chipComponentIndex]);
        }
        _currentChipComponent.Clear();
    }

    /// <summary>
    /// Handles the Click event of the Bet button.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The 
    /// <see cref="System.EventArgs"/> instance containing the event data.</param>
    void BetButton_OnClick(object sender, EventArgs e)
    {
        // Finish the bet
        int playerIndex = GetCurrentPlayer();
        // If the player did not bet, show that he has passed on this round
        if (_currentBet == 0)
        {
            ((BlackjackCardGame)_cardGame).ShowPlayerPass(playerIndex);
        }
        ((BlackjackPlayer)_players[playerIndex]).IsDoneBetting = true;
        _currentChipComponent.Clear();
        _currentBet = 0;
    }
    #endregion
}