Shader "URP/Unlit/Base"
{
    Properties
    {
        [Space(10)]
        [Enum(UnityEngine.Rendering.CullMode)]_CullMode("剔除模式", Float) = 2.0
        [Space(10)]
        [Main(PBRIntput, _, off, off)] _PBRIntput ("基础", Float) = 1
        [MainTexture][Sub(PBRIntput)] _BaseMap("基础贴图", 2D) = "white" {}
        [MainColor][Sub(PBRIntput)] _BaseColor("基础颜色", Color) = (1,1,1,1)
        _float_tooltip ("提示#无光；不透明；基础贴图", float) = 1
    }

    SubShader
    {
        Tags{"RenderType" = "Opaque" "Queue"="Geometry" "RenderPipeline" = "UniversalPipeline" }
        Pass
        {
            Tags{"LightMode" = "UniversalForward"}
            Cull[_CullMode]
            HLSLPROGRAM
            #pragma target 3.0
            
            // -------------------------------------
            // Universal Pipeline keywords
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Core.hlsl"
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/UnityInput.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                half2 texcoord     : TEXCOORD0;
            };
            struct Varyings
            {
                half2 uv                       : TEXCOORD0;
                float3 positionWS               : TEXCOORD1;
                float4 positionCS               : SV_POSITION;
            };
           
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            CBUFFER_START(UnityPerMaterial)
            //float4 _ReflectionCubeMap_HDR;
            half4 _BaseMap_ST;
            half4 _EmissionMap_ST;
            half4 _DetailAlbedoMap_ST;
            half4 _NormalMap_ST;
            half4 _BaseColor;
            half4 _SpecColor;
            half4 _EmissColor;
            half _Cutoff;
            half _Smoothness;
            half _Metallic;
            half _BumpScale;
            half _Parallax;
            half _OcclusionStrength;
            half _ClearCoatMask;
            half _ClearCoatSmoothness;
            half _DetailAlbedoMapScale;
            half _DetailNormalMapScale;
            half _Surface;
            half _EmissionStrength;
            half _NormalIntensity;
            half _LightIntensitiy;
            CBUFFER_END  
             
            //half _clipOff;
            
            // Used in Standard (Physically Based) shader
            Varyings LitPassVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.uv = input.texcoord;
                output.positionWS = vertexInput.positionWS;
                output.positionCS = vertexInput.positionCS;
                return output;
            }
            
            // Used in Standard (Physically Based) shader
            half4 LitPassFragment(Varyings input) : SV_Target
            {
                //Input data
                half2 UV  = input.uv;
                half4 BaseColorAlpha = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,UV* _BaseMap_ST.xy + _BaseMap_ST.zw) ;
                half4 color = half4(BaseColorAlpha.rgb *_BaseColor.rgb ,BaseColorAlpha.a * _BaseColor.a);
                return color;
            }
             ENDHLSL
            }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor "LWGUI.LWGUI"
}
