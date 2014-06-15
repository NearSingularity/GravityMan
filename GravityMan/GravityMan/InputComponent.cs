//----------------------------------------------------------------------------------------
//	Copyright 2009-2011 Matt Whiting, All Rights Reserved.
//  For educational purposes only.
//  Please do not distribute or republish in electronic or print form without permission.
//  Thanks - CrashLotus@gmail.com
//----------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


/// <summary>
/// This is the InputComponent.
/// Route all input detection through here
/// </summary>
public class InputComponent : GameComponent
{
    // Statics
    static InputComponent s_theInput;

    KeyboardState keyCurrent;
    KeyboardState keyLast;
    GamePadState[] padCurrent;
    GamePadState[] padLast;

    public InputComponent(Game game)
        : base(game)
    {
        Debug.Assert(s_theInput == null, "You cannot construct more than one InputComponent");
        keyCurrent = Keyboard.GetState();
        keyLast = keyCurrent;
        padCurrent = new GamePadState[4];
        padLast = new GamePadState[4];
        s_theInput = this;
    }

    /// <summary>
    /// Allows the game component to perform any initialization it needs to before starting
    /// to run.  This is where it can query for any required services and load content.
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();
    }

    /// <summary>
    /// Reads the current state of the keyboard
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    public override void Update(GameTime gameTime)
    {
        // TODO: Add your update code here
        keyLast = keyCurrent;
        keyCurrent = Keyboard.GetState();

        for (int i = 0; i < 4; ++i)
        {
            padLast[i] = padCurrent[i];
            padCurrent[i] = GamePad.GetState((PlayerIndex)i);
        }

        base.Update(gameTime);
    }

    /// <summary>
    /// Returns the InputComponent - use this to gain access to it from anywhere in your program
    /// "Singleton" pattern.
    /// </summary>
    static public InputComponent Get()
    {
        return s_theInput;
    }

    /// <summary>
    /// Returns true if the "key" is currently being held down
    /// </summary>
    /// <param name="key">The key to be tested.</param>
    public bool IsKeyDown(Keys key)
    {
        return keyCurrent.IsKeyDown(key);
    }

    /// <summary>
    /// Returns true if the "key" has just now been hit (leading edge test).
    /// </summary>
    /// <param name="key">The key to be tested.</param>
    public bool IsKeyHit(Keys key)
    {
        return keyCurrent.IsKeyDown(key) && keyLast.IsKeyUp(key);
    }

    /// <summary>
    /// Returns true if the "key" has just now been released (trailing edge test).
    /// </summary>
    /// <param name="key">The key to be tested.</param>
    public bool IsKeyRelease(Keys key)
    {
        return keyCurrent.IsKeyUp(key) && keyLast.IsKeyDown(key);
    }

    /// <summary>
    /// Returns true if the "button" is currently being held down
    /// </summary>
    /// <param name="index">The index of the controller to check.</param>
    /// <param name="button">The button to be tested.</param>
    public bool IsButtonDown(PlayerIndex index, Buttons button)
    {
        return padCurrent[(int)index].IsButtonDown(button);
    }

    /// <summary>
    /// Returns true if the "button" has just now been hit (leading edge test).
    /// </summary>
    /// <param name="index">The index of the controller to check.</param>
    /// <param name="button">The button to be tested.</param>
    public bool IsButtonHit(PlayerIndex index, Buttons button)
    {
        return padCurrent[(int)index].IsButtonDown(button) && padLast[(int)index].IsButtonUp(button);
    }

    /// <summary>
    /// Returns true if the "button" has just now been released (trailing edge test).
    /// </summary>
    /// <param name="index">The index of the controller to check.</param>
    /// <param name="button">The button to be tested.</param>
    public bool IsButtonRelease(PlayerIndex index, Buttons button)
    {
        return padCurrent[(int)index].IsButtonUp(button) && padLast[(int)index].IsButtonDown(button);
    }

    /// <summary>
    /// Returns the Left Stick
    /// </summary>
    /// <param name="index">The index of the controller to check.</param>
    public Vector2 GetLeftStick(PlayerIndex index)
    {
        return padCurrent[(int)index].ThumbSticks.Left;
    }

    /// <summary>
    /// Returns the Right Stick
    /// </summary>
    /// <param name="index">The index of the controller to check.</param>
    public Vector2 GetRightStick(PlayerIndex index)
    {
        return padCurrent[(int)index].ThumbSticks.Right;
    }

    /// <summary>
    /// Returns the Left Trigger
    /// </summary>
    /// <param name="index">The index of the controller to check.</param>
    public float GetLeftTrigger(PlayerIndex index)
    {
        return padCurrent[(int)index].Triggers.Left;
    }

    /// <summary>
    /// Returns the Right Trigger
    /// </summary>
    /// <param name="index">The index of the controller to check.</param>
    public float GetRightTrigger(PlayerIndex index)
    {
        return padCurrent[(int)index].Triggers.Right;
    }

    /// <summary>
    /// Returns true if the Left Trigger has just been hit
    /// </summary>
    /// <param name="index">The index of the controller to check.</param>
    public bool LeftTriggerHit(PlayerIndex index)
    {
        return (padCurrent[(int)index].Triggers.Left > 0.0f) && (padLast[(int)index].Triggers.Left == 0.0f);
    }

    /// <summary>
    /// Returns true if the Right Trigger has just been hit
    /// </summary>
    /// <param name="index">The index of the controller to check.</param>
    public bool RightTriggerHit(PlayerIndex index)
    {
        return (padCurrent[(int)index].Triggers.Right > 0.0f) && (padLast[(int)index].Triggers.Right == 0.0f);
    }
}