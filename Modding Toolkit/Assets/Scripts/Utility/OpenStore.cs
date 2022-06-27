using UnityEngine;

namespace SFS.Core
{
    public static class OpenStore
    {
        const string Android = "https://play.google.com/store/apps/details?id=com.StefMorojna.SpaceflightSimulator&hl=nl";
        const string IOS = "https://apps.apple.com/nl/app/spaceflight-simulator/id1308057272";

        public static void Open(bool closeApplication)
        {
            Application.OpenURL(Application.platform == RuntimePlatform.Android ? Android : IOS);

            if (closeApplication)
                Application.Quit();
        }
    }
}