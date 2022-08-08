/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Experimental
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(NRDisplayOverlay))]
    public class NRDisplayOverlayEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            NRDisplayOverlay overlay = (NRDisplayOverlay)target;
            if (overlay == null)
            {
                return;
            }

            overlay.targetEye = (NativeDevice)EditorGUILayout.EnumPopup(new GUIContent("Target Eye", "Which display this overlay should render to."), overlay.targetEye);
        }
    }
}
