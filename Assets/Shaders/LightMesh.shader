Shader "Effects/LightMesh" {
	Properties{
		_Opacity("Opacity", Range(0, 1)) = .5
		_Emission("Emission", Range(0, 1)) = .5
		_Numerator("Numerator", Float) = 3
	}
		SubShader{
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
		LOD 200
		Pass{
			ColorMask 0
		}
		// Render normally
		ZWrite Off
		BlendOp Add
		ColorMask RGB

		CGPROGRAM

#pragma surface surf Standard fullforwardshadows alpha:fade
#pragma target 3.0
	struct Input {
		float2 uv_MainTex;
		float3 worldPos;
	};
	float _Opacity;
	float _Emission;
	float _Numerator;

	#include "UnityBuiltin2xTreeLibrary.cginc"

	void surf(Input IN, inout SurfaceOutputStandard o) {
		float3 localPosition = IN.worldPos - mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
		// float opacity = 10/sqrt(localPosition.x*localPosition.x+ localPosition.z*localPosition.z);
		float opacity = _Numerator/localPosition.y;
		o.Alpha = _Opacity * opacity;
		o.Emission = _Emission* opacity;
	}
	ENDCG
	}
		Fallback "Diffuse"
}