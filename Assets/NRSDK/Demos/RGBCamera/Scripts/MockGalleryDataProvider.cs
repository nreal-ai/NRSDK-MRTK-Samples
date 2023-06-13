using System;
using System.IO;
using UnityEngine;

namespace NRKernal.NRExamples
{
    public class MockGalleryDataProvider
    {
        public void InsertImage(byte[] data, string displayName, string folderName)
        {
            string path = string.Format("{0}/NrealShots/{1}", Application.persistentDataPath, folderName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            File.WriteAllBytes(string.Format("{0}/{1}", path, displayName), data);
        }

        public AndroidJavaObject InsertVideo(string originFilePath, string displayName, string folderName)
        {
            return null;
        }
    }
}