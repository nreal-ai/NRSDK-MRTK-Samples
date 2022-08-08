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

    [CustomEditor(typeof(NROverlay))]
    public class NROverlayEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            NROverlay overlay = (NROverlay)target;
            if (overlay == null)
            {
                return;
            }

            EditorGUILayout.LabelField("Display Order", EditorStyles.boldLabel);
            overlay.compositionDepth = EditorGUILayout.IntField(new GUIContent("Composition Depth", "Depth value used to sort OVROverlays in the scene, smaller value appears in front"), overlay.compositionDepth);
            EditorGUILayout.Space();

            overlay.ActiveOnStart = EditorGUILayout.Toggle(new GUIContent("ActiveOnStart", "Whether active this overlay when script start"), overlay.ActiveOnStart);
            EditorGUILayout.Space();

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Textures", EditorStyles.boldLabel);
#if UNITY_ANDROID
            bool lastIsExternalSurface = overlay.isExternalSurface;
            overlay.isExternalSurface = EditorGUILayout.Toggle(new GUIContent("Is External Surface", "On Android, retrieve an Android Surface object to render to (e.g., video playback)"), overlay.isExternalSurface);
            if (overlay.isExternalSurface)
            {
                overlay.isDynamic = false;
            }

            if (lastIsExternalSurface)
            {
                overlay.externalSurfaceWidth = EditorGUILayout.IntField("External Surface Width", overlay.externalSurfaceWidth);
                overlay.externalSurfaceHeight = EditorGUILayout.IntField("External Surface Height", overlay.externalSurfaceHeight);
                overlay.isProtectedContent = EditorGUILayout.Toggle(new GUIContent("Is Protected Content", "The external surface has L1 widevine protection."), overlay.isProtectedContent);
            }
            else
#endif
            {
                var labelControlRect = EditorGUILayout.GetControlRect();
                EditorGUI.LabelField(new Rect(labelControlRect.x, labelControlRect.y, labelControlRect.width / 2, labelControlRect.height), new GUIContent("Texture", "Texture used for the left eye"));
                var textureControlRect = EditorGUILayout.GetControlRect(GUILayout.Height(64));
                overlay.texture = (Texture)EditorGUI.ObjectField(new Rect(textureControlRect.x, textureControlRect.y, 64, textureControlRect.height), overlay.texture, typeof(Texture), true);
                overlay.isDynamic = EditorGUILayout.Toggle(new GUIContent("Dynamic Texture", "This texture will be updated dynamically at runtime (e.g., Video)"), overlay.isDynamic);
            }

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Display Mode", EditorStyles.boldLabel);
            overlay.layerSide = (LayerSide)EditorGUILayout.EnumPopup(new GUIContent("LayerSide", "Which display this overlay should render to."), overlay.layerSide);
            overlay.isScreenSpace = EditorGUILayout.Toggle(new GUIContent("Screen Space", "Whether render this overlay as 0-dof."), overlay.isScreenSpace);
            if (overlay.isScreenSpace)
            {
                overlay.is3DLayer = false;
            }
            else
            {
                overlay.is3DLayer = EditorGUILayout.Toggle(new GUIContent("Is 3D rendering layer", "Whether this overlay is 3D rendering layer."), overlay.is3DLayer);
            }

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
            overlay.previewInEditor = EditorGUILayout.Toggle(new GUIContent("Preview in Editor (Experimental)", "Preview the overlay in the editor using a mesh renderer."), overlay.previewInEditor);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}
