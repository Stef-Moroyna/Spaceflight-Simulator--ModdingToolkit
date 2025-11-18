using System;
using UnityEditor;
using ModLoader;
using Newtonsoft.Json;
using SFS.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEngine;

// ReSharper disable InconsistentNaming
public class SFS_PartPackModBuilder : OdinEditorWindow
{
    [MenuItem("SFS/Build Pack")]
    static void Init()
    {
        SFS_PartPackModBuilder window = GetWindow<SFS_PartPackModBuilder>();
        window.assetBundleLabel = AssetBundles.Length > 0? AssetBundles[0] : "";
        window.Show();
    }
    
    static string[] AssetBundles => AssetDatabase.GetAllAssetBundleNames();

    [TitleGroup("Pack Information"), ShowInInspector]
    public string outputFileName;

    [TitleGroup("Pack Information"), Required, ShowInInspector, InlineEditor(InlineEditorModes.FullEditor, InlineEditorObjectFieldModes.Foldout, DrawHeader = false, Expanded = true, DrawPreview = false), Space]
    public PackData data;
    
    [TitleGroup("Pack Information"), Button, HideIf(nameof(data))]
    void NewPackData() => data = CreateInstance<PackData>();
    
    [ShowInInspector, ValueDropdown(nameof(AssetBundles)), TitleGroup("Assets")]
    public string assetBundleLabel;
    
    [PropertySpace]
    [Button(ButtonSizes.Large), ShowInInspector]
    void BuildPartPackModForAllPlatforms()
    {
        FolderPath buildFolder = new FolderPath("Assets").Extend("ModBuilder").CreateFolder();
        FolderPath cacheFolder = buildFolder.CloneAndExtend("Cache").CreateFolder();

        string[] bundles =
        {
            assetBundleLabel.Split(".")[0],
            assetBundleLabel.Split(".").Length > 1? assetBundleLabel.Split(".")[1] : ""
        };
        
        // Saving pack data as asset if new
        if (data == null)
        {
            Debug.LogError("Pack data is required!");
            return;
        }
        
        if (!AssetDatabase.Contains(data))
        {
            data.name = data.DisplayName.Replace(" ", "_") + ".asset";
            string assetPath = cacheFolder.ExtendToFile(data.name).GetRelativePath(Application.dataPath + "/../");
            AssetDatabase.CreateAsset(data, assetPath);
            AssetImporter.GetAtPath(assetPath).SetAssetBundleNameAndVariant(bundles[0], bundles[1]);
        }
        else
            AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(data)).SetAssetBundleNameAndVariant(bundles[0], bundles[1]);

        AssetBundlePack bundlePack = new();

        // Optional custom code assembly
        string path = EditorUtility.OpenFilePanel("Custom module (Optional)", Application.dataPath, "dll");
        if (!path.IsNullOrWhitespace())
            bundlePack.CodeAssembly = new FilePath(path).ReadBytes();
        
        // Building asset bundles for all platforms
        bundlePack.WindowsBuild = BuildAssetBundleForTarget("Windows", BuildTarget.StandaloneWindows64);
        bundlePack.MacBuild = BuildAssetBundleForTarget("Mac", BuildTarget.StandaloneOSX);
        bundlePack.AndroidBuild = BuildAssetBundleForTarget("Android", BuildTarget.Android);
        bundlePack.IOS_Build = BuildAssetBundleForTarget("IOS", BuildTarget.iOS);
        
        byte[] BuildAssetBundleForTarget(string platformFolderName, BuildTarget target)
        {
            string relativePath = cacheFolder.CloneAndExtend(platformFolderName).CreateFolder().GetRelativePath(Application.dataPath + "/../");
            BuildPipeline.BuildAssetBundles(relativePath, BuildAssetBundleOptions.None, target);
            
            FilePath file = cacheFolder.CloneAndExtend(platformFolderName).ExtendToFile(assetBundleLabel);

            if (file.FileExists())
                return file.ReadBytes();
            
            Debug.LogError($"Can't find asset bundle at path: {(string)file}");
            return null;
        }
        
        Debug.Log("Finished building asset bundles");

        // Serializing pack
        string text = JsonConvert.SerializeObject(bundlePack);
        buildFolder.ExtendToFile($"{outputFileName}.pack").WriteText(text);
        
        // Cleanup
        FileUtil.DeleteFileOrDirectory(cacheFolder);
        FileUtil.DeleteFileOrDirectory(cacheFolder + ".meta");
        Debug.Log("Finished building pack!");
        AssetDatabase.Refresh();
    }
}

[Serializable]
// ReSharper disable InconsistentNaming
public class AssetBundlePack
{
    public byte[] MacBuild, WindowsBuild, AndroidBuild, IOS_Build;
    public byte[] CodeAssembly;
}