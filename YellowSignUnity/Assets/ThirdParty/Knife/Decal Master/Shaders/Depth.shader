﻿Shader "Hidden/DepthToTarget"
{
    Properties
    {
		_MainTex("Main texture", 2D) = "white"
    }
    SubShader
    {
            Tags {"RenderType" = "Opaque"}

        Pass
        {
            Tags {"RenderType" = "Opaque"}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				float2 uv : TEXCOORD;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD;
            };

			sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
				float4 col = tex2D(_MainTex, i.uv);
                return i.vertex.z;
            }
            ENDCG
        }
    }
}
