Shader "Custom/Silhouette Sprite" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _AlphaCutoff ("AlphaCutoff", float) = 0
    }

    SubShader {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100

        Cull Off // Disable backface culling

        Pass {
            //Blend One OneMinusSrcAlpha
            ZWrite Off
            Offset -1, -1
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _AlphaCutoff;
            float4 _MainTex_ST;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 tex = tex2D(_MainTex, i.uv);
                fixed4 col = fixed4(1, 1, 1, tex.a); // set initial color to white with alpha from texture
                col.rgb *= tex.rgb; // multiply output color with texture color
                col.a = tex.a; // set output alpha to texture alpha
                if (tex.a <= _AlphaCutoff) discard; // discard transparent pixels
                return col;
            }
            ENDCG
        }

        Pass {
            ZTest Greater
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _AlphaCutoff;
            float4 _Color;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = _Color;
                col.a = tex2D(_MainTex, i.uv).a;
                if (col.a <= _AlphaCutoff) discard; // discard transparent pixels
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
