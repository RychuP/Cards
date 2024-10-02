using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Framework.Engine;
using Framework.UI;
using Framework.Assets;
using Blackjack.Misc;
using Blackjack.UI.ScreenElements;
using Blackjack.UI.Components;
using Blackjack.UI;
using Blackjack.Gameplay.Players;

namespace Blackjack.Gameplay;

public class BetGameComponent : DrawableGameComponent
{
    readonly List<Player> _players;
    readonly CardGame _cardGame;
    Vector2[] _positions = new Vector2[ChipAssets.ValueChips.Count];
    bool isKeyDown = false;
    Button _bet;
    Button _clear;
    static readonly float _insuranceYPosition = 120 * BlackjackGame.HeightScale;
    static Vector2 _secondHandOffset = new(25 * BlackjackGame.WidthScale, 30 * BlackjackGame.HeightScale);
    readonly List<AnimatedGameComponent> _currentChipComponent = new();
    int _currentBet = 0;

    /// <summary>
    /// Creates a new instance of the <see cref="BetGameComponent"/> class.
    /// </summary>
    /// <param name="players">A list of participating players.</param>
    /// <param name="input">An instance of 
    /// <see cref="InputState"/> which can be used to 
    /// check user input.</param>
    /// <param name="theme">The name of the selcted card theme.</param>
    /// <param name="cardGame">An instance of <see cref="CardGame"/> which
    /// is the current game.</param>
    public BetGameComponent(List<Player> players, CardGame cardGame) : base(cardGame.Game) =>
        (_players, _cardGame) = (players, cardGame);

    /// <summary>
    /// Initializes the component.
    /// </summary>
    public override void Initialize()
    {
        // Show mouse
        Game.IsMouseVisible = true;
        base.Initialize();

        // Calculate chips position for the chip buttons which allow placing the bet
        Rectangle size = ChipAssets.ValueChips[ChipAssets.Values[0]].Bounds;

        Rectangle bounds = Game.GraphicsDevice.Viewport.TitleSafeArea;

        _positions[ChipAssets.ValueChips.Count - 1] = new Vector2(bounds.Left + 10,
            bounds.Bottom - size.Height - 80);
        for (int chipIndex = 2; chipIndex <= ChipAssets.ValueChips.Count; chipIndex++)
        {
            size = ChipAssets.ValueChips[ChipAssets.Values[ChipAssets.ValueChips.Count - chipIndex]].Bounds;
            _positions[ChipAssets.ValueChips.Count - chipIndex] = _positions[ChipAssets.ValueChips.Count 
                - (chipIndex - 1)] - new Vector2(0, size.Height + 10);
        }

        // Initialize bet button
        _bet = new Button(_cardGame)
        {
            Bounds = new Rectangle(bounds.Left + 10, bounds.Bottom - 60, 100, 50),
            Text = "Deal",
        };
        _bet.Click += BetButton_OnClick;
        Game.Components.Add(_bet);

        // Initialize clear button
        _clear = new Button(_cardGame)
        {
            Bounds = new Rectangle(bounds.Left + 120, bounds.Bottom - 60, 100, 50),
            Text = "Clear",
        };
        _clear.Click += ClearButton_OnClick;
        Game.Components.Add(_clear);
        ShowAndEnableButtons(false);
    }

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
            if (((BlackjackPlayer)_players[^1]).IsDoneBetting)
            {
                BlackjackCardGame blackjackGame = (BlackjackCardGame)_cardGame;

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
        for (int chipIndex = 0; chipIndex < ChipAssets.ValueChips.Count; chipIndex++)
        {
            // Calculate the bounds of the asset
            size = ChipAssets.ValueChips[ChipAssets.Values[chipIndex]].Bounds;
            size.X = (int)_positions[chipIndex].X;
            size.Y = (int)_positions[chipIndex].Y;
            if (size.Intersects(touchTap))
            {
                return ChipAssets.Values[chipIndex];
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
        var sb = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
        sb.Begin();

        // Draws the chips
        for (int chipIndex = 0; chipIndex < ChipAssets.ValueChips.Count; chipIndex++)
        {
            sb.Draw(ChipAssets.ValueChips[ChipAssets.Values[chipIndex]], _positions[chipIndex],
                Color.White);
        }

        BlackjackPlayer player;

        // Draws the player balance and bet amount
        for (int playerIndex = 0; playerIndex < _players.Count; playerIndex++)
        {
            BlackJackTable table = (BlackJackTable)_cardGame.GameTable;
            Vector2 position = table[playerIndex] + table.RingOffset +
                new Vector2(Art.Ring.Bounds.Width, 0);
            player = (BlackjackPlayer)_players[playerIndex];
            sb.DrawString(Fonts.Moire.Regular, "$" + player.BetAmount.ToString(),
                position, Color.White);
            sb.DrawString(Fonts.Moire.Regular, "$" + player.Balance.ToString(),
                position + new Vector2(0, 30), Color.White);
        }

        sb.End();

        base.Draw(gameTime);
    }

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
            AnimatedGameComponent chipComponent = new(_cardGame, ChipAssets.ValueChips[chipValue])
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
            for (int chipIndex = 0; chipIndex < ChipAssets.ValueChips.Count; chipIndex++)
            {
                if (ChipAssets.Values[chipIndex] == chipValue)
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
        CardSounds.Bet.Play();
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
                for (int chipIndex = assetNames.Length - 1; chipIndex > 0; chipIndex--)
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
        AnimatedGameComponent chipComponent = new(_cardGame, ChipAssets.BlankChips["white"])
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
        CardSounds.Bet.Play();
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

        BlackJackTable table = (BlackJackTable)_cardGame.GameTable;
        offset = table.RingOffset +
            new Vector2(Art.Ring.Bounds.Width - ChipAssets.BlankChips["White"].Bounds.Width,
                Art.Ring.Bounds.Height - ChipAssets.BlankChips["White"].Bounds.Height) / 2f;

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

    /// <summary>
    /// Handles the Click event of the Clear button.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The 
    /// <see cref="EventArgs"/> instance containing the event data.</param>
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
    /// <see cref="EventArgs"/> instance containing the event data.</param>
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
}