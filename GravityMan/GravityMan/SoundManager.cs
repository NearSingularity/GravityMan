using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GravityMan
{
    public static class SoundManager
    {
        static AudioEngine engine;
        static WaveBank waveBank;
        static SoundBank soundBank;
        static Cue music;

        public static void Initialize(Game game, string engineFile, string waveBankFile, string soundBankFile)
        {
            string contentDir = game.Content.RootDirectory + "/";
            engine = new AudioEngine(contentDir + engineFile);
            waveBank = new WaveBank(engine, contentDir + waveBankFile);
            soundBank = new SoundBank(engine, contentDir + soundBankFile);
        }

        public static void PlayCue(string name)
        {
            soundBank.PlayCue(name);
        }

        public static void PlayMusic(string name)
        {
            music = soundBank.GetCue(name);
            music.Play();
        }

        public static void StopMusic()
        {
            if (music != null && music.IsPlaying)
            {
                music.Stop(AudioStopOptions.Immediate);
                music = null;
            }
        }

        public static void SetSoundFXVolume(float volume)
        {
            engine.GetCategory("SoundFx").SetVolume(volume);
        }

        public static void SetMusicVolume(float volume)
        {
            engine.GetCategory("Music").SetVolume(volume);
        }

        public static void Update()
        {
            engine.Update();
        }
    }
}
