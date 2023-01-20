using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static class SerializeUtility
{
    public static void SerializeTo(this JsonSerializer serializer, string path, object value)
    {
        SerializeTo(serializer, path, value, (Type) null);
    }

    public static void SerializeTo(this JsonSerializer serializer, string path, object value, Type type)
    {
        StreamWriter streamWriter = new StreamWriter(path);
        serializer.Serialize(streamWriter, value, type);
        streamWriter.Close();
    }

    public static void Populate(this JObject jObject, object target)
    {
        Populate(jObject, target, null);
    }

    public static void Populate(this JObject jObject, object target, JsonSerializer serializer)
    {
        if (serializer == null)
            serializer = DefaultSerializer;
        serializer.Populate(jObject.CreateReader(), target);
    }

    public static JsonSerializer DefaultSerializer => defaultSerializer ??= JsonSerializer.CreateDefault();

    static JsonSerializer defaultSerializer;
}