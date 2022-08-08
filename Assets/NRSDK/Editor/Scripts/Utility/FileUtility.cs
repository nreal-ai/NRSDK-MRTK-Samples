using System;
using System.IO;
using UnityEngine;


namespace NRKernal.Release
{
    public class FileUtility
    {
        public static void SaveStringFile(string path, string content)
        {
            EnsureFolder(path);
            FileStream fs = File.OpenWrite(path);
            fs.SetLength(0);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(content);
            sw.Dispose();
            fs.Dispose();
        }

        public static void SaveFileByteArray(string path, byte[] bytes)
        {
            EnsureFolder(path);
            FileStream fs = File.OpenWrite(path);
            fs.Write(bytes, 0, bytes.Length);
            fs.Dispose();
        }

        public static void EnsureFolder(string path)
        {
            string folder = Path.GetDirectoryName(path);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }

        public static bool IsDir(string path)
        {
            var attr = File.GetAttributes(path);
            bool isDir = ((attr & FileAttributes.Directory) == FileAttributes.Directory);

            return isDir;
        }

        public static string LoadStringFile(string path)
        {
            if (path.Contains("://"))
            {
                using (WWW www = new WWW(path))
                {
                    while (!www.isDone) { }
                    if (!string.IsNullOrEmpty(www.error))
                    {
                        Debug.LogError(www.error);
                    }
                    else
                    {
                        return www.text;
                    }
                }
            }
            else
            {
                if (File.Exists(path))
                {
                    using (StreamReader sr = File.OpenText(path))
                    {
                        string content = sr.ReadToEnd();
                        sr.Dispose();
                        return content;
                    }
                }
            }
            return null;
        }

        public static byte[] LoadFileByteArray(string path)
        {
            if (path.Contains("://"))
            {
                using (WWW www = new WWW(path))
                {
                    while (!www.isDone) { }
                    if (!string.IsNullOrEmpty(www.error))
                    {
                        Debug.LogError(www.error);
                    }
                    else
                    {
                        return www.bytes;
                    }
                }
            }
            else
            {
                if (!File.Exists(path))
                {
                    return null;
                }
                using (FileStream fs = File.OpenRead(path))
                {
                    byte[] bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, bytes.Length);
                    fs.Close();
                    return bytes;
                }
            }
            return null;
        }

        public static DirectoryInfo WalkDirectoryTree(DirectoryInfo root, Func<DirectoryInfo, bool> itFunc)
        {
            DirectoryInfo[] subDirs = null;

            if (itFunc.Invoke(root))
                return root;

            subDirs = root.GetDirectories();

            foreach (System.IO.DirectoryInfo dirInfo in subDirs)
            {
                // Resursive call for each subdirectory.
                var result = WalkDirectoryTree(dirInfo, itFunc);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }
    }
}
