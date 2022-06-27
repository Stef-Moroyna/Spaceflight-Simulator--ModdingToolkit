Shader "SFS/GL Drawer"
{
	Properties
	{
		_Depth("Depth", Range(0, 1)) = 1
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"PreviewType" = "Plane"
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				return o;
			}

			float _Depth;


            #if defined(SHADER_API_GLES)
			float4 frag(v2f IN) : SV_Target
			{
				return IN.color;
			}
            #else
			float4 frag(v2f IN, out float depth : SV_Depth) : SV_Target
			{
                #if defined(UNITY_REVERSED_Z)
				depth = _Depth;
                #else
				depth = 1.0f - _Depth;
                #endif
                
				return IN.color;
			}
            #endif
			ENDCG
		}
	}
}