 Shader "URP/Water/WaterWave"
{
    Properties
    {
        [Space(10)]
        [Main(Base, _, off, off)] _Base ("基础属性", Float) = 1.0
        [Space(10)]
        [Sub(Base)]_ShallowColor("浅水颜色", Color) = (0.4862745,1,0.8588235,0)
        [Sub(Base)]_DeepColor("深水颜色", Color) = (0,0.4666667,0.7450981,0)
        [Sub(Base)]_WaterDeep1("浅水范围", Float) = 1.5
        [Sub(Base)]_WaterDeep("深水范围", Float) = 5
    	[Sub(Base)]_ShoreDistance("边缘透明范围", Float) = 1.5
        [Sub(Base)]_Alpha("总体透明度控制", Float) = 1
    	[Sub(Base)]_DayIntensity("总体亮度控制", Float) = 0.75
//        [Space(10)]
//        [Main(SmallNormal, _, off, off)] _SmallNormal ("细波纹法线", Float) = 0
//        [Space(10)]
//        [Sub(SmallNormal)]_WaterNormalSmall("法线图", 2D) = "bump" {}
//        [Sub(SmallNormal)]_SmallNormalIntensity("强度", Range( 0 , 1)) = 0.1
//        [Sub(SmallNormal)]_SmallNormalTiling("Tiling", Float) = 10
//        [Sub(SmallNormal)]_SmallNormalSpeed("速度", Float) = 5
//        
        [Space(10)]
        [Main(LargeNormal, _, off, off)] _LargeNormal ("大波纹法线", Float) = 1.0
        [Space(10)]
        [Sub(LargeNormal)]_WaterNormalLarge("法线图", 2D) = "bump" {}
        [Sub(LargeNormal)]_LargeNormalIntensity("强度", Range( 0 , 1)) = 0.1
        [Sub(LargeNormal)]_LargeNormalTiling("Tiling", Float) = 10
        [Sub(LargeNormal)]_LargeNormalSpeed("速度", Float) = 5
       
        [Space(10)]
        [Main(Fresnel, _, off, off)] _Fresnel ("Fresnel效应", Float) = 1.0
        [Space(10)]
        [Sub(Fresnel)]_FresnelColor("颜色", Color) = (0.3686275,0.6431373,0.9137255,0)
        [Sub(Fresnel)]_FresnelIntensity("强度", Float) = 0.2
        [Sub(Fresnel)]_ReflectExhance("反射度增强", Float) = 1
        [Sub(Fresnel)]_ReflectionAngle("反射角度", Float) = 1
        
        [Space(10)]
        [Main(Splakes, _, off, off)] _Splakes ("波光", Float) = 0
        [Space(10)]
        [Sub(Splakes)]_SparklesIntensity("亮度", Float) = 10
		[Sub(Splakes)]_SparklesAmount("数量", Range( 0 , 1)) = 0.09
        
        [Space(10)]
        [Main(Under, _, off, off)] _Under ("水底", Float) = 1.0
        [Space(10)]
        [Sub(Under)]_UnderWaterDistort("折射", Float) = 3
        [Sub(Under)]_UnderWaterDark("压暗", Range( 0 , 1)) = 0
        
        [Space(10)]
        [Main(Caustics, _, off, off)] _Caustics ("焦散", Float) = 1.0
        [Space(10)]
        [Sub(Caustics)]_CausticsTex("焦散图", 2D) = "white" {}
		[Sub(Caustics)]_CausticsScale("大小", Float) = 5
		[Sub(Caustics)]_CausticsSpeed("速度", Vector) = (-8,0,0,0)
		[Sub(Caustics)]_CausticsIntensity("亮度", Float) = 1
        
        [Space(10)]
        [Main(Reflect, _, off, off)] _Reflect ("反射", Float) = 0
        [Space(10)]
        [Sub(Reflect)]_ReflectCube("反射cubeMap", CUBE) = "white" {}
        [Sub(Reflect)]_ReflectIntensity("强度", Float) = 1
        [Sub(Reflect)]_ReflectDistort("扭曲", Range( 0 , 1)) = 1
        
        [Space(10)]
        [Main(Foam, _, off, off)] _Foam ("岸边泡沫", Float) = 0
        [Space(10)]
        [Sub(Foam)]_FoamNoise("噪声图", 2D) = "white" {}
        [Sub(Foam)]_XTilling("泡沫TillingX", Float) = 10
		[Sub(Foam)]_YTilling("泡沫TillingY", Float) = 1
		[Sub(Foam)]_FoamNoiseSpeed("泡沫速度", Vector) = (0,-0.3,0,0)
		[Sub(Foam)]_FoamOffset("泡沫偏移", Float) = 0
		[Sub(Foam)]_FoamRange("泡沫范围", Float) = 1.5
		[Sub(Foam)]_FoamColor("泡沫颜色", Color) = (1,1,1,1)
    	
    	 [Space(10)]
        [Main(Wave, _, off, off)] _Wave ("水波动画", Float) = 0
        [Space(10)]
        [Sub(Wave)]_Direction("水波运动方向（XY）", Vector) = (1,1,0,0)
    	[Sub(Wave)]_SubWaveDirection("细节波形方向（XYZW）", Vector) = (-1,-1,-1,-1)
		[Sub(Wave)]_WaveSpeed("水波速度", Float) = 2.4
		[Sub(Wave)]_WaveDistance("水波大小", Range( 0 , 1)) = 0.7
		[Sub(Wave)]_WaveHeight("水波高度", Float) = 0.15 
        [Sub(Wave)]_WaveNormalStr("水波法线强度", Float) = 0.16
		[Sub(Wave)]_WaveFadeStart("水波渐隐Start", Float) = 25
		[Sub(Wave)]_WaveFadeEnd("水波渐隐End", Float) = 280
		[Sub(Wave)][HDR]_WaveColor("波峰颜色", Color) = (0.3686275,0.6431373,0.9137255,0)
		[Sub(Wave)]_Size("Size", Range( 0 , 1)) = 1
		[Sub(Wave)]_FallOff("FallOff", Range( 0 , 1)) = 1
		[Sub(Wave)]_MotionVertexDisplacement("交互顶点强度", Float) = 0.1
    	
	    [Space(10)]
        [Main(Stencil, _, off, off)] _Stencil ("模版测试", Float) = 1
        [Space(10)]
        [IntRange][Sub(Stencil)]_StencilRef("Stencil Ref",Range(0,255)) = 0
        [SubEnum(Stencil, UnityEngine.Rendering.CompareFunction)]_Comp ("Stencil Comp", Float) = 8
        [SubEnum(Stencil, UnityEngine.Rendering.StencilOp)]_Op ("Stencil Op", Float) = 2
    	
    	 [Toggle(_ISBULID)] _ISBULID("isBulid",Float) = 0.0
        
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline" = "UniversalPipeline"}
        Pass
        {
           Tags{"LightMode" = "UniversalForward"}
            Cull Back
        	ZWrite Off
        	ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        	stencil
        {
            Ref [_StencilRef]
            //ReadMask 1
            //writeMask 1
            Comp [_Comp]
            Pass [_Op]
            Fail Keep
            ZFail Keep
        }
			
           HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
            #pragma shader_feature_local_fragment _ISBULID
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Core.hlsl"
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Lighting.hlsl"
            #include  "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/DeclareOpaqueTexture.hlsl"
             #include "WavesFunction.hlsl"
           struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                float2 texcoord     : TEXCOORD0;
            };
            struct Varyings
            {
                float2 uv                       : TEXCOORD0;
                float3 positionWS               : TEXCOORD1;
                float3 normalWS                 : TEXCOORD2;
                half4 tangentWS                : TEXCOORD3;
                float3 positionOS               : TEXCOORD4;
            	float3 Offset               : TEXCOORD5;
            	float3 Offset1               : TEXCOORD6;
                float4 positionCS               : SV_POSITION;
            };
            //TEXTURE2D(_WaterNormalSmall);
            //SAMPLER(sampler_WaterNormalSmall);
            TEXTURE2D(_WaterNormalLarge);
            SAMPLER(sampler_WaterNormalLarge);
            //TEXTURE2D_X_FLOAT(_CameraDepthTexture);
            //SAMPLER(sampler_CameraDepthTexture);
            //TEXTURE2D(_CameraOpaqueTexture);
            TEXTURE2D(_CausticsTex);
            SAMPLER(sampler_CausticsTex);
            TEXTURE2D(_FoamNoise);
            SAMPLER(sampler_FoamNoise);
            TEXTURECUBE(_ReflectCube);
            SAMPLER(sampler_ReflectCube);
            TEXTURE2D(_WaterMotionRT);
            SAMPLER(sampler_WaterMotionRT);
           
            CBUFFER_START(UnityPerMaterial)
                // half _SmallNormalTiling;
                // half _SmallNormalSpeed;
                // half _SmallNormalIntensity;
                half _LargeNormalTiling;
                half _LargeNormalSpeed;
                half _LargeNormalIntensity;
                half _ReflectExhance;
                half _ReflectionAngle;
                half _FresnelIntensity;
                half _SparklesIntensity;
                half _SparklesAmount;
                half _UnderWaterDistort;
                half _UnderWaterDark;
	            half4 _FoamColor;
	            half _FoamRange;
	            half _FoamOffset;
	            half2 _FoamNoiseSpeed;
	            half _YTilling;
	            half _XTilling;
	            half _ReflectIntensity;
	            half _ReflectDistort;
	            half _WaterDeep;
	            half2 _CausticsSpeed;
	            half _CausticsScale;
	            half _CausticsIntensity;
	            half _WaterDeep1;
	            half4 _ShallowColor;
	            half4 _DeepColor;
	            half4 _FresnelColor;
	            half _FallOff;
	            half _Size;
	            half _ShoreDistance;
	            half _Alpha;
	            half _DayIntensity;
	            float _WaveDistance;
				float _WaveHeight;
				float _WaveNormalStr;
				float _WaveFadeStart;
				float _WaveFadeEnd;
				half _WaveSpeed;
				half2 _Direction;
				half4 _SubWaveDirection;
				half _MotionVertexDisplacement;
	            float4 _WaterFXCamPos;
	            half _WaterFXCamSize;
				half4 _WaveColor;
            CBUFFER_END  


           float3 ReconstructWorldPos( float2 ScreenUV, float rawdepth )
			{
				//#if UNITY_REVERSED_Z
				real depth = rawdepth;
				//#else
				//real depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, rawdepth);
				//#endif
				float3 worldPos = ComputeWorldSpacePosition(ScreenUV, depth, UNITY_MATRIX_I_VP);
				return worldPos;
			}
           half remapDepth(half input,half MinOld,half MaxOld,half MinNew,half MaxNew)
            {
	            half output = MinNew + (input - MinOld) * (MaxNew - MinNew) / (MaxOld - MinOld);
            	return output;
            }

           Varyings LitPassVertex(Attributes input)
            {
            	//Wave
            	Varyings output = (Varyings)0;
            	float3 positionWS = mul(GetObjectToWorldMatrix(), input.positionOS).xyz;
            	half2 waveTime = _WaveSpeed * _TimeParameters.x * _Direction;
            	float3 positionWSoffset= float3(0,0,0);
            	float3 normalWS= float3(0,0,0);
            	GetWaveInfo(positionWS.xz,waveTime,_SubWaveDirection,_WaveDistance,_WaveHeight,_WaveNormalStr,_WaveFadeStart,_WaveFadeEnd,positionWSoffset,normalWS);
				float3 worldToObjOffset = mul(GetWorldToObjectMatrix(), float4((positionWS + positionWSoffset),1)).xyz;
            	float3 finalOffset = (_MotionVertexDisplacement * 0.1) + worldToObjOffset;
            	float3 positionOS = input.positionOS + finalOffset;
            	float3 worldToObjNormal = normalize(mul(GetWorldToObjectMatrix(), float4(normalWS,0)).xyz);
            	
                VertexPositionInputs vertexInput = GetVertexPositionInputs(positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(worldToObjNormal, input.tangentOS);
                output.uv = input.texcoord;
            	 real sign = input.tangentOS.w * GetOddNegativeScale();
            	 half4 tangentWS = half4(normalInput.tangentWS.xyz, sign);
            	output.tangentWS = tangentWS;
				output.normalWS = normalInput.normalWS;
            	output.Offset = finalOffset;
            	output.positionOS = positionOS;
            	output.positionWS = vertexInput.positionWS;
            	output.positionCS = vertexInput.positionCS;
            	output.Offset1 = positionWSoffset;
            	return output;
            }

            float4 LitPassFragment (Varyings input) : SV_Target
            {
                half2 uv  = input.uv;
                half3 nDirWS = normalize(input.normalWS);
                half3 tDirWS = normalize(input.tangentWS.xyz);
                half3 bDirWS = normalize(cross(nDirWS,tDirWS) * input.tangentWS.w);
                half3x3 TBN  = half3x3(tDirWS,bDirWS,nDirWS);
            	float3 positionOS = input.positionOS;
                float3 positionWS = input.positionWS;
                float4 positionCS = input.positionCS;
            	
            	float4 deepCS = TransformObjectToHClip(float4(positionOS,0));
            	float4 positionCSNDC = float4(deepCS.xyz/ deepCS.w,1.0);
                half3 vDirWS = GetWorldSpaceNormalizeViewDir(positionWS);
            	
            	//Interactive
				half2 waterMotionUV =  ((positionWS.xz - _WaterFXCamPos.xz) / (_WaterFXCamSize * 2)) + half2(0.5,0.5);
            	half4 waterMotionRT = SAMPLE_TEXTURE2D(_WaterMotionRT,sampler_WaterMotionRT,waterMotionUV);
            	half2 waterMotionTrack = waterMotionRT.rg * 2 -1;
            	half trackDot = dot(waterMotionTrack,waterMotionTrack);
            	float3 motionNormal = normalize(float3(waterMotionTrack.x,waterMotionTrack.y,sqrt(1-saturate(trackDot))));
            	half motionPower = clamp(waterMotionRT.a,0,1);
            	
                // //small normal
                // half2 smallTilling = uv * _SmallNormalTiling;
                // half smallSpeed = _SmallNormalSpeed * _Time.y * 0.1;
                // half2 samllnormaUV = smallTilling + half2(0.1,0.1) * smallSpeed;
                // half3 samllNormal = lerp(half3(0,0,1),UnpackNormalScale(SAMPLE_TEXTURE2D(_WaterNormalSmall,sampler_WaterNormalSmall,samllnormaUV) ,1).xyz,_SmallNormalIntensity) ;
                
                //large normal
                 half2 lagerTilling = uv * _LargeNormalTiling;
                 half lagerSpeed = _LargeNormalSpeed * _Time.y * 0.1;
                 half2 lagernormaUV = lagerTilling + half2(0.1,0.1) * lagerSpeed;
                 half3 lagerNormal = lerp(half3(0,0,1),UnpackNormalScale(SAMPLE_TEXTURE2D(_WaterNormalLarge,sampler_WaterNormalLarge,lagernormaUV) ,1).xyz,_LargeNormalIntensity) ;
                 half3 nDirTS = lerp(lagerNormal,motionNormal,motionPower);
            	 nDirWS = normalize(mul(nDirTS,TBN));
                 //blendNormal
                 //half3 nDirTS = normalize(BlendNormal(samllNormal,lagerNormal));

                //Fresnel
                //nDirWS = normalize(mul(nDirTS,TBN));
                half NdotV = 1.0 - max(dot(nDirWS,vDirWS),0.0);
                half NdotVpow5 = NdotV * NdotV * NdotV * NdotV * NdotV;
                half fresnelFactor =lerp( clamp((_ReflectExhance * 0.04 ),0,0.5),1.0,NdotVpow5);
                half fresnelAngle = pow(fresnelFactor,_ReflectionAngle);
                half reflectFresnel = clamp(fresnelAngle,0.0,1.0);
                half FresnelIntensity = fresnelAngle * _FresnelIntensity * 5;
                half colorFresnel =  clamp(FresnelIntensity * FresnelIntensity,0.0,1.0) ;

                //splaks
                half splaks = step(_SparklesAmount,nDirTS.y) * _SparklesIntensity;
                
                //screen
                float4 screenPos =  ComputeScreenPos(positionCSNDC);
            	
            	//--editor
            	//#if UNITY_REVERSED_Z
            	float screenDepth = 0;
            	#if defined (_ISBULID)
            	float screenDepth1 =  SampleSceneDepth(screenPos.xy); ;
            	screenDepth = lerp(-1,1,screenDepth1);
            	#else
            	screenDepth =  SampleSceneDepth(screenPos.xy);
            	screenDepth = lerp(0,1,screenDepth);
            	#endif
				//--bulid
            	//#else
            		//float screenDepth = remapDepth(screenDepth1,0,1,-1,1);
            		//float screenDepth = lerp(-1,1,screenDepth1);
            		
            	//#endif
                //half2 screenDistort = nDirTS.xy * _UnderWaterDistort * 0.01;
            	//half screenDepth = SampleSceneDepth(screenPos.xy);
                
                //depth
                float waterDepth =(LinearEyeDepth(screenDepth,_ZBufferParams )-LinearEyeDepth(screenPos.z,_ZBufferParams));
            	float3 underWaterPos = ReconstructWorldPos(screenPos.xy,screenDepth);
                float waterHeight = (positionWS - underWaterPos).y;
                float cszDepth = LinearEyeDepth(positionCS.z,_ZBufferParams );
                float foamMask = step(0,LinearEyeDepth(screenDepth,_ZBufferParams  ) - waterDepth);
                //half2 screenDistortPos  = screenPos.xy +screenDistort;
            	//half4 screenDistortPos  = screenPos ;
               //half screenDistortPosDepth = LinearEyeDepth(SampleSceneDepth(screenDistortPos.xy),_ZBufferParams);
                //half refractMask = step(0,screenDistortPosDepth - cszDepth);
                float waterDeepRange = clamp(waterDepth/_WaterDeep,0.0,1.0);
                float waterDepthRange = clamp(max(waterDeepRange + fresnelFactor,fresnelFactor),0.0,1.0);
                
                //underWaterColor
                //half2 distortUV = lerp(float2(0,0),screenDistort,refractMask) + screenPos.xy;
            	//half4 screenColor = half4(SampleSceneColor(distortUV),1);
                //half4 underWaterColor =(1 - _UnderWaterDark) * screenColor;
               half4 underWaterColor =(1 - _UnderWaterDark) *1 ;
                //Caustics Color
                half2 causticsScale = underWaterPos.xz * rcp(_CausticsScale);
                half2 causticsSpeed = _CausticsSpeed * _Time.y * 0.01;
                half2 causticsUV1 = causticsScale + causticsSpeed;
                half2 causticsUV2 = causticsSpeed - causticsScale;
                half4 causticsMap1 = SAMPLE_TEXTURE2D(_CausticsTex,sampler_CausticsTex,causticsUV1);
                half4 causticsMap2 = SAMPLE_TEXTURE2D(_CausticsTex,sampler_CausticsTex,causticsUV2);
                half4 finalCaustics = min(causticsMap1,causticsMap2) * _CausticsIntensity *(1-  waterDepthRange);
            	//half4 finalCaustics = min(causticsMap1,causticsMap2)* _CausticsIntensity ;
                
                //waterColor
                half4 deepColor = lerp(lerp(_DeepColor,_FresnelColor,colorFresnel),_WaveColor,input.Offset1.y);
                half4 ShallowColor = lerp(underWaterColor,_ShallowColor,clamp(waterDepth /(max(_WaterDeep1,0)),0.0,1.0)) + finalCaustics;
            	//half4 ShallowColor = _ShallowColor + finalCaustics;
            	//half4 ShallowColor = lerp(underWaterColor,_ShallowColor,clamp(waterDepth /(max(_WaterDeep1,0)),0.0,1.0)) ;
                half4 waterColor = lerp(ShallowColor,deepColor,waterDepthRange);

                //reflectColor
                //half3 reflectUV =reflect(vDirWS, mul(lerp(half3(0,0,1),nDirTS,_ReflectDistort),TBN) );
                //half4 reflectColor = SAMPLE_TEXTURECUBE(_ReflectCube,sampler_ReflectCube,reflectUV) *  _ReflectIntensity * reflectFresnel;

                //Foam
            	half FoamRange = waterDepth * rcp(_FoamRange);
                half FoamOffset = 1 - clamp(FoamRange + _FoamOffset,0,1 );
            	half FoamTillingY = clamp(FoamRange + _FoamOffset,0,1 ) * _YTilling ;
            	half FoamTillingX = _XTilling * uv.x ;
            	half2 FoamNoiseUV = (FoamTillingY + FoamTillingX) + _FoamNoiseSpeed * _Time.y;
            	half FoamNoise = step(SAMPLE_TEXTURE2D(_FoamNoise,sampler_FoamNoise,FoamNoiseUV).r,FoamOffset);
            	half Foam = clamp((FoamOffset + 1  ) * foamMask * FoamNoise,0.0,1.0);
            	half4 FoamColor = Foam * _FoamColor * 2 ;
            	
            	//alpha
            	half waterOpacity =clamp( waterHeight * rcp(max(_ShoreDistance,0)),0.0,1.0);
            	half alphaRange =clamp(max( max(reflectFresnel * waterOpacity,Foam),finalCaustics.r),0.0,1.0);
            	half border = 1 - clamp(distance(max(abs(uv - half2(0.5,0.5)) - _Size,0),0) * rcp(_FallOff),0.0,1.0);
				half finalAlpha = clamp(lerp(waterOpacity,1.0,alphaRange) * _Alpha,0.0,1.0) * border  ;
            	//fianlColor
            	//half3 combineColor = (waterColor+reflectColor+splaks).rgb * _DayIntensity;
            	half3 combineColor = (waterColor+splaks).rgb * _DayIntensity;
            	half3 finalColor = lerp(combineColor,FoamColor.rgb,Foam);
                return float4(finalColor,finalAlpha);
            	//return ShallowColor;
            	//return float4(half3(0.5,0.5,0.5),finalAlpha);
            }   
            ENDHLSL
        }
    	
    }
     FallBack "Hidden/Universal Render Pipeline/FallbackError"
     CustomEditor "LWGUI.LWGUI"
}