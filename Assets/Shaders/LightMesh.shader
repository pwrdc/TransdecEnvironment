Shader "Unlit/InnerSpriteOutline HLSL"
{
	Properties{
		_Opacity("Opacity", Range(0, 2)) = .5
		_Numerator("Numerator", Float) = 3
		_Power("Power", Float) = 2
	}
    SubShader
    {
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
 
		Blend SrcAlpha OneMinusSrcAlpha
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
 
            #include "UnityCG.cginc"
 
            struct appdata
            {
                float4 vertex : POSITION;
            };
 
            struct v2f
            {
                float4 vertex : SV_POSITION;
				float3 viewPos : TEXCOORD0;
            };
 
			float _Opacity;
			float _Emission;
			float _Numerator;
			float _Power;
			float _VerticalMultiplier;
 
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.viewPos = UnityObjectToViewPos(v.vertex);
                return o;
            }
 
            fixed4 frag (v2f i) : SV_Target
            {
				float3 centerPosition = UnityObjectToViewPos(float4(0,0,0,0));
				float3 downPosition = UnityObjectToViewPos(float4(0, 0, -1, 0));
				// h dervied from ah/2=vw/2 where v and w are vectors
				float horizontalDistance = cross(i.viewPos - centerPosition, i.viewPos - downPosition)/distance(centerPosition, downPosition);

				float horizontal = horizontalDistance;
				float opacity= _Numerator * pow(abs(horizontal), _Power);
				opacity = min(opacity, 1);
				opacity *= _Opacity;
                return float4(1,1,1, opacity);
            }
            ENDCG
        }
    }
}