
Shader "FX/UVOffset_Mask" {
    Properties {
		
        [Enum(AlphaBlend or Additive, 5, Multiply, 0)] _SrcBlend ("Blend Mode Src", Float) = 5

        [Enum(AlphaBlend, 10,Additive, 1, Multiply, 3)] _DstBlend ("Blend Mode Dst", Float) = 10

        [Enum(UnityEngine.Rendering.CullMode)] _CullMode ("Cull Mode", Float) = 2

        [Enum(Off, 0, On, 1)] _ZWrite ("ZWrite", Float) = 0

        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Float) = 4

        [HDR]_Color ("Color", Color) = (1, 1, 1, 1)

		_MainTex("MainTex", 2D) = "white" {}

		_MainTexSpeed_U("MainTex Speed U", Float) = 0

		_MainTexSpeed_V("MainTex Speed V", Float) = 0

		_MainTexRotate("MainTex Rotate", Float) = 0
		
		[Space(30)]

		_MaskTex("MaskTex", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
			"PreviewType" = "Plane"  "RenderPipeline" = "UniversalPipeline"
        }

		Blend[_SrcBlend][_DstBlend]
		Cull[_CullMode]
		
		ZWrite[_ZWrite]
		ZTest[_ZTest]

		//ZTest LEqual
        //ZWrite Off

        Pass {
            
			Tags{"LightMode" = "UniversalForward"}

			HLSLPROGRAM

			//Pragmas 
			#pragma vertex vert
			#pragma fragment frag 

			//Include
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
           
			TEXTURE2D(_MainTex);	
			SAMPLER(sampler_MainTex);
			
			TEXTURE2D(_MaskTex);	
			SAMPLER(sampler_MaskTex);

			CBUFFER_START(UnityPerMaterial)

			uniform float4 _MainTex_ST;
			uniform float4 _MaskTex_ST;
			uniform float4 _Color;

            uniform float _MainTexSpeed_U;
            uniform float _MainTexSpeed_V;
			uniform float _MainTexRotate;
			uniform float _MainTwistScale;

			CBUFFER_END

		
			struct Attributes
			{
				float4 positionOS		:		POSITION;
				float4 Color	 		: 		COLOR;
				float2 uv				:		TEXCOORD0;
			};

			struct Varyings
			{
				float4 positionCS		:		SV_POSITION;
				float4 Color			:		COLOR;
				float2 uv           	:       TEXCOORD0;
				float3 viewDirWS		:		TEXCOORD1;
			};

			Varyings vert(Attributes IN)
			{
				Varyings OUT;

				VertexPositionInputs posInput = GetVertexPositionInputs(IN.positionOS.xyz);

				OUT.uv = IN.uv;

				OUT.viewDirWS = GetCameraPositionWS() - posInput.positionWS;

				OUT.Color = IN.Color;

				OUT.positionCS = posInput.positionCS;

				return OUT;
			}

			void Unity_Rotate_Degrees_float(float2 UV, float2 Center, float Rotation, out float2 Out)
			{
				Rotation = Rotation * (3.1415926f / 180.0f);
				UV -= Center;
				float s = sin(Rotation);
				float c = cos(Rotation);
				float2x2 rMatrix = float2x2(c, -s, s, c);
				rMatrix *= 0.5;
				rMatrix += 0.5;
				rMatrix = rMatrix * 2 - 1;
				UV.xy = mul(UV.xy, rMatrix);
				UV += Center;
				Out = UV;
			}

			half4 frag(Varyings IN, half facing : VFACE) : SV_Target{
                
				float2 uv = IN.uv;

				float2 uvMainLoop = TRANSFORM_TEX(uv, _MainTex) + float2(_MainTexSpeed_U, _MainTexSpeed_V) * _Time.g;
				Unity_Rotate_Degrees_float(uvMainLoop, float2(0.5, 0.5), _MainTexRotate, uvMainLoop);

				float4 mainTexColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvMainLoop) * _Color * IN.Color;

				float2 uvMask = TRANSFORM_TEX(uv, _MaskTex);

				float4 maskTexColor = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, uvMask);

				half mask = maskTexColor.r;

				half alpha = mainTexColor.a * mask;

				float4 finishColor = float4(mainTexColor.rgb, alpha);

				half3 color = mainTexColor.rgb;

				return float4(color, alpha);

            }
			ENDHLSL
        }
    }
	FallBack Off

    //CustomEditor "ShaderForgeMaterialInspector"
}


/*
public enum BlendMode
{
	//
	// ժҪ:
	//     Blend factor is (0, 0, 0, 0).
	Zero = 0,
	//
	// ժҪ:
	//     Blend factor is (1, 1, 1, 1).
	One = 1,
	//
	// ժҪ:
	//     Blend factor is (Rd, Gd, Bd, Ad).
	DstColor = 2,
	//
	// ժҪ:
	//     Blend factor is (Rs, Gs, Bs, As).
	SrcColor = 3,
	//
	// ժҪ:
	//     Blend factor is (1 - Rd, 1 - Gd, 1 - Bd, 1 - Ad).
	OneMinusDstColor = 4,
	//
	// ժҪ:
	//     Blend factor is (As, As, As, As).
	SrcAlpha = 5,
	//
	// ժҪ:
	//     Blend factor is (1 - Rs, 1 - Gs, 1 - Bs, 1 - As).
	OneMinusSrcColor = 6,
	//
	// ժҪ:
	//     Blend factor is (Ad, Ad, Ad, Ad).
	DstAlpha = 7,
	//
	// ժҪ:
	//     Blend factor is (1 - Ad, 1 - Ad, 1 - Ad, 1 - Ad).
	OneMinusDstAlpha = 8,
	//
	// ժҪ:
	//     Blend factor is (f, f, f, 1); where f = min(As, 1 - Ad).
	SrcAlphaSaturate = 9,
	//
	// ժҪ:
	//     Blend factor is (1 - As, 1 - As, 1 - As, 1 - As).
	OneMinusSrcAlpha = 10
}


//     Backface culling mode.
public enum CullMode
{
	//
	// ժҪ:
	//     Disable culling.
	Off = 0,
	//
	// ժҪ:
	//     Cull front-facing geometry.
	Front = 1,
	//
	// ժҪ:
	//     Cull back-facing geometry.
	Back = 2
}


//     Depth or stencil comparison function.
public enum CompareFunction
{
	//
	// ժҪ:
	//     Depth or stencil test is disabled.
	Disabled = 0,
	//
	// ժҪ:
	//     Never pass depth or stencil test.
	Never = 1,
	//
	// ժҪ:
	//     Pass depth or stencil test when new value is less than old one.
	Less = 2,
	//
	// ժҪ:
	//     Pass depth or stencil test when values are equal.
	Equal = 3,
	//
	// ժҪ:
	//     Pass depth or stencil test when new value is less or equal than old one.
	LessEqual = 4,
	//
	// ժҪ:
	//     Pass depth or stencil test when new value is greater than old one.
	Greater = 5,
	//
	// ժҪ:
	//     Pass depth or stencil test when values are different.
	NotEqual = 6,
	//
	// ժҪ:
	//     Pass depth or stencil test when new value is greater or equal than old one.
	GreaterEqual = 7,
	//
	// ժҪ:
	//     Always pass depth or stencil test.
	Always = 8
}

*/