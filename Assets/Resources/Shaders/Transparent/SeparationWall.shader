Shader "URP/Transparent/SeparationWall"
{
    Properties
    {
        [Space(10)]
        [Enum(UnityEngine.Rendering.CullMode)]_CullMode("剔除模式", Float) = 2.0
       [Space(10)]
        [Main(PBRIntput, _, off, off)] _PBRIntput ("基础", Float) = 1
        [Space(10)]
        [Sub(PBRIntput)][HDR]_BaseColor("基础颜色", Color) = (1,1,1,1)
        [Sub(PBRIntput)]_Alpha("透明度", Range( 0 , 1)) = 0
        [Sub(PBRIntput)]_dissolveAlpha("消融", Range( 0 , 1.5)) = 0
        [Space(10)]
        [Main(Mask,_, off, off)] _Mask ("遮罩", Float) = 0
        [Space(10)]
        [Sub(Mask)]_MaskPower("次幂", Float) = 0
        [Sub(Mask)]_MaskMultiplier("强度", Float) = 1
        [Space(10)]
        [Main(Noise,_, off, off)] _Noise ("噪声", Float) = 0
        [Space(10)]
        [Sub(Noise)]_NoiseMap("噪声图", 2D) = "white" {}
        [Sub(Noise)]_NoiseSpeed("速度", Vector) = (0.5, 0.2, 0, 0)
        [Space(10)]
        [Main(Border,_, off, off)] _Border ("边界", Float) = 0
        [Space(10)]
        [Sub(Border)]_BorderMap("Border Map", 2D) = "white" {}
        [Sub(Border)][HDR]_BorderColor("Border Color", Color) = (1,1,1,1)
        [Space(10)]
         _float_tooltip ("提示#无光照；不支持烘培光照；透明；发光 ;噪声移动", float) = 1

    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline" = "UniversalPipeline"}
        Pass
        {
           Tags{"LightMode" = "UniversalForward"}
            Cull[_CullMode]
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
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
            TEXTURE2D(_NoiseMap);
            SAMPLER(sampler_NoiseMap);
            TEXTURE2D(_BorderMap);
            SAMPLER(sampler_BorderMap);
            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half4 _BorderColor;
                half4 _NoiseMap_ST;
                half _Alpha;
                half _MaskPower;
                half _MaskMultiplier;
                half2 _NoiseSpeed;
                //half _DissolveAmount;
                //half _DissolveSpread;
            half _dissolveAlpha;
            CBUFFER_END  
           half2 Unity_PolarCoordinates(half2 UV, float2 Center, half RadialScale, half LengthScale)
            {
                float2 delta = UV - Center;
                float radius = length(delta) * 2 * RadialScale;
                float angle = atan2(delta.x, delta.y) * 1.0/6.28 * LengthScale;
                half2 Out = half2(radius, angle);
                return Out;
            } 

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
                //Mask
                half circleMask = pow(Unity_PolarCoordinates(uv,half2(0.5,0.5),1,1).x,_MaskPower) * _MaskMultiplier;
                //Noise
                half noise = SAMPLE_TEXTURE2D(_NoiseMap,sampler_NoiseMap,uv *_NoiseMap_ST.xy).r +0.1;
                half2 noiseSpeed = _NoiseSpeed * _Time.y;
                half2 flowUV = uv  *_NoiseMap_ST.xy+ noiseSpeed;
                half flowNoise = SAMPLE_TEXTURE2D(_NoiseMap,sampler_NoiseMap,flowUV).r;
                half2 minusFlowUV = uv  *_NoiseMap_ST.xy + (1 - noiseSpeed);
                half minusFlowNoise = pow(SAMPLE_TEXTURE2D(_NoiseMap,sampler_NoiseMap,minusFlowUV).r,2.35);
                half4 NoiseCombine = (flowNoise * minusFlowNoise + noise) * circleMask * _BaseColor;
                //Border
                half4 boder = SAMPLE_TEXTURE2D(_BorderMap,sampler_BorderMap,uv).a * _BorderColor;
                //half4 boder = circleMask * _BorderColor;
                //Dissolve
                //half dissolve = distance(half3(0,0,0),_DissolveAmount) * rcp(_DissolveSpread)
                half3 baseColor =( NoiseCombine+boder).rgb ;
                half alpha = ( NoiseCombine+boder).a;
                half grandint = lerp(1,0,uv.y);
                half finalAlpha = clamp(lerp(_Alpha,alpha,0.5) - _dissolveAlpha +grandint ,0,1);
                return half4(baseColor,finalAlpha);
            }   
            ENDHLSL
        }
    }
     FallBack "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor "LWGUI.LWGUI"
}