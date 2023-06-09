#region File Description
//-----------------------------------------------------------------------------
// Player.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

namespace CardsFramework;

/// <summary>
/// Represents base player to be extended by inheritance for each card game.
/// </summary>
public class Player
{
    #region Property
    public string Name { get; set; }
    public CardGame Game { get; set; }
    public Hand Hand { get; set; } 
    #endregion

    public Player(string name, CardGame game)
    {
        Name = name;
        Game = game;
        Hand = new Hand();
    }
}