using System;
using Newtonsoft.Json;
using SFS.IO;
using UnityEngine;

namespace SFS.Parsers.Json
{
    public static class JsonWrapper
    {
        // File
        public static bool TryLoadJson<T>(FilePath file, out T data)
        {
            try
            {
                if (!file.FileExists())
                {
                    data = default;
                    return false;
                }

                string json = file.ReadText();
                data = FromJson<T>(json);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                data = default;
                return false;
            }
        }
        public static void SaveAsJson(FilePath file, object data, bool pretty)
        {
            if (!file.GetParent().FolderExists())
                file.GetParent().CreateFolder();

            file.WriteText(ToJson(data, pretty));
        }
        
        // String
        public static T FromJson<T>(string json)
        {
            try
            {
                VersionedData<T> data = JsonConvert.DeserializeObject<VersionedData<T>>(json, serializerSettings);

                if (data.data == null)
                    goto OLD_FORMAT;

                return data.data;
            }
            catch
            {
            }

            OLD_FORMAT:
            return JsonConvert.DeserializeObject<T>(json, serializerSettings);
        }
        public static string ToJson(object data, bool pretty)
        {
            return JsonConvert.SerializeObject(data, pretty ? Formatting.Indented : Formatting.None, serializerSettings);
        }

        // Settings stuff...
        static readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new MainContractResolver(),
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };
        public class VersionedData<T>
        {
            // ReSharper disable once UnassignedField.Global
            public T data;

            // ReSharper disable once EmptyConstructor
            public VersionedData() {}
        }
    }
}