Shader "UnderAmsterdam/SubsurfaceTest"
{
    Properties
    {
        _MainTex ("Diffuse", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "white" {}
        _NormalPower("Normal Power", Range(-10,10)) = 0
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
            float3 viewDir;
        };

        sampler2D _MainTex;
        sampler2D _NormalMap;
        float _NormalPower;

        void surf(Input IN, inout SurfaceOutput o) {
            o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
            o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap)) * _NormalPower;
        }
        ENDCG
    }
    Fallback "Diffuse"
}
