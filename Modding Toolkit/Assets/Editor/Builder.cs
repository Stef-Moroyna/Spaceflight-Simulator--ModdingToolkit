using System;
using UnityEditor;
using System.Linq;
using ModLoader;
using Newtonsoft.Json;
using SFS.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor.Compilation;
using UnityEngine;


// ReSharper disable InconsistentNaming
public class SFS_PartPackModBuilder : OdinEditorWindow
{
    [MenuItem("SFS/Build Pack")]
    static void Init()
    {
        SFS_PartPackModBuilder window = GetWindow<SFS_PartPackModBuilder>();
        window.assetBundleLabel = AssetBundles.Length > 0 ? AssetBundles[0] : "";
        window.Show();
    }

    static string[] AssetBundles => AssetDatabase.GetAllAssetBundleNames();
    static string[] AssembliesNames => CompilationPipeline.GetPrecompiledAssemblyNames().Where(x => !x.ToLower().Contains("unity")).Append(String.Empty).OrderBy(x => x).ToArray();

    [TitleGroup("Pack Information"), ShowInInspector]
    public string outputFileName;

    [TitleGroup("Pack Information"), ShowInInspector, InlineEditor(InlineEditorModes.FullEditor, InlineEditorObjectFieldModes.Foldout, DrawHeader = false, Expanded = true, DrawPreview = false), Space]
    public PackData data;
    
    [TitleGroup("Pack Information"), Button, HideIf(nameof(data), (object)null)]
    void NewPackData() => data = CreateInstance<PackData>();
    
    [ShowInInspector, ValueDropdown(nameof(AssetBundles)), TitleGroup("Assets")]
    public string assetBundleLabel;

    [ShowInInspector, TitleGroup("Build Platforms")]
    public bool Windows, MacOS;

    [Button, ShowInInspector]
    void BuildMod()
    {
        // Checking if none platform selected
        if (!(Windows || MacOS))
            return;

        FolderPath buildFolder = new FolderPath("Assets").Extend("ModBuilder").CreateFolder();
        FolderPath cacheFolder = buildFolder.CloneAndExtend("Cache").CreateFolder();

        string[] bundles = new[]
        {
            assetBundleLabel.Split(".")[0],
            assetBundleLabel.Split(".").Length > 1 ? assetBundleLabel.Split(".")[1] : ""
        };
        
        // Saving pack data as asset if new
        if (!AssetDatabase.Contains(data))
        {
            data.name = data.DisplayName.Replace(" ", "_") + ".asset";
            string assetPath = cacheFolder.ExtendToFile(data.name).GetRelativePath(Application.dataPath + "/../");
            AssetDatabase.CreateAsset(data, assetPath);
            AssetImporter.GetAtPath(assetPath).SetAssetBundleNameAndVariant(bundles[0], bundles[1]);
        }

        AssetBundlePack bundlePack = new AssetBundlePack();

        string path = EditorUtility.OpenFilePanel("Custom module (Optional)", Application.dataPath, "dll");
        if (!path.IsNullOrWhitespace())
            bundlePack.CodeAssembly = new FilePath(path).ReadBytes();

        // Building AssetBundles
        if (Windows)
        {
            BuildPipeline.BuildAssetBundles(
                cacheFolder.CloneAndExtend("Windows").CreateFolder().GetRelativePath(Application.dataPath + "/../"),
                BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
            FilePath file = cacheFolder.CloneAndExtend("Windows").ExtendToFile(assetBundleLabel);
            if (file.FileExists())
                bundlePack.WindowsBuild = file.ReadBytes();
            else
                Debug.Log($"Can't find asset bundle at path: {(string)file}");
        }

        if (MacOS){
            BuildPipeline.BuildAssetBundles(cacheFolder.CloneAndExtend("Mac").CreateFolder().GetRelativePath(Application.dataPath + "/../"),
                BuildAssetBundleOptions.None, BuildTarget.StandaloneOSX);
            FilePath file = cacheFolder.CloneAndExtend("Mac").ExtendToFile(assetBundleLabel);
            if (file.FileExists())
                bundlePack.MacBuild = file.ReadBytes();
            else
                Debug.Log($"Can't find asset bundle at path: {(string)file}");
        }
        
        Debug.Log("Finished building asset bundles");

        string text = JsonConvert.SerializeObject(bundlePack);
        
        buildFolder.ExtendToFile($"{outputFileName}.pack").WriteText(text);
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
    public byte[] MacBuild, WindowsBuild;
    public byte[] CodeAssembly;
}