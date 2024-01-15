Shader "URP/Fur/DogFur"
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
        [Space(10)]
        [ToggleOff] _EnvironmentReflections("Environment Reflections", Float) = 1.0
        _AlphaClipThreshold("Alpha Clip Threshold", Range( 0 , 1)) = 0.5
        
        [Header(Fur)]
        [Space(10)]
        _FurPattern("FurPattern", 2D) = "white" {}
        _FurLength("FurLength", Float) = 0.5
        _FurDirection("FurDirection", Vector) = (0,0,0,0)
        _EdgeFade("EdgeFade", Range( 0 , 3)) = 1
    }

    SubShader
    {
        Tags{"RenderType" = "Opaque" "Queue"="AlphaTest+100"  "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit"  }
        LOD 300

        // ------------------------------------------------------------------
        //  Forward pass. Shades all light in a single pass. GI + emission + Fog
        Pass
        {
            Tags{"LightMode" = "UniversalForward"}
            Cull Back
            Blend One Zero, One Zero
            ZWrite On
            ZTest LEqual
            HLSLPROGRAM
            #pragma target 3.0
            
            // -------------------------------------
            // Universal Pipeline keywords
            #pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Core.hlsl"
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Lighting.hlsl"
            #include "Assets/Resources/Shaders/PBR/StandardLighting.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                half3 normalOS     : NORMAL;
                half4 tangentOS    : TANGENT;
                half2 texcoord     : TEXCOORD0;
                half4 vertexColor        : COLOR;
            };
            struct Varyings
            {
                half2 uv                       : TEXCOORD0;
                float3 positionWS               : TEXCOORD1;
                half3 normalWS                 : TEXCOORD2;
                half4 tangentWS                : TEXCOORD3;    
                half furLayer                   :TEXCOORD4;
                float4 positionCS               : SV_POSITION;
            };
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            TEXTURE2D(_MetallicMap);
            SAMPLER(sampler_MetallicMap);
            TEXTURE2D(_FurPattern);
            SAMPLER(sampler_FurPattern);
            CBUFFER_START(UnityPerMaterial)
                half4 _FurPattern_ST;
                half4 _BaseColor;
                half2 _FurDirection;
                half _NormalIntensity;
                half _Metallic;
                half _Smoothness;
                half _FurLength;
                half _EdgeFade;
                half _AlphaClipThreshold;
            CBUFFER_END  
            
            // Used in Standard (Physically Based) shader
            Varyings LitPassVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                //furLayer
                half furLayer = input.vertexColor.r;
                half3 vertexOffset = (furLayer * _FurLength) * input.normalOS * 0.01;
                input.positionOS.xyz += vertexOffset;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                output.uv = input.texcoord;
                output.normalWS = normalInput.normalWS;         
                real sign = input.tangentOS.w * GetOddNegativeScale();
                half4 tangentWS = half4(normalInput.tangentWS.xyz, sign);
                output.tangentWS = tangentWS;
                output.positionWS = vertexInput.positionWS;
                output.positionCS = vertexInput.positionCS;
                output.furLayer = furLayer;
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
                half Metallic = saturate(SAMPLE_TEXTURE2D(_MetallicMap,sampler_MetallicMap,UV).r * _Metallic);
                half Roughness = 1 - _Smoothness;
                half3 nDirTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap,sampler_NormalMap,UV ) ,_NormalIntensity) ;
                nDirWS = normalize(mul(nDirTS,TBN));
                half Occlusion = 1;
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
                //furWind
                // half2 furWindUV = UV + (_Time.y * _FurWindDirection * 0.01);
                // half2 furFlowMap = (SAMPLE_TEXTURE2D(_FlurFlowMap,sampler_FlurFlowMap,furWindUV).rg * 2 - 1 ) * _FurWindIntensity * input.furLayer ;
                // half2 furGravity = input.furLayer * input.furLayer * _FurDirection;
                //fur
                half2 furUV =  UV;
                half furPattern = SAMPLE_TEXTURE2D(_FurPattern,sampler_FurPattern,furUV * _FurPattern_ST.xy + _FurPattern_ST.zw).r * 2.0 ;
                half furLayerDouble = input.furLayer * input.furLayer * _EdgeFade;
                half furAplha =clamp(lerp((furPattern - furLayerDouble) , 1 ,step(input.furLayer,0.01) ),0,1.0);
               
                half4 color = half4(IndirectLighting ,furAplha);
                clip(furAplha - _AlphaClipThreshold);
               
                return color;
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
