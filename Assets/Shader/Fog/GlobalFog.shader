Shader "URP/GlobalFog"
{
    Properties
    {
        [Header(Bace Color)]
        [Space(10)]
        
       
        
        _FogColor("FogColor", Color) = (0,0.213793,1,0)
		_FogIntensity("Fog Intensity", Range( 0 , 1)) = 1
		_FogDistanceStart("Fog Distance Start", Float) = 0
		_FogDistanceEnd("Fog Distance End", Float) = 700
		_FogHeightStart("Fog Height Start", Float) = 0
		_FogHeightEnd("Fog Height End", Float) = 700
		_SunFogcolor("Sun Fog color", Color) = (1,0.5172414,0,0)
    	_lightDir("light Direction",Vector) = (1,1,1,0)
		_SunFogRange("Sun Fog Range", Float) = 10
		_SunFogIntensity("Sun Fog Intensity", Float) = 1
		

        
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+1000" "RenderPipeline" = "UniversalPipeline"}
    	
        Pass
        {
        	Name "Forward"
           Tags{"LightMode" = "UniversalForwardOnly"}
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZWrite Off
			ZTest Always
        	Cull Off
           HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Core.hlsl"
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Lighting.hlsl"
           #include "Assets/Resources/Shaders/PBR/FurnitureLighting.hlsl"
           #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
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
           
           
            CBUFFER_START(UnityPerMaterial)
           		float4 _FogColor;
				float4 _SunFogcolor;
				
				float _SunFogRange;
				float _SunFogIntensity;
				float _FogHeightEnd;
				float _FogHeightStart;
				float _FogDistanceEnd;
				float _FogDistanceStart;
				
				float _FogIntensity;
				float3 _lightDir;
            CBUFFER_END  
            

           
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
                return output;
            }

            half4 LitPassFragment (Varyings input) : SV_Target
            {
                half2 uv  = input.uv;
                 float3 positionWS = input.positionWS;
                half3 vDirWS = GetWorldSpaceNormalizeViewDir(positionWS);
				float2 UV = input.positionCS.xy / _ScaledScreenParams.xy;
            	#if UNITY_REVERSED_Z
				    real depth = SampleSceneDepth(UV);
				#else
				    // 调整 z 以匹配 OpenGL 的 NDC
				    real depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(UV));
				#endif
            	float3 worldPos = ComputeWorldSpacePosition(UV, depth, UNITY_MATRIX_I_VP);
            	//Distance fog
            	half fogdistance = distance(worldPos,_WorldSpaceCameraPos);
            	half fogdistance_linear = 0;
            	fogLinear(fogdistance,_FogDistanceStart,_FogDistanceEnd,fogdistance_linear);

            	//Height fog
            	half fogheight_linear = 0;
            	fogLinear(worldPos.y,_FogHeightStart,_FogHeightEnd,fogheight_linear);
            	half fogHeight = 1 - fogheight_linear;

            	//sun fog
            	half HdotL = dot(-vDirWS,_lightDir) * 0.5 + 0.5;
            	half sunFog = saturate(pow(HdotL,_SunFogRange)) * _SunFogIntensity;

            	//combine fog
            	half3 finalColor = lerp(_FogColor,_SunFogcolor,sunFog);
				half finalAlpha = saturate(fogdistance_linear * fogHeight * _FogIntensity);
            	
                return half4(finalColor,finalAlpha);
            }   
            ENDHLSL
        }
    }
     FallBack "Hidden/Universal Render Pipeline/FallbackError"
}