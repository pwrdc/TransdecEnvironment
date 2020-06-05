// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'

Shader "Effects/Underwater Surface" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_Opacity("Opacity", Range(0, 1)) = .5
		_Sharpness("Sharpness", Float) = 3.0
		_Speed("Speed", Float) = 1.0
		_Scale("Scale", Float) = 1.0
		_Fading("Fading", Float) = 30
	}

		Subshader{
		Tags{ "Queue" = "Transparent" }
		Pass{
		ZWrite Off
		ColorMask RGB
		Blend SrcAlpha OneMinusSrcAlpha
		Offset -1, -1

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fog
#include "UnityCG.cginc"
#include "UnderwaterSurface.cginc"

	struct v2f {
		float4 uvShadow : TEXCOORD0;
		float4 uvFalloff : TEXCOORD1;
		UNITY_FOG_COORDS(2)
		float4 worldPosition : TEXCOORD4;
		float4 pos : SV_POSITION;
	};

	float _Opacity;
	float _Sharpness;
	float _Speed;
	float _Scale;
	float _Fading;
	fixed4 _Color;

	float4x4 unity_Projector;
	float4x4 unity_ProjectorClip;

	v2f vert(float4 vertex : POSITION)
	{
		v2f o;
		o.worldPosition = mul(unity_ObjectToWorld, vertex);
		o.pos = UnityObjectToClipPos(vertex);
		o.uvShadow = mul(unity_Projector, vertex);
		o.uvFalloff = mul(unity_ProjectorClip, vertex);
		UNITY_TRANSFER_FOG(o, o.pos);
		return o;
	}

	float fading(float x) {
		float transformed = x * _Fading + 1;
		return 1 / transformed;
	}

	bool isValidUv(float4 uv) {
		return uv.x >= 0 && uv.x <= 1 && uv.y >= 0 && uv.y <= 1;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		fixed4 uv = UNITY_PROJ_COORD(i.uvShadow);
		if (i.uvFalloff.x < 0 || !isValidUv(uv))
		{
			return float4(0, 0, 0, 0);
		}
		else {
			float2 noisePosition = float2(i.worldPosition.x, i.worldPosition.z);

			fixed4 result = _Color;
			result.a = underwater_surface(float3(_Time[0] * _Speed, noisePosition*_Scale), _Sharpness) * _Opacity*fading(i.uvFalloff.x);

			UNITY_APPLY_FOG_COLOR(i.fogCoord, result, fixed4(0, 0, 0, 0));
			return result;
		}
	}
		ENDCG
	}
	}
}
