// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'

Shader "Projector/Alpha Blended" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_ShadowTex("Cookie", 2D) = "" {}
		_FalloffTex("FallOff", 2D) = "" {}
	}

		Subshader{
			Tags {"Queue" = "Transparent"}
			Pass {
				ZWrite Off
				ColorMask RGB
				Blend SrcAlpha OneMinusSrcAlpha
				Offset -1, -1

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fog
				#include "UnityCG.cginc"

				struct a2v
				{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
				};

				struct v2f {
					float4 uvShadow : TEXCOORD0;
					float4 uvFalloff : TEXCOORD1;
					UNITY_FOG_COORDS(2)
					float opacity : TEXCOORD3;
					float4 pos : SV_POSITION;
				};

				float4x4 unity_Projector;
				float4x4 unity_ProjectorClip;

				v2f vert(a2v v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uvShadow = mul(unity_Projector, v.vertex);
					o.uvFalloff = mul(unity_ProjectorClip, v.vertex);
					float3 relativeNormal = mul(unity_Projector, v.normal);
					o.opacity = abs(dot(float3(0, 0, 1), relativeNormal));
					UNITY_TRANSFER_FOG(o, o.pos);
					return o;
				}

				fixed4 _Color;
				sampler2D _ShadowTex;
				sampler2D _FalloffTex;

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 texS = tex2Dproj(_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
					texS *= _Color;
					// texS *= i.opacity;
					// texS.a = 1.0 - texS.a;

					// fixed4 texF = tex2Dproj(_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff));
					// fixed4 res = texS * texF.a;

					UNITY_APPLY_FOG_COLOR(i.fogCoord, texS, fixed4(0,0,0,0));
					return texS;
				}
				ENDCG
			}
	}
}
