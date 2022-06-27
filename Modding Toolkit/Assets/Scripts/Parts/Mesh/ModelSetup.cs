// Decompiled reference

using System;
using SFS;
using SFS.World;
using UnityEngine;

public class ModelSetup : MonoBehaviour, I_InitializePartModule
{
	private static readonly int ColorTex = Shader.PropertyToID("_ColorTex");

	private static readonly int NormalTex = Shader.PropertyToID("_NormalTex");

	private static readonly int DepthStart = Shader.PropertyToID("_DepthStart");

	private static readonly int DepthM = Shader.PropertyToID("_DepthM");

	private static readonly int LightNormal = Shader.PropertyToID("_LightNormal");

	public MeshRenderer[] meshRenderers;

	public Texture2D colorTex;

	public Texture2D normalTex;

	public float smoothness;

	public int renderQueueOffset;

	public bool useNormals;

	public bool dontSetMaterial;

	private string sortingLayer;

	private float depthOffset;

	int I_InitializePartModule.Priority => 0;

	private void Reset()
	{
		GetRenderers();
	}

	public void SetSortingLayer(string sortingLayer)
	{
		this.sortingLayer = sortingLayer;
		SetMesh();
	}

	public void GetRenderers()
	{
		meshRenderers = GetComponentsInChildren<MeshRenderer>();
	}

	public void SetMesh()
	{
		MeshRenderer[] array = meshRenderers;
		foreach (MeshRenderer meshRenderer in array)
		{
			if (meshRenderer == null || colorTex == null || normalTex == null)
			{
				Debug.LogWarning(new Exception("Something null at " + base.name));
				continue;
			}
			if (!dontSetMaterial)
			{
				meshRenderer.sharedMaterial = GetMaterial();
			}
			MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
			float globalDepth = GetGlobalDepth(0.5f + depthOffset * 0.05f);
			float value = (GetGlobalDepth(1f) - GetGlobalDepth(0f)) * 0.05f;
			materialPropertyBlock.SetFloat(DepthStart, globalDepth);
			materialPropertyBlock.SetFloat(DepthM, value);
			materialPropertyBlock.SetTexture(ColorTex, colorTex);
			materialPropertyBlock.SetTexture(NormalTex, normalTex);
			Vector3 normalized = meshRenderer.transform.InverseTransformVector(GetLightDirection()).normalized;
			materialPropertyBlock.SetVector(LightNormal, normalized);
			meshRenderer.SetPropertyBlock(materialPropertyBlock);
		}
	}

	public void SetDepthOffset(float depthOffset)
	{
		this.depthOffset = depthOffset;
		SetMesh();
	}

	private Material GetMaterial()
	{
		int renderQueue = RenderSortingManager.main.GetRenderQueue(sortingLayer) + renderQueueOffset;
		return RenderSortingManager.main.GetPartModelMaterial(renderQueue, useNormals);
	}

	private float GetGlobalDepth(float depth)
	{
		if (!(RenderSortingManager.main != null))
		{
			return depth;
		}
		return RenderSortingManager.main.GetGlobalDepth(depth, sortingLayer);
	}

	private Vector3 GetLightDirection()
	{
		Vector3 normalized = new Vector3(useNormals ? 0.2f : (-0.2f), useNormals ? (-0.4f) : 0.4f, 1f).normalized;
		return normalized;
	}

	void I_InitializePartModule.Initialize()
	{
		SetMesh();
	}
}
