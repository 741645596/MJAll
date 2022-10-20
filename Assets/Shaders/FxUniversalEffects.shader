// FxUniversalEffects.shader
// Author: shihongyang shihongyang@weile.com
// Data: 2021/11/11

Shader "FX/Universal Effects"
{
    Properties
    {
        // 主纹理
        _MainTex("主贴图", 2D) = "white" {}           // 0
		[HDR] _Color("主颜色", Color) = (1,1,1,1)

        // 扰动效果
        [NoScaleOffset]_DistortTex("扰动贴图", 2D) = "white" {} // 2
		_DistortAmount("扰动强度", Range(0,2)) = 0.5
		_DistortTexXSpeed("X轴速度", Range(-5,5)) = 1
		_DistortTexYSpeed("Y轴速度", Range(-5,5)) = 1

        // 溶解效果
		[NoScaleOffset]_DissolveTex("溶解贴图", 2D) = "white" {} // 6
		_DissolveAmount("溶解比例",  Range(0, 1)) = 0
		_DissolveEdge("边缘虚化",  Range(0, 100)) = 10

        // 模糊
        _BlurIntensity("模糊强度",  Range(0, 20)) = 10 // 9

        // 外发光
		_OutlineColor("发光颜色", Color) = (1,1,1,1) //10
        [Space]
		_OutlineGlow("外发光强度", Range(0, 100)) = 1.5 
		_OutlineAlpha("外发光透明度",  Range(0, 1)) = 1 
		_OutlineWidth("外发光宽度", Range(0, 0.2)) = 0.004
        [Space]
		_InnerOutlineGlow("内发光强度",  Range(0,100)) = 4
		_InnerOutlineAlpha("内发光透明度",  Range(0,1)) = 1
		_InnerOutlineThickness("内发光宽度",  Range(0,3)) = 1

        // 滚动
		_TextureScrollXSpeed("X轴速度", Range(-5, 5)) = 0 // 17
		_TextureScrollYSpeed("Y轴速度", Range(-5, 5)) = 0

		_FresnelColor("菲涅尔颜色", Color) = (1, 1, 1, 1) // 19
		_FresnelPower("菲涅尔指数", Float) = 30
		_FresnelStrenght("菲涅尔强度", Float) = 0

		_ZTestMode ("Z Test Mode", Float) = 4
		_ZWrite ("Depth Write", Float) = 0.0
        _MySrcMode ("SrcMode", Float) = 5
		_MyDstMode ("DstMode", Float) = 10
        _CullMode ("Cull Mode", Float) = 2
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
        }

		Blend [_MySrcMode] [_MyDstMode]
        Cull[_CullMode]
		ZWrite [_ZWrite]
		ZTest [_ZTestMode]

        Pass
        {
            HLSLPROGRAM

			#pragma multi_compile __ TEXTURESCROLL_ON
			#pragma multi_compile __ DISTORT_ON
			#pragma multi_compile __ DISSOLVE_ON
			#pragma multi_compile __ BLUR_ON
			#pragma multi_compile __ OUTLINE_ON

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
				half4 color : COLOR;
            };

            struct Varyings
            {
                float4 vertex : SV_POSITION;
                float2 uv: TEXCOORD0;
                float4 color : COLOR;

                #if DISTORT_ON
				half2 uvDistTex : TEXCOORD1;
				#endif

				float3 viewDirWS : TEXCOORD2;
				half3  normalWS	: TEXCOORD3;
            };

            TEXTURE2D(_MainTex);    SAMPLER(sampler_MainTex);

			#if DISTORT_ON
            TEXTURE2D(_DistortTex);    SAMPLER(sampler_DistortTex);
			#endif
            #if DISSOLVE_ON
            TEXTURE2D(_DissolveTex);    SAMPLER(sampler_DissolveTex);
			#endif

            CBUFFER_START(UnityPerMaterial)
            half4 _MainTex_ST, _MainTex_TexelSize;
            half4 _Color;

            // 注释掉keywords为了适配SRP Batcher
			// #if DISTORT_ON
			half4 _DistortTex_ST;
			half _DistortTexXSpeed, _DistortTexYSpeed, _DistortAmount;
			// #endif
            // #if DISSOLVE_ON
			half4 _DissolveTex_ST;
			half _DissolveAmount, _DissolveEdge;
			// #endif
            // #if BLUR_ON
			half _BlurIntensity;
			// #endif
            // #if OUTLINE_ON
			half4 _OutlineColor;
			half _OutlineAlpha, _OutlineGlow, _OutlineWidth;
			half _InnerOutlineThickness, _InnerOutlineAlpha, _InnerOutlineGlow;
			// #endif
            // #if TEXTURESCROLL_ON
			half _TextureScrollXSpeed, _TextureScrollYSpeed;
            // #endif

			float4 _FresnelColor;
			float _FresnelPower;
			float _FresnelStrenght;

            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                ZERO_INITIALIZE(Varyings, output);

                output.vertex = TransformObjectToHClip(input.vertex.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.color = input.color;
                #if DISTORT_ON
				output.uvDistTex = TRANSFORM_TEX(input.uv, _DistortTex);
				#endif
                output.normalWS = TransformObjectToWorldNormal(input.normal);
                output.viewDirWS = GetCameraPositionWS() - output.vertex.xyz;

                return output;
            }

            half4 Blur(half2 uv, half Intensity)
            {
                half step = 0.00390625f * Intensity;
                half4 result = half4 (0, 0, 0, 0);
                half2 texCoord = half2(0, 0);
                texCoord = uv + half2(-step, -step);
                result += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, texCoord);
                texCoord = uv + half2(-step, 0);
                result += 2.0 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, texCoord);
                texCoord = uv + half2(-step, step);
                result += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, texCoord);
                texCoord = uv + half2(0, -step);
                result += 2.0 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, texCoord);
                texCoord = uv;
                result += 4.0 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, texCoord);
                texCoord = uv + half2(0, step);
                result += 2.0 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, texCoord);
                texCoord = uv + half2(step, -step);
                result += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, texCoord);
                texCoord = uv + half2(step, 0);
                result += 2.0* SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, texCoord);
                texCoord = uv + half2(step, -step);
                result += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, texCoord);
                result = result * 0.0625;
                return result;
            }

            half4 frag(Varyings i) : SV_Target
            {
                half2 uvRect = i.uv;

                #if DISTORT_ON
                i.uvDistTex.x += (_Time.y * _DistortTexXSpeed) % 1;
                i.uvDistTex.y += (_Time.y * _DistortTexYSpeed) % 1;
                half distortAmnt = (SAMPLE_TEXTURE2D(_DistortTex, sampler_DistortTex, i.uvDistTex).r - 0.5) * 0.2 * _DistortAmount;
                i.uv.x += distortAmnt;
                i.uv.y += distortAmnt;
				#endif

                #if TEXTURESCROLL_ON
                i.uv.xy = frac(i.uv.xy + _Time.y * half2(_TextureScrollXSpeed, _TextureScrollYSpeed));
                #endif

                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                col *= _Color;

				float ndv_inv = 1 - saturate(dot( normalize(i.viewDirWS), i.normalWS));
				float3 fresnel = _FresnelColor.rgb * pow(ndv_inv, _FresnelPower) * _FresnelStrenght;
				col.rgb += fresnel.rgb;

				half originalAlpha = col.a;

                #if BLUR_ON
				col = Blur(i.uv, _BlurIntensity) * _Color;
                col.rgb += fresnel.rgb;
				#endif

				#if OUTLINE_ON
					// half2 destUv = half2(_OutlinePixelWidth * _MainTex_TexelSize.x, _OutlinePixelWidth * _MainTex_TexelSize.y);
					half2 destUv = half2(_OutlineWidth * _MainTex_TexelSize.x * 200, _OutlineWidth * _MainTex_TexelSize.y * 200);

					half spriteLeft = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + half2(destUv.x, 0)).a;
					half spriteRight = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv - half2(destUv.x, 0)).a;
					half spriteBottom = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + half2(0, destUv.y)).a;
					half spriteTop = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv - half2(0, destUv.y)).a;
					half result = spriteLeft + spriteRight + spriteBottom + spriteTop;
					half spriteTopLeft = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + half2(destUv.x, destUv.y)).a;
					half spriteTopRight = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + half2(-destUv.x, destUv.y)).a;
					half spriteBotLeft = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + half2(destUv.x, -destUv.y)).a;
					half spriteBotRight = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + half2(-destUv.x, -destUv.y)).a;
					result = result + spriteTopLeft + spriteTopRight + spriteBotLeft + spriteBotRight;
					
					result = step(0.05, saturate(result));
					result *= (1 - originalAlpha) * _OutlineAlpha;

					half4 outline = _OutlineColor;
					outline.rgb *= _OutlineGlow;
					outline.a = result;
					col = lerp(col, outline, result);

                    // in int offsetX, in int offsetY, half2 uv, sampler2D tex
                    half3 y1 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, (i.uv + half2(0, _InnerOutlineThickness * _MainTex_TexelSize.y))).rgb;
                    half3 y2 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, (i.uv + half2(0, -_InnerOutlineThickness * _MainTex_TexelSize.y))).rgb;
                    half3 x1 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, (i.uv + half2(_InnerOutlineThickness * _MainTex_TexelSize.x, 0))).rgb;
                    half3 x2 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, (i.uv + half2(-_InnerOutlineThickness * _MainTex_TexelSize.x, 0))).rgb;

				    half3 innerT = abs(y1 - y2);
				    innerT += abs(x1 - x2);
				    innerT = (innerT / 2.0) * col.a * _InnerOutlineAlpha;
				    col.rgb += length(innerT) * _OutlineColor.rgb * _InnerOutlineGlow;
				#endif

                #if DISSOLVE_ON
                half2 uvDissolve = uvRect;
                half4 dissolveColor = SAMPLE_TEXTURE2D(_DissolveTex, sampler_DissolveTex, uvDissolve);
                half k = -_DissolveEdge * 0.01 + _DissolveAmount * (1 + _DissolveEdge * 0.01);
                col.a *= smoothstep(k, k + _DissolveEdge * 0.01, dissolveColor.r);
                #endif

                return col;
            }
            ENDHLSL
        }
    }
    FallBack Off
    CustomEditor "EffectShader"
}

