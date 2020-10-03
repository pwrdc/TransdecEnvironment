// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Effects/Underwater Surface Projector" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_Opacity("Opacity", Range(0, 1)) = .5
		_Sharpness("Sharpness", Float) = 3.0
		_Speed("Speed", Float) = 1.0
		_Scale("Scale", Float) = 1.0
		_OpacityByDepth("Opacity By Depth", Float) = 50
		_OpacityByDistance("Opacity By Distance", Float) = 25
	}

	Subshader{
		Tags{ "Queue" = "Transparent" }
		Pass{
			ZWrite Off
			ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha
			Offset -1, -1

			CGPROGRAM

			// IMPORTANT: comment out line below if you don't have Aura 2
			#define HAVE_AURA_2

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#include "UnityCG.cginc"
			#include "UnderwaterSurface.cginc"

			struct a2v
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;

			};

			struct v2f {
				float4 uvShadow : TEXCOORD0;
				float4 uvFalloff : TEXCOORD1;
				float4 worldPosition : TEXCOORD2;
				UNITY_FOG_COORDS(3)
				float cameraDistance : TEXCOORD4;
				// float3 frustrumSpacePosition : TEXCOORD5;
				float3 normal : TEXCOORD6;
				float4 pos : SV_POSITION;
			};

			float _Opacity;
			float _Sharpness;
			float _Speed;
			float _Scale;
			float _OpacityByDepth;
			float _OpacityByDistance;
			fixed4 _Color;
			uniform float4 _LightColor0;

			float4x4 unity_Projector;
			float4x4 unity_ProjectorClip;

			v2f vert(a2v v)
			{
				v2f o;
				o.worldPosition = mul(unity_ObjectToWorld, v.vertex);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.normal = mul(unity_ObjectToWorld, float4(v.normal, 0.0)).xyz;
				o.uvShadow = mul(unity_Projector, v.vertex);
				o.uvFalloff = mul(unity_ProjectorClip, v.vertex);
				o.cameraDistance = length(_WorldSpaceCameraPos - o.worldPosition);
				UNITY_TRANSFER_FOG(o, o.pos);
				return o;
			}

			float depthFading(float x) {
				float transformed = x * _OpacityByDepth + 1;
				return 1 / transformed;
			}

			// using aura fog generates something like waves of light
			// so this is a bug-free approximation
			float distanceFading(float x) {
				float result = x / _OpacityByDistance;
				return 1 / result;
			}

#define PI 3.14

			float angleFading(float3 normal) {
				// angle is <0, pi>
				// where 0 is parallel
				// you want it to ooutput 0 starting from pi/2
				float angle = acos(dot(normal, float3(0, 1, 0)) / length(normal));
				float opacity = angle/PI;
				opacity = 1 - opacity;
				opacity = smoothstep(0.5, 1, opacity);
				return opacity;
			}

			bool isValidUv(float4 uv) {
				return uv.x >= 0 && uv.x <= 1 && uv.y >= 0 && uv.y <= 1;
			}

			// All components are in the range [0…1], including hue.
			float3 rgb2hsv(float3 c)
			{
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
				float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

				float d = q.x - min(q.w, q.y);
				float e = 1.0e-10;
				return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 uv = UNITY_PROJ_COORD(i.uvShadow);

				// if uv is not in bounds then the pixel isn't lit by the projector
				// if i.uvFalloff.x is lesser than zero then the pixel is behind the projector
				// without this check every object on the scene would have the effect
				if (i.uvFalloff.x < 0 || !isValidUv(uv))
				{
					return float4(0, 0, 0, 0);
				}
				else {
					float2 noisePosition = float2(i.worldPosition.x, i.worldPosition.z);

					fixed4 result = _Color;
					// get lightness from 0th light on the scene 
					float lightness = rgb2hsv(_LightColor0)[2];// second component corresponds to value in hsv
					// this adds the voronoi noise texture
					float surfaceTexture = underwater_surface(float3(_Time[0] * _Speed, noisePosition*_Scale), _Sharpness);
					// mix all of the values in alpha channel
					result.a = surfaceTexture * _Opacity * depthFading(i.uvFalloff.x) * lightness * distanceFading(i.cameraDistance) * angleFading(i.normal);
					result.a = clamp(result.a, 0, 1);

					UNITY_APPLY_FOG_COLOR(i.fogCoord, result, fixed4(0, 0, 0, 0));
					return result;
				}
			}
			ENDCG
		}
	}
}
