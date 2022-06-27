Shader "SFS/Shock"
{
	Properties
	{
	    [PerRendererData] _Opacity("Opacity", float) = 1
	
	    _MainTex("_MainTex", 2D) = "white" {}
        _Vertical("_Vertical", 2D) = "white" {}
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"	
			"PreviewType" = "Plane"
		}

		Cull Off
		Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha
	
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
                fixed4 color : COLOR;
                float3 UVQ_0 : TEXCOORD0;
        
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 UVQ_0 : TEXCOORD0;
                fixed2 alpha : fixed2;
        
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            v2f vert(appdata_t IN)
            {
                v2f OUT;
        
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
        
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                
                OUT.UVQ_0 = IN.UVQ_0;
                OUT.alpha = fixed2(IN.color.b, IN.color.a) * OUT.UVQ_0.z;
        
                return OUT;
            }
            
            sampler2D _MainTex, _Vertical;
            float _Opacity;
        
            #if defined(SHADER_API_GLES)
            fixed4 frag(v2f IN) : SV_Target
            #else
            fixed4 frag(v2f IN, out float depth : SV_Depth) : SV_Target
            #endif
            {
                // Depth
                #if defined(SHADER_API_GLES)
                #else
                    #if defined(UNITY_REVERSED_Z)
                        depth = 1;
                    #else
                        depth = 0;
                    #endif
                #endif
            
                float2 uv = IN.UVQ_0.xy / IN.UVQ_0.z;
                IN.alpha /= IN.UVQ_0.z;
        
                fixed r = tex2D(_MainTex, uv).r * (0.5 + tex2D(_Vertical, float2(uv.x, uv.y / 7 - _Time.a)).r);
                fixed V = r * _Opacity * (IN.alpha.x * IN.alpha.y);
                
                fixed4 c = fixed4(1, 1, 1, V);
                
                c.rgb = 1.05 - V * 0.1;
                
                c.rgb *= min(c.a, 1);
                
                return c;
            }
            ENDCG
        }
	}
}