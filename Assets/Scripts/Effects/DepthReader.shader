Shader "Hidden/Custom/DepthReader"
{
	HLSLINCLUDE

	#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
	#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/Builtins/Fog.hlsl"

	TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);
	float _MaxDepth;
	float4 _ClearColor;

	float4 Frag(VaryingsDefault i) : SV_Target
	{
		float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoordStereo);
		if (depth == 0)
			return _ClearColor;
		depth = Linear01Depth(depth);
		float value = 1 - depth / _MaxDepth;
		value *= value;

		return float4(value.xxx, 1);
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