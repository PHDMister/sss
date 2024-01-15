Shader "URP/VFX/AirWall"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.CullMode)]_CullMode("剔除模式", Float) = 2.0
        [Header(Bace Color)]
        [Space(10)]
        _BaseMap("Base Map", 2D) = "white" {}
        //_MulMap("Mul Map", 2D) = "white" {}
        [HDR]_BaseColor("Base Color", Color) = (1,1,1,1)
        _Speed("Flow速度", Float) = 0
        _Alpha("Alpha", Range( 0 , 1)) = 0
        //_playPos("playPos",Vector) = (1,1,1,0)
        _EgdeRange("EgdeRange",Vector) = (2,6,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline" = "UniversalPipeline"}
        Pass
        {
            Tags{"LightMode" = "UniversalForward"}
            Cull [_CullMode]
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
               // float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                float2 texcoord     : TEXCOORD0;
            };
            struct Varyings
            {
                float2 uv                       : TEXCOORD0;
                float3 positionWS               : TEXCOORD1;
                //float3 normalWS                 : TEXCOORD2;
                half4 tangentWS                : TEXCOORD3;
                //float3 positionOS               : TEXCOORD4;
                float4 positionCS               : SV_POSITION;
            };
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_MulMap);
            SAMPLER(sampler_MulMap);
           
            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
            half4 _BaseMap_ST;
            half4 _MulMap_ST;
                half _Alpha;
                float4 _playPos;
                float4 _EgdeRange;
            half _Speed;
            CBUFFER_END  

           Varyings LitPassVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                //VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                output.uv = input.texcoord;
                //output.normalWS = normalInput.normalWS;         
                output.positionWS = vertexInput.positionWS;
                output.positionCS = vertexInput.positionCS;
                 //output.positionOS = input.positionOS;
                return output;
            }

            half4 LitPassFragment (Varyings input) : SV_Target
            {
                half2 uv  = input.uv;
                float3 positionWS = input.positionWS;
                float3 playerPos = float3(_playPos.x,_playPos.y + 3,_playPos.z);
                float dis = abs(distance(positionWS,playerPos.xyz));
                half4 baseMap = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,(uv * _BaseMap_ST.xy + _BaseMap_ST.zw)+ _Speed* _Time.y);
                half MulMap = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,uv+ _Speed* _Time.y).g;
                baseMap.a = baseMap.r;
                baseMap.a *= 1 - smoothstep(_EgdeRange.x,_EgdeRange.y,dis);
                return half4( _BaseColor.xyz,baseMap.a *MulMap * _Alpha);
            }   
            ENDHLSL
        }
    }
     FallBack "Hidden/Universal Render Pipeline/FallbackError"
}