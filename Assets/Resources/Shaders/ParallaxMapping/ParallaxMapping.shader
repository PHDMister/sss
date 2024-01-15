Shader "URP/ParallaxMapping"
{
    Properties
    {
        [Header(BaseColor)]
        [Space(10)]
    	_BaseMap("BaseMap", 2D) = "white" {}
        _BaseColor("BaseColor", Color) = (1,1,1,1)
        [Header(Normal)]
        [Space(10)]
        _NormalMap("NormalMap", 2D) = "bump" {}
        _NormalIntensity("NormalIntensity", Float) = 1
		[Header(HeightMap)]
        [Space(10)]
		_HeightMap("HeightMap", 2D) = "white" {}
        _Tilling("Tilling", Float) = 5
		_POMScale("POMScale", Range( -0.5 , 0.5)) = 0
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline"  "Queue"="AlphaTest-150"}
        Pass
        {
           Tags{"LightMode" = "UniversalForward"}
            Cull Back
            HLSLPROGRAM
            #pragma target 3.0
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Core.hlsl"
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Lighting.hlsl"
           #include "Assets/Resources/Shaders/PBR/FurnitureLighting.hlsl"
           struct Attributes
            {
                float4 positionOS   : POSITION;
                half3 normalOS     : NORMAL;
                half4 tangentOS    : TANGENT;
                half2 texcoord     : TEXCOORD0;
           		half2 staticLightmapUV : TEXCOORD1;
            };
            struct Varyings
            {
                half2 uv                       : TEXCOORD0;
                float3 positionWS               : TEXCOORD1;
                half3 normalWS                 : TEXCOORD2;
                half4 tangentWS                : TEXCOORD3;
                float3 positionOS               : TEXCOORD4;
            	half2 staticLightmapUV : TEXCOORD5;
                float4 positionCS               : SV_POSITION;
            };
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_HeightMap);
            SAMPLER(sampler_HeightMap);
            CBUFFER_START(UnityPerMaterial)
				half4 _HeightMap_ST;
				half4 _NormalMap_ST;
				half4 _BaseMap_ST;
                half4 _BaseColor;
                half _NormalIntensity;
                half _Tilling;
                half _POMScale;
            CBUFFER_END  
            
   //          inline float2 POM( sampler2D heightMap, half2 uvs, half2 dx, half2 dy, half3 normalWorld, half3 viewWorld, half3 viewDirTan, int minSamples, int maxSamples, half parallax, half refPlane, half2 tilling, half2 curv, int index )
			// {
			// 	half3 result = 0;
			// 	int stepIndex = 0;
			// 	int numSteps = ( int )lerp( (half)maxSamples, (half)minSamples, saturate( dot( normalWorld, viewWorld ) ) );
			// 	half layerHeight = 1.0 * rcp(numSteps) ;
			// 	half2 plane = parallax * ( viewDirTan.xy * rcp(viewDirTan.z)  );
			// 	uvs.xy += refPlane * plane;
			// 	half2 deltaTex = -plane * layerHeight;
			// 	half2 prevTexOffset = 0;
			// 	half prevRayZ = 1.0f;
			// 	half prevHeight = 0.0f;
			// 	half2 currTexOffset = deltaTex;
			// 	half currRayZ = 1.0f - layerHeight;
			// 	half currHeight = 0.0f;
			// 	half intersection = 0;
			// 	half2 finalTexOffset = 0;
			// 	while ( stepIndex < numSteps + 1 )
			// 	{
			// 		
			// 	 	currHeight = tex2Dgrad( heightMap, uvs + currTexOffset, dx, dy ).r;
			// 	 	if ( currHeight > currRayZ )
			// 	 	{
			// 	 	 	stepIndex = numSteps + 1;
			// 	 	}
			// 	 	else
			// 	 	{
			// 	 	 	stepIndex++;
			// 	 	 	prevTexOffset = currTexOffset;
			// 	 	 	prevRayZ = currRayZ;
			// 	 	 	prevHeight = currHeight;
			// 	 	 	currTexOffset += deltaTex;
			// 	 	 	currRayZ -= layerHeight;
			// 	 	}
			// 	}
			// 	finalTexOffset = currTexOffset;
			// 	return uvs.xy + finalTexOffset;
			// }
           inline float2 ParallaxOffset(  half height, half3 viewDir )
			{
			    half h =0.7 ;
			    	h = h * height - height/2.0;
			    half3 v = normalize(viewDir);
			    v.z += 0.42;
			    return h * (v.xy / v.z);
			}

           Varyings LitPassVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                output.uv = input.texcoord;
                output.normalWS = normalInput.normalWS;
            	real sign = input.tangentOS.w * GetOddNegativeScale();
                half4 tangentWS = half4(normalInput.tangentWS.xyz, sign);
                output.tangentWS = tangentWS;
                output.positionWS = vertexInput.positionWS;
                output.positionCS = vertexInput.positionCS;
            	output.positionOS = input.positionOS;
            	output.staticLightmapUV = input.staticLightmapUV * unity_LightmapST.xy + unity_LightmapST.zw;
                return output;
            }

            half4 LitPassFragment (Varyings input) : SV_Target
            {
            	float3 positionWS = input.positionWS;
            	half2 uv = positionWS.xz * _Tilling;
            	half scale = _POMScale * 0.1;
            	half3 nDirWS = normalize(input.normalWS);
                half3 tangentWS = normalize(input.tangentWS.zyx);
            	half3 bDirWS = normalize(cross(nDirWS,tangentWS) * input.tangentWS.w);
                half3x3 TBN  = half3x3(tangentWS,bDirWS,nDirWS);
            	half3 vDirWS = GetWorldSpaceNormalizeViewDir(positionWS);
            	half3 vDirTS = TransformWorldToTangent(vDirWS,CreateTangentToWorld(nDirWS,tangentWS,input.tangentWS.w));
            	//half2 normalUV = POM(_HeightMap,uv,ddx(uv),ddy(uv),nDirWS,vDirWS,vDirTS,8,8,scale,0,_HeightMap_ST.xy, float2(0,0), 0 );
            	half height = SAMPLE_TEXTURE2D(_HeightMap,sampler_HeightMap,uv).r;
            	half2 offset = ParallaxOffset(height,vDirTS);
            	uv += offset;
            	half3 nDirTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap,sampler_NormalMap,uv * _NormalMap_ST.xy + _NormalMap_ST.zw) ,_NormalIntensity) ;
                nDirWS = normalize(mul(nDirTS,TBN));
            	//half baseUV = POM(_HeightMap,positionWS.xz,ddx(positionWS.xz),ddy(positionWS.xz),nDirWS,vDirWS,vDirTS,8,8,scale,0,_HeightMap_ST.xy, float2(0,0), 0 );
            	half3 BaseColor = _BaseColor.rgb * SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,uv * _BaseMap_ST.xy + _BaseMap_ST.zw).rgb;
            	half Metallic = 0;
                half Roughness = 1;
                half Occlusion = 1;
            	half3 DiffuseColor = lerp(BaseColor,half3(0.0,0.0,0.0),Metallic);
                half3 SpecularColor = lerp(half3(0.04,0.04,0.04),BaseColor,Metallic);
                Roughness = max(Roughness,0.001f);
            	 //IndirectLight
                half3 IndirectLighting = half3(0,0,0);
                IndirectLighting_float(DiffuseColor,SpecularColor,Roughness,positionWS,nDirWS,vDirWS,Occlusion,input.staticLightmapUV,1,IndirectLighting);
                half4 color = half4(IndirectLighting ,1);
                return color;
            }   
            ENDHLSL
        }
    	   Pass
        {
            Name "Meta"
            Tags{"LightMode" = "Meta"}

            Cull Off

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 3.0

            #pragma vertex UniversalVertexMeta
            #pragma fragment UniversalFragmentMeta

            #pragma shader_feature_local_fragment _SPECULAR_SETUP
            #pragma shader_feature_local_fragment _EMISSION
            #pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED

            #pragma shader_feature_local_fragment _SPECGLOSSMAP

            #include "LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitMetaPass.hlsl"
            ENDHLSL
        }
    
    }
     FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
