Shader "URP/Char/DissolveVC"
{
    Properties
    {
        [Header(Base  Color)]
        [Space(10)]
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        [MainColor] _BaseColor("Base Color", Color) = (1,1,1,1)
       
        
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
        _Smoothness("Smoothness", Range(-1.0, 1.0)) = 0.0
        [Space(10)]
        [ToggleOff] _EnvironmentReflections("Environment Reflections", Float) = 1.0
        
        [Header(Emission)]
        [Space(10)]
        [Toggle(_EMISSION)] _Emission("Emission", Float) = 0.0
        _EmissionMap("EmissionMap", 2D) = "white" {}
        [HDR]_EmissColor("Emission Color", Color) = (0,0,0,1)
        _EmissionStrength("EmissionStrength", Range(0.0, 1.0)) =1.0
        
         [Header(Dissolve)]
         [Space(10)]
         _DissolveAmount("DissolveAmount", Range( 0 , 1.5)) = 1.5
         _DissolveOffset("DissolveOffset", Float) = -4
		 _DissolveSpread("DissolveSpread", Float) = 1.5
         _DissloveEdgeOffset("DissloveEdgeOffset", Float) = 0.94
		 [HDR]_EdgeEmissColor("EdgeEmissColor", Color) = (16.9411774,17.9450989,23.9686279,1)
        
         [Header(Noise)]
         [Space(10)]
         _NoiseMap("Noise  Map", 2D) = "white" {}
         _NoiseScale("NoiseScale", Vector) = (1,1,1,0)
        
         [Header(VertexDissolve)]
         [Space(10)]
         _VertexDissolveSpread("VertexDissolveSpread", Float) = 1
		 _VertexDissolveOffset("VertexDissolveOffset", Float) = -4
		 _VertexOffsetIntensity("VertexOffsetIntensity", Float) = 1
        
         _CutOff("CutOff", Range(0.0, 1.0)) = 0.826
    }

    SubShader
    {
        Tags{"RenderType" = "Opaque"  "Queue"="AlphaTest" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit" }
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
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Lighting.hlsl"
            #include "Assets/Resources/Shaders/PBR/FurnitureLighting.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                half3 normalOS     : NORMAL;
                half4 tangentOS    : TANGENT;
                half2 texcoord     : TEXCOORD0;
                half4 vertexColor :  COLOR;
            };
            struct Varyings
            {
                half2 uv                       : TEXCOORD0;
                float3 positionWS               : TEXCOORD1;
                half3 normalWS                 : TEXCOORD2;
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
            TEXTURE2D(_NoiseMap);
            SAMPLER(sampler_NoiseMap);
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
                half _DissolveAmount;
                half _DissolveOffset;
                half _DissolveSpread;
                half _DissloveEdgeOffset;
                half _VertexDissolveSpread;
                half _VertexDissolveOffset;
                half _VertexOffsetIntensity;
                half4 _EdgeEmissColor;
                half4 _NoiseScale;
                half _CutOff;
                half _EmissionStrength;
                half4 _EmissColor;
                float _SpecShininess;
                half _SpecIntensity;
            CBUFFER_END  
            
            // Used in Standard (Physically Based) shader
            Varyings LitPassVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                output.uv = input.texcoord;
                output.normalWS = normalInput.normalWS;         
                real sign = input.tangentOS.w * GetOddNegativeScale();
                half4 tangentWS = half4(normalInput.tangentWS.xyz, sign);
                output.tangentWS = tangentWS;
                float4 postionOS =  input.positionOS;
                postionOS.x = postionOS.x *  input.vertexColor.r;
                postionOS.y = postionOS.y *  input.vertexColor.g;
                postionOS.z = postionOS.z *  input.vertexColor.b;
                
                float3 positionWS = mul(GetObjectToWorldMatrix(), postionOS).xyz;
                half3 vertexOffset = max(((positionWS.y + (-3.0 + ((1-_DissolveAmount) - 1.0) * (3.5) )) - _VertexDissolveOffset) * rcp(_VertexDissolveSpread) ,0);
                half4 vertexOffset1 = half4(half3(0,1,0) * _VertexOffsetIntensity * vertexOffset + positionWS,1);
                half3 vertexOffset2 = mul(GetWorldToObjectMatrix(),vertexOffset1).xyz - input.positionOS.xyz;
                input.positionOS.xyz  +=vertexOffset2 ;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
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
                half4 MRmap = SAMPLE_TEXTURE2D(_MetallicMap,sampler_MetallicMap,UV);
                half3 BaseColor = BaseColorAlpha.rgb * _BaseColor.rgb ;
                
                half Metallic = MRmap.g * _Metallic;
                half smoothness = (_Smoothness )+MRmap.a;
                half Roughness =saturate( 1 -smoothness );
                half3 nDirTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap,sampler_NormalMap,UV * _NormalMap_ST.xy + _NormalMap_ST.zw) ,_NormalIntensity) ;
                nDirWS = normalize(mul(nDirTS,TBN));
                half Occlusion = 1;
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
                
                //Dissolve
                float ObjToWorld = (1-(positionWS.y-mul( GetObjectToWorldMatrix(),float4(float3(0,0,0),1)).y)) - (-3.0 + ((1-_DissolveAmount) - 1.0) * (0.5 - -3.0) - _DissolveOffset) * rcp(_DissolveSpread);
                half dissolveStep = smoothstep(0.8,1.0,ObjToWorld);
                half dissoloveNoise = SAMPLE_TEXTURE2D(_NoiseMap,sampler_NoiseMap, positionWS * _NoiseScale.xyz).r;
                half dissolve = clamp((dissolveStep + (ObjToWorld - dissoloveNoise)),0.0,1.0);

                //EdgeColor
                half edge =pow((1- distance(ObjToWorld,_DissloveEdgeOffset)),3);
                half3 edgeColor = smoothstep(0.0,1.0,(edge - dissoloveNoise)) * _EdgeEmissColor.rgb;
                
               
                //----IndirectLight----
                half3 IndirectLighting = half3(0,0,0);
                IndirectLighting_float(DiffuseColor,SpecularColor,Roughness,positionWS,nDirWS,vDirWS,Occlusion,0,1,IndirectLighting);
                half4 color = half4(IndirectLighting +edgeColor+EmissonColor  ,dissolve);
                clip(color.a - _CutOff);
                return color;
               
            }
            ENDHLSL
        }

    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
