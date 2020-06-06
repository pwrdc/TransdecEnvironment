Shader "Effects/WavesMask"
{
    Properties
    {
		_Speed("Speed", Float) = 1.0
		// there are two layers of noise to create a clumping effect
		_Sharpness_1("Sharpness 1", Float) = 1.5
		_Scale_1("Scale 1", Float) = 2.0
		_Sharpness_2("Sharpness 2", Float) = 1.3
		_Scale_2("Scale 2", Float) = 1.0
		// which layer should be more important in mixing
		_Layers_Mixing("Layers Mixing", Range(0,1)) = 0.5
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

		float _Speed;
		float _Sharpness_1;
		float _Scale_1;
		float _Sharpness_2;
		float _Scale_2;
		float _Layers_Mixing;

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
			float layer1= 1-underwater_surface(float3(_Time[0] * _Speed, xzPos*_Scale_1), _Sharpness_1);
			float layer2 = 1-underwater_surface(float3(_Time[0] * _Speed, xzPos*_Scale_2), _Sharpness_2);
			o.Alpha = -lerp(-layer1, -layer2, _Layers_Mixing);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
