Shader "YAWater"
{
	Properties
	{
		[HideInInspector]_Phases("Phases", Vector) = (0.28, 0.50, 0.07, 0.)
		[HideInInspector]	_Amplitudes("Amp", Vector) = (4.02, 0.34, 0.65, 0.)
		[HideInInspector]	_Frequencies("Freq", Vector) = (0.00, 0.48, 0.08, 0.)
		[HideInInspector]	_Offsets("Offset", Vector) = (0.00, 0.16, 0.00, 0.)
		_Sheight("波纹方向密度", Vector) = (0.1, 1, 0, 0)
		_Distortion("水底扰动程度", Range(0, 250)) = 8
		_Volume("水色渐变区间", Range(1, 500)) = 10
	    _CustomViewDir("波纹位置偏移", Vector) = (0.00, 0.00, 0.00, 0.00)
	    _middleRange("渐变过度", Range(0,1)) = 0.5
        //_AlphaScale("_AlphaScale", Range(0, 1)) = 1
	    [Toggle(_CUSTOM_COLOR_GRAD_ON)]_CUSTOM_COLOR_GRAD("颜色渐变", Float) = 0
	    _AlphaTex("海水透明度", 2D) = "white"{}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		//GrabPass { "_CameraColorTexture" }
		Pass
		{
			HLSLPROGRAM
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
            #pragma shader_feature _CUSTOM_COLOR_GRAD_ON

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				//UNITY_FOG_COORDS(4)
				float4 vertex : SV_POSITION;

				float3 worldNormal : TEXCOORD1;
				float4 projPos : TEXCOORD2;
				float3 worldPos : TEXCOORD3;
			};
			half4 _Phases;
			half4 _Frequencies;
			half4 _Amplitudes;
			half4 _Offsets;
			half4 _CustomViewDir;
			float _Distortion;
			float2 _Sheight;
			float _Volume;
			float _middleRange;
		    //float _AlphaScale;
			float _CUSTOM_COLOR_GRAD;
float4 _AlphaTex_ST;
			TEXTURE2D(_AlphaTex);
			SAMPLER(sampler_AlphaTex);
			TEXTURE2D(_CameraOpaqueTexture);
			SAMPLER(sampler_CameraOpaqueTexture);
			TEXTURE2D_X_FLOAT(_CameraDepthTexture);
			SAMPLER(sampler_CameraDepthTexture);
			/*const float4 _Phases = (0.28, 0.50, 0.07, 0.);
			const float4 _Amplitudes = (4.02, 0.34, 0.65, 0.);
			const float4 _Frequencies = (0.00, 0.48, 0.08, 0.);
			const float4 _Offsets = (0.00, 0.16, 0.00, 0.);*/
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = TransformObjectToHClip(v.vertex);
				//UNITY_TRANSFER_FOG(o,o.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _AlphaTex);
				o.projPos = ComputeScreenPos(o.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.worldNormal = TransformObjectToWorldNormal(v.normal);
				o.projPos.z = -TransformWorldToView( o.worldPos ).z;//COMPUTE_EYEDEPTH(o.projPos.z);
				return o;
			}

			half4 cosine_gradient(float x,  half4 phase, half4 amp, half4 freq, half4 offset){
				const float TAU = 2. * 3.14159265;
  				phase *= TAU;
  				x *= TAU;

  				return half4(
    				offset.r + amp.r * 0.5 * cos(x * freq.r + phase.r) + 0.5,
    				offset.g + amp.g * 0.5 * cos(x * freq.g + phase.g) + 0.5,
    				offset.b + amp.b * 0.5 * cos(x * freq.b + phase.b) + 0.5,
    				offset.a + amp.a * 0.5 * cos(x * freq.a + phase.a) + 0.5
  				);
			}
			half3 toRGB(half3 grad){
  				 return grad.rgb;
			}
			float2 rand(float2 st, int seed)
			{
				float2 s = float2(dot(st, float2(127.1, 311.7)) + seed, dot(st, float2(269.5, 183.3)) + seed);
				return -1 + 2 * frac(sin(s) * 43758.5453123);
			}
			float noise(float2 st, int seed)
			{
				st.y += _Time[1];

				float2 p = floor(st);
				float2 f = frac(st);

				float w00 = dot(rand(p, seed), f);
				float w10 = dot(rand(p + float2(1, 0), seed), f - float2(1, 0));
				float w01 = dot(rand(p + float2(0, 1), seed), f - float2(0, 1));
				float w11 = dot(rand(p + float2(1, 1), seed), f - float2(1, 1));

				float2 u = f * f * (3 - 2 * f);

				return lerp(lerp(w00, w10, u.x), lerp(w01, w11, u.x), u.y);
			}
			float3 swell(float3 normal , float3 pos , float anisotropy, float2 sheight){
				float height = noise(pos.xz * sheight,0);
				height *= anisotropy ;
				normal = normalize(
					cross (
						float3(0,ddy(height),1),
						float3(1,ddx(height),0)
					)
				);
				return normal;
			}

			half4 frag (v2f i) : SV_Target
			{
				half4 col;
				float sceneZ = LinearEyeDepth(SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, i.projPos.xy/i.projPos.w).r, _ZBufferParams);

    			//float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
				float partZ = i.projPos.z;
				float volmeZ = saturate( (sceneZ - partZ)/_Volume);


				half4 cos_grad = cosine_gradient(1-volmeZ, _Phases, _Amplitudes, _Frequencies, _Offsets);
  				cos_grad = clamp(cos_grad, 0., 1.);
  				col.rgb = toRGB(cos_grad);

				half3 worldViewDir = normalize(_WorldSpaceCameraPos - i.worldPos - _CustomViewDir.xyz) ;

				float3 v = i.worldPos+_CustomViewDir.xyz - _WorldSpaceCameraPos;
				float anisotropy = saturate(1/(ddy(length ( v.xz )))/5);


			    float pro = i.projPos.y/i.projPos.w;
			    pro = max(1.0 - pow(pro*2.0-1.0, 2),  _middleRange);
                //if (pro > 0.5)
                //    pro =1.0 - pro;

				float3 swelledNormal = swell(i.worldNormal , i.worldPos+_CustomViewDir.xyz , anisotropy, _Sheight);
                swelledNormal.xz *=pro;
                half3 reflDir = (reflect(-worldViewDir, swelledNormal));
				half4 reflectionColor = SAMPLE_TEXTURECUBE_LOD(unity_SpecCube0, samplerunity_SpecCube0, reflDir, 0);
				/* speclar
				float spe = pow( saturate(dot( reflDir, normalize(_lightPos.xyz))),100);
				float3 lightColor = float3(1,1,1);
				reflectionColor += 0.4 * half4((spe * lightColor).xxxx);
				*/

				half isRefration = step(i.projPos.z - sceneZ, 1.0);


				if (i.projPos.z < sceneZ)
				{
					i.projPos.xy += swelledNormal.xz * _Distortion;
				}
				half3 refractionColor = SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, i.projPos/i.projPos.w).rgb;

				col.rgb*=refractionColor;
				// fresnel reflect
				float f0 = 0.02;
    			float vReflect = f0 + (1-f0) * pow(
					(1 - dot(worldViewDir,swelledNormal)),
				5);
				vReflect = saturate(vReflect * 2.0);

				col = lerp(col , reflectionColor , vReflect);

				float alpha = saturate(volmeZ);
			    float al = SAMPLE_TEXTURE2D(_AlphaTex, sampler_AlphaTex, i.uv).a;
  				alpha *= al;

                #if _CUSTOM_COLOR_GRAD_ON
			        col.rgb *= pro;
			    #endif
				return float4(col.rgb, al);
			}
			ENDHLSL
		}
	}
}
