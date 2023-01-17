Shader "Custom/RootShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,0)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _AlphaClip ("AlphaClip", Range(0,1)) = 0.5
        _GrowSteps("GrowSteps", Range(0, 1)) = 0.75
        _Smoothness("Smoothness", Range(0, 1)) = 0.5
        _Scale("Scale", Float) = 0.6
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent" "Queue" = "Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        LOD 200

        CGPROGRAM// Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alpha vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input {
            float2 uv_MainTex;
            float4 worldPos;
            float4 worldNormal;
            float4 screenPos;

            float test;
        };

        half   _Metallic;
        fixed4 _Color;
        float  _AlphaClip;
        float  _GrowSteps;
        half   _Smoothness;
        float  _Scale;
        float  util;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
        // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


        void vert(inout appdata_full v, out Input IN)
        {
            UNITY_INITIALIZE_OUTPUT(Input, IN);
            IN.test = saturate(v.texcoord.y - (_GrowSteps - 0.03) / 0.97);
            v.vertex.xyz += v.normal * (IN.test * _Scale);
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float alpha = 1 - IN.test;
            clip(alpha - _AlphaClip);

#if UNITY_SINGLE_PASS_STEREO

            // If Single-Pass Stereo mode is active, transform the

            // coordinates to get the correct output UV for the current eye.

            float4 scaleOffset = unity_StereoScaleOffset[unity_StereoEyeIndex];
            screenUV = (screenUV - scaleOffset.zw) / scaleOffset.xy;
#endif

            o.Alpha = alpha;

            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;

            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
        }
        
        ENDCG
    }
    FallBack "Diffuse"
}