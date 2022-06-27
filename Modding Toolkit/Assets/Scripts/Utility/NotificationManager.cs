using System;
using UnityEngine;
using SFS.IO;
using SFS.Parsers.Json;


public class NotificationManager : MonoBehaviour
{
    public int displayAfter = 10;
    public string title = "Test";
    public string message;
    public string iOSIdent;
    public int androidIdent;
    public bool isEnabled;
    FilePath NotifPath;

    void Start()
    {
    }

    bool CheckStatus()
    {
        if (!JsonWrapper.TryLoadJson(NotifPath, out bool offered))
        {
            JsonWrapper.SaveAsJson(NotifPath, false, false);
            offered = false;
        }

        return offered;
    }
}
