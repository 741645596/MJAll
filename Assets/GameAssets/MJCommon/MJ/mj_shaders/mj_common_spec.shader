Shader "MJ/Model/MJ_MJCommonSpec"
{
	Properties
	{
		_BaseMap("BaseMap", 2D) = "white" {}
		_XTitles("X_Titles",  Range(0, 8)) = 0
		_YTitles("Y_Titles",  Range(0, 3)) = 0
		[Space(30)]
		_SpecExp ("Exp", Range(0, 200)) = 50
		_SpecIntensity("SpecIntensity", Range(0, 2)) = 0.65
		_Offset("Offset", Vector) = (0, 0.4, 0.6, 1)	
		[Space(30)]
		_SpecExp2 ("Exp2", Range(0, 200)) = 50
		_SpecIntensity2("SpecIntensity2", Range(0, 2)) = 0.65
		_Offset2("Offset2", Vector) = (0, 0.4, 0.6, 1)	
		[Space(30)]
		_DiffuseIntensity("DiffuseIntensity", Range(0, 2)) = 0.65
		_Color ("Main Color", Color) = (1, 1, 1, 1)	
		[Space(30)]
		_FresnelColor("Fresnel Color", Color) = (1, 1, 1, 1)
		_FresnelPower("Fresnel Power", Float) = 15
		_FresnelStrenght("Fresnel Strenght", Float) = 0
	}

	SubShader
	{
		Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True"  }
		LOD 300
		Pass
		{
			Name "ForwardLit"
			Tags{"LightMode" = "UniversalForward"}

			//Cull off
			
			HLSLPROGRAM

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma multi_compile_instancing
			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" 
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl" 

			CBUFFER_START(UnityPerMaterial)
			float4 _BaseMap_ST;
			half3  _Color,  _SpecColor;
			float  _XTitles, _YTitles;
			float _DiffuseIntensity, _SpecExp, _SpecIntensity, _SpecExp2, _SpecIntensity2;
			float3 _Offset, _Offset2;
			float4 _FresnelColor;
			float _FresnelPower, _FresnelStrenght;
			CBUFFER_END

			TEXTURE2D(_BaseMap);            SAMPLER(sampler_BaseMap);
				
			struct Attributes
			{
				float4 positionOS				: POSITION;
				float3 normalOS					: NORMAL;
				float4 tangentOS				: TANGENT;
				float2 texcoord					: TEXCOORD0;
				float2 lightmapUV				: TEXCOORD1;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			struct Varyings
			{
				float2 uv                       : TEXCOORD0;
				float3 normal                   : TEXCOORD1;
				float3 offset1					: TEXCOORD2;
				float3 offset2					: TEXCOORD3;
				float3 viewDirWS				: TEXCOORD4;
				float4 positionCS               : SV_POSITION;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			Varyings vert(Attributes input)
			{
				Varyings output = (Varyings)0;

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);

				VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
				VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
				output.positionCS = vertexInput.positionCS;
				float3 posWS = TransformObjectToWorld(input.positionOS.xyz);
				output.viewDirWS = normalize(GetCameraPositionWS() - posWS);//正负相反光源

				output.normal = half3(normalInput.normalWS);
				output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
				output.offset1 = _Offset;
				output.offset2 = _Offset2;
				
				return output;
			}

			float4 frag(Varyings input, half inface : VFACE) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				float4 Inface = (((inface > 0) ? (1.0) : (0)));

				float2 uv = input.uv;
				uv.x += floor(_XTitles) / 9.0;
				uv.y -= floor(_YTitles) / 6.0;
				
				float3 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv).rgb * _DiffuseIntensity * _Color;

				float3 normalWS = SafeNormalize(input.normal.xyz);
				float3 viewDirWS = SafeNormalize(input.viewDirWS.xyz);

				float ndvFresnel = 1 - saturate(dot(normalWS, viewDirWS));
				float3 fresnel = _FresnelColor.rgb * pow(ndvFresnel, _FresnelPower) * _FresnelStrenght;
				
				float3 customSpecView = SafeNormalize(input.offset1.xyz);
				float3 customSpecView2 = SafeNormalize(input.offset2.xyz);

				float ndv = saturate(dot(normalWS, customSpecView));
				float spec = pow(ndv, _SpecExp) * _SpecIntensity;

				float ndv2 = saturate(dot(normalWS, customSpecView2));
				float spec2 = pow(ndv2, _SpecExp2) * _SpecIntensity2;

				return float4(albedo + spec + spec2 + fresnel, 1);
			}
			ENDHLSL
		}
	}
}
