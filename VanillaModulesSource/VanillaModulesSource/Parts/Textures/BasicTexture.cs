using UnityEngine;
using System;

namespace SFS.Parts
{
    [CreateAssetMenu]
    public class BasicTexture : ScriptableObject
    {
        public Layer colorTex, shapeTex, shadowTex;

        [Serializable]
        public class Layer
        {
            public Texture2D texture;
            public bool flipToLight_X = true;
            public bool flipToLight_Y = true;

            public Line2 Get_Rect(Transform holder)
            {
                Line2 A = Line2.StartSize(Vector2.zero, Vector2.one);

                // Orientation
                if (flipToLight_X && Vector2.Angle(holder.TransformVector(Vector2.left), new Vector2(-1, 1)) > 90)
                    A.FlipHorizontally();
                if (flipToLight_Y && Vector2.Angle(holder.TransformVector(Vector2.up), new Vector2(-1, 1)) > 90)
                    A.FlipVertically();

                return A;
            }
        }
    }
}