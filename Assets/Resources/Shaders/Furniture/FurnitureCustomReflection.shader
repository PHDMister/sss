Shader "URP/Furniture/CustomReflection"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.CullMode)]_CullMode("Cull Mode", Float) = 2.0
        [Header(Surface Inputs)]
        [Space(10)]
        [Toggle(_BASEMAP)] _Base("BaseMap Toggle", Float) = 1.0
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        [MainColor] _BaseColor("Base Color", Color) = (1,1,1,1)
        _Metallic("Metallic",Range(0.0, 1.5)) = 0.0
        _Smoothness("Smoothness", Range(0.0, 2.0)) = 0.0
        
        [Space(10)]
        [Header(Normal)]
        [Space(10)]
        [Toggle(_NORMAL)] _Normal("NormalMap Toggle", Float) = 0.0
        _NormalMap("NormalMap", 2D) = "bump" {}
        _NormalIntensity("NormalIntensity", Float) = 1.0
        
        [Space(10)]
        [Header(Emission)]
        [Space(10)]
        [Toggle(_EMISSION)] _Emission("EmissionMap Toggle", Float) = 0.0
        _EmissionMap("EmissionMap", 2D) = "white" {}
        [HDR]_EmissColor("Emission Color", Color) = (0,0,0,1)
        _EmissionStrength("EmissionStrength", Range(0.0, 1.0)) =1.0
        
        [Space(10)]
        [ToggleOff] _EnvironmentReflections("Environment Reflections", Float) = 1.0
        [Toggle(_CUSTOMREFLECTION)] _CustomReflection("Use Custom ReflectionProbe", Float) = 0.0
        _ReflectionCubeMap("Reflection CubeMap", Cube) = "white" {}
    }

    SubShader
    {
        Tags{"RenderType" = "Opaque" "Queue"="Geometry +100" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit" }
        Pass
        {
            Tags{"LightMode" = "UniversalForward"}
            Cull[_CullMode]
            HLSLPROGRAM
            #pragma target 3.0
            
            // -------------------------------------
            // Universal Pipeline keywords
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
            #pragma shader_feature_local _CUSTOMREFLECTION
            #pragma shader_feature_local _EMISSION
            #pragma shader_feature_local _BASEMAP
            #pragma shader_feature_local _NORMAL

            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Core.hlsl"
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/UnityInput.hlsl"
            #include "Assets/Resources/Shaders/PBR/FurnitureLighting.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                half2 texcoord     : TEXCOORD0;
                half2 staticLightmapUV : TEXCOORD1;
            };
            struct Varyings
            {
                half2 uv                       : TEXCOORD0;
                float3 positionWS               : TEXCOORD1;
                float3 normalWS                 : TEXCOORD2;
                half4 tangentWS                : TEXCOORD3;    
                half2 staticLightmapUV : TEXCOORD5;
                float4 positionCS               : SV_POSITION;
            };
            #if defined (_BASEMAP)
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            #endif
            #if defined (_NORMAL)
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            #endif
            #if defined (_EMISSION)
            TEXTURE2D(_EmissionMap);
            SAMPLER(sampler_EmissionMap);
            #endif
             #if defined (_CUSTOMREFLECTION)
            TEXTURECUBE(_ReflectionCubeMap);
            SAMPLER(sampler_ReflectionCubeMap);
             #endif
             CBUFFER_START(UnityPerMaterial)
                
            float4 _ReflectionCubeMap_HDR;
            half4 _BaseMap_ST;
            half4 _DetailAlbedoMap_ST;
            half4 _NormalMap_ST;
            half4 _BaseColor;
            half4 _SpecColor;
            half _Cutoff;
            half _Smoothness;
            half _Metallic;
            half _BumpScale;
            half _Parallax;
            half _OcclusionStrength;
            half _ClearCoatMask;
            half _ClearCoatSmoothness;
            half _DetailAlbedoMapScale;
            half _DetailNormalMapScale;
            half _Surface;
            half _EmissionStrength;
            half4 _EmissColor;
            half _NormalIntensity;
           
            CBUFFER_END  

            // Used in Standard (Physically Based) shader
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
                output.staticLightmapUV = input.staticLightmapUV * unity_LightmapST.xy + unity_LightmapST.zw;
                return output;
            }
            
            // Used in Standard (Physically Based) shader
            half4 LitPassFragment(Varyings input) : SV_Target
            {
                //Input data
                half2 UV  = input.uv;
                float3 positionWS = input.positionWS;
                half3 vDirWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
                
                //Material Keywords
                #if defined (_BASEMAP)
                    half4 BaseColorAlpha = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,UV* _BaseMap_ST.xy + _BaseMap_ST.zw) * _BaseColor;
                #else
                    half4 BaseColorAlpha = half4(1,1,1,1)* _BaseColor;
                #endif
                half3 nDirWS = normalize(input.normalWS);
                half3 tDirWS = normalize(input.tangentWS.xyz);
                half3 bDirWS = normalize(cross(nDirWS,tDirWS) * input.tangentWS.w);
                half3x3 TBN  = half3x3(tDirWS,bDirWS,nDirWS);
                #if defined (_NORMAL)
                    half3 nDirTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap,sampler_NormalMap,UV * _NormalMap_ST.xy + _NormalMap_ST.zw) ,_NormalIntensity) ;
                    nDirWS = normalize(mul(nDirTS,TBN));
                #else
                    nDirWS = normalize(input.normalWS);
                #endif
                half3 BaseColor = BaseColorAlpha.rgb;
                #if defined (_EMISSION)
                    half4 EmissonMap = SAMPLE_TEXTURE2D(_EmissionMap,sampler_EmissionMap,UV);
                #else
                    half4 EmissonMap = half4(1,1,1,1);
                #endif
                half4 EmissonColor = EmissonMap* _EmissionStrength * _EmissColor;
                half Metallic = _Metallic;
                half Roughness = saturate(1 - _Smoothness);
                half Occlusion = 1;
                //BRDF
                half3 DiffuseColor = lerp(BaseColor,float3(0.0,0.0,0.0),Metallic);
                half3 SpecularColor = lerp(float3(0.04,0.04,0.04),BaseColor,Metallic);
                Roughness = max(Roughness,0.001f);
                //IndirectLight
                half3 IndirectLighting = half3(0,0,0);
                 #if defined (_CUSTOMREFLECTION)
                half mip = PerceptualRoughnessToMipmapLevel(Roughness);
                half3 R = reflect(-vDirWS,nDirWS);
	            half4 encodedIrradiance = SAMPLE_TEXTURECUBE(_ReflectionCubeMap, sampler_ReflectionCubeMap, R);
                IndirectLighting_customReflection(DiffuseColor,SpecularColor,Roughness,positionWS,nDirWS,vDirWS,Occlusion,input.staticLightmapUV,encodedIrradiance,_ReflectionCubeMap_HDR,IndirectLighting);
                #else
                IndirectLighting_float(DiffuseColor,SpecularColor,Roughness,positionWS,nDirWS,vDirWS,Occlusion,input.staticLightmapUV,1,IndirectLighting);
                 #endif
                half4 color = half4(IndirectLighting +  EmissonColor,1);
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
            #include "Assets/Resources/Shaders/PBR/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitMetaPass.hlsl"
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
