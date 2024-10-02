using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.IO;

namespace Framework.Assets;

public static class CardSounds
{
    public static SoundEffectInstance Deal { get; private set; }
    public static SoundEffectInstance Shuffle { get; private set; }
    public static SoundEffectInstance Flip { get; private set; }
    public static SoundEffectInstance Bet { get; private set; }

    public static void Initialize(Game game)
    {
        Deal = LoadSound("Deal");
        Shuffle = LoadSound("Shuffle");
        Flip = LoadSound("Flip");
        Bet = LoadSound("Bet");

        SoundEffectInstance LoadSound(string soundName)
        {
            SoundEffect soundEffect = game.Content.Load<SoundEffect>(Path.Combine("Sounds", soundName));
            return soundEffect.CreateInstance();
        }
    }
}