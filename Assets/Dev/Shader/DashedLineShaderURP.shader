Shader "Custom/DashedLineShaderURP"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        _BackgroundColor ("Background Color", Color) = (0, 0, 0, 0)
        _DashLength ("Dash Length", Range(0.01, 1.0)) = 0.1
        _GapLength ("Gap Length", Range(0.01, 1.0)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType" = "transparent" }
        LOD 200

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _BaseColor;
            float4 _BackgroundColor;
            float _DashLength;
            float _GapLength;

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS);
                output.uv = input.uv;
                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                // Calculate the position within the pattern repeat
                float totalLength = _DashLength + _GapLength;
                float pos = fmod(input.uv.x, totalLength);

                // Determine if we're in a dash or a gap
                float4 color = pos < _DashLength ? _BaseColor : _BackgroundColor;

                return color;
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/InternalErrorShader"
}
