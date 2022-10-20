Shader "UI/Blur"    // 背景模糊
{
    Properties
    {
        [PerRendererData]_MainTex("Texture2D", 2D) = "white" {}
        _Blur("Blur", Range(0, 10)) = 5
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
            Name "Blur"
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
            float4 _ScreenTransparentTexture_TexelSize;
            half _Blur;
            CBUFFER_END
            
            // Object and Global properties
            TEXTURE2D(_ScreenTransparentTexture);
            SAMPLER(sampler_ScreenTransparentTexture);

            Varyings vert(Attributes input)
            {
                Varyings output;
                ZERO_INITIALIZE(Varyings, output);
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                float3 positionWS = TransformObjectToWorld(input.positionOS);
                output.positionCS = TransformWorldToHClip(positionWS);
                output.uv = input.uv;
                output.color = input.color;
                return output;
            }

            half4 frag(Varyings input) : SV_TARGET 
            {    
                UNITY_SETUP_INSTANCE_ID(input);
                UnityTexture2D unity_texture = UnityBuildTexture2DStructNoScale(_ScreenTransparentTexture);

                half offset = _Blur / 1000;
                half4 color = SAMPLE_TEXTURE2D_X(unity_texture.tex, unity_texture.samplerstate, input.uv.xy);
                color += SAMPLE_TEXTURE2D_X(unity_texture.tex, unity_texture.samplerstate, input.uv.xy + half2(offset, offset));
                color += SAMPLE_TEXTURE2D_X(unity_texture.tex, unity_texture.samplerstate, input.uv.xy + half2(-offset, -offset));
                color += SAMPLE_TEXTURE2D_X(unity_texture.tex, unity_texture.samplerstate, input.uv.xy + half2(-offset, offset));
                color += SAMPLE_TEXTURE2D_X(unity_texture.tex, unity_texture.samplerstate, input.uv.xy + half2(offset, -offset));
                color += SAMPLE_TEXTURE2D_X(unity_texture.tex, unity_texture.samplerstate, input.uv.xy + half2(0, -offset));
                color += SAMPLE_TEXTURE2D_X(unity_texture.tex, unity_texture.samplerstate, input.uv.xy + half2(0, offset));
                color += SAMPLE_TEXTURE2D_X(unity_texture.tex, unity_texture.samplerstate, input.uv.xy + half2(-offset, 0));
                color += SAMPLE_TEXTURE2D_X(unity_texture.tex, unity_texture.samplerstate, input.uv.xy + half2(offset, 0));
				return half4(color.rgb * input.color.rgb / 8, 1);
            } 
            ENDHLSL
        }
    }

    FallBack Off
}