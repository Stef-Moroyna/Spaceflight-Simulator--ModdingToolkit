using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

public class MetaConverterWindow : OdinEditorWindow
{
    string metaDumpFile;
    List<MonoBehaviourInfo> metaDump;
    Dictionary<string, MonoBehaviourInfo> typeToMonoInfo;
    Dictionary<string, MonoBehaviourInfo> guidToMonoInfo;

    string GetMetaDumpInfoText()
    {
        if (metaDumpFile == null || metaDump == null)
            return "Import a meta dump to get started";
        
        return $"Imported {Path.GetFileNameWithoutExtension(metaDumpFile)}. Contains {metaDump.Count} types";
    }
    [Button, InfoBox("@" + nameof(GetMetaDumpInfoText) + "()")]
    void ImportMetaDump()
    {
        metaDump = null;
        metaDumpFile = null;
        
        string path = EditorUtility.OpenFilePanel("Open meta dump", Application.dataPath, "json");
        
        if (string.IsNullOrWhiteSpace(path))
            return;

        try
        {
            metaDump = JsonConvert.DeserializeObject<List<MonoBehaviourInfo>>(File.ReadAllText(path));
            metaDumpFile = path;
            
            typeToMonoInfo = new Dictionary<string, MonoBehaviourInfo>();
            guidToMonoInfo = new Dictionary<string, MonoBehaviourInfo>();

            foreach (MonoBehaviourInfo monoBehaviourInfo in metaDump)
            {
                typeToMonoInfo.Add(monoBehaviourInfo.name, monoBehaviourInfo);
                guidToMonoInfo.Add(monoBehaviourInfo.guid, monoBehaviourInfo);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load meta dump!");
            Debug.LogException(e);
        }
    }

    Dictionary<string, string> guidToAssemblyGuid = new();
    List<(string path, Assembly assembly, string guid)> assemblies = new();
    [NonSerialized] int viableTypes;
    bool CanSelectAssembly => metaDump != null;
    string GetAssemblyText()
    {
        return $"Found {viableTypes} viable types in {assemblies.Count} assemblies.";
    }
    [Button, EnableIf(nameof(CanSelectAssembly)), InfoBox("@"+nameof(GetAssemblyText)+"()", visibleIfMemberName: nameof(CanSelectAssembly))]
    void AddAssembly()
    {
        string path =  EditorUtility.OpenFilePanel("Add assembly", Application.dataPath, "dll");
        
        if (string.IsNullOrWhiteSpace(path))
            return;

        string localPath = Path.GetFullPath(path).Replace("\\", "/").Replace(Path.GetFullPath(Path.GetDirectoryName(Application.dataPath)!).Replace("\\", "/"), "").TrimStart('/');
        string assemblyGuid = AssetDatabase.AssetPathToGUID(localPath);

        if (string.IsNullOrWhiteSpace(assemblyGuid))
        {
            Debug.LogError("Failed to get guid for assembly, perhaps the file is not in the project folder! It must have a .meta file");
            return;
        }
        
        Assembly assembly = Assembly.Load(File.ReadAllBytes(path));
        
        foreach (Type type in assembly.GetTypes())
            if (type.AssemblyQualifiedName != null && typeToMonoInfo.TryGetValue(type.AssemblyQualifiedName, out MonoBehaviourInfo monoBehaviourInfo))
            {
                viableTypes++;
                guidToAssemblyGuid.Add(monoBehaviourInfo.guid, assemblyGuid);
            }

        assemblies.Add((path, assembly, assemblyGuid));
    }


    Dictionary<string, List<ScriptReference>> allReferences = new();
    Dictionary<string, List<ScriptReference>> foundReferences;
    [NonSerialized] bool scannedReferences;

    bool CanFindReferences => assemblies.Count > 0;
    string GetFoundRefText()
    {
        return $"Found {allReferences.Sum(p => p.Value.Count)} references in {allReferences.Count} files.";
    }
    [Button, EnableIf(nameof(CanFindReferences)), InfoBox("@"+nameof(GetFoundRefText)+"()", visibleIfMemberName: nameof(CanFindReferences))]
    void FindReferences()
    {
        scannedReferences = false;
        allReferences = new();
        RunAsync().Forget();

        // local helpers
        async UniTaskVoid RunAsync()
        {
            // Build file list up-front for progress
            List<string> files = new(4096);
            foreach (var f in Directory.EnumerateFiles(Application.dataPath, "*.*", SearchOption.AllDirectories))
                files.Add(f);

            int total = files.Count;
            int processed = 0;

            int refFileCount = 0;
            int refCount = 0;

            try
            {
                for (int fIndex = 0; fIndex < total; fIndex++)
                {
                    string file = files[fIndex];

                    // progress + allow cancel
                    float p = total == 0 ? 1f : (float)fIndex / total;
                    if (EditorUtility.DisplayCancelableProgressBar($"Found {refCount} refs in {refFileCount} files", $"[{fIndex + 1}/{total}] {NicePath(file)}", p))
                        break;

                    await UniTask.Yield(PlayerLoopTiming.Update);

                    if (!IsYaml(file))
                        continue;

                    string content;
                    try
                    {
                        content = await File.ReadAllTextAsync(file);
                    }
                    catch
                    {
                        continue;
                    }

                    List<ScriptReference> localRefs = new();
                    allReferences[file] = localRefs;

                    bool markedFile = false;
                    
                    int i = 0;
                    while (true)
                    {
                        int ms = content.IndexOf("m_Script:", i, StringComparison.Ordinal);
                        if (ms == -1) break;

                        int brace = content.IndexOf('{', ms);
                        if (brace == -1) break;

                        int endBrace = content.IndexOf('}', brace);
                        if (endBrace == -1) break;

                        // keep spans relative to full content (not the inner block)
                        int fidKey = content.IndexOf("fileID:", brace, StringComparison.Ordinal);
                        int guidKey = content.IndexOf("guid:", brace, StringComparison.Ordinal);
                        int typeKey = content.IndexOf("type:", brace, StringComparison.Ordinal);

                        if (fidKey == -1 || fidKey > endBrace ||
                            guidKey == -1 || guidKey > endBrace ||
                            typeKey == -1 || typeKey > endBrace)
                        {
                            i = endBrace + 1;
                            continue;
                        }

                        int fidValStart = fidKey + 7;
                        int fidValEnd = content.IndexOf(',', fidValStart);
                        if (fidValEnd == -1 || fidValEnd > endBrace) fidValEnd = endBrace;

                        int guidValStart = guidKey + 5;
                        int guidValEnd = content.IndexOf(',', guidValStart);
                        if (guidValEnd == -1 || guidValEnd > endBrace) guidValEnd = endBrace;

                        int typeValStart = typeKey + 5;
                        int typeValEnd = content.IndexOf('}', typeValStart);
                        if (typeValEnd == -1) typeValEnd = endBrace;

                        if (fidValEnd <= fidValStart || guidValEnd <= guidValStart || typeValEnd <= typeValStart)
                        {
                            i = endBrace + 1;
                            continue;
                        }

                        if (!long.TryParse(content.Substring(fidValStart, fidValEnd - fidValStart).Trim(), out long fileID) ||
                            !int.TryParse(content.Substring(typeValStart, typeValEnd - typeValStart).Trim(), out int type))
                        {
                            i = endBrace + 1;
                            continue;
                        }

                        string guid = content.Substring(guidValStart, guidValEnd - guidValStart).Trim();

                        localRefs.Add(new ScriptReference(
                            new TextSpan(fidValStart, fidValEnd),
                            new TextSpan(guidValStart, guidValEnd),
                            new TextSpan(typeValStart, typeValEnd),
                            fileID,
                            guid,
                            type));
                        refCount++;

                        if (!markedFile)
                        {
                            markedFile = true;
                            refFileCount++;
                        }

                        i = endBrace + 1;

                        // yield occasionally during heavy single-file parsing too
                        if ((localRefs.Count & 127) == 0) // every 128 refs
                            await UniTask.Yield(PlayerLoopTiming.Update);
                    }

                    processed++;
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
            
            // Update foundReferences
            foundReferences = new Dictionary<string, List<ScriptReference>>();
            foreach (KeyValuePair<string, List<ScriptReference>> pair in allReferences)
            foreach (ScriptReference reference in pair.Value)
                if (guidToAssemblyGuid.ContainsKey(reference.guid))
                {
                    if (!foundReferences.TryGetValue(pair.Key, out List<ScriptReference> list))
                        list = foundReferences[pair.Key] = new List<ScriptReference>();
                    
                    list.Add(reference);
                }

            scannedReferences = true;
        }
        
        static bool IsYaml(string file)
        {
            try
            {
                using var stream = File.OpenRead(file);
                using var reader = new StreamReader(stream);
                var line = reader.ReadLine();
                return line != null && line.Trim() == "%YAML 1.1";
            }
            catch
            {
                return false;
            }
        }
    }

    bool CanUpdateReferences => scannedReferences && foundReferences != null;
    string GetUpdateRefText()
    {
        return $"Found {foundReferences.Sum(p => p.Value.Count)} recognized references in {foundReferences.Count} files.";
    }
    [Button, EnableIf(nameof(CanUpdateReferences)), InfoBox("@"+nameof(GetUpdateRefText)+"()", visibleIfMemberName: nameof(CanUpdateReferences))]
    void UpdateReferences()
    {
        RunAsync().Forget();
        
        async UniTaskVoid RunAsync()
        {
            int total = foundReferences.Count;
            int processed = 0;

            foreach (KeyValuePair<string, List<ScriptReference>> pair in foundReferences)
            {
                // progress + allow cancel
                float p = total == 0 ? 1f : (float)processed / total;
                EditorUtility.DisplayProgressBar($"Updated {processed} refs in {foundReferences.Count} files", $"[{processed + 1}/{total}] {NicePath(pair.Key)}", p);
                
                await UniTask.Yield(PlayerLoopTiming.Update);
                
                string content = await File.ReadAllTextAsync(pair.Key);

                List<ScriptReference> refs = pair.Value.OrderByDescending(r => r.fileIdSpan.start).ToList();
                foreach (ScriptReference reference in refs)
                {
                    reference.guidSpan.Replace(ref content, guidToAssemblyGuid[reference.guid]);
                    reference.fileIdSpan.Replace(ref content, guidToMonoInfo[reference.guid].fileId);
                }
                
                await File.WriteAllTextAsync(pair.Key, content);
            }
        }
    }
    
    static string NicePath(string fullPath)
    {
        fullPath = fullPath.Replace('\\', '/');
        var a = Application.dataPath.Replace('\\', '/');
        return fullPath.StartsWith(a, StringComparison.OrdinalIgnoreCase)
            ? "Assets/" + fullPath.Substring(a.Length).TrimStart('/')
            : fullPath;
    }

    
    [MenuItem("SFS/Meta Converter")]
    static void Open()
    {
        MetaConverterWindow dumper = CreateWindow<MetaConverterWindow>("Meta Converter");
        dumper.Show();
    }

    struct ScriptReference
    {
        public readonly TextSpan fileIdSpan, guidSpan, typeSpan;
        public readonly long fileId;
        public readonly string guid;
        public readonly int type;

        public ScriptReference(TextSpan fileIdSpan, TextSpan guidSpan, TextSpan typeSpan, long fileId, string guid, int type)
        {
            this.fileIdSpan = fileIdSpan;
            this.guidSpan = guidSpan;
            this.typeSpan = typeSpan;
            this.fileId = fileId;
            this.guid = guid;
            this.type = type;
        }
    }
    struct TextSpan
    {
        public readonly int start, end;

        public TextSpan(int start, int end)
        {
            this.start = start;
            this.end = end;
        }

        public readonly void Replace(ref string content, string replacement)
        {
            content = content.Remove(start, end - start);
            content = content.Insert(start, replacement);
        }
    }
}