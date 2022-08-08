/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/
using UnityEngine;
using UnityEditor;

namespace NRKernal
{
	[CustomEditor(typeof(NRSessionConfig))]
	public class NRSessionConfigEditor : Editor
	{
		SerializedProperty PlaneFindingMode;
		SerializedProperty ImageTrackingMode;
		SerializedProperty TrackingImageDatabase;
		SerializedProperty EnableNotification;
		SerializedProperty GlassesErrorTipPrefab;
		SerializedProperty TrackingModeChangeTipPrefab;
		SerializedProperty ProjectConfig;

		private void OnEnable() 
		{
			NRSessionConfig sessionConfig = (NRSessionConfig)target;
			sessionConfig.SetProjectConfig(NRProjectConfigHelper.GetProjectConfig());

	        // Setup the SerializedProperties
			PlaneFindingMode 			= serializedObject.FindProperty("PlaneFindingMode");
			ImageTrackingMode 			= serializedObject.FindProperty("ImageTrackingMode");
			TrackingImageDatabase 		= serializedObject.FindProperty("TrackingImageDatabase");
			EnableNotification 			= serializedObject.FindProperty("EnableNotification");
			GlassesErrorTipPrefab 		= serializedObject.FindProperty("GlassesErrorTipPrefab");
			TrackingModeChangeTipPrefab = serializedObject.FindProperty("TrackingModeChangeTipPrefab");
			ProjectConfig				= serializedObject.FindProperty("ProjectConfig");
        }
		override public void OnInspectorGUI()
		{
			NRSessionConfig sessionConfig = (NRSessionConfig)target;
			serializedObject.Update();

			// EditorGUILayout.PropertyField(ProjectConfig);
			NRProjectConfig projectConfig = NRProjectConfigHelper.GetProjectConfig();
			//NRProjectConfigEditor.DrawUniqueProjectConfig(projectConfig);
			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(PlaneFindingMode);
			EditorGUILayout.PropertyField(ImageTrackingMode);
			EditorGUILayout.PropertyField(TrackingImageDatabase);
			EditorGUILayout.PropertyField(EnableNotification);

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(GlassesErrorTipPrefab);
			EditorGUILayout.PropertyField(TrackingModeChangeTipPrefab);

			// Apply values to the target
			serializedObject.ApplyModifiedProperties();

			//Provide link to the unique NRProjectConfig
			if (GUILayout.Button("Open NRProjectConfig"))
			{
				Selection.activeObject = sessionConfig.GlobalProjectConfig;
			}
		}
	}
}