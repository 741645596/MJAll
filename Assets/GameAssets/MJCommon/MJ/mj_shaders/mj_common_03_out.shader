Shader "MJ/Model/MJ_MJCommon_03_out"
{
		Properties
	{
		_BaseMap("BaseMap", 2D) = "white" {}
		_Mask("BaseMap", 2D) = "white" {}
		_XTitles("X_Titles",  Range(0, 8)) = 0
		_YTitles("Y_Titles",  Range(0, 4)) = 0

	//	_Mask("_Mask", 2D) = "white" {}

		_Color ("Main Color", Color) = (0,0,0,1)
		_SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
		_EnvColor("EnvColor", Color) = (0.5,0.5,0.5,1)

		_LightDir ("SpeDir", Vector) = (0.5,0.5,0.5,1)
		_LightDir1("LightDir", Vector) = (0.5,0.5,0.5,1)
  		_Shininess ("Shininess", Range(0.01, 50)) = 0.078125
  		_AmbientColor ("Ambient Color", Color) = (0,0,0,1)
		_Ao("Ao",  Range(0,10)) = 0.3
		_Smooth("Smooth",  Range(0, 1)) = 0.3
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
 				half4  _Color,  _SpecColor;
				half4  _LightDir, _LightDir1, _AmbientColor, _EnvColor;
				half   _Shininess ;
		  
				half   _Transparency;
	 
 				float  _Cutoff, _Ao, _Smooth, _XTitles, _YTitles;

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
					float3 viewDir                  : TEXCOORD5;
					float4 positionCS               : SV_POSITION;
					DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 6);//	float4 lightmapUVOrVertexSH : TEXCOORD0;
					half4 fogFactorAndVertexLight   : TEXCOORD7;
					float4 shadowCoord              : TEXCOORD8;
  					float3 ambientColor_var			: TEXCOORD9;
					float3 viewDirWS				: TEXCOORD10;
					float3 lightDir2_1				: TEXCOORD11;
  
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

				 	half3 lightDir_1  = normalize(TransformWorldToObject(_LightDir.xyz ));

					half3 lightDir1_1 =  normalize(TransformWorldToObject(_LightDir1.xyz));
 
					output.lightDir2_1 = normalize(lightDir_1.xyz - input.positionOS.xyz);//正负相反光源
 
					output.viewDirWS = normalize(GetCameraPositionWS() - output.posWS);//正负相反光源

					half3 ambientColor = (_AmbientColor.xyz * _Color.xyz);

					half3 lightDir = rsqrt(dot(lightDir1_1, lightDir1_1)) * _Color.xyz;

 					output.ambientColor_var = saturate(ambientColor + lightDir * dot(normalInput.normalWS, _LightDir1.xyz)) ;

 

 					output.normal = half4(normalInput.normalWS, output.viewDirWS.x);
					output.tangent = half4(normalInput.tangentWS, output.viewDirWS.y);
					output.bitangent = half4(normalInput.bitangentWS, output.viewDirWS.z);

			 
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
					
	 				real smooth =  _Smooth;
 				//	real Ao = mask *_Ao;

				//	half3 normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, uv));
				//	half3 normalWS_var = normalize(TransformTangentToWorld(normalTS,
				//		   half3x3(input.tangent.xyz, input.bitangent.xyz, input.normal.xyz)));
				//	normalWS_var = NormalizeNormalPerPixel(normalWS_var);
					 
					InputData inputData;
					inputData.positionWS = input.posWS;
				//	inputData.normalWS = normalWS_var;
					inputData.viewDirectionWS = SafeNormalize(half3(input.normal.w, input.tangent.w, input.bitangent.w));
					inputData.shadowCoord = input.shadowCoord;
					inputData.fogCoord = input.fogFactorAndVertexLight.x;
					inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
					inputData.bakedGI = SAMPLE_GI(input.lightmapUV, input.vertexSH, input.normal.xyz);

					Light mainLight = GetMainLight(inputData.shadowCoord);
					MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI, half4(0, 0, 0, 0));

					half3 lightDir_1 = normalize(TransformWorldToObject(_LightDir.xyz));
  
				//	float3 Nos = normalize(TransformWorldToObject(normalWS_var));
 					float3 Vo =  input.lightDir2_1.xyz ;
  
					float3 NOS = normalize(TransformWorldToObject(input.normal.xyz));
   
					float  NosoVo = pow(saturate(dot(NOS, Vo)), _Shininess);
					float3 viewSpec = (_SpecColor * NosoVo);
  
					float3 reflectVector = reflect(-inputData.viewDirectionWS, input.normal.xyz);

					half3 indirectSpecular    = GlossyEnvironmentReflection(reflectVector, PerceptualSmoothnessToPerceptualRoughness(smooth), _Ao);

                    half3 indirectSpecular_var = albedo * indirectSpecular;

                    //	indirectSpecular_var = lerp(albedo, indirectSpecular_var * _EnvColor, mask);

 
					half3 final = indirectSpecular_var* input.ambientColor_var + viewSpec;
				 	final = lerp(albedo, final,  mask);
					return real4(final, 1);
					}
				ENDHLSL
				}

	}

}
