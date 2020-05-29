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

	//get a scalar random value from a 3d value
	float rand3dTo1d(float3 value, float3 dotDir = float3(12.9898, 78.233, 37.719)) {
		//make value smaller to avoid artefacts
		float3 smallValue = sin(value);
		//get scalar value from 3d vector
		float random = dot(smallValue, dotDir);
		//make value more random by making it bigger and then taking teh factional part
		random = frac(sin(random) * 143758.5453);
		return random;
	}

	float3 rand3dTo3d(float3 value) {
		return float3(
			rand3dTo1d(value, float3(12.989, 78.233, 37.719)),
			rand3dTo1d(value, float3(39.346, 11.135, 83.155)),
			rand3dTo1d(value, float3(73.156, 52.235, 09.151))
			);
	}

	// source: https://www.ronja-tutorials.com/2018/09/29/voronoi-noise.html
	float3 voronoiNoise(float3 value) {
		float3 baseCell = floor(value);

		//first pass to find the closest cell
		float minDistToCell = 10;
		float3 toClosestCell;
		float3 closestCell;
		[unroll]
		for (int x1 = -1; x1 <= 1; x1++) {
			[unroll]
			for (int y1 = -1; y1 <= 1; y1++) {
				[unroll]
				for (int z1 = -1; z1 <= 1; z1++) {
					float3 cell = baseCell + float3(x1, y1, z1);
					float3 cellPosition = cell + rand3dTo3d(cell);
					float3 toCell = cellPosition - value;
					float distToCell = length(toCell);
					if (distToCell < minDistToCell) {
						minDistToCell = distToCell;
						closestCell = cell;
						toClosestCell = toCell;
					}
				}
			}
		}

		//second pass to find the distance to the closest edge
		float minEdgeDistance = 10;
		[unroll]
		for (int x2 = -1; x2 <= 1; x2++) {
			[unroll]
			for (int y2 = -1; y2 <= 1; y2++) {
				[unroll]
				for (int z2 = -1; z2 <= 1; z2++) {
					float3 cell = baseCell + float3(x2, y2, z2);
					float3 cellPosition = cell + rand3dTo3d(cell);
					float3 toCell = cellPosition - value;

					float3 diffToClosestCell = abs(closestCell - cell);
					bool isClosestCell = diffToClosestCell.x + diffToClosestCell.y + diffToClosestCell.z < 0.1;
					if (!isClosestCell) {
						float3 toCenter = (toClosestCell + toCell) * 0.5;
						float3 cellDifference = normalize(toCell - toClosestCell);
						float edgeDistance = dot(toCenter, cellDifference);
						minEdgeDistance = min(minEdgeDistance, edgeDistance);
					}
				}
			}
		}

		float random = rand3dTo1d(closestCell);
		return float3(minDistToCell, random, minEdgeDistance);
	}

	v2f vert(float4 vertex : POSITION)
	{
		v2f o;
		o.worldPosition = mul(unity_ObjectToWorld, vertex);
		o.pos = UnityObjectToClipPos(vertex);
		o.uvShadow = mul(unity_Projector, vertex);
		o.uvFalloff = mul(unity_ProjectorClip, vertex);
		UNITY_TRANSFER_FOG(o,o.pos);
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

			float noiseValue = voronoiNoise(float3(_Time[0] * _Speed, noisePosition*_Scale));
			float transformed = pow(noiseValue, _Sharpness)*_Opacity*fading(i.uvFalloff.x);
			result.a = transformed;

			UNITY_APPLY_FOG_COLOR(i.fogCoord, result, fixed4(0, 0, 0, 0));
			return result;
		}
	}
		ENDCG
	}
	}
}
