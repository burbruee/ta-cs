using System;
using Microsoft.Xna.Framework.Audio;

namespace TROLLArena
{
    static class SoundHelper
    {
        static AudioEngine audioEngine;
        static WaveBank waveBank;
        static SoundBank soundBank;        
        
        static string[] sounds;

        public static void Initialize()
        {
            audioEngine = new AudioEngine(@"Content\Sounds\Audio.xgs");
                        
            waveBank = new WaveBank(audioEngine, @"Content\Sounds\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, @"Content\Sounds\Sound Bank.xsb");

            sounds = new string[] { "jallabertmrkruuuuk", "bonk", "gameover" };
        }

        public static void Update()
        {
            audioEngine.Update();
        }

        public static void PlaySound(int index)
        {
            soundBank.PlayCue(sounds[index]);            
        }

        public static void StopSound(int index)
        {
            soundBank.Dispose();
        }
    }
}
