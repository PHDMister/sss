Shader "URP/Transparent/Bean"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _Lerp("Lerp", Float) = 0
        [Enum(UnityEngine.Rendering.CullMode)]_CullMode("CullMode", Float) = 0
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent+180"}
        Pass
        {
           Tags{"LightMode" = "UniversalForward"}
            Cull [_CullMode]
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZWrite Off
            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Core.hlsl"
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Lighting.hlsl"
           struct Attributes
            {
                float4 positionOS   : POSITION;
            };
            struct Varyings
            {
                float3 positionWS               : TEXCOORD1;
                float3 positionOS               : TEXCOORD4;
                float4 positionCS               : SV_POSITION;
            };
            
           
            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half _Lerp;
            CBUFFER_END  
            

           Varyings LitPassVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionWS = vertexInput.positionWS;
                output.positionCS = vertexInput.positionCS;
                output.positionOS = input.positionOS;
                return output;
            }

            half4 LitPassFragment (Varyings input) : SV_Target
            {
                float3 positionWS = input.positionWS;
                half obToWorld = positionWS.y - mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).y - _Lerp;
                half3 baseColor = _BaseColor.rgb;
                half alpha = lerp(0.8,0,obToWorld);
                return half4(baseColor,alpha);
            }   
            ENDHLSL
        }
    }
     FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
