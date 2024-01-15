Shader "URP/Water/ParticlesWater"
{
    Properties
    {
        [Main(Base, _, off, off)]_Base ("Base", Float) = 1.0
        [Space(10)]
        [Sub(Base)]_BaseMap("Base Map", 2D) = "white" {}
        [Sub(Base)]_DirectionStrength("DirectionStrength",Float) = 3
        
        [Space(10)]
        [Main(Noise, _, off, off)] _Noise ("Noise", Float) = 1.0
        [Space(10)]
        [Sub(Noise)]_NoiseSpeed("速度(XY)", Vector) = (-8,0,0,0)
        [Sub(Noise)]_VectorNoise("方向图", 2D) = "white" {}
        [Sub(Noise)]_NoiseIntensity("强度", Float) = 0
        
        [Space(10)]
        [Main(Alpha, _, off, off)] Alpha ("Alpha", Float) = 1.0
        [Space(10)]
        [Sub(Alpha)]_AlphaMin("最小值", Float) = 0.167
        [Sub(Alpha)]_AlphaMax("最大值", Float) = 1
        
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline" = "UniversalPipeline"}
        Pass
        {
           Tags{"LightMode" = "UniversalForward"}
            Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest LEqual
            Cull Off
           HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Core.hlsl"
           struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                float2 texcoord     : TEXCOORD0;
                half4 vetextColor   :COLOR;
                
            };
            struct Varyings
            {
                float2 uv                       : TEXCOORD0;
                float3 positionWS               : TEXCOORD1;
                float3 normalWS                 : TEXCOORD2;
                half4 tangentWS                : TEXCOORD3;
                float3 positionOS               : TEXCOORD4;
                half4 vetextColor   :TEXCOORD5;
                float4 positionCS               : SV_POSITION;
            };
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
           TEXTURE2D(_VectorNoise);
            SAMPLER(sampler_VectorNoise);
           
            CBUFFER_START(UnityPerMaterial)
                half2 _NoiseSpeed;
                half _NoiseIntensity;
                half _AlphaMin;
                half _AlphaMax;
                half _DirectionStrength;
           half4 _VectorNoise_ST;
               
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
                output.vetextColor = input.vetextColor;
                return output;
            }

            half4 LitPassFragment (Varyings input) : SV_Target
            {
                half2 uv  = input.uv;
                //Noise
                half2 noiseUV = (uv * _VectorNoise_ST.xy + _VectorNoise_ST.zw) * _NoiseSpeed * _Time.y;
                half4 VectorNoise = SAMPLE_TEXTURE2D(_VectorNoise,sampler_VectorNoise,noiseUV);
                half2 noise = (VectorNoise.rg * 2 - 1) * _NoiseIntensity;
                
                half4 baseMap = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,uv);
                float3 baseColor =float3( clamp((baseMap.rg * 2 - 1 )*_DirectionStrength, half2(-1,-1),half2(1,1)) * 0.5 + 0.5 + noise,0);

                half alpha =saturate(baseMap.a - _AlphaMin ) / (( _AlphaMax - _AlphaMin ) +0.01) * input.vetextColor.a;
                
                half3 finalColor = baseColor  ;
                half finalAlpha = alpha;
                return half4(finalColor,finalAlpha);
            }   
            ENDHLSL
        }
    }
     FallBack "Hidden/Universal Render Pipeline/FallbackError"
     CustomEditor "LWGUI.LWGUI"
}