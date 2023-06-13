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
	using System.IO;
	using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using LitJson;

#if UNITY_EDITOR
	[UnityEditor.InitializeOnLoad]
#endif
	public static class NRProjectConfigHelper
    {
		static NRProjectConfigHelper()
        {
			EditorApplication.update += OnceUpdate;
        }

		static void OnceUpdate()
		{
			var projectConfig = GetProjectConfig();
			// Avoid to import asset while beginning to run application in editor.
			if (!Application.isPlaying)
				ApplyProjectConfigToSessionConfig(projectConfig);
            EditorApplication.update -= OnceUpdate;
        }

		// Load projectConfig from default path, if it doesnot exist, create the asset.
        public static NRProjectConfig GetProjectConfig()
		{
			NRProjectConfig projectConfig = null;
			string projectConfigAssetPath = GetProjectConfigAssetPath();
			try
			{
				projectConfig = AssetDatabase.LoadAssetAtPath(projectConfigAssetPath, typeof(NRProjectConfig)) as NRProjectConfig;
			}
			catch (System.Exception e)
			{
				NRDebugger.Warning("Unable to load NRProjectConfig from {0}, error {1}", projectConfigAssetPath, e.Message);
			}
			// BuildPipeline.isBuildingPlayer cannot be called in static constructor
			if (projectConfig == null && !BuildPipeline.isBuildingPlayer)
			{
				projectConfig = ScriptableObject.CreateInstance<NRProjectConfig>();
				projectConfig.targetDeviceTypes = new List<NRDeviceType>();
				projectConfig.targetDeviceTypes.Add(NRDeviceType.NrealLight);
				projectConfig.targetDeviceTypes.Add(NRDeviceType.NrealAir);
				AssetDatabase.CreateAsset(projectConfig, projectConfigAssetPath);
			}
			return projectConfig;
		}

		public static void CommitProjectConfig(NRProjectConfig projectConfig)
		{
			string projectConfigAssetPath = GetProjectConfigAssetPath();
			string customConfigAssetPath = AssetDatabase.GetAssetPath(projectConfig);
			if (customConfigAssetPath != projectConfigAssetPath)
			{
				NRDebugger.Warning("The asset path of NRProjectConfig is legal only for: {0}, error path: {1}", projectConfigAssetPath, customConfigAssetPath);
			}
			EditorUtility.SetDirty(projectConfig);
		}

		private static string GetProjectConfigAssetPath()
		{
			var so = ScriptableObject.CreateInstance(typeof(NRStubHelper));
			var script = MonoScript.FromScriptableObject(so);
			string assetPath = AssetDatabase.GetAssetPath(script);
			string editorDir = Directory.GetParent(assetPath).FullName;
			string nrsdkDir = Directory.GetParent(editorDir).FullName;

			if (NRStubHelper.IsInsideUnityPackage())
			{
				nrsdkDir = Path.GetFullPath(Path.Combine(Application.dataPath, "NRSDK"));
				if (!Directory.Exists(nrsdkDir))
				{
					Directory.CreateDirectory(nrsdkDir);
				}
			}

			string configAssetPath = Path.GetFullPath(Path.Combine(nrsdkDir, "NRProjectConfig.asset"));
			Uri configUri = new Uri(configAssetPath);
			Uri projectUri = new Uri(Application.dataPath);
			Uri relativeUri = projectUri.MakeRelativeUri(configUri);

			return relativeUri.ToString();
		}

        public static void ApplyProjectConfigToSessionConfig(NRProjectConfig projectConfig)
        {
            var sessionConfigGuids = AssetDatabase.FindAssets("t:NRSessionConfig");
            foreach (var item in sessionConfigGuids)
            {
                var sessionConfig = AssetDatabase.LoadAssetAtPath<NRSessionConfig>(
                    AssetDatabase.GUIDToAssetPath(item));
				sessionConfig.SetProjectConfig(projectConfig);
                EditorUtility.SetDirty(sessionConfig);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
	}
}
