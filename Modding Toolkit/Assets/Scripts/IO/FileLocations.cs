using SFS.IO;
using UnityEngine;

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
    
    // Notifications (Popups)
    public static bool GetOneTimeNotification(string name)
    {
        FilePath path = GetNotificationsPath(name);
        if (path.FileExists())
            return false;

        path.WriteText("");
        return true;
    }
    public static bool HasNotification(string name)
    {
        return GetNotificationsPath(name).FileExists();
    }
    public static void WriteNotification(string name)
    {
        GetNotificationsPath(name).WriteText("");
    }
    public static FilePath GetNotificationsPath(string name) => SavingFolder.Extend("Notifications").CreateFolder().ExtendToFile(name + ".txt");


    // Base
    static FolderPath SavingFolder => BaseFolder.Extend(Application.isMobilePlatform || Application.isEditor? "Saving" : "/../Saving");
    static FolderPath CacheFolder => BaseFolder.Extend("Cache").CreateFolder();
    public static FolderPath BaseFolder => new FolderPath((SaveInGameFolder? Application.dataPath : Application.persistentDataPath) + (Application.isEditor? "/Editor" : ""));
    static bool SaveInGameFolder => Application.isEditor || SystemInfo.operatingSystemFamily == OperatingSystemFamily.Windows || SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX;
}
