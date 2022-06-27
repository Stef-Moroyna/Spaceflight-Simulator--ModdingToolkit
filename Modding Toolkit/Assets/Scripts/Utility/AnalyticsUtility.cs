using System.Collections.Generic;
using UnityEngine;

namespace SFS.Analytics
{
    public class AnalyticsUtility : MonoBehaviour
    {
        // Base
        public static void SendEvent(string eventName, params (string, object)[] eventData)
        {
            if (Application.isEditor)
                return;
            
            Dictionary<string, object> data = new Dictionary<string, object>() {  };

            foreach ((string n, object d) in eventData)
                data.Add(n, d);

            UnityEngine.Analytics.Analytics.CustomEvent(eventName, data);
        }
    }
}