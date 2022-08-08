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
	[CustomEditor(typeof(NRSessionBehaviour))]
	public class NRSessionBehaviourEditor : Editor
	{
		SerializedProperty LogLevel;
		SerializedProperty SessionConfig;

		private void OnEnable() 
		{
	        // Setup the SerializedProperties
			LogLevel = serializedObject.FindProperty("LogLevel");
			SessionConfig = serializedObject.FindProperty("SessionConfig");
        }
		override public void OnInspectorGUI()
		{
			NRSessionBehaviour sessionBehav = (NRSessionBehaviour)target;
			serializedObject.Update();

			EditorGUILayout.PropertyField(LogLevel);
			EditorGUILayout.PropertyField(SessionConfig);
			EditorGUILayout.Space();

			// if (GUILayout.Button("Open NRSDK Configuration"))
			// {
			// 	Selection.activeObject = NRProjectConfigHelper.GetProjectConfig();
			// }

			// Apply values to the target
			serializedObject.ApplyModifiedProperties();

			//Provide link to the unique NRProjectConfig
			if (GUILayout.Button("Open NRProjectConfig"))
			{
				Selection.activeObject = (sessionBehav.SessionConfig).GlobalProjectConfig;
			}
		}
	}
}