Shader "Hidden/Custom/Distortions"
{
	HLSLINCLUDE

#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
#include "NoiseSimplex.cginc"
#include "Utils.cginc"

	TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
	float _Scale;
	float _Intensity;
	float _Speed;

	/*
	Uses screen coordinates and time to index pseudo-random 3d to 2d vector field.
	The field is created by using 3d to 1d simplex noise two times, 
	one for each output vector coordinate with different offsets.
	The pixel color is sampled from (uv + random_vector) which results
	in screen surface stretching and distortion effect.
	*/
	float4 Frag(VaryingsDefault i) : SV_Target
	{
		float2 uv = i.texcoord;
		float3 noise_coordinates = float3(stretch_screen_uv_coordinates(uv) * _Scale, _Speed * _Time[0]);

		float x_to_edge = pow(min(uv.x, 1 - uv.x), 2);
		float y_to_edge = pow(min(uv.y, 1 - uv.y), 2);
		// pow function makes the transition less harsh 
		float delta_x = min(snoise(noise_coordinates * _Scale) *  _Intensity, pow(x_to_edge, 2));
		// 0.5 is used as an offset here because the interval of noise function is one 
		float3 offset = float3(0.5, 0.5, 0.5);
		float delta_y = min(snoise(noise_coordinates * _Scale + offset) *  _Intensity, pow(y_to_edge, 2));
		uv.x += delta_x;
		uv.y += delta_y;
		float4 result = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
		// uncomment this to see the noise values, max is used here because it visualises them best
		// result = max(delta_x, delta_y) / _Intensity;
		return result;
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
