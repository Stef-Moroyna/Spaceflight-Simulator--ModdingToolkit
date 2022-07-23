using UnityEditor;
using System.IO;

public static class AssetBundlesBuilder
{

    static void BuildAllAssetBundles(BuildTarget buildTarget, string pathExtend)
    {
        string assetBundleDirectory = "Assets/AssetBundles/" + pathExtend;
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }

        BuildPipeline.BuildAssetBundles(assetBundleDirectory,
            BuildAssetBundleOptions.None,
            buildTarget);
    }

    [MenuItem("Build Pack/Windows")]
    static void BuildAllAssetBundlesWindows() => BuildAllAssetBundles(BuildTarget.StandaloneWindows, "Windows");

    [MenuItem("Build Pack/Mac")]
    static void BuildAllAssetBundlesMac() => BuildAllAssetBundles(BuildTarget.StandaloneOSX, "Mac");
    
    [MenuItem("Build Pack/Android")]
    static void BuildAllAssetBundlesAndroid() => BuildAllAssetBundles(BuildTarget.Android, "Android");
    
    [MenuItem("Build Pack/iOS")]
    static void BuildAllAssetBundles_iOS() => BuildAllAssetBundles(BuildTarget.iOS, "iOS");
}