Shader "Hidden/Custom/VaryingBlur"
{
	HLSLINCLUDE

#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
#include "NoiseSimplex.cginc"

		float4 _Color;
		float _NoiseScale;
		float _NoiseChangeRate;
		TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);

		float4 Frag(VaryingsDefault i) : SV_Target
		{
			float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);

			float noiseValue = snoise(float3(i.texcoord.x*_NoiseScale, i.texcoord.y*_NoiseScale, _Time[0] * _NoiseChangeRate));

			return lerp(color, _Color, noiseValue);
		}

		ENDHLSL

		SubShader
	{
	Cull Off ZWrite Off ZTest Always

	Pass
		{
			HLSLPROGRAM

				#pragma vertex VertDefault
				#pragma fragment Frag

			ENDHLSL
		}
	}
}