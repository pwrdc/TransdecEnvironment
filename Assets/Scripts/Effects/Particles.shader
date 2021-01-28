Shader "Hidden/Custom/Particles"
{
	HLSLINCLUDE

#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
#include "NoiseSimplex.cginc"
#include "Utils.cginc"

		TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
	float _Scale;
	float _Scale2;
	float _Speed;
	float _Cutout;
	float4 _Color;

	float random(float2 st) {
		return frac(sin(dot(st,
			float2(12.9898, 78.233)))*
			43758.5453123);
	}

	float random(float3 st) {
		return frac(sin(dot(st,
			float3(12.9898, 78.233, 21.37)))*
			43758.5453123);
	}

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
		
		float4 original = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
		
		return lerp(original, max(_Color, original), (random(floor(float3(stretch_screen_uv_coordinates(uv)*_Scale, _Speed * _Time[0]))))
			*random(floor(float3(stretch_screen_uv_coordinates(uv)*_Scale2, _Speed * _Time[0]))) > _Cutout );
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
