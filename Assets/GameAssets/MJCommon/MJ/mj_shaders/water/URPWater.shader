Shader "Unlit/Water" {
    Properties {
        _Direction1("Direction1", Vector) = (1, 0, 0, 0)
        _Direction2("Direction2", Vector) = (-1, -0.5, 0, 0)
        _WaveSpeed("WaveSpeed", Float) = 0.17
        _WaveScale("WaveScale", Range(0.05, 1)) = 0.05
        [NoScaleOffset]_NormalTex("NormalTex", 2D) = "bump" {}
        _NormalStrength("NormalStrength", Range(0, 1)) = 0.5
        _Distort("Distort", Float) = 0.06
        _Color("ShallowColor", Color) = (0, 1, 0.7149546, 0.4666667)
        [HideInInspector] _DeepColor("DeepColor", Color) = (0, 0, 0, 0)
        _LerpColorB("LerpColorB", Range(0.01, 0.5)) = 0.1
        _Smoothness("Smoothness", Range(0, 1)) = 0.5
        _Strength("Strength", Float) = 0.5
        _Depth("Depth", Float) = 0.5
        [HideInInspector] [NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
        [HideInInspector] [NoScaleOffset]_WaveMaskTex("WaveMaskTex", 2D) = "white" {}

        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader {
        Tags {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType"="Lit"
            "Queue"="Transparent"
        }
        Pass {
            Name "Universal Forward"
            Tags {
                "LightMode" = "UniversalForward"
            }

            Cull Back

            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha

            ZTest LEqual
            ZWrite Off

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            #pragma multi_compile_fog// make fog work
            #pragma multi_compile_instancing
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS

            CBUFFER_START(UnityPerMaterial)
            float2 _Direction1;
            float2 _Direction2;
            float _WaveSpeed;
            float _WaveScale;
            float4 _NormalTex_TexelSize;
            float _NormalStrength;
            float _Distort;
            float4 _Color;
            float4 _DeepColor;
            float _LerpColorB;
            float _Smoothness;
            float _Strength;
            float _Depth;
            float4 _MainTex_TexelSize;
            float4 _WaveMaskTex_TexelSize;
            CBUFFER_END

            TEXTURE2D(_NormalTex);
            SAMPLER(sampler_NormalTex);
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_WaveMaskTex);
            SAMPLER(sampler_WaveMaskTex);

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;

                // to support GPU instancing and Single Pass Stereo rendering(VR), add the following section
                //------------------------------------------------------------------------------------------------------------------------------
                UNITY_VERTEX_INPUT_INSTANCE_ID
                // in non OpenGL / non PSSL, will turn into -> uint instanceID : SV_InstanceID;
                //------------------------------------------------------------------------------------------------------------------------------
                // #if UNITY_ANY_INSTANCING_ENABLED
                // uint instanceID : INSTANCEID_SEMANTIC;
                // #endif
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS: TEXCOORD0;

                float3 normalWS: TEXCOORD1;
                float4 tangentWS: TEXCOORD2; // xyz: tangent, w: viewDir.y
                float3 bitangentWS : TEXCOORD5;

                float2 texCoord0: TEXCOORD3;

                float3 viewDirectionWS: TEXCOORD4;

                DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 6);
                float4 fogFactorAndVertexLight: TEXCOORD7;
                float4 shadowCoord: TEXCOORD8;


                // to support GPU instancing and Single Pass Stereo rendering(VR), add the following section
                //------------------------------------------------------------------------------------------------------------------------------
                UNITY_VERTEX_INPUT_INSTANCE_ID
                // will turn into this in non OpenGL / non PSSL -> uint instanceID : SV_InstanceID;
                UNITY_VERTEX_OUTPUT_STEREO
                // will turn into this in non OpenGL / non PSSL -> uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                // #if UNITY_ANY_INSTANCING_ENABLED
                // uint instanceID : CUSTOM_INSTANCE_ID;
                // #endif
                // #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                // FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                // #endif
            };

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

            inline void InitializeStandardLitSurfaceData(float2 uv,
                                                         half3 normalTS,
                                                         out SurfaceData outSurfaceData)
            {
                float4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                outSurfaceData.alpha = 1;
                outSurfaceData.albedo = albedo.rgb;

                outSurfaceData.metallic = 0;
                outSurfaceData.specular = half3(0.0h, 0.0h, 0.0h);

                outSurfaceData.smoothness = _Smoothness;
                outSurfaceData.normalTS = normalTS;
                //SampleNormal(uv, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap), _BumpScale);
                outSurfaceData.occlusion = 1; //SampleOcclusion(uv);
                outSurfaceData.emission = 0;

                outSurfaceData.clearCoatMask = 0.0h;
                outSurfaceData.clearCoatSmoothness = 1;
            }


            Varyings vert(Attributes input)
            {
                Varyings vert_out = (Varyings)0;
                // to support GPU instancing and Single Pass Stereo rendering(VR), add the following section
                UNITY_SETUP_INSTANCE_ID(input);
                // will turn into this in non OpenGL / non PSSL -> UnitySetupInstanceID(input.instanceID);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                // will turn into this in non OpenGL / non PSSL -> output.instanceID = input.instanceID;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                // will turn into this in non OpenGL / non PSSL -> output.stereoTargetEyeIndexAsRTArrayIdx = unity_StereoEyeIndex;

                float3 positionOS = input.positionOS.xyz;

                //VertexPositionInputs vertexInput = GetVertexPositionInputs(positionOS);
                VertexPositionInputs vertexInput;

                vertexInput.positionWS = TransformObjectToWorld(positionOS);
                vertexInput.positionVS = TransformWorldToView(vertexInput.positionWS);
                vertexInput.positionCS = TransformWorldToHClip(vertexInput.positionWS);

                float4 ndc = vertexInput.positionCS * 0.5f;
                vertexInput.positionNDC.xy = float2(ndc.x, ndc.y * _ProjectionParams.x) + ndc.w;
                vertexInput.positionNDC.zw = vertexInput.positionCS.zw;


                // normalWS and tangentWS already normalize.
                // this is required to avoid skewing the direction during interpolation
                // also required for per-vertex lighting and SH evaluation
                //
                // VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                vert_out.texCoord0 = input.uv0;

                // already normalized from normal transform to WS.

                VertexNormalInputs tbn;
                tbn = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                vert_out.normalWS = tbn.normalWS;
                vert_out.bitangentWS = tbn.bitangentWS;
                vert_out.tangentWS.xyz = tbn.tangentWS;

                // vert_out.normalWS.xyz = TransformObjectToWorldNormal(input.normalOS);
                // float crossSign = (vert_out.normalWS.w > 0.0 ? 1.0 : -1.0) * GetOddNegativeScale();
                // float3 bitangent = crossSign * cross(vert_out.normalWS.xyz, vert_out.normalWS.xyz);
                // vert_out.bitangent.xyz = bitangent;
                // vert_out.tangentWS.xyz = TransformObjectToWorldDir(input.tangentOS.xyz);

                vert_out.tangentWS.w = input.tangentOS.w;
                half3 viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
                vert_out.viewDirectionWS = viewDirWS;

                #if defined(REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR) || defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
    real sign = input.tangentOS.w * GetOddNegativeScale();
    half4 tangentWS = half4(normalInput.tangentWS.xyz, sign);
                #endif
                #if defined(REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR)
    output.tangentWS = tangentWS;
                #endif

                #if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
    half3 viewDirTS = GetViewDirectionTangentSpace(tangentWS, output.normalWS, viewDirWS);
    output.viewDirTS = viewDirTS;
                #endif

                OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);
                OUTPUT_SH(vert_out.normalWS.xyz, vert_out.vertexSH);

                half3 vertexLight = VertexLighting(vertexInput.positionWS, vert_out.normalWS);
                half fogFactor = ComputeFogFactor(vertexInput.positionCS.z);

                vert_out.fogFactorAndVertexLight = half4(fogFactor, vertexLight);

                #if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
                vert_out.positionWS = vertexInput.positionWS;
                #endif

                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                output.shadowCoord = GetShadowCoord(vertexInput);
                #endif

                vert_out.positionCS = vertexInput.positionCS;
                return vert_out;
            }

            void Unity_NormalBlend_float(float3 A, float3 B, out float3 Out)
            {
                Out = SafeNormalize(float3(A.rg + B.rg, A.b * B.b));
            }

            void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
            {
                Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
            }

            void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
            {
                Out = UV * Tiling + Offset;
            }

            void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
            {
                Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
            }

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float offset = _TimeParameters.x * _WaveSpeed;
                float4 dir_offset = float4(_Direction1.x,
                                           _Direction1.y,
                                           _Direction2.x,
                                           _Direction2.y) * offset
                    + input.positionWS.xzxz;

                dir_offset = dir_offset * _WaveScale;
                float3 nrm1 = UnpackNormal(SAMPLE_TEXTURE2D(_NormalTex, sampler_NormalTex, dir_offset.xy));
                float3 nrm2 = UnpackNormal(SAMPLE_TEXTURE2D(_NormalTex, sampler_NormalTex, dir_offset.zw));

                float3 NormalBlended;
                Unity_NormalBlend_float(nrm1, nrm2, NormalBlended);


                float3 _normalStrengthOut;
                Unity_NormalStrength_float(NormalBlended, _NormalStrength, _normalStrengthOut);
                float3 ofs = _normalStrengthOut * _Distort;


                float4 ScreenPosition = ComputeScreenPos(
                    TransformWorldToHClip(input.positionWS)
                    //,_ProjectionParams.x
                );
                float4 _ScreenPos = float4(ScreenPosition.xy / ScreenPosition.w, 0, 0);
                float2 _TilingAndOffset;
                Unity_TilingAndOffset_float((_ScreenPos.xy), float2(1, 1),
                                            (ofs.xy), _TilingAndOffset);

                float3 _SceneColor = SampleSceneColor(_TilingAndOffset.xy);
                //SHADERGRAPH_SAMPLE_SCENE_COLOR(_TilingAndOffset.xy);
                float4 _ShallowColor = _Color;
                float3 sceneAddShallowColor;
                sceneAddShallowColor = _SceneColor + _ShallowColor.xyz;

                float _FresnelEffect;
                Unity_FresnelEffect_float(input.normalWS, input.viewDirectionWS, 1, _FresnelEffect);
                float3 _baseColor = lerp(sceneAddShallowColor, (_LerpColorB.xxx), (_FresnelEffect.xxx));

                SurfaceData surfaceData;
                InitializeStandardLitSurfaceData(input.texCoord0, NormalBlended, surfaceData);
                surfaceData.albedo = _baseColor;
                //surfaceData.alpha = _Color.a;

                InputData inputData;
                inputData = (InputData)0;
                {
                    #if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
                    inputData.positionWS = input.positionWS;
                    #endif

                    //half3 viewDirWS = SafeNormalize(input.viewDirectionWS);
                    // inputData.normalWS = normalTS;
                    inputData.normalWS = TransformTangentToWorld(NormalBlended,
                                                                 half3x3(
                                                                     input.tangentWS.xyz,
                                                                     input.bitangentWS.xyz,
                                                                     input.normalWS.xyz
                                                                 )
                    );
                    inputData.viewDirectionWS = input.viewDirectionWS;
                    //inputData.viewDirectionWS = viewDirWS;

                    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
            inputData.shadowCoord = input.shadowCoord;
                    #elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
            inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
                    #else
                    inputData.shadowCoord = float4(0, 0, 0, 0);
                    #endif

                    inputData.fogCoord = input.fogFactorAndVertexLight.x;
                    inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;

                    inputData.bakedGI = SAMPLE_GI(input.lightmapUV, input.vertexSH, inputData.normalWS);
                    inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
                    inputData.shadowMask = SAMPLE_SHADOWMASK(input.lightmapUV);
                }

                half4 color = UniversalFragmentPBR(inputData, surfaceData);
                color.rgb = MixFog(color.rgb, inputData.fogCoord);
                color.a = _Color.a;
                return color;
            }

            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL
        }
    }
}
