Shader "SFS/Post Processing"
{
	Properties
	{
        _MainTex("Screen", 2D) = "white" {}	

		_Hue("_Hue", float) = 0
		_Saturation("_Saturation", float) = 1
		_Contrast("_Contrast", float ) = 1
		_Multiplier("_Multiplier", vector) = (1, 1, 1, 1)
	}

	SubShader
	{
		Cull Off
		ZWrite Off
		ZTest Off
		Lighting Off
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
                float4 vertex   : POSITION;
                float3 texcoord : TEXCOORD0;
            };
        
            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float3 texcoord  : TEXCOORD0;
            };
        
            v2f vert(appdata_t IN)
            {
                v2f OUT;
        
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
        
                return OUT;
            }
        
            sampler2D _MainTex;	
        
            float _Hue;
            float _Saturation;
            float _Contrast;
            float4 _Multiplier;
        
            inline float3 Saturation(float3 sourcePixel, float aSaturation)
            {
                float luminance = dot( sourcePixel, float3(0.22, 0.707, 0.071) );
                return lerp( luminance, sourcePixel, aSaturation);
            }
        
            inline float3 Hue(float3 sourcePixel, float aHue)
            {
                float angle = radians(aHue);
                float3 coeff = float3(0.57735, 0.57735, 0.57735);
                return coeff * dot(coeff, sourcePixel) * (1 - cos(angle)) + cross(coeff, sourcePixel) * sin(angle) + sourcePixel  * cos(angle);
            }
        
            fixed4 frag(v2f IN) : SV_Target
            {
                float4 c = tex2D( _MainTex, IN.texcoord);
        
                // Contrast
                c.rgb = float3(c.r > 0.5? ((c.r - 0.5) * _Contrast + 0.5) : c.r, c.g > 0.5? ((c.g - 0.5) * _Contrast + 0.5) : c.g, c.b > 0.5? ((c.b - 0.5) * _Contrast + 0.5) : c.b);
        
                // Hue
                c.rgb = Hue(c.rgb, _Hue);
        
                // Saturation
                c.rgb = Saturation(c.rgb, _Saturation );
        
                // Mostly used for temperature
                c.rgb *= _Multiplier.rgb;
                
                return c;
            }
            ENDCG
        }
	}
}