//  Copyright (C) 2022 Andrew Booze <42682253+awbooze@users.noreply.github.com>
//
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;

namespace SpaceMiner.Input
{
    /// <summary>
    /// Represents a binding from various types of buttons to an action
    /// </summary>
    public class InputAction
    {
        private readonly Buttons[] _buttons;
        private readonly Keys[] _keys;
        private readonly MouseButton[] _mouseButtons;
        private readonly bool _firstPressOnly;

        // These delegate types map to the methods on InputState. We use these to simplify the Occurred method
        // by allowing us to map the appropriate delegates and invoke them, rather than having two separate code paths.
        private delegate bool ButtonPress(Buttons button);
        private delegate bool KeyPress(Keys key);
        private delegate bool MouseButtonPress(MouseButton button);

        /// <summary>
        /// Constructs a new InputMapping by binding the supplied input options to the action
        /// </summary>
        /// <param name="triggerButtons">The buttons that trigger this action</param>
        /// <param name="triggerKeys">The keys that trigger this action</param>
        /// <param name="triggerMouseButtons">The mouse buttons which trigger this action</param>
        /// <param name="firstPressOnly">If this action only triggers on the initial key/button press</param>
        public InputAction(Buttons[] triggerButtons, Keys[] triggerKeys, MouseButton[] triggerMouseButtons, bool firstPressOnly)
        {
            // Store the buttons and keys. If the arrays are null, we create a 0 length array so we don't
            // have to do null checks in the Occurred method
            _buttons = triggerButtons != null ? triggerButtons.Clone() as Buttons[] : new Buttons[0];
            _keys = triggerKeys != null ? triggerKeys.Clone() as Keys[] : new Keys[0];
            _mouseButtons = triggerMouseButtons != null ? triggerMouseButtons.Clone() as MouseButton[] : new MouseButton[0];
            _firstPressOnly = firstPressOnly;
        }

        /// <summary>
        /// Determines whether any button, key, or mouse button presses have occurred that would 
        /// cause this InputAction to trigger.
        /// </summary>
        /// <param name="stateToTest">The current input state</param>
        /// <returns>true if this InputAction should trigger, false otherwise</returns>
        public bool Occurred(InputState stateToTest)
        {
            // Figure out which delegate methods to map from the state
            ButtonPress buttonTest;
            KeyPress keyTest;
            MouseButtonPress mouseTest;

            if (_firstPressOnly)
            {
                buttonTest = stateToTest.IsNewGamePadButtonPress;
                keyTest = stateToTest.CurrentKeyboardState.WasKeyJustUp;
                mouseTest = stateToTest.CurrentMouseState.WasButtonJustUp;
            }
            else
            {
                buttonTest = stateToTest.IsGamePadButtonPressed;
                keyTest = stateToTest.CurrentKeyboardState.IsKeyDown;
                mouseTest = stateToTest.CurrentMouseState.IsButtonDown;
            }

            // Now we simply need to invoke the appropriate methods for each button and key in our collections
            foreach (var button in _buttons)
            {
                if (buttonTest(button))
                {
                    return true;
                }
            }
            foreach (var key in _keys)
            {
                if (keyTest(key))
                {
                    return true;
                }
            }
            foreach (var mouseButton in _mouseButtons)
            {
                if (mouseTest(mouseButton))
                {
                    return true;
                }
            }

            // If we got here, the action is not matched
            return false;
        }
    }
}
