/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal
{
    using UnityEngine;

    /// <summary>
    /// The class parses the touch position of smart phone to usable states by invoking parsing
    /// method every frame. </summary>
    public class NRPhoneControllerStateParser : IControllerStateParser
    {
        /// <summary> The buttons down. </summary>
        private bool[] _buttons_down = new bool[3];
        /// <summary> The buttons up. </summary>
        private bool[] _buttons_up = new bool[3];
        /// <summary> The buttons. </summary>
        private bool[] _buttons = new bool[3];
        private bool[] _lastButtons = new bool[3];
        /// <summary> The down. </summary>
        private bool[] _down = new bool[3];
        /// <summary> The touch. </summary>
        private Vector2 _touch;
        /// <summary> True to physical button. </summary>
        private bool _physical_button;
        /// <summary> The current down button. </summary>
        private int _current_down_btn = -1;
        /// <summary> True to last physical button. </summary>
        private bool _last_physical_button;
        /// <summary> The precision. </summary>
        private const float PRECISION = 0.000001f;

        /// <summary> Parser controller state. </summary>
        /// <param name="state"> The state.</param>
        public void ParserControllerState(ControllerState state)
        {
            _last_physical_button = _physical_button;
            _physical_button = (Mathf.Abs(state.touchPos.x) > PRECISION || Mathf.Abs(state.touchPos.y) > PRECISION);

            var sysbtnState = NRVirtualDisplayer.SystemButtonState;
            if (NRVirtualDisplayer.displayMode == NRVirtualDisplayer.DisplayMode.AndroidNative)
            {
                lock (sysbtnState)
                {
                    lock (_buttons)
                    {
                        for (int i = 0; i < sysbtnState.buttons.Length; i++)
                        {
                            _buttons[i] = sysbtnState.buttons[i];
                        }
                    }

                    lock (_buttons_up)
                    {
                        lock (_buttons_down)
                        {
                            for (int i = 0; i < _buttons.Length; ++i)
                            {
                                _buttons_up[i] = (_lastButtons[i] & !_buttons[i]);
                                _buttons_down[i] = (!_lastButtons[i] & _buttons[i]);
                            }
                        }
                    }

                    lock (_lastButtons)
                    {
                        for (int i = 0; i < _lastButtons.Length; i++)
                        {
                            _lastButtons[i] = _buttons[i];
                        }
                    }
                    state.touchPos = NRVirtualDisplayer.SystemButtonState.touch;
                }
            }
            else
            {
                lock (sysbtnState)
                {
                    lock (_buttons)
                    {
                        lock (_down)
                        {
                            for (int i = 0; i < _buttons.Length; ++i)
                            {
                                _down[i] = _buttons[i];
                            }
                        }

                        if (_current_down_btn != -1)
                        {
                            _buttons[_current_down_btn] = _physical_button;
                            if (!_buttons[_current_down_btn])
                                _current_down_btn = -1;
                        }
                        else
                        {
                            _buttons[0] = false;  //Trigger
                            _buttons[1] = false;  //App
                            _buttons[2] = false;  //Home

                            for (int i = 0; i < sysbtnState.buttons.Length; i++)
                            {
                                _buttons[i] = sysbtnState.buttons[i];
                            }

                            _current_down_btn = -1;
                            for (int i = 0; i < 3; i++)
                            {
                                if (_buttons[i])
                                {
                                    _current_down_btn = i;
                                    break;
                                }
                            }
                        }

                        lock (_buttons_up)
                        {
                            lock (_buttons_down)
                            {
                                for (int i = 0; i < _buttons.Length; ++i)
                                {
                                    _buttons_up[i] = (_down[i] & !_buttons[i]);
                                    _buttons_down[i] = (!_down[i] & _buttons[i]);
                                }
                            }
                        }

                        NRVirtualDisplayer.SystemButtonState.originTouch = state.touchPos;
                        NRVirtualDisplayer.SystemButtonState.pressing = _physical_button;
                        NRVirtualDisplayer.SystemButtonState.pressDown = (_physical_button && !_last_physical_button);
                        NRVirtualDisplayer.SystemButtonState.pressUp = (!_physical_button && _last_physical_button);
                        state.touchPos = NRVirtualDisplayer.SystemButtonState.touch;
                    }
                }
            }

            state.isTouching = _buttons[0];
            if (!state.isTouching && !_buttons_up[0])
            {
                state.touchPos = Vector2.zero;
            }

            state.buttonsState =
               (_buttons[0] ? ControllerButton.TRIGGER : 0)
               | (_buttons[1] ? ControllerButton.APP : 0)
               | (_buttons[2] ? ControllerButton.HOME : 0);
            state.buttonsDown =
                (_buttons_down[0] ? ControllerButton.TRIGGER : 0)
                | (_buttons_down[1] ? ControllerButton.APP : 0)
                | (_buttons_down[2] ? ControllerButton.HOME : 0);
            state.buttonsUp =
                (_buttons_up[0] ? ControllerButton.TRIGGER : 0)
                | (_buttons_up[1] ? ControllerButton.APP : 0)
                | (_buttons_up[2] ? ControllerButton.HOME : 0);
        }
    }
}
