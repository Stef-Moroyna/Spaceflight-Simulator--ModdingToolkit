#if UNITY_EDITOR
using SFS.World;
using SFS.Builds;
using SFS.Parts;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DepthScanner : SerializedMonoBehaviour
{
    Texture2D scan;

    public Dictionary<Part, Color> parts = new Dictionary<Part, Color>();

    public float threshold = 0.5f;

    [Required] public Camera cam;

    public Image output;

    [Button]
    public void Scan(Rocket rocket)
    {
        scan = new Texture2D(cam.pixelWidth / 4, cam.pixelHeight / 4);

        Color[] pixels = new Color[scan.width * scan.height];

        StartCoroutine(Scan(rocket.partHolder.GetArray(), pixels));
    }

    [Button]
    public void ScanBuild()
    {
        scan = new Texture2D(cam.pixelWidth / 4, cam.pixelHeight / 4);

        Color[] pixels = new Color[scan.width * scan.height];

        StartCoroutine(Scan(BuildManager.main.buildGrid.activeGrid.partsHolder.GetArray(), pixels));
    }

    IEnumerator Scan(Part[] parts, Color[] pixels)
    {
        int x = 0;
        while (x < scan.width)
        {
            PartialScan(parts, x, pixels);
            x++;
            yield return null;
        }

        scan.SetPixels(pixels);

        File.WriteAllBytes(Application.dataPath + "/Scan.png", scan.EncodeToPNG());

        if (output != null)
            output.gameObject.SetActive(true);

        AssetDatabase.Refresh();
    }

    void PartialScan(Part[] parts, int x, Color[] pixels)
    {
        for (int y = 0; y < scan.height; y++)
        {
            int yA = y * 4;
            int yB = yA + 2;
            int xA = x * 4;
            int xB = xA + 2;
            Vector2 posA = cam.ScreenToWorldPoint(new Vector3(xA, yA) + new Vector3(0, 0, -cam.transform.position.z));
            Vector2 posB = cam.ScreenToWorldPoint(new Vector3(xB, yB) + new Vector3(0, 0, -cam.transform.position.z));
            Debug.DrawLine(posA, posB, Color.red, 0.5f);
            Vector2 pos = Vector2.Lerp(posA, posB, 0.5f);
            Part_Utility.RaycastParts(parts, pos, threshold, out PartHit hit);
            pixels[x + (y * scan.width)] = GetColor(hit.part);
        }
    }

    Color GetColor(Part part)
    {
        if(part != null)
        {
            if (parts.ContainsKey(part))
            {
                return parts[part];
            }
            else
            {
                Color color = GenerateColor();
                parts.Add(part, color);
                return color;
            }
        }
        return Color.clear;
    }

    Color GenerateColor()
    {
        Color color;

        do
            color = Random.ColorHSV();
        while (color == Color.clear || parts.ContainsValue(color));

        return color;
    }
}
#endif