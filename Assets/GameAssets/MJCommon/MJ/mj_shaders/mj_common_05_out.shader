Shader "MJ/Model/MJCommon_05_out"
{
		Properties
	{
		_BaseMap("BaseMap", 2D) = "white" {}
		_Mask("MaskMap", 2D) = "white" {}
		_XTitles("X_Titles",  Range(0, 8)) = 0
		_YTitles("Y_Titles",  Range(0, 4)) = 0

	//	_Mask("_Mask", 2D) = "white" {}

		_Color ("Main Color", Color) = (0,0,0,1)
		_SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
 		_EnvColor("EnvColor", Color) = (0.5,0.5,0.5,1)

        [HDR]_EmissionColor("EmissionColor", Color) = (0,0,0,0)
        _EmissionInt("EmissionInt",Range(0, 1)) = 0

		_SpeDir ("SpeDir", Vector) = (0.5,0.5,0.5,1)
 		_LightDir("LightDir", Vector) = (0.5,0.5,0.5,1)
  		_Shininess ("Shininess", Range(0.01, 50)) = 0.078125
		_ShininessI("ShininessI", Range(0.01, 50)) = 0.078125
		_XRimDir("XRimDir", Vector) = (1.07,5.42,1.06,1)
		_YRimDir("YRimDir", Vector) = (4.16,6.14,2.74,1)
		_XRimShininess("XRimShininess", Range(0.01, 500)) = 0.078125
		_YRimShininess("YRimShininess", Range(0.01, 500)) = 0.078125
 
  		_AmbientColor ("Ambient Color", Color) = (0,0,0,1)
		_Ao("Ao",  Range(0,10)) = 0.3
		_SmoothEnv("SmoothEnv",  Range(0, 1)) = 0.3
        _SmoothSide("SmoothSide",  Range(0, 1)) = 0.3
        _FrontInt("FrontInt",  Range(1, 1.1)) = 0.3
		_SideInt("SideInt",  Range(1, 1.1)) = 0.3

        _DarkSide("DarkSide",  Range(0, 1)) = 0.3
     //   _DarkInt("DarkInt",  Range(0, 200)) = 0.3

	}

		SubShader
		{
			Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True"  }
			LOD 300
			Pass
			{
				Name "ForwardLit"
				Tags{"LightMode" = "UniversalForward"}

			//	Blend SrcAlpha OneMinusSrcAlpha

			//  Blend[_SrcBlend][_DstBlend]
				Cull off
				ZWrite ON
			// 	Offset -1,-1
				ZTest LEqual
				ColorMask RGBA
				HLSLPROGRAM

				#pragma prefer_hlslcc gles
				#pragma exclude_renderers d3d11_9x
				#pragma multi_compile_instancing
				#pragma vertex vert
				#pragma fragment frag

				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" 
				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl" 

				CBUFFER_START(UnityPerMaterial)
				float4 _BaseMap_ST,_RMAMask_ST;
 				half4  _Color,  _SpecColor, _EmissionColor;
				half4  _SpeDir,_LightDir, _XRimDir, _YRimDir, _AmbientColor, _EnvColor;
				half   _Shininess,_XRimShininess, _YRimShininess, _ShininessI;
		  
				half   _Transparency, _FrontInt, _SideInt;
	 
 				float  _Cutoff, _Ao, _SmoothEnv, _SmoothSide, _XTitles, _YTitles, _EmissionInt, _DarkSide, _DarkInt;

				CBUFFER_END
				TEXTURE2D(_BaseMap);            SAMPLER(sampler_BaseMap);
 
				TEXTURE2D(_Mask);				SAMPLER(sampler_Mask);
				 
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

					float3 posWS                    : TEXCOORD1;
					float4 normal                   : TEXCOORD2;
					float4 tangent                  : TEXCOORD3;
					float4 bitangent                : TEXCOORD4;
 					float4 positionCS               : SV_POSITION;
					DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 5);//	float4 lightmapUVOrVertexSH : TEXCOORD0;
					half4 fogFactorAndVertexLight   : TEXCOORD6;
					float4 shadowCoord              : TEXCOORD7;
  					float3 ambientColor_var			: TEXCOORD8;
 					float3 viewDirOS				: TEXCOORD9;
					float3 viewDirXRS				: TEXCOORD10;
					float3 viewDirYRS               : TEXCOORD11;
					float3 viewDirOE                : TEXCOORD12;
                    float3 normalROS                : TEXCOORD13;
 					UNITY_VERTEX_INPUT_INSTANCE_ID
					UNITY_VERTEX_OUTPUT_STEREO
				};

				Varyings vert(Attributes input)
				{
					Varyings output = (Varyings)0;

					UNITY_SETUP_INSTANCE_ID(input);
					UNITY_TRANSFER_INSTANCE_ID(input, output);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

					VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
					VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
 					output.posWS = TransformObjectToWorld(input.positionOS.xyz);
					output.positionCS = vertexInput.positionCS;

				 	half3 SpeDir  = normalize(TransformWorldToObject(_SpeDir.xyz ));
 
					half3 LightDir =  normalize(TransformWorldToObject(_LightDir.xyz));
 
					output.viewDirOS = normalize(SpeDir.xyz - input.positionOS.xyz);//正负相反光源
 
                    half3 viewDirWS = normalize(GetCameraPositionWS() - output.posWS);//正负相反光源


					output.viewDirXRS = normalize(_XRimDir.xyz * input.positionOS.xyz);
					output.viewDirYRS = normalize(_YRimDir.xyz * input.positionOS.xyz);

					half3 ambientColor = (_AmbientColor.xyz * _Color.xyz);

					half3 LightDir_var = rsqrt(dot(LightDir, LightDir)) * _Color.xyz;

 					output.ambientColor_var = saturate(ambientColor + LightDir_var * dot(normalInput.normalWS, _LightDir.xyz)) ;
					output.normalROS = input.normalOS; 

 					output.normal = half4(normalInput.normalWS, viewDirWS.x);
					output.tangent = half4(normalInput.tangentWS, viewDirWS.y);
					output.bitangent = half4(normalInput.bitangentWS, viewDirWS.z);

			 
					output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);

					OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);
					OUTPUT_SH(output.normal.xyz, output.vertexSH);

					half3 vertexLight = VertexLighting(vertexInput.positionWS, output.normal.xyz);
					half fogFactor = ComputeFogFactor(vertexInput.positionCS.z);

					output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
					output.shadowCoord = GetShadowCoord(vertexInput);
					return output;
				}

				real4 frag(Varyings input, half inface : VFACE) : SV_Target
				{
					UNITY_SETUP_INSTANCE_ID(input);
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

					float4 Inface = (((inface > 0) ? (1.0) : (0)));

					float2 uv = input.uv;
 
					half x_titles = 1.0/9.0;
					half y_titles = 1.0/6.0;
					half X_titles = floor(_XTitles);
					half Y_titles = floor(_YTitles);
		 
					if (Y_titles >= 4&& X_titles>=6)
					{
						Y_titles = 4;
						X_titles = 6;
					}

					uv.x += X_titles * x_titles;
					uv.y -= Y_titles * y_titles;
					


					real3 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv).rgb * _Color.xyz;

				//	real3 pbrMask = SAMPLE_TEXTURE2D(_RMAMask, sampler_RMAMask, uv).rgb;
 
					real mask = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, uv).r;
					
	 				real smooth_fir = _SmoothEnv;
                    real smooth_sec = _SmoothSide;

					InputData inputData;
					inputData.positionWS = input.posWS;
 					inputData.viewDirectionWS = SafeNormalize(half3(input.normal.w, input.tangent.w, input.bitangent.w));
					inputData.shadowCoord = input.shadowCoord;
					inputData.fogCoord = input.fogFactorAndVertexLight.x;
					inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
					inputData.bakedGI = SAMPLE_GI(input.lightmapUV, input.vertexSH, input.normal.xyz);

					Light mainLight = GetMainLight(inputData.shadowCoord);
					MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI, half4(0, 0, 0, 0));

   
  					float3 Vo =  input.viewDirOS.xyz;
                    float3 VoE = inputData.viewDirectionWS;
					float3 XRo = input.viewDirXRS * _FrontInt;
					float3 YRo = input.viewDirYRS * _SideInt;

					float3 NRos = input.normalROS;
					float3 NOS = normalize(TransformWorldToObject(input.normal.xyz));
   
					float  NosoVo_fir = pow(saturate(dot(NOS, Vo)), _Shininess);
					float  NosoVo_sec = pow(saturate(dot(input.normal.xyz, VoE)), _ShininessI);

  
					float NosoXRo = dot(NRos, XRo);
					float NosoYRo = dot(NRos, YRo);
					float NosoXRo_var = saturate(pow(saturate(NosoXRo), _XRimShininess));
					float NosoYRo_var = saturate(pow(saturate(NosoYRo), _YRimShininess));

					float FinRim = NosoXRo_var+ NosoYRo_var;
					float FinVo_var = lerp(0, NosoVo_sec, FinRim);
			//		float3 viewSpec1 = saturate((_SpecColor.xyz) * FinVo_var);

					float3 viewSpec = saturate(_SpecColor.xyz* NosoVo_fir);
                    viewSpec = saturate(viewSpec * albedo + viewSpec * 0.5);

					float3 reflectVector = reflect(-inputData.viewDirectionWS, input.normal.xyz);

					half3 indirectSpecular_Fir   = GlossyEnvironmentReflection(reflectVector, PerceptualSmoothnessToPerceptualRoughness(smooth_fir), _Ao);
					half3 indirectSpecular_Sec   = GlossyEnvironmentReflection(reflectVector, PerceptualSmoothnessToPerceptualRoughness(FinVo_var * smooth_sec), _Ao);
					half3 indirectSpecular_var = albedo.xyz * lerp(indirectSpecular_Fir, indirectSpecular_Sec * _EnvColor.xyz, FinRim);

				    indirectSpecular_var = lerp(albedo.xyz, indirectSpecular_var * _EnvColor.xyz, mask);

					half3 final = indirectSpecular_var* input.ambientColor_var + viewSpec;
				  	final = lerp(albedo.xyz, final,  mask);


                    half fresnelTerm = pow(1.0 - saturate(dot(input.normal.xyz, VoE.xyz)), _DarkSide);

                    final = lerp(final, final*_EmissionColor, _EmissionInt*pow(fresnelTerm, 10));
                    return real4(final, 1);
 					}
				ENDHLSL
				}

	}

}
