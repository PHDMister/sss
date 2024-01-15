Shader "URP/Transparent/Rim"
{
    Properties
    {
        [Header(Rim)]
        [Space(10)]
		_RimColor("Rim Color", Color) = (0.2101577,0.8511046,0.8679245,1)
        _InnerColor("Inner Color", Color) = (1,1,1,1)
		_RimMin("Rim Min", Float) = 0
		_RimMax("Rim Max", Float) = 1
		_RimIntensity("RimIntensity", Float) = 1
        
		[Header(Flow)]
        [Space(10)]
		_FlowTex("FlowTex", 2D) = "black" {}
		_FlowIntensity("FlowIntensity", Float) = 0.5
        _Speed("Speed", Float) = 1
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent+150"}
        Pass
        {
           Tags{"LightMode" = "UniversalForward"}
            Cull Back
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            ZWrite On
            
           HLSLPROGRAM
            #pragma target 3.0
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
            };
            struct Varyings
            {
                half2 uv                       : TEXCOORD0;
                float3 positionWS               : TEXCOORD1;
                half3 normalWS                 : TEXCOORD2;
                //half4 tangentWS                : TEXCOORD3;
                float3 positionOS               : TEXCOORD4;
                float4 positionCS               : SV_POSITION;
            };
            // TEXTURE2D(_NormalMap);
            // SAMPLER(sampler_NormalMap);
            TEXTURE2D(_FlowTex);
            SAMPLER(sampler_FlowTex);
            CBUFFER_START(UnityPerMaterial)
                half4 _InnerColor;
				half4 _RimColor;
				half _RimIntensity;
				half _RimMax;
				half _RimMin;
				half _Speed;
				half _FlowIntensity;
            CBUFFER_END  

           Varyings LitPassVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                output.uv = input.texcoord;
                output.normalWS = normalInput.normalWS;
                //real sign = input.tangentOS.w * GetOddNegativeScale();
                //half4 tangentWS = half4(normalInput.tangentWS.xyz, sign);
                //output.tangentWS = tangentWS;
                output.positionWS = vertexInput.positionWS;
                output.positionCS = vertexInput.positionCS;
                output.positionOS = input.positionOS;
                return output;
            }

            half4 LitPassFragment (Varyings input) : SV_Target
            {
                //input data
                half2 uv  = input.uv;
                float3 positionWS = input.positionWS;
                // //normal
                half3 nDirWS = normalize(input.normalWS);
                // half3 tDirWS = normalize(input.tangentWS.xyz);
                // half3 bDirWS = normalize(cross(nDirWS,tDirWS) * input.tangentWS.w);
                //  half3x3 TBN  = half3x3(tDirWS,bDirWS,nDirWS);
                // half3 nDirTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap,sampler_NormalMap,uv) ,1) ;
                // nDirWS = normalize(mul(nDirTS,TBN));
                //rim
                half3 vDirWS = GetWorldSpaceNormalizeViewDir(positionWS);
                half fresnel = 1 - dot(nDirWS,vDirWS);
                half fresnelContral = saturate(smoothstep(_RimMin,_RimMax,fresnel));
                half4 rim = lerp(_InnerColor,_RimColor * _RimIntensity,fresnelContral);

                //flow
                half2 orignWS = positionWS.xy -  mul( GetObjectToWorldMatrix(), half4(0,0,0,1) ).xy;
                half speed = _Time.y * _Speed;
                half2 flowUV = orignWS + speed;
                half flowMap = SAMPLE_TEXTURE2D(_FlowTex,sampler_FlowTex,flowUV).r * _FlowIntensity;

                //final
                half3 finalColor = rim.rgb + flowMap;
                half finalAlpha = saturate(_InnerColor.a + fresnelContral + flowMap);
                return half4(finalColor,finalAlpha);
            }   
            ENDHLSL
        }
    }
     FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
