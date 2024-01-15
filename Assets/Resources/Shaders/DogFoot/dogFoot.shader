Shader "URP/DogFoot"
{
    Properties
    {
        [Header(Bace Color)]
        [Space(10)]
        _BaseMap("Base Map", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _Alpha("Alpha", Range( 0 , 1)) = 0
        _ColorIntensity("ColorIntensity", Float) = 2
    }
    SubShader
    {
        Tags {  "RenderPipeline" = "UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent"}
        Pass
        {
           //Tags{"LightMode" = "UniversalForward"}
            Cull Back
             Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZWrite Off
            AlphaToMask Off
            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
            //#pragma multi_compile _ LIGHTMAP_ON
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Core.hlsl"
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Lighting.hlsl"
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
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half _Alpha;
                half _ColorIntensity;
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
                 //float3 positionWS = input.positionWS;
                // float3 postionOS = input.positionOS;
                //  half3 vDirWS = GetWorldSpaceNormalizeViewDir(positionWS);
                //  half3 nDirWS = normalize(input.normalWS);
                half4 baseMap = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,uv);
                half3 baseColor = baseMap.rgb;
                half alpha = baseMap.a;
                half3 finalColor =  _BaseColor.rgb * _ColorIntensity ;
                half finalAlpha = alpha * _Alpha;
                return half4(finalColor,finalAlpha);
            }   
            ENDHLSL
        }
    }
    // FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
