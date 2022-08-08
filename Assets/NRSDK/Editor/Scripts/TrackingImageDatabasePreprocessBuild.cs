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
    using UnityEditor;

    /// <summary> A tracking image database preprocess build. </summary>
    internal class TrackingImageDatabasePreprocessBuild : PreprocessBuildBase
    {
        /// <summary> Executes the 'preprocess build' action. </summary>
        /// <param name="target"> Target for the.</param>
        /// <param name="path">   Full pathname of the file.</param>
        public override void OnPreprocessBuild(BuildTarget target, string path)
        {
            var augmentedImageDatabaseGuids = AssetDatabase.FindAssets("t:NRTrackingImageDatabase");
            foreach (var databaseGuid in augmentedImageDatabaseGuids)
            {
                var database = AssetDatabase.LoadAssetAtPath<NRTrackingImageDatabase>(
                    AssetDatabase.GUIDToAssetPath(databaseGuid));

                TrackingImageDatabaseInspector.BuildDataBase(database);
                database.BuildIfNeeded();
            }
        }
    }
}
