﻿Shader "ZView/PointCloud"
{
   Properties {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _PointSize ("Point Size", Range(1,30)) = 3
        _Bottom ("Bottom", Range(-5,0)) = -1
    }
    SubShader {
        Tags { "RenderType"="Transparent" }
        Cull Off ZWrite On Blend SrcAlpha OneMinusSrcAlpha
        Pass {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct v2f {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float size: PSIZE;
            };
            float4 _Color;
            float _PointSize;
            float _Bottom;

            v2f vert(appdata_base v)
            {
                v2f o;
				float3 n = UnityObjectToWorldNormal(v.normal);
                o.pos = UnityObjectToClipPos (v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.size = _PointSize;
                return o;
            }
            half4 frag (v2f i) : COLOR
            {
            	// float4 red  = float4(255.0/255,70.0/255,150.0/255,1);
            	// float4 blue = float4(90.0/255,90.0/255,250.0/255,1);
                // return lerp(red, blue, i.worldPos.y*0.2);
                if(i.worldPos.y < _Bottom)
                    return float4(1,0,0,1);
                else
                    return _Color;
            }
            ENDCG
        }
    } 
    FallBack Off
}
