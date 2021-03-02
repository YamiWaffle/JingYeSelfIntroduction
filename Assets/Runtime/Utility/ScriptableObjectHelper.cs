using System.IO;
using UnityEngine;

namespace JingYe.SelfIntro
{
    public static class ScriptableObjectHelper
    {
#if UNITY_EDITOR
        public static void Save<T>(T target) where T : ScriptableObject
        {
            if (target == null) return;
            UnityEditor.EditorUtility.SetDirty(target);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }

        public static T GetOrCreate<T>(string directoryPath, string fileName, string extension = ".asset") where T : ScriptableObject
        {
            if (string.IsNullOrEmpty(directoryPath) || string.IsNullOrEmpty(fileName))
                return null;

            string[] tokens = directoryPath.Split(Path.DirectorySeparatorChar);
            for (int i = 0; i < tokens.Length - 1; ++i)
            {
                // Combine path
                string parentDir = "";
                for (int j = 0; j <= i; ++j)
                {
                    parentDir = Path.Combine(parentDir, tokens[j]);
                }

                if (!UnityEditor.AssetDatabase.IsValidFolder(Path.Combine(parentDir, tokens[i + 1])))
                    UnityEditor.AssetDatabase.CreateFolder(parentDir, tokens[i + 1]);
            }

            string filePath = Path.Combine(directoryPath, fileName) + extension;
            T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(filePath);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();
                UnityEditor.AssetDatabase.CreateAsset(asset, filePath);
                UnityEditor.EditorUtility.SetDirty(asset);
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();
            }

            return asset;
        }

        public static bool Delete(string directoryPath, string fileName, string extension = ".asset")
        {
            if (string.IsNullOrEmpty(directoryPath) || string.IsNullOrEmpty(fileName))
                return false;

            string filePath = Path.Combine(directoryPath, fileName) + extension;

            return UnityEditor.AssetDatabase.DeleteAsset(filePath);
        }
#endif
    } // END class
} // END namespace
