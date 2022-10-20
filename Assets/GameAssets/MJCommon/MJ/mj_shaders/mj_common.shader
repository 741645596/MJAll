Shader "MJ/Model/MJ_MJCommon"
{
		Properties
	{
		_BaseMap("BaseMap", 2D) = "white" {}
		_RMAMask("RMAMask", 2D) = "white" {}
		_BumpMap("BumpMap", 2D) = "white" {}

		_Color ("Main Color", Color) = (0,0,0,1)
		_SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
		_LightDir ("LightDir", Vector) = (0.5,0.5,0.5,1)
		_LightDir1 ("LightDir1", Vector) = (0.5,0.5,0.5,1)
		_Shininess ("Shininess", Range(0.01, 50)) = 0.078125
		_Emission ("Emission", Range(0.01, 10)) = 0.3
		_Intensity ("_Intensity", Float) = 1
		_AmbientColor ("Ambient Color", Color) = (0,0,0,1)
		_Ao("_Ao",  Range(0,10)) = 0.3
		_Smooth("_Smooth",  Range(0, 1)) = 0.3
	}

		SubShader
		{
			Tags{"RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True" "Queue" = "Transparent"}
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
				half4  _LightDir, _LightDir1, _AmbientColor;
				half   _Shininess, _Emission, _Intensity;
		  
				half   _Transparency;
	 
 				float  _Cutoff, _Ao, _Smooth;

				CBUFFER_END
				TEXTURE2D(_BaseMap);            SAMPLER(sampler_BaseMap);
				TEXTURE2D(_RMAMask);            SAMPLER(sampler_RMAMask);
				TEXTURE2D(_BumpMap);		    SAMPLER(sampler_BumpMap);
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
					float3 rimVector                : TEXCOORD9;
					float  viewDirDS				: TEXCOORD10;
					float3 ambientColor_var			: TEXCOORD11;
					float3 viewDirWS				: TEXCOORD12;
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

				 	half3 lightDir_1  = normalize(TransformWorldToObject(_LightDir.xyz));

					half3 lightDir1_1 = normalize(TransformWorldToObject(_LightDir1.xyz));
			

					half3 lightDir2_1 = normalize(_LightDir.xyz - input.positionOS.xyz);//正负相反光源
					output.viewDirWS = normalize(GetCameraPositionWS() - output.posWS);//正负相反光源

					half3 ambientColor = (_AmbientColor.xyz * _Color.xyz);

					half3 lightDir = rsqrt(dot(lightDir1_1, lightDir1_1)) * _Color.xyz;

					output.ambientColor_var = saturate(ambientColor + lightDir * dot(normalize(input.normalOS), lightDir1_1));

					output.viewDirDS = 1/sqrt(dot(lightDir2_1, lightDir2_1));


				//	half3 viewDirWS = normalize(GetCameraPositionWS() - output.posWS);//正负相反光源
 
				//	output.viewpos = normalize(TransformWorldToView(_CustomView.xyz));
					 
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
					half3 color = _Color.xyz*0.5;

					real3 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv).rgb * color;

					real3 pbrMask = SAMPLE_TEXTURE2D(_RMAMask, sampler_RMAMask, uv).rgb;
	 				real smooth = (pbrMask.r) * _Smooth;
			//		real metallic = pbrMask.g * _Metallic;
					real Ao = pbrMask.b * _Ao; 

					half3 normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, uv));
					half3 normalWS_var = normalize(TransformTangentToWorld(normalTS,
						   half3x3(input.tangent.xyz, input.bitangent.xyz, input.normal.xyz)));
					normalWS_var = NormalizeNormalPerPixel(normalWS_var);
					 
					InputData inputData;
					inputData.positionWS = input.posWS;
					inputData.normalWS = normalWS_var;
					inputData.viewDirectionWS = SafeNormalize(half3(input.normal.w, input.tangent.w, input.bitangent.w));
					inputData.shadowCoord = input.shadowCoord;
					inputData.fogCoord = input.fogFactorAndVertexLight.x;
					inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
					inputData.bakedGI = SAMPLE_GI(input.lightmapUV, input.vertexSH, input.normal.xyz);

					Light mainLight = GetMainLight(inputData.shadowCoord);
					MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI, half4(0, 0, 0, 0));

					half3 lightDir_1 = normalize(TransformWorldToObject(_LightDir.xyz));

				//	half3 lightDir1_1 = normalize(TransformWorldToObject(_LightDir1.xyz));

					float3 Nos = normalize(TransformWorldToObject(normalWS_var));
					// 		float3 Vo = -(normalize(TransformWorldToObject(input.viewDirWS)));
					float3 Vo = -normalize(TransformWorldToObject(input.viewDirWS.xyz));

					float3 NosoVo = normalize(Vo - 2.0 * dot(Nos, Vo) * Nos);
					float NosoVooNos = pow(saturate(dot(NosoVo, Nos)), _Shininess); 
					float3 viewSpec = ((input.viewDirDS * _SpecColor.xyz) * NosoVooNos); 

				 	float3 reflectVector = reflect(-inputData.viewDirectionWS, input.normal.xyz);
					half3 indirectSpecular = GlossyEnvironmentReflection(reflectVector, PerceptualSmoothnessToPerceptualRoughness(smooth), Ao);
					 
					half3 indirectSpecular_var = indirectSpecular* albedo;
				 

				//	half3 ambientColor_var = saturate(_AmbientColor.xyz + lightDir * dot(normalWS_var, lightDir1_1));
					 
					float3 final = (albedo * input.ambientColor_var* _Intensity+ (viewSpec+ indirectSpecular_var))+ (albedo * _Emission);
					final = lerp(albedo, final, Ao);
					 
					return real4(final, 1);
					}
				ENDHLSL
				}

	}

}