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
    using UnityEngine.EventSystems;

    
    /// <summary> A nr execute pointer events. </summary>
    public class NRExecutePointerEvents
    {

        /// <summary> The press enter handler. </summary>
        public static readonly ExecuteEvents.EventFunction<IEventSystemHandler> PressEnterHandler = ExecuteEnter;
        /// <summary> Executes the 'enter' operation. </summary>
        /// <param name="handler">   The handler.</param>
        /// <param name="eventData"> Information describing the event.</param>
        private static void ExecuteEnter(IEventSystemHandler handler, BaseEventData eventData)
        {

        }

        /// <summary> The press exit handler. </summary>
        public static readonly ExecuteEvents.EventFunction<IEventSystemHandler> PressExitHandler = ExecuteExit;
        /// <summary> Executes the 'exit' operation. </summary>
        /// <param name="handler">   The handler.</param>
        /// <param name="eventData"> Information describing the event.</param>
        private static void ExecuteExit(IEventSystemHandler handler, BaseEventData eventData)
        {

        }
    }
    
}
