using System;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

#if UNITY_STANDALONE
using Steamworks;
#endif

[Serializable]
public class PlatformUtilities
{
    public string SocialToken;
    public bool initialized;
    public bool useSocial = false;
    
}