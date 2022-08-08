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
    using System;
    using UnityEngine;

    /// <summary>
    /// The class parses the raw states of Nreal Controller to usable states by invoking
    /// parsing method every frame. </summary>
    public class NrealLightControllerStateParser : IControllerStateParser
    {
        /// <summary> Values that represent touch area enums. </summary>
        private enum TouchAreaEnum
        {
            /// <summary> An enum constant representing the none option. </summary>
            None,
            /// <summary> An enum constant representing the center option. </summary>
            Center,
            /// <summary> An enum constant representing the up option. </summary>
            Up,
            /// <summary> An enum constant representing the down option. </summary>
            Down,
            /// <summary> An enum constant representing the left option. </summary>
            Left,
            /// <summary> An enum constant representing the right option. </summary>
            Right
        }

        /// <summary> The buttons down. </summary>
        private bool[] _buttons_down = new bool[3];
        /// <summary> The buttons up. </summary>
        private bool[] _buttons_up = new bool[3];
        /// <summary> The buttons. </summary>
        private bool[] _buttons = new bool[3];
        /// <summary> The down. </summary>
        private bool[] _down = new bool[3];
        /// <summary> The touch. </summary>
        private Vector2 _touch;
        /// <summary> The current touch area. </summary>
        private TouchAreaEnum _currentTouchArea = TouchAreaEnum.None;
        /// <summary> True to touch status. </summary>
        private bool _touch_status;
        /// <summary> True to physical button. </summary>
        private bool _physical_button;
        /// <summary> True to physical button down. </summary>
        private bool _physical_button_down;
        /// <summary> The current down button. </summary>
        private int _current_down_btn = -1;

        /// <summary> The second parameters sqrt. </summary>
        private const float Params_Sqrt_2 = 1.414f;
        /// <summary> Length of the center half side. </summary>
        private const float CenterHalfSideLength = 0.9f / Params_Sqrt_2;
        /// <summary> Length of the ok half side. </summary>
        private const float OKHalfSideLength = 0.9f / Params_Sqrt_2 * 0.5f;
        /// <summary> The precision. </summary>
        private const float PRECISION = 0.000001f;

        /// <summary> Parser controller state. </summary>
        /// <param name="state"> The state.</param>
        public void ParserControllerState(ControllerState state)
        {
            try
            {
                _touch_status = (Mathf.Abs(state.touchPos.x) > PRECISION || Mathf.Abs(state.touchPos.y) > PRECISION);
                if (!_touch_status)
                {
                    _touch.x = 0f;
                    _touch.y = 0f;
                }
                else
                {
                    _touch.x = state.touchPos.x;
                    _touch.y = state.touchPos.y;
                }

                UpdateCurrentTouchArea();
                lock (_buttons)
                {
                    lock (_down)
                    {
                        for (int i = 0; i < _buttons.Length; ++i)
                        {
                            _down[i] = _buttons[i];
                        }
                    }

                    _physical_button_down = _physical_button;
                    _physical_button = state.GetButton(ControllerButton.TRIGGER);

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

                        bool _is_down = !_physical_button_down & _physical_button;
                        if (_currentTouchArea == TouchAreaEnum.Center)
                            _buttons[0] = _physical_button && _is_down;
                        else if (_currentTouchArea == TouchAreaEnum.Up)
                            _buttons[1] = _physical_button && _is_down;
                        else if (_currentTouchArea == TouchAreaEnum.Down)
                            _buttons[2] = _physical_button && _is_down;

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
                }
            }
            catch (Exception e)
            {
                NRDebugger.Error("Controller Data Error :" + e.ToString());
            }

            state.isTouching = _touch_status;
            state.touchPos = _touch;
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

        /// <summary> Updates the current touch area. </summary>
        private void UpdateCurrentTouchArea()
        {
            _currentTouchArea = TouchAreaEnum.None;
            if (!_touch_status)
                return;
            if (_touch.y < -CenterHalfSideLength * 0.9f)
                _currentTouchArea = TouchAreaEnum.Down;
            else if (_touch.y > CenterHalfSideLength * 0.8f)
                _currentTouchArea = TouchAreaEnum.Up;
            else
                _currentTouchArea = TouchAreaEnum.Center;
        }
    }
}
