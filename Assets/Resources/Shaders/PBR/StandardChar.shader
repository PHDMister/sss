Shader "URP/StandardChar"
{
    Properties
    {
        [Header(Base  Color)]
        [Space(10)]
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        [MainColor] _BaseColor("Color", Color) = (1,1,1,1)
        [Header(Normal)]
        [Space(10)]
        _NormalMap("NormalMap", 2D) = "bump" {}
        _NormalIntensity("NormalIntensity", Float) = 1.0
        [Header(Metallic)]
        [Space(10)]
        _MetallicMap("Metallic  Map", 2D) = "white" {}
        _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        [Header(Smoothness)]
        [Space(10)]
        _Smoothness("Smoothness", Range(0.0, 1.0)) = 0.0
        
        [Header(Emission)]
        [Space(10)]
        [Toggle(_EMISSION)] _Emission("Emission", Float) = 0.0
        _EmissionMap("EmissionMap", 2D) = "white" {}
        [HDR]_EmissColor("Emission Color", Color) = (0,0,0,1)
        _EmissionStrength("EmissionStrength", Range(0.0, 1.0)) =1.0
        [Space(10)]
        [ToggleOff] _EnvironmentReflections("Environment Reflections", Float) = 1.0
    }

    SubShader
    {
        Tags{"RenderType" = "Opaque" "Queue"="AlphaTest-50" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit" }
        Pass
        {
            Tags{"LightMode" = "UniversalForward"}
            Cull Back
            HLSLPROGRAM
            #pragma target 3.0
            // -------------------------------------
            // Universal Pipeline keywords
            //#pragma multi_compile _ LIGHTMAP_ON
            #pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
            #pragma shader_feature_local _EMISSION
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Core.hlsl"
            //#include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Lighting.hlsl"
            #include "StandardLighting.hlsl"
            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                half2 texcoord     : TEXCOORD0;
            };
            struct Varyings
            {
                half2 uv                       : TEXCOORD0;
                float3 positionWS               : TEXCOORD1;
                float3 normalWS                 : TEXCOORD2;
                half4 tangentWS                : TEXCOORD3;    
                float4 shadowCoord              : TEXCOORD4;
                float4 positionCS               : SV_POSITION;
            };
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            TEXTURE2D(_MetallicMap);
            SAMPLER(sampler_MetallicMap);
            #if defined (_EMISSION)
            TEXTURE2D(_EmissionMap);
            SAMPLER(sampler_EmissionMap);
            #endif
            CBUFFER_START(UnityPerMaterial)
                half4 _NormalMap_ST;
                half4 _BaseColor;
                half _NormalIntensity;
                half _Metallic;
                half _Smoothness;
                half _EmissionStrength;
                half4 _EmissColor;
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
                output.shadowCoord = GetShadowCoord(vertexInput);
                output.positionCS = vertexInput.positionCS;
                return output;
            }
            
            // Used in Standard (Physically Based) shader
            half4 LitPassFragment(Varyings input) : SV_Target
            {
                //Input data
                half2 UV  = input.uv;
                float3 positionWS = input.positionWS;
                half3 vDirWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
                half3 nDirWS = normalize(input.normalWS);
                half3 tDirWS = normalize(input.tangentWS.xyz);
                half3 bDirWS = normalize(cross(nDirWS,tDirWS) * input.tangentWS.w);
                half3x3 TBN  = half3x3(tDirWS,bDirWS,nDirWS);
                //Material Keywords
                half4 BaseColorAlpha = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,UV) ;
                half3 BaseColor = BaseColorAlpha.rgb * _BaseColor.rgb;
                half4 MetallicSmoothness = SAMPLE_TEXTURE2D(_MetallicMap,sampler_MetallicMap,UV);
                half Metallic = saturate((MetallicSmoothness).g * _Metallic);
                half Roughness = 1 - (_Smoothness * MetallicSmoothness.a);
                half3 nDirTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap,sampler_NormalMap,UV * _NormalMap_ST.xy + _NormalMap_ST.zw) ,_NormalIntensity) ;
                nDirWS = normalize(mul(nDirTS,TBN));
                half Occlusion = 1;
                //Emiss
                #if defined (_EMISSION)
                    half4 EmissonMap = SAMPLE_TEXTURE2D(_EmissionMap,sampler_EmissionMap,UV);
                #else
                    half4 EmissonMap = half4(0,0,0,0);
                #endif
                half4 EmissonColor = EmissonMap* _EmissionStrength * _EmissColor;
                //BRDF
                half3 DiffuseColor = lerp(BaseColor,float3(0.0,0.0,0.0),Metallic);
                half3 SpecularColor = lerp(float3(0.04,0.04,0.04),BaseColor,Metallic);
                Roughness = max(Roughness,0.001f);
                //----DirectLight----
                //half3 DirectLighting = half3(0,0,0);
                //DirectLighting_float(DiffuseColor,SpecularColor,Roughness,positionWS,nDirWS,vDirWS,DirectLighting);
                //----IndirectLight----
                half3 IndirectLighting = half3(0,0,0);
                IndirectLighting_float(DiffuseColor,SpecularColor,Roughness,positionWS,nDirWS,vDirWS,Occlusion,0,IndirectLighting);
                half4 color = half4(IndirectLighting + EmissonColor ,1);
                return color;
            }
            ENDHLSL
        }
