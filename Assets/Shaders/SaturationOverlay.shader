Shader "Custom/SaturationOverlay"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Saturation ("Saturation", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Saturation;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Pobierz teksturę i pomnóż przez kolor
                fixed4 texColor = tex2D(_MainTex, i.uv);
                fixed4 col = texColor * _Color;
                
                // Konwersja do grayscale
                float gray = dot(col.rgb, float3(0.299, 0.587, 0.114));
                
                // Interpolacja między grayscale a oryginalnym kolorem
                col.rgb = lerp(float3(gray, gray, gray), col.rgb, _Saturation);
                
                return col;
            }
            ENDCG
        }
    }
}