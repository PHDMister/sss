Shader "URP/Furniture/UVanim"
{
    Properties
    {
        [Space(10)]
        [Enum(UnityEngine.Rendering.CullMode)]_CullMode("剔除模式", Float) = 2.0
        [Space(10)]
        _Number("数字", Range( 0 , 9)) = 0
        [Space(10)]
        [Main(PBRIntput, _, off, off)] _PBRIntput ("基础", Float) = 1
        [Space(10)]
        [Sub(PBRIntput)]_BaseColor("基础颜色", Color) = (1,1,1,1)
        [Sub(PBRIntput)]_BaseMap("基础贴图", 2D) = "white" {}
        [Sub(PBRIntput)]_Alpha("透明度", Range( 0 , 2)) = 0
        
        [Space(10)]
        [Main(Emission, _EMISSION, off)] _Emission ("发光", Float) = 0
        [Space(10)]
        [Sub(Emission)]_EmissionMap("发光贴图", 2D) = "white" {}
        [Sub(Emission)][HDR]_EmissColor("发光颜色", Color) = (0,0,0,1)
        [Sub(Emission)]_EmissionStrength("发光强度", Range(0.0, 1.0)) =1.0
        [Space(10)]
        _float_tooltip ("提示#无光照；不支持烘培光照；透明；发光 ; 渐变；UV偏移", float) = 1
       

    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue"="Transparent+320" "RenderPipeline" = "UniversalPipeline" }
        Pass
        {
           Tags{"LightMode" = "UniversalForward"}
            Cull[_CullMode]
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZWrite Off
            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
            #pragma shader_feature_local _EMISSION
            //#pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Core.hlsl"
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Lighting.hlsl"


           struct Attributes
            {
                float4 positionOS   : POSITION;
                half2 texcoord     : TEXCOORD0;
            };
            struct Varyings
            {
                half2 uv                       : TEXCOORD0;
                float3 positionWS               : TEXCOORD1;
                float3 positionOS               : TEXCOORD4;
                float4 positionCS               : SV_POSITION;
            };
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
             #if defined (_EMISSION)
                TEXTURE2D(_EmissionMap);
                SAMPLER(sampler_EmissionMap);
            #endif
            
            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half _Alpha;
                half _EmissionStrength;
                half4 _EmissColor;
                half4 _BaseMap_ST;
                half _Number;
            CBUFFER_END  
            

           Varyings LitPassVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.uv = input.texcoord;
                output.positionWS = vertexInput.positionWS;
                output.positionCS = vertexInput.positionCS;
                output.positionOS = input.positionOS;
                return output;
            }

            half4 LitPassFragment (Varyings input) : SV_Target
            {
                half2 uv  = input.uv;
                half4 BaseColorAlpha = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,uv * _BaseMap_ST.xy + half2(floor(_Number)*0.053,0)) * _BaseColor;
                #if defined (_EMISSION)
                    half4 EmissonMap = SAMPLE_TEXTURE2D(_EmissionMap,sampler_EmissionMap,uv * _BaseMap_ST.xy + half2(floor(_Number)*0.052,0));
                #else
                    half4 EmissonMap = half4(0,0,0,0);
                #endif
                half4 EmissonColor = EmissonMap* _EmissionStrength * _EmissColor;
                half alpha = BaseColorAlpha.a * _Alpha;
                half3 baseColor =BaseColorAlpha.rgb* _BaseColor + EmissonColor ;
                return half4(baseColor,alpha);
            }   
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor "LWGUI.LWGUI"
}
