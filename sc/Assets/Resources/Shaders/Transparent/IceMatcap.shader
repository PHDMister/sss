Shader "URP/VFX/Matcap"
{
    Properties
    {
        [Header(Bace Color)]
        [Space(10)]
        _BaseMap("Base Map", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1,1,1,1)
        [Header(Edge)]
        [Space(10)]
        _Min("Edge Min", Float) = 0
		_Max("Edge Max", Float) = 1
        [Header(Refraction)]
        [Space(10)]
        _Matcap("Matcap", 2D) = "white" {}
        _RefracMatcap("Refraction Matcap", 2D) = "white" {}
        _Refintensity("Refraction Intensity", Float) = 1
    }
    SubShader
    {
        Tags{"RenderType" = "Transparent" "Queue"="Transparent+50" "RenderPipeline" = "UniversalPipeline" }
        Pass
        {
            Tags{"LightMode" = "UniversalForward"}
            Cull Back
            Blend SrcAlpha OneMinusSrcAlpha
            HLSLPROGRAM
            #pragma target 3.0
            //#pragma multi_compile _ LIGHTMAP_ON
            //#pragma multi_compile _ DYNAMICLIGHTMAP_ON
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Core.hlsl"
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Lighting.hlsl"
            
           struct Attributes
            {
                float4 positionOS   : POSITION;
                half3 normalOS     : NORMAL;
                half4 tangentOS    : TANGENT;
                half2 texcoord     : TEXCOORD0;
                //float2 staticLightmapUV : TEXCOORD1;
            };
            struct Varyings
            {
                half2 uv                       : TEXCOORD0;
                float3 positionWS               : TEXCOORD1;
                half3 normalWS                 : TEXCOORD2;
                half4 tangentWS                : TEXCOORD3;    
                //float2 staticLightmapUV : TEXCOORD5;
                float4 positionCS               : SV_POSITION;
            };
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_Matcap);
            SAMPLER(sampler_Matcap);
            TEXTURE2D(_RefracMatcap);
            SAMPLER(sampler_RefracMatcap);
            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half _Min;
                half _Max;
                half _Refintensity;
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
                //output.staticLightmapUV = input.staticLightmapUV * unity_LightmapST.xy + unity_LightmapST.zw;
                return output;
            }
            half4 LitPassFragment (Varyings input) : SV_Target
            {
                half2 uv  = input.uv;
                //mapcapUV
                float3 positionWS = input.positionWS;
                half3 vDirWS = GetWorldSpaceNormalizeViewDir(positionWS);
                half3 nDirWS = normalize(input.normalWS);
                half3 vRn =normalize(mul(UNITY_MATRIX_V,half4(reflect(-vDirWS , nDirWS ),0)));
                half2 matcapUVImpoved = vRn.xy / (sqrt((vRn.x * vRn.x + vRn.y * vRn.y + (vRn.z+1) * (vRn.z+1))) * 2) + 0.5;

                //Thickness
                half ndotv = dot(nDirWS,vDirWS);
                half Thickness =clamp((1 - smoothstep(_Min,_Max,ndotv)),0.0,1.0);

                half4 baseColor = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,uv) * _BaseColor;
                half4 matcap = SAMPLE_TEXTURE2D(_Matcap,sampler_Matcap,matcapUVImpoved) ;
                half4 refracMatcap = SAMPLE_TEXTURE2D(_RefracMatcap,sampler_RefracMatcap,matcapUVImpoved+(Thickness * _Refintensity)) * baseColor;
                half4 finalColor = lerp(baseColor * 0.5,refracMatcap,Thickness * _Refintensity) + matcap;
                half alpha = clamp(max(matcap.r, Thickness),0.0,1.0);
                return half4(finalColor.xyz,alpha);
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
