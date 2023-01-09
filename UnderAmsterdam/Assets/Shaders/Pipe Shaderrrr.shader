Shader "Ocias/Diffuse (Stipple Transparency)"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Range ("Range", float) = 1.0
        _Offset ("Offset", float) = 0.0
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Transparency ("Transparency", Range(0,1)) = 1.0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 150
        CGPROGRAM
        #pragma surface surf Lambert noforwardadd
        sampler2D _MainTex;

        struct Input {
            float2 uv_MainTex;
            float4 screenPos;
        };

        half _Transparency;
        
        half   _Glossiness;
        half   _Metallic;
        fixed4 _Color;
        float  _Range;
        float  _Offset;
        
        void surf(Input IN, inout SurfaceOutput o)
        {
            o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;

            float2 screenUV = IN.screenPos.xy / IN.screenPos.w;

#if UNITY_SINGLE_PASS_STEREO

            // If Single-Pass Stereo mode is active, transform the

            // coordinates to get the correct output UV for the current eye.

            float4 scaleOffset = unity_StereoScaleOffset[unity_StereoEyeIndex];

            screenUV = (screenUV - scaleOffset.zw) / scaleOffset.xy;
#endif
            
            // Screen-door transparency: Discard pixel if below threshold.
            float4x4 thresholdMatrix =
            {1.0 / 17.0, 9.0 / 17.0, 3.0 / 17.0, 11.0 / 17.0,
             13.0 / 17.0, 5.0 / 17.0, 15.0 / 17.0, 7.0 / 17.0,
             4.0 / 17.0, 12.0 / 17.0, 2.0 / 17.0, 10.0 / 17.0,
             16.0 / 17.0, 8.0 / 17.0, 14.0 / 17.0, 6.0 / 17.0
            };
            float4x4 _RowAccess = {1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1};
            
            float2   pos = IN.screenPos.xy / IN.screenPos.w;
            pos *= _ScreenParams.xy; // pixel position
            
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            
            _Transparency = saturate(ComputeScreenPos((IN.screenPos).a ) * _Range - _Offset);
            clip(_Transparency - thresholdMatrix[fmod(pos.x, 4)] * _RowAccess[fmod(pos.y, 4)]);
        }
        ENDCG
    }
    Fallback "Mobile/VertexLit"
}