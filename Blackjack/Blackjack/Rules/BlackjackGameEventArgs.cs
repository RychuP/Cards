#region File Description
//-----------------------------------------------------------------------------
// BlackjackGameEventArgs.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using CardsFramework;

namespace Blackjack;

public class BlackjackGameEventArgs : EventArgs
{
    public Player Player { get; set; }
    public HandTypes Hand { get; set; }
}