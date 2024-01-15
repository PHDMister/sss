Shader "URP/Transparent/Dissolve"
{
    Properties
    {
        [MainTexture] _BaseMap("Albedo", 2D) = "white" {}
        [MainColor] _BaseColor("Color", Color) = (1,1,1,1)
        _Alpha("_Alpha",Range(0.0, 1.5)) = 1.0
        
        _NormalMap("NormalMap", 2D) = "bump" {}
        _NormalIntensity("NormalIntensity", float) = 1.0
        
        _RoughnessMap("RoughnessMap", 2D) = "white" {}
        _Roughness("Roughness", Range(0.0, 1.0)) = 1.0
        
         [Space(10)]
        [Header(Emission)]
        _EmissionMap("EmissionMap", 2D) = "white" {}
        _EmissionStrength("EmissionStrength", Range(0.0, 1.0)) = 1.0
        
         [Space(10)]
         [Header(Dissolve)]
        _DissolveAmount("DissolveAmount", Range( 0 , 1.5)) = 1.5
         _DissolveOffset("DissolveOffset", Float) = 0
		 _DissolveSpread("DissolveSpread", Float) = 0
         _DissloveEdgeOffset("DissloveEdgeOffset", Float) = 0
		 [HDR]_EdgeEmissColor("EdgeEmissColor", Color) = (0.003114995,0.6603774,0.6381032,0)
        
         [Space(10)]
         [Header(Noise)]
        _NoiseMap("Noise  Map", 2D) = "white" {}
         _NoiseScale("NoiseScale", Vector) = (1,1,1,0)
        
         [Space(10)]
         [Header(VertexDissolve)]
         _VertexDissolveSpread("VertexDissolveSpread", Float) = 3
		 _VertexDissolveOffset("VertexDissolveOffset", Float) = -1
		 _VertexOffsetIntensity("VertexOffsetIntensity", Float) = 45
        
        _CutOff("CutOff", Range(0.0, 1.0)) = 0.7
    }

    SubShader
    {
        Tags{"RenderType" = "Transparent" "Queue"="Transparent+350" "RenderPipeline" = "UniversalPipeline" }
        
        Pass
        {
            Tags{"LightMode" = "UniversalForward"}
            Cull Back
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma target 3.0
            // -------------------------------------
            // Universal Pipeline keywords
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
               
            };
            struct Varyings
            {
                half2 uv                       : TEXCOORD0;
                float3 positionWS               : TEXCOORD1;
                half3 normalWS                 : TEXCOORD2;
                half4 tangentWS                : TEXCOORD3;   
                float4 positionCS               : SV_POSITION;
                
            };
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_RoughnessMap);
            SAMPLER(sampler_RoughnessMap);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            TEXTURE2D(_EmissionMap);
            SAMPLER(sampler_EmissionMap);
            TEXTURE2D(_NoiseMap);
            SAMPLER(sampler_NoiseMap);
            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half _Roughness;
                half _NormalIntensity;
                half _Alpha;
                half _EmissionStrength;
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
                float3 positionWS = mul(GetObjectToWorldMatrix(), input.positionOS).xyz;
                half3 vertexOffset = max(((positionWS.y + (-3.0 + ((1-_DissolveAmount) - 1.0) * (0.5 - -3.0) )) - _VertexDissolveOffset) / _VertexDissolveSpread,0);
                half4 vertexOffset1 = half4(half3(0,1,0) * _VertexOffsetIntensity * vertexOffset + positionWS,1);
                half3 vertexOffset2 = mul(GetWorldToObjectMatrix(),vertexOffset1).xyz - input.positionOS.xyz;
                input.positionOS.xyz  +=vertexOffset2 ;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionWS = vertexInput.positionWS;
                output.positionCS = vertexInput.positionCS;
                return output;
            }
            
            // Used in Standard (Physically Based) shader
            half4 LitPassFragment(Varyings input) : SV_Target
            {
                //输入数据
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
                
                half4 EmissonColor = SAMPLE_TEXTURE2D(_EmissionMap,sampler_EmissionMap,UV) * _EmissionStrength;
                half Metallic = 0.2;
                half Roughness = saturate(SAMPLE_TEXTURE2D(_RoughnessMap,sampler_RoughnessMap,UV).r * _Roughness);
                half3 nDirTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap,sampler_NormalMap,UV) * _NormalIntensity,_NormalIntensity);
                nDirWS = normalize(mul(nDirTS,TBN));
                half Occlusion = 1;
                //BRDF
                float3 DiffuseColor = lerp(BaseColor,float3(0.0,0.0,0.0),Metallic);
                float3 SpecularColor = lerp(float3(0.04,0.04,0.04),BaseColor,Metallic);
                Roughness = max(Roughness,0.001f);
                 //Dissolve
                float ObjToWorld = (1-(positionWS.y-mul( GetObjectToWorldMatrix(),float4(float3(0,0,0),1)).y)) - (-3.0 + ((1-_DissolveAmount) - 1.0) * (0.5 - -3.0) - _DissolveOffset)/_DissolveSpread;
                half dissolveStep = smoothstep(0.8,1.0,ObjToWorld);
                half dissoloveNoise = SAMPLE_TEXTURE2D(_NoiseMap,sampler_NoiseMap, positionWS * half3(2,2,2)).r;
                half dissolve = clamp((dissolveStep + (ObjToWorld - dissoloveNoise)),0.0,1.0);
                //EdgeColor
                half edge =pow((1- distance(ObjToWorld,_DissloveEdgeOffset)),5);
                half3 edgeColor = smoothstep(0.0,1.0,(edge - dissoloveNoise)) * _EdgeEmissColor.rgb;
                
                //----IndirectLight----
                half3 IndirectLighting = half3(0,0,0);
                IndirectLighting_float(DiffuseColor,SpecularColor,Roughness,positionWS,nDirWS,vDirWS,Occlusion,0,1,IndirectLighting);
                half BaseAlpha = clamp(_BaseColor.a * _Alpha * dissolve,0.0,1.0);
                half4 color = half4(IndirectLighting +edgeColor + EmissonColor,BaseAlpha);
                clip(color.a - _CutOff);
                return color;
            }
            ENDHLSL
        }

    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
