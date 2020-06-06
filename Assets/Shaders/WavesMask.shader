﻿Shader "Effects/WavesMask"
{
    Properties
    {
		_Sharpness("Sharpness", Float) = 3.0
		_Speed("Speed", Float) = 1.0
		_Scale("Scale", Float) = 1.0
		// this variable is created and managed by alphatest option implementation
		_Cutoff("Alpha Cutoff", Range(0,1)) = 0.7
    }
    SubShader
    {
		Tags{ "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard alphatest:_Cutoff addshadow

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

#include "UnderwaterSurface.cginc"

		float _Sharpness;
		float _Speed;
		float _Scale;

        struct Input
        {
            float2 uv_MainTex;
			float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutputStandard  o)
        {
            o.Albedo = float3(1,1,1);
			float3 localPos = IN.worldPos - mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
			float2 xzPos = float2(localPos.x, localPos.z);
            o.Alpha = 1-underwater_surface(float3(_Time[0] * _Speed, xzPos*_Scale), _Sharpness);
        }
        ENDCG
    }
    FallBack "Diffuse"
}