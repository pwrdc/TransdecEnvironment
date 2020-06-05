Shader "Effects/Waves Translucency"
{
	Properties
	{
		_Opacity("Opacity", Range(0, 1)) = .5
		_Sharpness("Sharpness", Float) = 3.0
		_Speed("Speed", Float) = 1.0
		_Scale("Scale", Float) = 1.0
		_Cutoff("Alpha Cutoff", Range(0,1)) = 0.7
	}
		SubShader
	{
		Tags{ "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
		LOD 300

		Pass
	{

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask RGB

		CGPROGRAM
#pragma vertex vert Lambert alphatest : _Cutoff addshadow
#pragma fragment frag
	// make fog work
#pragma multi_compile_fog

#include "UnityCG.cginc"
#include "../Aura/Shaders/Aura.cginc"
#include "UnderwaterSurface.cginc"

	float _Opacity;
	float _Sharpness;
	float _Speed;
	float _Scale;

	struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float4 frustrumSpacePosition : TEXCOORD1;
		float4 worldPosition : TEXCOORD2;
		UNITY_FOG_COORDS(3)
		float4 vertex : SV_POSITION;
	};

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.worldPosition = mul(unity_ObjectToWorld, v.vertex);
		UNITY_TRANSFER_FOG(o,o.vertex);
		Aura_GetFrustumSpaceCoordinates(v.vertex);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		// sample the texture
		fixed4 col = float4(1,1,1,1);
		// apply fog
		UNITY_APPLY_FOG(i.fogCoord, col);

		float3 localPos = i.worldPosition - mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
		float2 xzPos = float2(localPos.x, localPos.z);

		col.a = underwater_surface(float3(_Time[0] * _Speed, xzPos*_Scale), _Sharpness) * _Opacity;

		float3 color = float3(col.xyz);
		//Aura_ApplyLighting(color, i.frustrumSpacePosition, 1.0f);
		//Aura_ApplyFog(color, i.frustrumSpacePosition);
		col = fixed4(color, col.a);
		if (col.a > 0.5) {
			col.a = 1;
		}
		else {
			col.a = 0;
		}

		return col;
	}
		ENDCG
	}
	}
}
