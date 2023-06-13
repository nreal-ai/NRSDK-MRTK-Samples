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
using NRKernal;

[CustomEditor(typeof(NRProjectConfig))]
public class NRProjectConfigEditor : Editor
{
	override public void OnInspectorGUI()
	{
		NRProjectConfig projectConfig = (NRProjectConfig)target;
		DrawUniqueProjectConfig(projectConfig);
		EditorGUILayout.Space();
	}

	public static void DrawUniqueProjectConfig(NRProjectConfig projectConfig)
    {
		//Target Devices properties
		EditorGUILayout.LabelField("Target Devices", EditorStyles.boldLabel);
		EditorGUILayout.BeginVertical(EditorStyles.helpBox);
		bool modify = false;
		foreach (NRDeviceType deviceType in System.Enum.GetValues(typeof(NRDeviceType)))
		{
			bool curSupport = projectConfig.targetDeviceTypes.Contains(deviceType);
			bool newSupport = curSupport;
			NREditorUtility.BoolField(projectConfig, ObjectNames.NicifyVariableName(deviceType.ToString()), ref newSupport, ref modify);

			if (newSupport && !curSupport)
			{
				projectConfig.targetDeviceTypes.Add(deviceType);
			}
			else if (curSupport && !newSupport)
			{
				projectConfig.targetDeviceTypes.Remove(deviceType);
			}
		}
		EditorGUILayout.EndVertical();

		if (modify)
		{
			NRProjectConfigHelper.CommitProjectConfig(projectConfig);
		}
	}
}