Shader "Hidden/Custom/Distortions"
{
	HLSLINCLUDE

#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
#include "NoiseSimplex.cginc"

		TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
	float _Scale;
	float _Intensity;
	float _Speed;

	/*
	Uses screen coordinates and time to index pseudo-random 3d to 2d vector field.
	The field is created by using 3d to 1d simplex noise two times, 
	one for each output vector coordinate with different offsets.
	*/
	float4 Frag(VaryingsDefault i) : SV_Target
	{
		float2 uv = i.texcoord;
		float screenHeight = _ScreenParams.y;
		float ratio = _ScreenParams.x / _ScreenParams.y;
		float3 noise_coordinates = float3(uv.x / screenHeight * ratio * _Scale, uv.y / screenHeight * _Scale, _Speed * _Time[0]);
		float x_to_edge = min(uv.x, 1 - uv.x);
		float y_to_edge = min(uv.y, 1 - uv.y);
		uv.x += snoise(noise_coordinates) * _Intensity * x_to_edge;
		// 0.5 is used as an offset here because the interval of noise function is one 
		uv.y += snoise(noise_coordinates*_Scale + 0.5) * _Intensity * y_to_edge;

		uv.x %= 1;
		uv.y %= 1;
		float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
		// uncomment this to see the noise values
		// col = float4(snoise(noise_coordinates) * _Intensity * x_to_edge, snoise(noise_coordinates) * _Intensity * x_to_edge, snoise(noise_coordinates) * _Intensity * x_to_edge, 1);
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
