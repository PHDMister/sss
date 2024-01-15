Shader "URP/Char/SSS"
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
        _NormalIntensity("NormalIntensity", float) = 1.0
        [Header(Metallic)]
        [Space(10)]
        _MetallicMap("Metallic  Map", 2D) = "black" {}
        _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        [Header(Smoothness)]
        [Space(10)]
        _Smoothness("Smoothness", Range(0.0, 1.0)) = 0.0
        [Header(sss)]
        [Space(10)]
        _LutOffset("LutOffset",Range(0, 1)) = 0
        _LutMap  ("LutMap", 2D) = "white" {}
        _sssPower("sssPower",Float) = 1
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
        Tags{"RenderType" = "Opaque"  "Queue"="AlphaTest-10"   "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit"}
        Pass
        {
            Tags{"LightMode" = "UniversalForward"}
            Cull Back
            
            HLSLPROGRAM
            #pragma target 3.0
            
            // -------------------------------------
            // Universal Pipeline keywords
            #pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
            #pragma shader_feature_local _EMISSION
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Core.hlsl"
            #include "Assets/Resources/Shaders/PBR/StandardLighting.hlsl"

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
            TEXTURE2D(_LutMap);
            SAMPLER(sampler_LutMap);
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
                half _LutOffset;
                half _sssPower;
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
                half4 BaseColorAlpha = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,UV) * _BaseColor;
                half3 BaseColor = BaseColorAlpha.rgb * _BaseColor.rgb;
                half4 MetallicSmoothness = SAMPLE_TEXTURE2D(_MetallicMap,sampler_MetallicMap,UV);
                half Metallic = saturate((MetallicSmoothness).r * _Metallic);
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
                //SSS
                half2 lutuv = half2(0,0);
                SSSuv_float(DiffuseColor,nDirWS,positionWS,Occlusion,_LutOffset,_sssPower,lutuv);
                half3 lutMap = SAMPLE_TEXTURE2D(_LutMap,sampler_LutMap, lutuv) * 0.5 +0.5;;
                //----IndirectLight----
                half3 IndirectLighting = half3(0,0,0);
                IndirectLightingSSS_float(DiffuseColor,SpecularColor,Roughness,positionWS,nDirWS,vDirWS,Occlusion,0,lutMap,MetallicSmoothness,IndirectLighting);
                half4 color = half4(IndirectLighting + EmissonColor ,1);
                return color;
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