//         Pass
//        {
//            Name "Meta"
//            Tags{"LightMode" = "Meta"}
//
//            Cull Off
//
//            HLSLPROGRAM
//            #pragma target 3.0
//
//            #pragma vertex UniversalVertexMeta
//            #pragma fragment UniversalFragmentMetaLit
//            #pragma shader_feature EDITOR_VISUALIZATION
//            #pragma shader_feature_local_fragment _EMISSION
//            //#pragma shader_feature_local_fragment _ALPHATEST_ON
//            //#pragma shader_feature_local_fragment _SPECGLOSSMAP
//            
//            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
//            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
//
//            struct Attributes
//            {
//                float4 positionOS   : POSITION;
//                float3 normalOS     : NORMAL;
//                float2 uv0          : TEXCOORD0;
//                float2 uv1          : TEXCOORD1;
//                float2 uv2          : TEXCOORD2;
//                 float4 uv1ST   : TEXCOORD3;
//                float4 uv2ST   : TEXCOORD4;
//            };
//
//            struct Varyings
//            {
//                float4 positionCS   : SV_POSITION;
//                float2 uv           : TEXCOORD0;
//            #ifdef EDITOR_VISUALIZATION
//                float2 VizUV        : TEXCOORD1;
//                float4 LightCoord   : TEXCOORD2;
//            #endif
//            };
//
//            TEXTURE2D(_BaseMap);        SAMPLER(sampler_BaseMap);
//            //TEXTURE2D(_MetallicGlossMap);    SAMPLER(sampler_MetallicGlossMap);
//            //TEXTURE2D(_BumpMap);    SAMPLER(sampler_BumpMap);
//            TEXTURE2D(_EmissionMap); SAMPLER(sampler_EmissionMap);
//            CBUFFER_START(UnityPerMaterial)
//                half4 _NormalMap_ST;
//                half4 _BaseColor;
//                half _NormalIntensity;
//                half _Metallic;
//                half _Smoothness;
//            half _EmissionStrength;
//            half4 _EmissColor;
//            CBUFFER_END  
//        
//            Varyings UniversalVertexMeta(Attributes input)
//            {
//                Varyings output = (Varyings)0;
//                output.positionCS = MetaVertexPosition(input.positionOS, input.uv1, input.uv2,input.uv1ST,input.uv2ST);
//                output.uv = input.uv0;
//                return output;
//            }
//
//            half4 UniversalFragmentMetaLit(Varyings input) : SV_Target
//            {
//                //Material Keywords
//                half4 BaseColorAlpha = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,input.uv) * _BaseColor;
//                half3 BaseColor = BaseColorAlpha.rgb;
//                half BaseAlpha = BaseColorAlpha.a;
//                 // #if defined(_ALPHATEST_ON)
//                 //     clip(BaseAlpha - _Cutoff);
//                 // #endif
//                //half4 specGloss = SAMPLE_TEXTURE2D(_MetallicGlossMap,sampler_MetallicGlossMap,input.uv);
//                half Metallic =  _Metallic;
//                //BRDF
//                half3 DiffuseColor = lerp(BaseColor,half3(0.0,0.0,0.0),Metallic);
//                half3 Emission = half3(0,0,0);
//                #if defined(_EMISSION)
//                    Emission = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, input.uv).rgb * _EmissionColor * _EmissionStrength;;
//                #endif
//                MetaInput metaInput;
//                metaInput.Albedo = DiffuseColor;
//                metaInput.Emission = Emission;
//                return MetaFragment(metaInput);
//           }
//            ENDHLSL
//        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
