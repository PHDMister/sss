Shader "Unlit/FakeVolumeLight"
{
    Properties
    {
         [Space(10)]
        [Enum(UnityEngine.Rendering.CullMode)]_CullMode("剔除模式", Float) = 2.0
        
        [Space(10)]
        [Main(Base,_, off, off)] _Base ("光照", Float) = 1.0
        [Space(10)]
        [Sub(Base)]_LightColor("颜色", Color) = (0,0,0,0)
		[Sub(Base)]_LightIntensity("强度", Float) = 1
        
        [Space(10)]
        [Main(Fresnel,_, off, off)] _Fresnel ("Fresnel效应", Float) = 1.0
        [Space(10)]
        [Sub(Fresnel)]_F_Power("次幂", Float) = 1
		[Sub(Fresnel)]_F_Scale("强度", Float) = 1
		[Sub(Fresnel)]_F_Bias("偏移度", Float) = 0
        
        [Space(10)]
        [Main(Noise,_, off, off)] _Noise ("噪声", Float) = 1.0
        [Space(10)]
        [Sub(Noise)]_NoiseMap("噪声图", 2D) = "white" {}
		[Sub(Noise)]_NoiseSpeed("速度", Vector) = (0,0,0,0)
       
        
        [Space(10)]
        [Main(Shape,_, off, off)] _Shape ("形状", Float) = 1.0
        [Space(10)]
        [Sub(Shape)]_FadeOffset("消隐偏移度", Float) = 0
		[Sub(Shape)]_FadePower("次幂", Float) = 1
        [Sub(Shape)]_StartAngle("起始角度", Float) = 0
		[Sub(Shape)]_EndAngle("末尾角度", Float) = 0
		
        
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent"  "Queue" = "Transparent+0" "RenderPipeline" = "UniversalPipeline"}
        Pass
        {
           Tags{"LightMode" = "UniversalForward"}
           Cull[_CullMode]
            ZWrite Off
		    Blend SrcAlpha One
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
           
            CBUFFER_START(UnityPerMaterial)
                half4 _LightColor;
                half4 _NoiseMap_ST;
                half _LightIntensity;
           half _F_Power;
           half _F_Scale;
           half _F_Bias;
           half2 _NoiseSpeed;
           half _FadeOffset;
           half _FadePower;
           half _StartAngle;
           half _EndAngle;
           
               
            CBUFFER_END  
            

           Varyings LitPassVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                //start
                half3 start = input.normalOS * (1-  input.texcoord.x) * _StartAngle;
                //end
                half3 endd =  input.normalOS * input.texcoord.x * _EndAngle;
                half3 vertextOffset = start + endd;
                input.positionOS.xyz += vertextOffset;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
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
                float3 positionWS = input.positionWS;
                // float3 postionOS = input.positionOS;
                half3 vDirWS = GetWorldSpaceNormalizeViewDir(positionWS);
                half3 nDirWS = normalize(input.normalWS);
                
                half2 noiseUV = (uv * _NoiseMap_ST.xy +_NoiseMap_ST.zw ) + _NoiseSpeed * _Time.y;
                half noiseMap = SAMPLE_TEXTURE2D(_NoiseMap,sampler_NoiseMap,noiseUV).r;
                half3 finalColor = (noiseMap * _LightColor * _LightIntensity).rgb;

                //Fade
                half fresnel = saturate(pow(abs(dot(nDirWS,vDirWS)),_F_Power) * _F_Scale + _F_Bias);
                half topFade = pow(((1 - uv.x ) - _FadeOffset),_FadePower);
                half finalAlpha = fresnel * topFade;
                
                return half4(finalColor,finalAlpha);
            }   
            ENDHLSL
        }
    }
     FallBack "Hidden/Universal Render Pipeline/FallbackError"
     CustomEditor "LWGUI.LWGUI"
}