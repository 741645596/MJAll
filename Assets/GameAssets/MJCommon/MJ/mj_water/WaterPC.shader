//https://zhuanlan.zhihu.com/p/423247159
Shader "URP/URPUnlitShader" {
    Properties {
        _MainTex("MainTex",2D)="black"{}
        [Normal]_NormalMap("normalMap法线扰动",2D) = "bump"{}
        _NormalStrength("NormalIntensity法线扰动强度",Range(0,5)) = 1
        _NormalScale("NormalScale法线缩放",Range(0,5)) = 1

        _WaveXSpeed("WaveXSpeed",Range(0,1)) = 0.5
        _WaveYSpeed("WaveYSpeed",Range(0,1)) = 0.5
        _NormalRefract("NormalRefract",Range(0,1)) = 0.5

        _DepthGradientShallow("Depth Gradient Shallow浅", Color) = (0.325, 0.807, 0.971, 0.725)
        _DepthGradientDeep("Depth Gradient Deep深", Color) = (0.086, 0.407, 1, 0.749)
        _DepthMaxDistance("Depth Maximum Distance距离", Float) = 1
        _Range ("Range", vector) = (0.13, 1.53, 0.37, 0.78)
        //边缘泡沫
        _WaveTex ("Gradient", 2D) = "white" {} //海水渐变  
        _WaveSpeed ("WaveSpeed", float) = -12.64 //海浪速度
        _WaveRange ("WaveRange", float) = 0.3
        _NoiseRange ("NoiseRange", float) = 6.43
        _WaveDelta ("WaveDelta", float) = 2.43

        _NoiseTex ("Noise海浪躁波", 2D) = "white" {} //海浪躁波 
        _SurfaceNoiseCutoff("Surface Noise Cutoff海浪躁波系数", Range(0, 1)) = 0.777
        _FoamDistance("Foam Distance泡沫", Float) = 0.4
        [NoScaleOffset]_CubeMap ("Cubemap", CUBE) = "white" {}
        _CubemapMip("CubemapMip",Range(0,7)) = 0
        _Amount("amount折射强度",float)=100
        _FresnelPower("Fresnel Power菲尼尔强度", Range(0.1,50)) = 5
    }

    SubShader {
        Tags {
            "RenderPipeline"="UniversalRenderPipeline"
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"
        #define REQUIRE_DEPTH_TEXTURE	//直接这样定义可以省去声明纹理的步骤(直接使用内部hlsl中的定义)

        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_ST;
        float3 _NormalMap_ST;

        float1 _NormalStrength;
        float1 _NormalScale;
        float1 _WaveYSpeed;
        float1 _WaveXSpeed;
        float1 _NormalRefract;

        half _WaveSpeed;
        half _WaveDelta;
        half _WaveRange;
        half _NoiseRange;
        float4 _DepthGradientShallow;
        float4 _DepthGradientDeep;
        float _DepthMaxDistance;
        float4 _Range;
        float4 _NoiseTex_ST;
        float _SurfaceNoiseCutoff;
        float _FoamDistance;
        float1 _CubemapMip;
        float _Amount;
        float4 _WaveTex_ST;
        float _FresnelPower;
        CBUFFER_END
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_NormalMap);
        SAMPLER(sampler_NormalMap);
        TEXTURE2D(_NoiseTex);
        SAMPLER(sampler_NoiseTex);
        //反射
        sampler2D _ReflectionTex;

        TEXTURECUBE(_CubeMap);
        SAMPLER(sampler_CubeMap);
        SAMPLER(_CameraColorTexture);
        float4 _CameraColorTexture_TexelSize; //该向量是非本shader独有，不能放在常量缓冲区

        TEXTURE2D(_WaveTex);
        SAMPLER(sampler_WaveTex);

        struct vertexInput
        {
            float4 positionOS:POSITION;
            float3 normalOS:NORMAL;
            float4 tangentOS:TANGENT;
            float2 texcoord:TEXCOORD;
        };

        struct vertexOutput
        {
            float4 positionCS:SV_POSITION;
            float3 positionWS : TRXCOORD1;
            float3 normalWS : TRXCOORD2;
            float4 tangentWS:TANGENT; //切线  
            float2 uv:TEXCOORD0;
            float4 screenPosition : TEXCOORD3;
            float2 noiseUV : TEXCOORD4;
        };
        ENDHLSL

        pass {
            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Pixel
            #include <HLSLSupport.cginc>

            vertexOutput Vertex(vertexInput v)
            {
                vertexOutput o;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(v.positionOS.xyz);
                //计算不同空间（视图空间、世界空间、齐次裁剪空间）中的位置
                VertexNormalInputs normalInputs = GetVertexNormalInputs(v.normalOS, v.tangentOS); //计算世界空间中的法线和切线

                o.positionCS = positionInputs.positionCS; //裁剪空间顶点位置
                o.positionWS = positionInputs.positionWS; //世界空间下顶点位置
                o.normalWS = normalInputs.normalWS;
                o.tangentWS = half4(normalInputs.tangentWS, v.tangentOS.w * GetOddNegativeScale()); //世界空间的切线

                o.screenPosition = ComputeScreenPos(o.positionCS);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.noiseUV = TRANSFORM_TEX(v.texcoord, _NoiseTex);

                return o;
            }

            half2 calcNormalUVOffset(vertexOutput i)
            {
                float2 float6 = float2(_WaveYSpeed * _Time.x, 0);
                float4 sample = SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, i.uv.yx+ float6);

                float2 float5 = float2(_WaveXSpeed * _Time.x, 0);
                //uv偏移
                float4 offsetColor = (SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, i.uv
                                                       + float5)
                    + sample) * 0.5;
                half2 offset = UnpackNormal(offsetColor).xy * _NormalRefract; //法线偏移程度可控之后offset被用于这里
                return offset;
            }

            half3 blendNormalTS(vertexOutput i, float2 offset)
            {
                //切线转世界
                half3 normalTS1 = UnpackNormal(
                    SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, i.uv*_NormalScale+offset)); //对法线纹理采样（切线）
                half3 normalTS2 = UnpackNormal(
                    SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, i.uv*_NormalScale-offset)); //对法线纹理采样（切线）
                half3 normalTS3 = normalize(normalTS1 + normalTS2);
                normalTS3.xy *= _NormalStrength;
                normalTS3.z = sqrt(1 - saturate(dot(normalTS3.xy, normalTS3.xy)));
                return normalTS3;
            }

            half4 sampleWave(float noiseR, float depthDifference, float depthRange, half wave_delta, float2 uv_offset)
            {
                float sin2 = sin(_Time.x * _WaveSpeed + wave_delta + noiseR * _NoiseRange);
                float2 UVWave2 = float2(1 - min(depthRange, depthDifference) / depthRange + _WaveRange * sin2, 1) +
                    uv_offset;

                half4 waveColor2 = SAMPLE_TEXTURE2D(_WaveTex, sampler_WaveTex, UVWave2);
                waveColor2.rgb *= (1 - (sin2 + 1) / 2) * noiseR;
                return waveColor2;
            }

            half3 sampleNoise(float2 noiseUV, float2 uv_offset, float depthDifference)
            {
                float surfaceNoiseSample1 = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, noiseUV+uv_offset).r;
                float surfaceNoiseSample2 = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, noiseUV-uv_offset).r;
                float surfaceNoiseSample = surfaceNoiseSample1 + surfaceNoiseSample2;

                float foamDepthDifference01 = saturate(depthDifference / _FoamDistance);
                float surfaceNoiseCutoff = foamDepthDifference01 * _SurfaceNoiseCutoff;
                float surfaceNoise = surfaceNoiseSample > surfaceNoiseCutoff * 2 ? 1 : 0;

                float depthRange = _Range.z;
                float noiseR = surfaceNoiseSample.r;

                half4 haianxian = sampleWave(noiseR, depthDifference, depthRange, 0, uv_offset);
                half4 waveColor2 = sampleWave(noiseR, depthDifference, depthRange, _WaveDelta, uv_offset);

                half water_A = 1 - min(depthRange, depthDifference) / depthRange;
                half3 surfaceNoise1 = (haianxian.rgb + waveColor2.rgb) * water_A + surfaceNoise;
                return surfaceNoise1;
            }

            half3 sampleRefract(vertexOutput i, half3 normalTS)
            {
                return float3(0, 0, 0);
                float2 SS_texcoord = i.positionCS.xy / _ScreenParams.xy; //获取屏幕UV
                float2 SS_bias = normalTS.xy * _Amount * GET_TEXELSIZE_NAME(_CameraColorTexture);
                //如果取的是切线空间的法线则执行它计算偏移，但是切线空间的法线不随着模型的旋转而变换；

                half3 refract = tex2D(_CameraColorTexture, SS_texcoord + SS_bias).rgb;
                return refract;
            }

            half4 Pixel(vertexOutput i):SV_TARGET
            {
                float3 worldPos = i.positionWS;
                Light mylight = GetMainLight();
                float3 lightDir = normalize(TransformObjectToWorldDir(mylight.direction));
                float3 viewDirectionWS = normalize(_WorldSpaceCameraPos.xyz - worldPos.xyz);


                float3 hdir = normalize(lightDir + viewDirectionWS); //光方向+视方向

                float2 uv_offset = calcNormalUVOffset(i);
                half3 normalTS = blendNormalTS(i, uv_offset);

                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv+uv_offset/50);

                //世界空间下的副切线
                half3 binormalWS = cross(i.normalWS, normalTS.xyz) * i.tangentWS.w;
                //将切线空间中的法线转换到世界空间中
                float3 normalWS = normalize(mul(normalTS, half3x3(i.tangentWS.xyz, binormalWS, i.normalWS)));

                //深度图
                float2 screenPos = i.screenPosition.xy / i.screenPosition.w;
                float depth = LinearEyeDepth(SampleSceneDepth(screenPos), _ZBufferParams);
                float depthDifference = depth - i.screenPosition.w;
                float waterDepthDifference01 = saturate(depthDifference / _DepthMaxDistance);

                float4 waterColor = lerp(_DepthGradientShallow, _DepthGradientDeep, waterDepthDifference01);
                //return waterColor;

                half3 surfaceNoise1 = sampleNoise(i.noiseUV, uv_offset, depthDifference); //海浪泡沫                
                //return half4(surfaceNoise1,1);

                float3 reflectVector = reflect(-viewDirectionWS, normalWS); // 反射
                // 采样Cubemap
                half3 SampleCubeMap = SAMPLE_TEXTURECUBE_LOD(_CubeMap, sampler_CubeMap, reflectVector, _CubemapMip).rgb;

                half3 refract = sampleRefract(i, normalTS); //折射                

                half fresnel = pow((1 - (dot(normalWS, viewDirectionWS))), _FresnelPower); //菲尼尔

                half3 ref = lerp(refract, SampleCubeMap, fresnel) + pow(fresnel, 100);

                half3 fcolor = (
                    waterColor +
                    surfaceNoise1 +
                    ref);
                fcolor += tex;

                float Alpha = min(_Range.w, depth) / _Range.w; //透明度
                Alpha = waterColor.a;

                return half4(fcolor, Alpha);
            }
            ENDHLSL
        }
    }
}