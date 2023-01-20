// Decompiled reference

using System;
using SFS;
using SFS.World;
using UnityEngine;

public class ModelSetup : MonoBehaviour
{
	public MeshRenderer[] meshRenderers;

	public Texture2D colorTex;

	public Texture2D normalTex;

	public float smoothness;

	public int renderQueueOffset;

	public bool useNormals;

	public bool dontSetMaterial;
}
