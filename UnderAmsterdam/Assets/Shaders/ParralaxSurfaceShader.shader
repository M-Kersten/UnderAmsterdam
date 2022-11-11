Shader "UnderAmsterdam/ParralaxSurfaceShader"
{
    Properties
    {
        _MainTex ("Diffuse", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "white" {}
        _HeightTex("Height map", 2D) = "white" {}
        _HeightPower("HeightPower", Range(-10,10)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        CGPROGRAM

        #pragma surface surf Lambert
        struct Input {
            float2 uv_MainTex;
            float2 uv_NormalMap;
            float2 uv_HeightMap;
            float3 viewDir;
        };

        sampler2D _MainTex;
        sampler2D _NormalMap;
        sampler2D _HeightMap;
        float _HeightPower;

        void surf(Input IN, inout SurfaceOutput o) {
            float2 texOffset = ParallaxOffset(tex2D(_HeightMap, IN.uv_HeightMap).r, _HeightPower, IN.viewDir);

            o.Albedo = tex2D(_MainTex, IN.uv_MainTex + texOffset).rgb;
            o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap + texOffset));
        }
        ENDCG
    }
    Fallback "Diffuse"
}
