
Shader "FX/ScreenTwist" {
    Properties {
		
        

		_TwistTex("Twist Tex", 2D) = "black"{}

		_TwistMaskTex("Twist Mask Tex", 2D) = "white"{}

		_TwistScale("Twist Scale", float) = 0

		_TwistScrollingX("Twist Scrolling X", Float) = 0

		_TwistScrollingY("Twist Scrolling Y", Float) = 0

		

		
    }
    SubShader {
		Name "ScreenTwist"
        Tags {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent" 
            "Queue" = "Transparent"
        }

		Blend SrcAlpha OneMinusSrcAlpha
		Cull Back
		ZTest LEqual
		ZWrite Off

        Pass {
            
			Tags{"LightMode" = "UniversalForward"}

			HLSLPROGRAM

			//Pragmas 
			#pragma vertex vert
			#pragma fragment frag 

			//Include
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			// #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"
			
			TEXTURE2D_X(_ScreenTransparentTexture);
			SAMPLER(sampler_ScreenTransparentTexture);
			
			TEXTURE2D(_TwistTex);
			SAMPLER(sampler_TwistTex);

			TEXTURE2D(_TwistMaskTex);
			SAMPLER(sampler_TwistMaskTex);

			CBUFFER_START(UnityPerMaterial)

			
			uniform float4 _TwistTex_ST;
			uniform float4 _TwistMaskTex_ST;

			uniform half _TwistScrollingX;
			uniform half _TwistScrollingY;

			uniform half _TwistScale;


			CBUFFER_END

		
			struct Attributes
			{
				float4 positionOS		:		POSITION;
				float3 normalOS 		:		NORMAL;
				float4 Color	 		: 		COLOR;
				float2 uv				:		TEXCOORD0;
				

			};
			struct Varyings
			{
				float4 positionCS		:		SV_POSITION;
				float4 Color			:		COLOR;
				float2 uv           	:       TEXCOORD0;
				float3 viewDirWS		:		TEXCOORD1;
				half3  normalWS			:		TEXCOORD2;
				float2 screenUV         :       TEXCOORD3;
			};

			float3 SampleSceneColor(float2 uv)
			{
				return SAMPLE_TEXTURE2D_X(_ScreenTransparentTexture, sampler_ScreenTransparentTexture, UnityStereoTransformScreenSpaceTex(uv)).rgb;
			}

			Varyings vert(Attributes IN)
			{
				Varyings OUT;

				VertexPositionInputs posInput = GetVertexPositionInputs(IN.positionOS.xyz);
				VertexNormalInputs norInput = GetVertexNormalInputs(IN.normalOS);

				OUT.uv = IN.uv;

				OUT.viewDirWS = GetCameraPositionWS() - posInput.positionWS;

				OUT.Color = IN.Color;

				OUT.normalWS = norInput.normalWS;

				OUT.positionCS = posInput.positionCS;

				float4 screenPos = ComputeScreenPos(OUT.positionCS);

                OUT.screenUV = screenPos.xy/screenPos.w;

				return OUT;
			}

			

			half4 frag(Varyings IN, half facing : VFACE) : SV_Target{
                
				

				float2 uv = IN.uv;

				float2 twistUvST = TRANSFORM_TEX(uv, _TwistTex) + half2(_TwistScrollingX, _TwistScrollingY) * _Time.g;

				half4 twistColor = SAMPLE_TEXTURE2D(_TwistTex, sampler_TwistTex, twistUvST);

				half twist = twistColor.r * max(_TwistScale, 0);


				float2 twistMaskUvST = TRANSFORM_TEX(uv, _TwistMaskTex);

				float4 maskTexColor = SAMPLE_TEXTURE2D(_TwistMaskTex, sampler_TwistMaskTex, twistMaskUvST);

				half mask = maskTexColor.r;
				
				
				float2 screenUv = IN.screenUV + twist * mask;

				float3 screenColor = SampleSceneColor(screenUv);

				float4 finishColor = float4(screenColor, 1);

				return finishColor;

            }
			ENDHLSL
        }
    }
	FallBack Off
}

