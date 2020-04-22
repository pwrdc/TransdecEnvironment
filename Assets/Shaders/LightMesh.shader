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
				float3 difference = i.viewPos - centerPosition;
				float opacity= _Numerator / pow(abs(difference.x/difference.y), _Power);
				opacity = min(opacity, 1);
				opacity *= _Opacity;
                return float4(1,1,1, opacity);
            }
            ENDCG
        }
    }
}