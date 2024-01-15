Shader "URP/Water/UnderCaustics"
{
    Properties
    {
        [Space(10)]
        [Main(Base, _, off, off)] _Base ("基础属性", Float) = 1.0
        [Space(10)]
        [Sub(Base)]_WaterDeep("深水范围", Float) = 5
        [Sub(Base)]_Alpha("总体透明度控制", Float) = 1
    	[Sub(Base)]_DayIntensity("总体亮度控制", Float) = 0.75
	    [Sub(Base)]_Soft("_Soft",Range(0,5)) = 1.0
	    [Sub(Base)]_SoftFade("_SoftFade",Float) = 1.0

        [Space(10)]
        [Main(Fresnel, _, off, off)] _Fresnel ("Fresnel效应", Float) = 1.0
        [Space(10)]
        [Sub(Fresnel)]_ReflectExhance("反射度增强", Float) = 1
        
        [Space(10)]
        [Main(Caustics, _, off, off)] _Caustics ("焦散", Float) = 1.0
        [Space(10)]
        [Sub(Caustics)]_CausticsTex("焦散图", 2D) = "white" {}
		[Sub(Caustics)]_CausticsScale("大小", Float) = 5
		[Sub(Caustics)]_CausticsSpeed("速度", Vector) = (-8,0,0,0)
		[Sub(Caustics)]_CausticsIntensity("亮度", Float) = 1
        
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
			ZWrite Off
        	ZTest LEqual
           HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
            #pragma shader_feature_local_fragment _ISBULID
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Core.hlsl"
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Lighting.hlsl"
           #include  "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/DeclareDepthTexture.hlsl"
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
                float4 positionCS               : SV_POSITION;
            };
         
           
            TEXTURE2D(_CausticsTex);
            SAMPLER(sampler_CausticsTex);
            CBUFFER_START(UnityPerMaterial)
                half _ReflectExhance;
	            half _WaterDeep;
	            half2 _CausticsSpeed;
	            half _CausticsScale;
	            half _CausticsIntensity;
	            half _Alpha;
	            half _DayIntensity;
				half _Soft;
				half _SoftFade;
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
           Varyings LitPassVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
            	
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                output.uv = input.texcoord;
                output.normalWS = normalInput.normalWS;         
                output.positionWS = vertexInput.positionWS;
                output.positionCS = vertexInput.positionCS;
                output.positionOS = input.positionOS;
            	 real sign = input.tangentOS.w * GetOddNegativeScale();
            	 half4 tangentWS = half4(normalInput.tangentWS.xyz, sign);
            	output.tangentWS = tangentWS;
                return output;
            }

            float4 LitPassFragment (Varyings input) : SV_Target
            {
                half2 uv  = input.uv;
                half3 nDirWS = normalize(input.normalWS);
            	float3 positionOS = input.positionOS;
                float3 positionWS = input.positionWS;
            	float4 deepCS = TransformWorldToHClip(TransformObjectToWorld( positionOS));
            	float4 positionCSNDC = float4(deepCS.xyz/ deepCS.w,1.0);
                half3 vDirWS = GetWorldSpaceNormalizeViewDir(positionWS);
            	
                //Fresnel
                half NdotV = 1.0 - max(dot(nDirWS,vDirWS),0.0);
                half NdotVpow5 = NdotV * NdotV * NdotV * NdotV * NdotV;
                half fresnelFactor =lerp( clamp((_ReflectExhance * 0.04 ),0,0.5),1.0,NdotVpow5);
            	
                //screen
                half4 screenPos =  ComputeScreenPos(positionCSNDC);
            	float screenDepth = 0;
            	#if defined (_ISBULID)
            	float screenDepth1 =  SampleSceneDepth(screenPos.xy); ;
            	screenDepth = lerp(-1,1,screenDepth1);
            	#else
            	screenDepth =  SampleSceneDepth(screenPos.xy);
            	#endif
				half LinearDepth = LinearEyeDepth(screenDepth,_ZBufferParams);
                half fade = saturate((LinearDepth - screenPos.w - _Soft)/_SoftFade );       
                
                //depth
                float waterDepth =(LinearEyeDepth(screenDepth,_ZBufferParams  )-LinearEyeDepth(screenPos.z,_ZBufferParams  )) ;
            	float3 underWaterPos = ReconstructWorldPos(screenPos,screenDepth);
                float waterDeepRange = clamp(waterDepth * rcp(_WaterDeep),0.0,1.0);
                float waterDepthRange = clamp(max(waterDeepRange + fresnelFactor,fresnelFactor),0.0,1.0);
            	
                //Caustics Color
                half2 causticsScale = underWaterPos.xz * rcp(_CausticsScale);
                half2 causticsSpeed = _CausticsSpeed * _Time.y * 0.01;
                half2 causticsUV1 = causticsScale + causticsSpeed;
                half2 causticsUV2 = causticsSpeed - causticsScale;
                half4 causticsMap1 = SAMPLE_TEXTURE2D(_CausticsTex,sampler_CausticsTex,causticsUV1);
                half4 causticsMap2 = SAMPLE_TEXTURE2D(_CausticsTex,sampler_CausticsTex,causticsUV2);
                half4 finalCaustics = min(causticsMap1,causticsMap2) * _CausticsIntensity *(1-  waterDepthRange);
            	
            	half alphaRange =clamp(finalCaustics.r,0.0,1.0);
				half finalAlpha = clamp(alphaRange* _Alpha,0.0,1.0) * clamp(0,1,fade);
            	half3 combineColor = (finalCaustics).rgb * _DayIntensity;
                
                return float4(combineColor,finalAlpha);
            }   
            ENDHLSL
        }
    	
    }
     FallBack "Hidden/Universal Render Pipeline/FallbackError"
     CustomEditor "LWGUI.LWGUI"
}