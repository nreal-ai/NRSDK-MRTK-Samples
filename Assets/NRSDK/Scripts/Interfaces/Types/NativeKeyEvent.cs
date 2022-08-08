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
    /// <summary> Key event Info. </summary>
    public struct NRKeyEventInfo
    {
        /// <summary> Physics key type of key event. </summary>
        public NRKeyType keyType;
        /// <summary> Key function of key event. </summary>
        public NRKeyFunction keyFunc;
        /// <summary> Key parameter of key event. </summary>
        public int keyParam;
    }

    /// <summary> Physics key type. </summary>
    public enum NRKeyType
    {
        /// <summary> None. </summary>
        NONE = 0,
        /// <summary> Multiple use key. </summary>
        MULTI_KEY = 1,
        /// <summary> Increase key. </summary>
        INCREASE_KEY,
        /// <summary> Decrease key. </summary>
        DECREASE_KEY,
    }

    /// <summary> Key function issued based on physics key. </summary>
    public enum NRKeyFunction
    {
        /// <summary> None. </summary>
        NONE = 0,
        /// <summary> Single click on physics key. </summary>
        CLICK = 1,
        /// <summary> Double click on physics key. </summary>
        DOUBLE_CLICK,
        /// <summary> Long press on physics key. </summary>
        LONG_PRESS,
        /// <summary> Open display while MULTI_KEY is pressed. </summary>
        OPEN_DISPLAY,
        /// <summary> Close display while MULTI_KEY is pressed. </summary>
        CLOSE_DISPLAY,
        /// <summary> Increase brightness while INCREASE_KEY is pressed. </summary>
        INCREASE_BRIGHTNESS,
        /// <summary> Decrease brightness while DECREASE_KEY is pressed. </summary>
        DECREASE_BRIGHTNESS,
    }

    /// <summary> Glass control key event delegate. </summary>
    /// <param name="keyEvtInfo"> The key event information.</param>
    public delegate void NRGlassControlKeyEvent(NRKeyEventInfo keyEvtInfo);
}