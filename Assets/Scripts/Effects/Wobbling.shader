Shader "Hidden/Custom/Wobble"
{
	HLSLINCLUDE

		#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

		TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
		float _Scale;
		float _Intensity;
		float _Speed;

		float4 Frag(VaryingsDefault i) : SV_Target
		{
			float2 uv = i.texcoord;
			float sine = sin((uv.y*_Scale + _Speed * _Time[0]) * _Intensity);
			uv.x += sine * 0.01;

			float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
			return col;
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
