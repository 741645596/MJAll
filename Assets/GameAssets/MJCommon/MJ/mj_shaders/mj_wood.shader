Shader "MJ/Model/MJ_Wood"
{
	Properties
	{
 		_BaseMap("BaseMap", 2D) = "white" {}
		[NoScaleOffset] _BumpMap("Normal Map", 2D) = "bump" {}
		[NoScaleOffset] _GMA("R(Gloss)M(Metallic)A", 2D) = "white" { }
		_Color("Color", Color) = (0,0,0,0)
		_Metallic("Metallic", Range(0, 3)) = 0
		_Gloss("Smooth", Range(0, 1)) = 0
		_Ao("Ao", Range(0,100)) = 0

		_LightDir("LightDir", Vector) = (0,1,1,1)
		_LightCol("LightCol",  Color) = (0,0,0,0)

 
		_IndirectDiffuseIntensity("IndirectDiffuseIntensity", Range(0,15)) = 0
		_DirectSpecularScale("DirectSpecularScale", Range(0,2)) = 0
		_MetallicThesold("MetallicThesold", Range(0,1)) = 0
		_CustomView("CustomView", Vector) = (0,1,1,1)	
		_RimIntensity("RimIntensity", Range(0, 1)) = 1
		_RimPower("RimPower", Range(0, 1)) = 1
		_RimColor("RimColor", Color) = (0,0,0,0)
		_Transparency("Transparency", Range(0, 1)) = 1
	}

	SubShader
	{
 		Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True" }
		LOD 300
		Pass
		{
			Name "ForwardLit"
			Tags{"LightMode" = "UniversalForward"}

			Blend SrcAlpha OneMinusSrcAlpha
 
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
			float4 _BaseMap_ST,_Color,_CustomView,_LightDir,_ViewDir,_RimColor,_LightCol;

  			half   _Gloss;
  			half   _Metallic;
			half   _Ao;
			half   _RimIntensity;
			half   _RimPower;
 			half   _Transparency;
			half   _Cutoff;
			half   _IndirectDiffuseIntensity, _DirectSpecularScale, _MetallicThesold;

 			CBUFFER_END
			TEXTURE2D(_BaseMap);            SAMPLER(sampler_BaseMap);
			TEXTURE2D(_BumpMap);		    SAMPLER(sampler_BumpMap);
 			TEXTURE2D(_GMA);			    SAMPLER(sampler_GMA);
  
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
 				DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 6);
 				half4 fogFactorAndVertexLight   : TEXCOORD7;
				float4 shadowCoord              : TEXCOORD8;
				float3 rimVector				: TEXCOORD9;
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
				output.positionCS = TransformWorldToHClip(output.posWS);
   
				output.rimVector = TransformWorldToView(_CustomView.xyz);

				half3 viewDirWS = GetCameraPositionWS() - output.posWS;
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

				float Inface = (((inface > 0) ? (1.0) : (0)));

				float2 uv = input.uv;

  				real3 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv).rgb;

				real3 pbrMask = SAMPLE_TEXTURE2D(_GMA, sampler_GMA, uv).rgb;
				real smooth   = ( 1 - pbrMask.r) *_Gloss;
				real metallic = ( pbrMask.g) *_Metallic;
				real Mask = pbrMask.b;

 				half3 normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, uv));
				half3 normalWS_var = normalize(TransformTangentToWorld(normalTS,
					half3x3(input.tangent.xyz, input.bitangent.xyz, input.normal.xyz)));
				normalWS_var = NormalizeNormalPerPixel(normalWS_var);
  
				real3 Emission = 0;
				real3 Specular = 0.5;
			//	real Occlusion = pbrMask.b;
				real Alpha = 1;
				 
 
				real alpha = saturate(_Transparency * SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv).a);

				clip(alpha* _Transparency * 4 - 0.2);
				 
				InputData inputData;
				inputData.positionWS = input.posWS;
				inputData.normalWS = normalWS_var;
				inputData.viewDirectionWS = SafeNormalize(half3(input.normal.w, input.tangent.w, input.bitangent.w));
				inputData.shadowCoord = input.shadowCoord;
				inputData.fogCoord = input.fogFactorAndVertexLight.x;
				inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
				inputData.bakedGI = SAMPLE_GI(input.lightmapUV, input.vertexSH, input.normal.xyz);

				Light mainLight = GetMainLight(inputData.shadowCoord);
 
				real3 albedo_var= albedo*_Color.rgb;
				real3 metallic_var = lerp(0.04, albedo_var, (1.0 - metallic));
				real smoothness = pow(1 - smooth,4);
				real roughness = (1 - smooth)* (1 - smooth);

				float3 L = normalize(_LightDir.xyz);
			//	float3 L = normalize(mainLight.direction.xyz);
				float3 V = inputData.viewDirectionWS;
				float3 H = normalize(L + V);

	

				float NoL = saturate(dot(L, normalWS_var));
				float NoL_c = saturate(NoL * 0.5 + 0.5);

				float NoH = saturate(dot(normalWS_var, H));

				float NoV = abs(dot(normalWS_var, V));
				float LoH = saturate(dot(L, H));

				float smoothSpec = (NoH * smoothness - NoH) * NoH + 1;
				float smoothSpec_var = (0.3183099 * smoothness) / (smoothSpec * smoothSpec);
				  
				float NoLv = NoL * (NoV * (1 - roughness) + roughness);
				float NoVl = NoV * (NoL * (1 - roughness) + roughness);
				float directSpec = ((0.5 / (NoLv + NoVl)) * smoothSpec_var);
				directSpec *= 3.141593;
				directSpec *= NoL;
				directSpec = lerp(directSpec, 0,  Mask);

//rim
				float3 R = input.rimVector.xyz;
 
				real NoR = saturate(dot(normalWS_var, R));

				float3 directLight = _LightCol.xyz* NoL_c;

				float  oLoH = pow(1 - LoH,5);

				float3 backLight = metallic_var+ oLoH * (1.0 - metallic_var);


				float3 finalColor = metallic_var * 0.88 * directLight + directSpec * backLight * _DirectSpecularScale;
					
	 			float oneMeta = metallic >= _MetallicThesold ? metallic : _MetallicThesold;

				finalColor+=  ((1 - oneMeta)* Inface); 
				float3 Rim = _RimColor.xyz * saturate(pow((1.0 - NoL) * NoR, _RimPower)) * _RimIntensity+ inputData.bakedGI * _IndirectDiffuseIntensity* Mask;
				finalColor += Rim;
				finalColor *= _LightCol.xyz;

				float3 final = lerp(finalColor, albedo, Mask);


    			return real4(final, alpha);
 			}
		ENDHLSL
		}

	}

}