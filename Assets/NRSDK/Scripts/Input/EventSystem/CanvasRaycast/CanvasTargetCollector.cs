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
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    
    /// <summary> The canvas target collector. </summary>
    public class CanvasTargetCollector : MonoBehaviour
    {
        /// <summary> The canvases. </summary>
        private static readonly List<ICanvasRaycastTarget> canvases = new List<ICanvasRaycastTarget>();

        /// <summary> Adds a target. </summary>
        /// <param name="obj"> The object.</param>
        public static void AddTarget(ICanvasRaycastTarget obj)
        {
            if(obj != null)
                canvases.Add(obj);
        }

        /// <summary> Removes the target described by obj. </summary>
        /// <param name="obj"> The object.</param>
        public static void RemoveTarget(ICanvasRaycastTarget obj)
        {
            if (obj != null)
                canvases.Remove(obj);
        }

        /// <summary> Gets the canvases. </summary>
        /// <returns> The canvases. </returns>
        public static List<ICanvasRaycastTarget> GetCanvases()
        {
            return canvases;
        }
    }
    
}