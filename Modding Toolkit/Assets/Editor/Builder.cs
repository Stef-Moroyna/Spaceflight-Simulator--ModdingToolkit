using UnityEditor;
using System.IO;

public class AssetBundlesBuilder
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

    [MenuItem("Assets/Build AssetBundles/Windows")]
    static void BuildAllAssetBundlesWindows() => BuildAllAssetBundles(BuildTarget.StandaloneWindows, "Windows");

    [MenuItem("Assets/Build AssetBundles/Mac")]
    static void BuildAllAssetBundlesMac() => BuildAllAssetBundles(BuildTarget.StandaloneOSX, "Mac");
}