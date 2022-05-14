//  Copyright (C) 2022 Andrew Booze <42682253+awbooze@users.noreply.github.com>
//
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using System;

namespace SpaceMiner.Input
{
    /// <summary>
    /// A class which contains a state of inputs via any input method to the game.
    /// </summary>
    public class InputState
    {
        #region Input State Items
        /// <summary>
        /// The current tick's extended KeyboardState from MonoGame.Extended
        /// </summary>
        public KeyboardStateExtended CurrentKeyboardState { get; private set; }

        /// <summary>
        /// The current tick's extended MouseState from MonoGame.Extended
        /// </summary>
        public MouseStateExtended CurrentMouseState { get; private set; }

        /// <summary>
        /// The previous tick's GamePadState.
        /// </summary>
        public GamePadState PriorGamePadState { get; private set; }

        /// <summary>
        /// The current tick's GamePadState.
        /// </summary>
        public GamePadState CurrentGamePadState { get; private set; }

        public Point Position { get; private set; }
        #endregion

        public InputState()
        {

        }

        /// <summary>
        /// Reads the latest user input state.
        /// </summary>
        public void Update()
        {
            CurrentKeyboardState = KeyboardExtended.GetState();

            CurrentMouseState = MouseExtended.GetState();

            PriorGamePadState = CurrentGamePadState;
            CurrentGamePadState = GamePad.GetState(PlayerIndex.One);

            if (CurrentMouseState.PositionChanged)
            {
                Position = CurrentMouseState.Position;
            }
            else if (Math.Abs(CurrentGamePadState.ThumbSticks.Left.X) > 0.1 || Math.Abs(CurrentGamePadState.ThumbSticks.Left.Y) > 0.1)
            {
                Position += CurrentGamePadState.ThumbSticks.Left.ToPoint();
            }
        }

        /// <summary>
        /// Checks if a gamepad button is currently pressed.
        /// </summary>
        /// <param name="button">The button to check</param>
        /// <returns>true if the button is currently pressed, false otherwise</returns>
        public bool IsGamePadButtonPressed(Buttons button)
        {
            return CurrentGamePadState.IsButtonDown(button);
        }

        /// <summary>
        /// Checks of a gamepad button just started being pressed
        /// </summary>
        /// <param name="button">The button to check</param>
        /// <returns>true if the button just started to be pressed, false otherwise</returns>
        public bool IsNewGamePadButtonPress(Buttons button)
        {
            return IsGamePadButtonPressed(button) && 
                PriorGamePadState.IsButtonUp(button);
        }
    }
}
