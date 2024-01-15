Shader "URP/Furniture/KKAniso"
{
    Properties
    {
        [Header(Bace Color)]
        [Space(10)]
        _BaseMap("Base Map", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1,1,1,1)
        [Header(Specular)]
        [Space(10)]
        _Shininess("Specular",Range(0.01,100)) = 1.0
        _SpecIntensity("SpecIntensity",Range(0.01,5)) = 1.0
        [Header(Shift)]
        [Space(10)]
        _ShiftMap ("_ShiftMap", 2D) = "white" {}
        _ShiftOffset("_ShiftOffset",Range(-1,1)) = 0.0
        _NoiseIntensity("_ShiftIntensity",Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry +280" "RenderPipeline" = "UniversalPipeline"}

        Pass
        {
            Cull Back
             HLSLPROGRAM
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
            #pragma target 3.0
             #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Core.hlsl"
             #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                half2 texcoord : TEXCOORD0;
                float3 normalOS :NORMAL;
                float4 tangentOS : TANGENT;
            };

            struct Varyings
            {
                half2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 tangentWS : TEXCOORD2;
                float3 bDir : TEXCOORD3;
                float3 positionWS : TEXCOORD4;
                float4 positionCS : SV_POSITION;

            };
             TEXTURE2D(_BaseMap);
             SAMPLER(sampler_BaseMap);
             TEXTURE2D(_ShiftMap);
             SAMPLER(sampler_ShiftMap);
             CBUFFER_START(UnityPerMaterial)
                 half4 _BaseColor;
                 half4 _ShiftMap_ST;
                 half _Shininess;
                 half _SpecIntensity;
                 half _ShiftOffset;
                 half _NoiseIntensity;
             CBUFFER_END  
           

            Varyings LitPassVertex (Attributes input)
            {
                Varyings output = (Varyings)0;
                 VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                output.positionCS = vertexInput.positionCS;
                output.uv = input.texcoord;
                output.normalWS = normalInput.normalWS;         
                 real sign = input.tangentOS.w * GetOddNegativeScale();
                half4 tangentWS = half4(normalInput.tangentWS.xyz, sign);
                output.tangentWS = tangentWS;
                output.bDir = normalize(cross(output.normalWS,output.tangentWS)) * sign;
                output.positionWS = vertexInput.positionWS;
                return output;
            }

            half4 LitPassFragment (Varyings input) : SV_Target
            {
                half3 vDir = GetWorldSpaceNormalizeViewDir(input.positionWS);
                float4 ShadowCoord = TransformWorldToShadowCoord(input.positionWS);
                float4 ShadowMask = float4(1.0,1.0,1.0,1.0);
                Light light = GetMainLight(ShadowCoord,input.positionWS,ShadowMask);
    		    half3 lDir= light.direction;
                half3 nDir = normalize(input.normalWS);
                half3 tDir = normalize(input.tangentWS.xyz); 
                half3 bDir = normalize(input.bDir);
                half3 hDir = normalize(lDir + vDir);
                
                half2 anisoDir = 1 *2.0 -0.5 ;
                bDir = normalize(tDir * anisoDir.r + bDir *anisoDir.g);
                half2 uvShift = input.uv * _ShiftMap_ST.xy +_ShiftMap_ST.zw;
                half shiftnosie = SAMPLE_TEXTURE2D(_ShiftMap,sampler_ShiftMap,uvShift).r; 
                shiftnosie = (shiftnosie * 2.0 - 1.0) * _NoiseIntensity;
                half3 bOffset = nDir *(_ShiftOffset + shiftnosie);
                bDir = normalize(bDir +bOffset);
                half TdotH = dot(bDir,hDir);//unity bdir UE4 
                half sinTH =sqrt(1 - TdotH * TdotH);
                half3 specColor = pow(max(0.0,sinTH),_Shininess)*_SpecIntensity;
                half3  col = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap, input.uv).rgb *_BaseColor.rgb  + specColor;
                return half4(col,1.0);
            }
             ENDHLSL
        }
    }
     FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
