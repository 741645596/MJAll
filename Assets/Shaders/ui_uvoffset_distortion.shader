Shader "UI/UVOffset Distortion"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _DistortionTex("Distortion Texture", 2D) = "white" {}
        _MaskTex("Mask Texture", 2D) = "white" {}
        _Speed("UV Speed", Vector) = (0, 0, 0, 0)
        _DistortionSpeed("Distortion Speed", Vector) = (0, 0, 0, 0)
        _DistortionScale("Distortion Scale", Range(0, 1)) = 0.1
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Transparent"
        }
        Pass
        {
            Name "UI Distortion"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            ZTest LEqual
            ZWrite Off

            HLSLPROGRAM

            // Pragmas
            #pragma vertex vert
            #pragma fragment frag

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
            {
                float3 positionOS : POSITION;
                float4 uv : TEXCOORD0;
                float4 color : COLOR;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 uv: TEXCOORD0;
                float4 color : COLOR;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_TexelSize;
            float4 _MaskTex_TexelSize;
            float4 _DistortionTex_TexelSize;
            float2 _DistortionSpeed;
            float _DistortionScale;
            float2 _Speed;
            CBUFFER_END
            
            // Object and Global properties
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);

            TEXTURE2D(_DistortionTex);
            SAMPLER(sampler_DistortionTex);
           
            Varyings vert(Attributes input)
            {
                Varyings output;
                ZERO_INITIALIZE(Varyings, output);
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                output.positionCS = TransformObjectToHClip(input.positionOS);
                output.uv = input.uv;
                output.color = input.color;
                return output;
            }

            half4 frag(Varyings input) : SV_TARGET 
            {    
                UNITY_SETUP_INSTANCE_ID(input);
                half2 distortion_uv = _Time.y * _DistortionSpeed.xy + input.uv.xy;

                UnityTexture2D distortion_texture = UnityBuildTexture2DStructNoScale(_DistortionTex);
                half4 distortion_color = SAMPLE_TEXTURE2D(distortion_texture.tex, distortion_texture.samplerstate, distortion_uv);
                half offset = distortion_color.r * input.color.a * _DistortionScale;

                float4 maskTexColor = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, input.uv.xy);
				half mask = maskTexColor.r;

                // half2 main_uv = input.uv.xy + offset * mask + frac(input.uv.xy + _Time.y * _Speed.xy);
                half2 main_uv = frac(input.uv.xy + _Time.y * _Speed.xy + offset);
                UnityTexture2D main_texture = UnityBuildTexture2DStructNoScale(_MainTex);
                half4 main_color = SAMPLE_TEXTURE2D(main_texture.tex, main_texture.samplerstate, main_uv);

				return half4(main_color.rgb, main_color.a * mask);
            } 
            ENDHLSL
        }
    }

    FallBack Off
}