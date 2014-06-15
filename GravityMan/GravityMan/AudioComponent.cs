   //----------------------------------------------------------------------------------------
//	Copyright 2009-2011 Matt Whiting, All Rights Reserved.
//  For educational purposes only.
//  Please do not distribute or republish in electronic or print form without permission.
//  Thanks - CrashLotus@gmail.com
//----------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

using AudioHandle = StaticPool<AudioCue>.Handle;


/// <summary>
/// This is the AudioComponent.
/// All sound effects requests come through here.
/// 
/// Requires StaticPool.cs so be sure to include that too
/// </summary>
public class AudioComponent : GameComponent
{
    // Constants
    const int MAX_AUDIO_CUE = 256;

    // Statics
    static AudioComponent s_theAudio;

    // XACT objects.
    string m_filename;
    AudioEngine m_audioEngine;
    WaveBank m_waveBank;
    SoundBank m_soundBank;
    AudioListener m_listener;

    // pool of all AudioCues
    StaticPool<AudioCue> m_cuePool;

    public AudioComponent(Game game, string audioFile)
        : base(game)
    {
        Debug.Assert(s_theAudio == null, "You can only construct one AudioComponent");
        s_theAudio = this;
        m_filename = audioFile;
        m_cuePool = new StaticPool<AudioCue>(MAX_AUDIO_CUE);
        m_listener = new AudioListener();
    }

    /// <summary>
    /// Allows the game component to perform any initialization it needs to before starting
    /// to run.  This is where it can query for any required services and load content.
    /// </summary>
    public override void Initialize()
    {
        m_audioEngine = new AudioEngine("Content/" + m_filename + ".xgs");

        base.Initialize();
    }

    /// <summary>
    /// Allows the game component to update itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    public override void Update(GameTime gameTime)
    {
        // Update the XACT engine.
        m_audioEngine.Update();

        // Go through all the AudioCues looking for any that are finished
        for (UInt16 i = 0; i < MAX_AUDIO_CUE; ++i)
        {
            AudioHandle handle = m_cuePool.GetHandleByIndex(i);
            AudioCue cue = m_cuePool.GetItem(handle);
            if (null != cue)
            {   // this cue is alive
                if (AudioCue.State.PLAYING == cue.m_state)
                {   // it thinks it is playing
                    if (false == cue.m_cue.IsPlaying)
                    {   // the sound isn't playing anymore - must have stopped
                        cue.Free();
                        cue.m_state = AudioCue.State.AVAILABLE;
                        m_cuePool.Free(handle);
                    }
                }   //if (AudioCue.State.PLAYING == cue.state)
            }   //if (null != cue)
        }   //for (UInt16 i = 0; i < MAX_AUDIO_CUE; ++i)

        base.Update(gameTime);
    }

