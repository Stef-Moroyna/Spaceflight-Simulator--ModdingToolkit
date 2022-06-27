using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

namespace Assets.Scripts
{
    public class ForceIncludeZip : MonoBehaviour
    {
        // This is here so unity wont strip away the zip library
        void Start()
        {
            try
            {
                SHA1Managed sha1 = new SHA1Managed();
                sha1.ComputeHash((Stream)null);
                ZipFile zipFile = new ZipFile("Hack = not epic");
                ZipEntry entry = new ZipEntry("Yes not ok");
                var stream = zipFile.GetInputStream(entry);
                Debug.Log(zipFile);
                Debug.Log(entry);
            }
            catch
            { }
        }
    }
}
