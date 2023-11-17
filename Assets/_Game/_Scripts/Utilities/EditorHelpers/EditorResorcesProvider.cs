#if UNITY_EDITOR
namespace _Game._Scripts.Utilities.EditorHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using _Game._Scripts.Managers.LevelsManagement.LevelTypes;
    using UnityEditor;
    using UnityEngine;

    public static class EditorResourcesProvider
    {

        public static Texture2D GenerateGridTexture(Color line, Color bg)
        {
            Texture2D tex = new Texture2D(64, 64);
            Color[] cols = new Color[64 * 64];
            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    Color col = bg;
                    if (y % 16 == 0 || x % 16 == 0) col = Color.Lerp(line, bg, 0.65f);
                    if (y == 63 || x == 63) col = Color.Lerp(line, bg, 0.35f);
                    cols[(y * 64) + x] = col;
                }
            }
            tex.SetPixels(cols);
            tex.wrapMode = TextureWrapMode.Repeat;
            tex.filterMode = FilterMode.Bilinear;
            tex.name = "Grid";
            tex.Apply();
            return tex;
        }

        public static Texture2D GenerateCrossTexture(Color line)
        {
            Texture2D tex = new Texture2D(64, 64);
            Color[] cols = new Color[64 * 64];
            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    Color col = line;
                    if (y != 31 && x != 31) col.a = 0;
                    cols[(y * 64) + x] = col;
                }
            }
            tex.SetPixels(cols);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;
            tex.name = "Grid";
            tex.Apply();
            return tex;
        }

        public static Texture2D ColorTexture(Color color)
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, color);
            tex.Apply();
            return tex;
        }
        
        public static Texture2D ColorTexture(string color)
        {
            var cTex = ColorTexture(HexColor(color));
            return cTex;
        }

        public static Color HexColor(string color)
        {
            var validChars = new char[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};
            
            if (string.IsNullOrWhiteSpace(color))
                return Color.black;

            color = color.ToUpper();

            for (int i = 0; i < 6; i++)
            {
                if (i < color.Length)
                {
                    if (validChars.Contains(color[i]) == false)
                        return Color.black;
                }
                else
                {
                    color = $"{color}0";
                }
            }

            var r = 16f * validChars.ToList().IndexOf(color[0]) + validChars.ToList().IndexOf(color[1]);
            var g = 16f * validChars.ToList().IndexOf(color[2]) + validChars.ToList().IndexOf(color[3]);
            var b = 16f * validChars.ToList().IndexOf(color[4]) + validChars.ToList().IndexOf(color[5]);

            r /= 256f;
            g /= 256f;
            b /= 256f;

            return new Color(r, g, b, 1);
        }
        
        public static string ConvertAssetsToResourcesPath(string path)
        {
            var res = "Resources/";
            var result = path.Split(new string[] {".asset"}, StringSplitOptions.None)[0]
                .Split(new string[] {res}, StringSplitOptions.None)[1];
            return result;
        }
        
        public static string ConvertResourcesToAssetsPath(string path)
        {
            return "Assets/Data/Resources/" + path + ".asset";
        }
        
        public static List<T> GetAllMonoAssets<T>() where T:MonoBehaviour
        {
            var tName = typeof(T).Name;
            var guids = AssetDatabase.FindAssets($"t: {tName}").ToList();
                
            var assets = guids.Select(guid =>
            {
                var asset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(T)) as T;
                return asset;
            }).ToList();
            
            return assets;
        }
        
        public static List<T> GetAllMonoAssets<T>(string[] folders) where T:MonoBehaviour
        {
            var tName = typeof(T).Name;
            var guids = AssetDatabase.FindAssets($"t: {tName}", folders).ToList();
                
            var assets = guids.Select(guid =>
            {
                var asset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(T)) as T;
                return asset;
            }).ToList();
            
            return assets;
        }
        
        public static List<T> GetAllSOAssets<T>() where T:ScriptableObject
        {
            var tName = typeof(T).Name;
            var guids = AssetDatabase.FindAssets($"t: {tName}").ToList();
                
            var assets = guids.Select(guid =>
            {
                var asset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(T)) as T;
                return asset;
            }).ToList();
            
            return assets;
        }
        
        public static List<T> GetAllSOAssets<T>(string[] folders) where T:ScriptableObject
        {
            var tName = typeof(T).Name;
            var guids = AssetDatabase.FindAssets($"t: {tName}", folders).ToList();
                
            var assets = guids.Select(guid =>
            {
                var asset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(T)) as T;
                return asset;
            }).ToList();
            
            return assets;
        }

        public static List<T> GetAllPrefabs<T>(string folder) where T:MonoBehaviour
        {
            var guids = AssetDatabase.FindAssets($"t:prefab", new[]{folder}).ToList();
            //var guids = AssetDatabase.FindAssets($"t:prefab").ToList();
            //Debug.Log($"GetAllPrefabs  guids len =  {guids.Count}  path [ {folder} ]");

            var assets = guids.Select(guid =>
            {
                var asset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(T)) as T;
                //Debug.Log($"  asset = {asset},  path [ {AssetDatabase.GUIDToAssetPath(guid)} ]");
                return asset;
            }).ToList();
            
            //PrefabUtility.LoadPrefabContents();
            
            return assets.Where(asset => asset is T).ToList();
        }

        public static bool AssetsIsReady<T>(List<T> assets)
        {
            if (assets is null)
                return false;

            foreach (var asset in assets)
            {
                if (asset == null)
                    return false;
            }

            return true;
        }
    }
}
#endif