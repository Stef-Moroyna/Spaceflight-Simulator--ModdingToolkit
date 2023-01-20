using System;
using System.Globalization;
using SFS.IO;
using UnityEngine;
using UnityEngine.Analytics;

public static class FileLocations
{
    // Planets
    public static FolderPath SolarSystemsFolder => BaseFolder.Extend("Custom Solar Systems").CreateFolder();

    // Translations
    public static FolderPath PublicTranslationFolder => BaseFolder.Extend(Application.isEditor? "Translations" : "Custom Translations").CreateFolder();
    public static FolderPath TranslationCacheFolder => CacheFolder.Extend("Translations").CreateFolder();


    // Saving
    public static FolderPath BlueprintsFolder => SavingFolder.Extend("Blueprints");
    public static FolderPath WorldsFolder => SavingFolder.Extend("Worlds");
    //
    public static FilePath GetSettingsPath(string name) => SavingFolder.Extend("Settings").CreateFolder().ExtendToFile(name + ".txt");
    
    
    // Notifications (Utility)
    public static void IncreaseCount(string name)
    {
        GetNotificationsPath(name).WriteText((GetCount(name) + 1).ToString());
    }
    public static int GetCount(string name)
    {
        FilePath path = GetNotificationsPath(name);
        return path.FileExists() && int.TryParse(path.ReadText(), out int count)? count : 0;
    }
    //
    public static bool GetOneTimeNotification(string name)
    {
        FilePath path = GetNotificationsPath(name);
        if (path.FileExists())
            return false;

        Write(path);
        return true;
    }
    public static bool GetOneTimeNotification_Repeated(string name, TimeSpan repeatTime, int repeatSessions, bool onParseFailed)
    {
        FilePath path = GetNotificationsPath(name);
        if (!path.FileExists())
        {
            Write(path);
            return true;
        }
        
        string[] data = path.ReadText().Split('|');

        DateTime lastTime = default;
        int lastSession = default;
        bool parsedSuccessfully = data.Length == 2 && DateTime.TryParse(data[0], out lastTime) && int.TryParse(data[1], out lastSession);
        
        if (!parsedSuccessfully)
        {
            Write(path);
            return onParseFailed;
        }

        return false;
    }
    //
    public static bool HasNotification(string name)
    {
        return GetNotificationsPath(name).FileExists();
    }
    public static void WriteNotification(string name)
    {
        FilePath path = GetNotificationsPath(name);
        Write(path);
    }
    //
    static void Write(FilePath path) => path.WriteText(DateTime.Now.ToString(CultureInfo.InvariantCulture) + "|");
    public static FilePath GetNotificationsPath(string name) => SavingFolder.Extend("Notifications").CreateFolder().ExtendToFile(name + ".txt");



    // Base
    static FolderPath SavingFolder => BaseFolder.Extend(Application.isMobilePlatform || Application.isEditor? "Saving" : "/../Saving");
    public static FolderPath CacheFolder => BaseFolder.Extend("Cache").CreateFolder();
    public static FolderPath LogsFolder => BaseFolder.Extend("Logs").CreateFolder();
    public static FolderPath BaseFolder => new FolderPath((SaveInGameFolder? Application.dataPath : Application.persistentDataPath) + (Application.isEditor? "/Editor" : ""));
    #if !MAC_APP_STORE
    static bool SaveInGameFolder => Application.isEditor || SystemInfo.operatingSystemFamily == OperatingSystemFamily.Windows || SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX;
    #else
    static bool SaveInGameFolder => Application.isEditor || SystemInfo.operatingSystemFamily == OperatingSystemFamily.Windows;
    #endif
}