    /// <summary>
    /// Unloads the XACT data.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        try
        {
            if (disposing)
            {
                UnloadBank();
                m_audioEngine.Dispose();
            }
        }
        finally
        {
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Unloads the sound bank and the wave bank which are currently loaded
    /// to make room for a new one or just to shut down.
    /// </summary>
    protected void UnloadBank()
    {
        if (m_soundBank != null)
        {
            m_soundBank.Dispose();
            m_soundBank = null;
        }
        if (m_waveBank != null)
        {
            m_waveBank.Dispose();
            m_waveBank = null;
        }
    }

    /// <summary>
    /// Use this to gain access to the one and only AudioComponent from anywhere.
    /// "Singleton" pattern.
    /// </summary>
    static public AudioComponent Get()
    {
        return s_theAudio;
    }

    /// <summary>
    /// Load a new wave bank and sound bank.
    /// Only one of each bank can be loaded at a time.
    /// </summary>
    /// <param name="waveBankName">Name of the wave bank to load.</param>
    /// <param name="soundBankName">Name of the sound bank to load.</param>
    public void LoadBank(String waveBankName, String soundBankName)
    {
        UnloadBank();
        m_waveBank = new WaveBank(m_audioEngine, "Content/" + waveBankName + ".xwb");
        m_soundBank = new SoundBank(m_audioEngine, "Content/" + soundBankName + ".xsb");
    }

    /// <summary>
    /// Attempts to play the sound.
    /// Returns a Handle to the AudioCue that gets played.
    /// </summary>
    /// <param name="soundName">The name of the sound to play.</param>
    public AudioHandle PlaySound(String soundName)
    {
        AudioHandle handle = m_cuePool.Allocate();
        AudioCue cue = m_cuePool.GetItem(handle);
        if (null != cue)
        {
            cue.SetUp(m_soundBank, soundName);
            cue.Play();
        }
        return handle;
    }

    /// <summary>
    /// Attempts to play the sound with 3D effects.
    /// Returns a Handle to the AudioCue that gets played.
    /// </summary>
    /// <param name="soundName">The name of the sound to play.</param>
    public AudioHandle PlaySound3D(String soundName, AudioEmitter emitter)
    {
        AudioHandle handle = m_cuePool.Allocate();
        AudioCue cue = m_cuePool.GetItem(handle);
        if (null != cue)
        {
            cue.SetUp(m_soundBank, soundName);
            cue.m_cue.Apply3D(m_listener, emitter);
            cue.Play();
        }
        return handle;
    }

    /// <summary>
    /// Stops the specified sound
    /// </summary>
    /// <param name="handle">The Handle of the sound to stop.</param>
    public void StopSound(AudioHandle handle)
    {
        AudioCue cue = m_cuePool.GetItem(handle);
        if (null != cue)
        {
            cue.m_cue.Stop(AudioStopOptions.AsAuthored);
        }
    }

    /// <summary>
    /// Check whether or not the specified sound is currently playing.
    /// </summary>
    /// <param name="handle">The Handle of the sound to stop.</param>
    /// <returns>true if the sound is currently playing</returns>
    public bool IsPlaying(AudioHandle handle)
    {
        AudioCue cue = m_cuePool.GetItem(handle);
        if (null != cue)
        {
            return cue.m_cue.IsPlaying;
        }
        return false;
    }

    /// <summary>
    /// This function is used to move the "listener" for 3D audio.
    /// </summary>
    /// <param name="listenerMat">Give this a matrix where the listener is.  Most likely Camera.GetMatrix().</param>
    /// <param name="velocity">This this a vector describing how the listener is moving.</param>
    public void MoveListener(Matrix listenerMat, Vector3 velocity)
    {
        m_listener.Forward = listenerMat.Forward;
        m_listener.Up = listenerMat.Up;
        m_listener.Position = listenerMat.Translation;
        m_listener.Velocity = velocity;
    }

    /// <summary>
    /// Updates a 3D sound with its latest emitter data
    /// </summary>
    /// <param name="handle">A handle to the sound being played</param>
    /// <param name="emitter">The emitter that contains the position and velocity data</param>
    public void UpdateSound3D(AudioHandle handle, AudioEmitter emitter)
    {
        AudioCue cue = m_cuePool.GetItem(handle);
        if (null != cue)
        {
            cue.m_cue.Apply3D(m_listener, emitter);
        }    
    }
}

/// <summary>
/// This is an instance of a sound for use only by the AudioComponent
/// These are kept in a pool and accessed only by Handle
/// </summary>
public class AudioCue
{
    public enum State
    {
        AVAILABLE,
        SETUP,
        PLAYING,
    };

    public Cue m_cue;
    public State m_state;

    public AudioCue()
    {
        m_state = State.AVAILABLE;
    }

    /// <summary>
    /// Set the AudioCue up to be ready to play the specified sound.
    /// </summary>
    /// <param name="soundBank">The SoundBank to find the sound in.</param>
    /// <param name="soundName">The name of the sound to play.</param>
    public void SetUp(SoundBank soundBank, String soundName)
    {
        Debug.Assert(m_state == State.AVAILABLE);
        m_cue = soundBank.GetCue(soundName);
        m_state = State.SETUP;
    }

    /// <summary>
    /// Starts the sound playing.
    /// The AudioCue must first be SetUp()
    /// </summary>
    public void Play()
    {
        Debug.Assert(m_state == State.SETUP);
        m_cue.Play();
        m_state = State.PLAYING;
    }

    /// <summary>
    /// Marks this AudioCue as no longer in use
    /// </summary>
    public void Free()
    {
        Debug.Assert(m_state != State.AVAILABLE);
        Debug.Assert(false == m_cue.IsPlaying);
        m_cue.Dispose();
        m_state = State.AVAILABLE;
    }
}

