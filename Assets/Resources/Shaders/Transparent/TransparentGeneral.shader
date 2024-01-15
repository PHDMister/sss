Shader "URP/Transparent/General"
{
    Properties
    {
        [MainTexture] _BaseMap("Albedo", 2D) = "white" {}
        [MainColor] _BaseColor("Color", Color) = (1,1,1,1)
        _Alpha("_Alpha",Range(0.0, 1.5)) = 1.0
        [Space(10)]
        [Header(Normal)]
        _NormalMap("NormalMap", 2D) = "bump" {}
        _NormalIntensity("NormalIntensity", float) = 1.0
        [Space(10)]
        [Header(MS)]
        _RoughnessMap("RoughnessMap", 2D) = "white" {}
        _Roughness("Roughness", Range(0.0, 1.0)) = 1.0
        
        [Space(10)]
        [Header(Emission)]
        [Space(10)]
        [Toggle(_EMISSION)] _Emission("EmissionMap Toggle", Float) = 0.0
        _EmissionMap("EmissionMap", 2D) = "white" {}
        _EmissionStrength("EmissionStrength", Range(0.0, 1.0)) = 1.0
        [Space(10)]
        [ToggleOff] _EnvironmentReflections("Environment Reflections", Float) = 0.0
    }
    SubShader
    {
        Tags{"RenderType" = "Transparent" "Queue"="Transparent+300" "RenderPipeline" = "UniversalPipeline" }
        LOD 300
        Pass
        {
            Tags{"LightMode" = "UniversalForward"}
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            HLSLPROGRAM
            #pragma target 3.0
            
            // -------------------------------------
            // Universal Pipeline keywords
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
            // #pragma shader_feature_local _EMISSION
            
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
                float4 positionCS               : SV_POSITION;
                half2 staticLightmapUV : TEXCOORD5;
            };
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_RoughnessMap);
            SAMPLER(sampler_RoughnessMap);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            TEXTURE2D(_EmissionMap);
            SAMPLER(sampler_EmissionMap);
            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half _Roughness;
                half _OcclusionStrength;
                half _NormalIntensity;
                half _Alpha;
                half _EmissionStrength;
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
                UNITY_SETUP_INSTANCE_ID(input);
                //input data
                half2 UV  = input.uv;
                float3 positionWS = input.positionWS;
                half3 vDirWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
                half3 nDirWS = normalize(input.normalWS);
                half3 tDirWS = normalize(input.tangentWS.xyz);
                half3 bDirWS = normalize(cross(nDirWS,tDirWS) * input.tangentWS.w);
                half3x3 TBN  = half3x3(tDirWS,bDirWS,nDirWS);
               
                //Material Keywords
                half4 BaseColorAlpha = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,UV) ;
                half3 BaseColor = BaseColorAlpha.rgb *  _BaseColor;
                half BaseAlpha = BaseColorAlpha.a * _Alpha;
                half4 EmissonColor = SAMPLE_TEXTURE2D(_EmissionMap,sampler_EmissionMap,UV) * _EmissionStrength;
                half Metallic = 0;
                half Roughness = saturate(SAMPLE_TEXTURE2D(_RoughnessMap,sampler_RoughnessMap,UV).r * _Roughness);
                half3 nDirTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap,sampler_NormalMap,UV) * _NormalIntensity,_NormalIntensity);
                nDirWS = normalize(mul(nDirTS,TBN));
                half Occlusion = 1;
                //BRDF
                float3 DiffuseColor = lerp(BaseColor,float3(0.0,0.0,0.0),Metallic);
                float3 SpecularColor = lerp(float3(0.04,0.04,0.04),BaseColor,Metallic);
                Roughness = max(Roughness,0.001f);
                //----IndirectLight----
                half3 IndirectLighting = half3(0,0,0);
                IndirectLighting_float(DiffuseColor,SpecularColor,Roughness,positionWS,nDirWS,vDirWS,Occlusion,input.staticLightmapUV,1,IndirectLighting);
                half4 color = half4(IndirectLighting +  EmissonColor,BaseAlpha);
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
