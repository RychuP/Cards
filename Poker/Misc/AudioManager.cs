using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace Poker.Misc;

/// <summary>
/// Component that manages audio playback for all sounds.
/// </summary>
public class AudioManager : IDisposable
{
    public Game Game { get; }

    static readonly string s_soundAssetLocation = "Sounds/";

    // Audio Data        
    Dictionary<string, SoundEffectInstance> _soundBank = new();

    public AudioManager(Game game)
    {
        Game = game;
        LoadSounds();
    }

    /// <summary>
    /// Loads a single sound into the sound manager, giving it a specified alias.
    /// </summary>
    /// <param name="contentName">The content name of the sound file. Assumes all sounds are located under
    /// the "Sounds" folder in the content project.</param>
    /// <param name="alias">Alias to give the sound. This will be used to identify the sound uniquely.</param>
    /// <remarks>Loading a sound with an alias that is already used will have no effect.</remarks>
    public void LoadSound(string contentName, string alias)
    {
        SoundEffect soundEffect = Game.Content.Load<SoundEffect>(s_soundAssetLocation + contentName);
        SoundEffectInstance soundEffectInstance = soundEffect.CreateInstance();

        if (!_soundBank.ContainsKey(alias))
            _soundBank.Add(alias, soundEffectInstance);
    }

    /// <summary>
    /// Loads and organizes the sounds used by the game.
    /// </summary>
    public void LoadSounds()
    {
        LoadSound("CardsShuffle", "Shuffle");
    }

    /// <summary>
    /// Indexer. Return a sound instance by name.
    /// </summary>
    public SoundEffectInstance this[string soundName]
    {
        get
        {
            if (_soundBank.ContainsKey(soundName))
                return _soundBank[soundName];
            return null;
        }
    }

    /// <summary>
    /// Plays a sound by name.
    /// </summary>
    /// <param name="soundName">The name of the sound to play.</param>
    public void PlaySound(string soundName)
    {
        // If the sound exists, start it
        if (_soundBank.ContainsKey(soundName))
            _soundBank[soundName].Play();
    }

    /// <summary>
    /// Plays a sound by name.
    /// </summary>
    /// <param name="soundName">The name of the sound to play.</param>
    /// <param name="isLooped">Indicates if the sound should loop.</param>
    public void PlaySound(string soundName, bool isLooped)
    {
        // If the sound exists, start it
        if (_soundBank.ContainsKey(soundName))
        {
            if (_soundBank[soundName].IsLooped != isLooped)
                _soundBank[soundName].IsLooped = isLooped;

            _soundBank[soundName].Play();
        }
    }


    /// <summary>
    /// Plays a sound by name.
    /// </summary>
    /// <param name="soundName">The name of the sound to play.</param>
    /// <param name="isLooped">Indicates if the sound should loop.</param>
    /// <param name="volume">Indicates if the volume</param>
    public void PlaySound(string soundName, bool isLooped, float volume)
    {
        // If the sound exists, start it
        if (_soundBank.ContainsKey(soundName))
        {
            if (_soundBank[soundName].IsLooped != isLooped)
                _soundBank[soundName].IsLooped = isLooped;

            _soundBank[soundName].Volume = volume;
            _soundBank[soundName].Play();
        }
    }

    /// <summary>
    /// Stops a sound mid-play. If the sound is not playing, this
    /// method does nothing.
    /// </summary>
    /// <param name="soundName">The name of the sound to stop.</param>
    public void StopSound(string soundName)
    {
        // If the sound exists, stop it
        if (_soundBank.ContainsKey(soundName))
            _soundBank[soundName].Stop();
    }

    /// <summary>
    /// Stops all currently playing sounds.
    /// </summary>
    public void StopSounds()
    {
        foreach (SoundEffectInstance sound in _soundBank.Values)
            if (sound.State != SoundState.Stopped)
                sound.Stop();
    }

    /// <summary>
    /// Pause or resume all sounds.
    /// </summary>
    /// <param name="resumeSounds">True to resume all paused sounds or false
    /// to pause all playing sounds.</param>
    public void PauseResumeSounds(bool resumeSounds)
    {
        SoundState state = resumeSounds ? SoundState.Paused : SoundState.Playing;

        foreach (SoundEffectInstance sound in _soundBank.Values)
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

    /// <summary>
    /// Clean up the component when it is disposing.
    /// </summary>
    public void Dispose()
    {
        foreach (var item in _soundBank)
            item.Value.Dispose();

        _soundBank.Clear();
        _soundBank = null;
    }
}