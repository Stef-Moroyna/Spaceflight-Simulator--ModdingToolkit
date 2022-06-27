Shader "SFS/Line"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}

	Category
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
		}

		Blend SrcAlpha One
		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Color(0,0,0,0) }

		BindChannels
		{
			Bind "Color", color
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
		}

		SubShader
		{
			Pass
			{
				SetTexture[_MainTex]
				{
					combine texture * primary
				}
			}
		}
	}
}