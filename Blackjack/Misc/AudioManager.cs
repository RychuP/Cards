#region File Description
//-----------------------------------------------------------------------------
// AudioManager.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Blackjack;

/// <summary>
/// Component that manages audio playback for all sounds.
/// </summary>
public class AudioManager : GameComponent
{
    #region Fields
    /// <summary>
    /// The singleton for this type.
    /// </summary>
    static AudioManager s_audioManager = null;
    public static AudioManager Instance =>
        s_audioManager;

    static readonly string s_soundAssetLocation = "Sounds/";

    // Audio Data        
    Dictionary<string, SoundEffectInstance> _soundBank = new();
    #endregion

    #region Initialization
    private AudioManager(Game game) : base(game) { }

    /// <summary>
    /// Initialize the static AudioManager functionality.
    /// </summary>
    /// <param name="game">The game that this component will be attached to.</param>
    public static void Initialize(Game game)
    {
        s_audioManager = new(game);
        game.Components.Add(s_audioManager);
    }
    #endregion

    #region Loading Methods
    /// <summary>
    /// Loads a single sound into the sound manager, giving it a specified alias.
    /// </summary>
    /// <param name="contentName">The content name of the sound file. Assumes all sounds are located under
    /// the "Sounds" folder in the content project.</param>
    /// <param name="alias">Alias to give the sound. This will be used to identify the sound uniquely.</param>
    /// <remarks>Loading a sound with an alias that is already used will have no effect.</remarks>
    public static void LoadSound(string contentName, string alias)
    {
        SoundEffect soundEffect = s_audioManager.Game.Content.Load<SoundEffect>(s_soundAssetLocation + contentName);
        SoundEffectInstance soundEffectInstance = soundEffect.CreateInstance();

        if (!s_audioManager._soundBank.ContainsKey(alias))
            s_audioManager._soundBank.Add(alias, soundEffectInstance);
    }

    /// <summary>
    /// Loads and organizes the sounds used by the game.
    /// </summary>
    public static void LoadSounds()
    {
        LoadSound("Bet", "Bet");
        LoadSound("CardFlip", "Flip");
        LoadSound("CardsShuffle", "Shuffle");
        LoadSound("Deal", "Deal");
    }
    #endregion

    #region Sound Methods
    /// <summary>
    /// Indexer. Return a sound instance by name.
    /// </summary>
    public SoundEffectInstance this[string soundName]
    {
        get
        {
            if (s_audioManager._soundBank.ContainsKey(soundName))
                return s_audioManager._soundBank[soundName];
            return null;
        }
    }

    /// <summary>
    /// Plays a sound by name.
    /// </summary>
    /// <param name="soundName">The name of the sound to play.</param>
    public static void PlaySound(string soundName)
    {
        // If the sound exists, start it
        if (s_audioManager._soundBank.ContainsKey(soundName))
            s_audioManager._soundBank[soundName].Play();
    }

    /// <summary>
    /// Plays a sound by name.
    /// </summary>
    /// <param name="soundName">The name of the sound to play.</param>
    /// <param name="isLooped">Indicates if the sound should loop.</param>
    public static void PlaySound(string soundName, bool isLooped)
    {
        // If the sound exists, start it
        if (s_audioManager._soundBank.ContainsKey(soundName))
        {
            if (s_audioManager._soundBank[soundName].IsLooped != isLooped)
                s_audioManager._soundBank[soundName].IsLooped = isLooped;

            s_audioManager._soundBank[soundName].Play();
        }
    }


    /// <summary>
    /// Plays a sound by name.
    /// </summary>
    /// <param name="soundName">The name of the sound to play.</param>
    /// <param name="isLooped">Indicates if the sound should loop.</param>
    /// <param name="volume">Indicates if the volume</param>
    public static void PlaySound(string soundName, bool isLooped, float volume)
    {
        // If the sound exists, start it
        if (s_audioManager._soundBank.ContainsKey(soundName))
        {
            if (s_audioManager._soundBank[soundName].IsLooped != isLooped)
                s_audioManager._soundBank[soundName].IsLooped = isLooped;

            s_audioManager._soundBank[soundName].Volume = volume;
            s_audioManager._soundBank[soundName].Play();
        }
    }

    /// <summary>
    /// Stops a sound mid-play. If the sound is not playing, this
    /// method does nothing.
    /// </summary>
    /// <param name="soundName">The name of the sound to stop.</param>
    public static void StopSound(string soundName)
    {
        // If the sound exists, stop it
        if (s_audioManager._soundBank.ContainsKey(soundName))
            s_audioManager._soundBank[soundName].Stop();
    }

    /// <summary>
    /// Stops all currently playing sounds.
    /// </summary>
    public static void StopSounds()
    {
        foreach (SoundEffectInstance sound in s_audioManager._soundBank.Values)
            if (sound.State != SoundState.Stopped)
                sound.Stop();
    }

    /// <summary>
    /// Pause or resume all sounds.
    /// </summary>
    /// <param name="resumeSounds">True to resume all paused sounds or false
    /// to pause all playing sounds.</param>
    public static void PauseResumeSounds(bool resumeSounds)
    {
        SoundState state = resumeSounds ? SoundState.Paused : SoundState.Playing;

        foreach (SoundEffectInstance sound in s_audioManager._soundBank.Values)
        {
            if (sound.State == state)
            {
                if (resumeSounds)
                    sound.Resume();
                else
                    sound.Pause();
            }
        }
    }
    #endregion

    #region Instance Disposal Methods
    /// <summary>
    /// Clean up the component when it is disposing.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        try
        {
            if (disposing)
            {
                foreach (var item in _soundBank)
                    item.Value.Dispose();

                _soundBank.Clear();
                _soundBank = null;
            }
        }
        finally
        {
            base.Dispose(disposing);
        }
    }
    #endregion
}