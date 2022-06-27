Shader "SFS/Flame Aditive"
{
	Properties
	{
		_Tex("Texture Atlas", 2D) = "white" {}
	}

	SubShader
	{
		Tags
		{		
			"Queue" = "Transparent"		
			"IgnoreProjector" = "True"		
			"RenderType" = "Transparent"		
			"PreviewType" = "Plane"	
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One One

		Pass
		{
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnityCG.cginc"
        
            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float3 UV : TEXCOORD0;
        
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float3 UV : TEXCOORD0;
        
                UNITY_VERTEX_OUTPUT_STEREO
            };
        
            v2f vert(appdata_t IN)
            {
                v2f OUT;
        
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
        
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
        
                OUT.UV = IN.UV;
                OUT.color = IN.color;
        
                return OUT;
            }
        
            sampler2D _Tex;
        
            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = tex2D(_Tex, IN.UV);
                c.rgb *= c.a;
                return c;
            }
            ENDCG
        }
	}
}