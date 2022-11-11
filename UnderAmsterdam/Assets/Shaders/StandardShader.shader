Shader "UnderAmsterdam/StandardShader"
{
    Properties
    {
        [MainTexture] _MainTex ("Diffuse", 2D) = "white" {}
        _HeightTex("Height map", 2D) = "white" {}
        _HeightPower("HeightPower", Range(0,1)) = 0
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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _HeightTex;
            float4 _HeightTex_ST;
            float _HeightPower;

            v2f vert (appdata v)
            {
                v2f o;
                
                //Height stuff
                float2 uv = TRANSFORM_TEX(v.uv, _HeightTex);
                fixed displacement = tex2Dlod(_HeightTex, float4(uv, 0, 0)).rgb * _HeightPower;

                v.vertex.xyz += v.normal * displacement;

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
