Shader "SFS/Flame Distorted"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_PerlinMap ("Shade Texture", 2D) = "white" {}
		_Multiplier ("Multiplier", float) = 1
		_Depth("Depth",  float) = 0
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
		Blend One OneMinusSrcAlpha

		Pass
		{
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnityCG.cginc"
            
            struct appdata_t {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
        
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
        
            struct v2f {
                float4 vertex   : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord  : TEXCOORD0;
        
                UNITY_VERTEX_OUTPUT_STEREO
            };
        
            v2f vert(appdata_t IN){
        
                v2f OUT;
        
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
        
                OUT.vertex = UnityObjectToClipPos(IN.vertex );
        
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color;
        
                return OUT;
            }
        
            sampler2D _MainTex;
            sampler2D _PerlinMap;
            float _Multiplier;
            float _Depth;


            #if defined(SHADER_API_GLES)
            fixed4 frag(v2f IN) : SV_Target
            #else
            fixed4 frag(v2f IN, out float depth : SV_Depth) : SV_Target
            #endif
            {
                #if defined(SHADER_API_GLES)
                #else
                    #if defined(UNITY_REVERSED_Z)
                        depth = _Depth;
                    #else
                        depth = 1.0f - _Depth;
                    #endif
                #endif
            
                float x = IN.texcoord * 4 - 2; // Convets x into a -1 to 1 range (double)
        
                // Adds distortion
                float offset = tex2D( _PerlinMap, float2( _Time.w, IN.texcoord.y * 0.4 * _Multiplier) ).r * 2 - 1; // -1 to 1 range
        
                x += offset * 3 * (1 - IN.texcoord.y);
        
                // Adds streching by verticality
                x *= IN.texcoord.y * 0.8 + 0.2;
        
                x = (x + 1) * 0.5; // Converts back into 0 to 1 range
        
                fixed4 c = tex2D( _MainTex, float2( x, IN.texcoord.y)) * IN.color;
                
                c.a *= IN.texcoord.y + 0.2;
                
                c.rgb *= c.a;
                return c;
            }
            
		    ENDCG
	    }
	}
}