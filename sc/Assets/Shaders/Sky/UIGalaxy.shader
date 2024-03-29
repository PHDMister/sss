Shader "UI/Galaxy"
{
    Properties
    {
        [Space(10)]
        [Enum(UnityEngine.Rendering.CullMode)]_CullMode("剔除模式", Float) = 2.0
        [Space(10)]
        [Main(Base, _BASE, off,off)] _Base ("基础", Float) = 0
        [Space(10)]
        [Sub(Base)][MainTexture] _BaseMap("基础贴图", 2D) = "black" {}
        [Sub(Base)][HDR]_BaseColor("基础颜色", Color) = (1,1,1,1)
        
        [Space(10)]
        [Main(Star, _STAR, off,off)] _Star ("星空", Float) = 0
        [Space(10)]
        [Sub(Star)]_StarTex("星星贴图",2D) = "back" {}
        [Sub(Star)]_StarsSpeed ("星星速度（XY)", Vector) = (0,0,0,0)
        [Sub(Star)]_NoiseTex("噪声图",2D) = "back" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline" = "UniversalPipeline"}
        Pass
        {
           Tags{"LightMode" = "Universal2D"}
            Cull [_CullMode]
            ZWrite Off
            
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
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_NoiseTex);
             SAMPLER(sampler_NoiseTex);
            TEXTURE2D(_StarTex);
            SAMPLER(sampler_StarTex);
            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half4 _BaseMap_ST;
                float4 _StarTex_ST;
                half2 _StarsSpeed;
               
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
                half4 BaseColorAlpha = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,uv* _BaseMap_ST.xy + _BaseMap_ST.zw) ;
                //star
                float2 StarsOffset = _Time.y * _StarsSpeed * 0.01;
                half2 starUV = uv + StarsOffset;
                half noise =saturate( pow(SAMPLE_TEXTURE2D(_NoiseTex,sampler_NoiseTex,starUV).r,2));
                half3 starTex = SAMPLE_TEXTURE2D(_StarTex,sampler_StarTex,uv * _StarTex_ST.xy + _StarTex_ST.zw);
                half starlerp = lerp(0,starTex.r * noise ,uv.y);
                half3 baseColor = _BaseColor.rgb * starlerp + BaseColorAlpha.rgb;
                half alpha = 1;
                return half4(baseColor,alpha);
            }   
            ENDHLSL
        }
    }
     FallBack "Hidden/Universal Render Pipeline/FallbackError"
     CustomEditor "LWGUI.LWGUI"
}