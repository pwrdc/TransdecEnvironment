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

				// IMPORTANT: comment out line below if you don't have Aura 2
				#define HAVE_AURA_2

				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fog
				#include "UnityCG.cginc"
				#ifdef HAVE_AURA_2
					#include "../Aura 2/Core/Code/Shaders/Aura.cginc"
				#endif

				struct v2f {
					float4 uvShadow : TEXCOORD0;
					float4 uvFalloff : TEXCOORD1;
					UNITY_FOG_COORDS(2)
					float3 frustrumSpacePosition : TEXCOORD3;
					float4 pos : SV_POSITION;
				};

				float4x4 unity_Projector;
				float4x4 unity_ProjectorClip;

				v2f vert(float4 vertex : POSITION)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(vertex);
					o.uvShadow = mul(unity_Projector, vertex);
					o.uvFalloff = mul(unity_ProjectorClip, vertex);
					UNITY_TRANSFER_FOG(o, o.pos);
					#ifdef HAVE_AURA_2
						o.frustrumSpacePosition = Aura2_GetFrustumSpaceCoordinates(vertex);
					#endif
					return o;
				}

				fixed4 _Color;
				sampler2D _ShadowTex;
				sampler2D _FalloffTex;

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 texS = tex2Dproj(_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
					fixed4 texF = tex2Dproj(_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff));
					fixed4 res = texS * texF.a* _Color;

					UNITY_APPLY_FOG_COLOR(i.fogCoord, texS, fixed4(0,0,0,0));
					#ifdef HAVE_AURA_2
						Aura2_ApplyFog(res, i.frustrumSpacePosition);
					#endif
					return res;
				}
				ENDCG
			}
	}
}
