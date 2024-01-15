Shader "URP/Transparent/WindowGlass"
{
    Properties
    {
        [Space(10)]
        [Enum(UnityEngine.Rendering.CullMode)]_CullMode("剔除模式", Float) = 2.0
        
        [Space(10)]
        [Main(PBRIntput, _, off, off)] _PBRIntput ("颜色属性", Float) = 1
        [Space(10)]
        [Sub(PBRIntput)][HDR]_BaseColor("基础颜色", Color) = (1,1,1,1)
        [Sub(PBRIntput)]_Alpha("透明度", Range( 0 , 2)) = 0
       
        [Space(10)]
        [Main(Reflection, _, off, off)] _Reflection ("反射", Float) = 1
        [Space(10)]
        [Sub(Reflection)]_Matcap("Matcap", 2D) = "white" {}
        [Sub(Reflection)]_ReflectMap("反射Matcap", 2D) = "white" {}
        [Sub(Reflection)]_Center("反射中心坐标（XYZ)", Vector) = (0,0,0,0)
        [Sub(Reflection)]_ReflectionIntensity("反射强度", Float) = 1
        [Sub(Reflection)]_GlassMask("玻璃遮罩", 2D) = "white" {}
        
        [Space(10)]
        [Main(Fresnel, _, off, off)] _Fresnel ("Fresnel 效应", Float) = 1
        [Space(10)]
        [Sub(Fresnel)]_FresnelScale("Fresnel强度", Float) = 1
		[Sub(Fresnel)]_FresnelBias("Fresnel偏差", Float) = 0
		[Sub(Fresnel)]_FresnelPower("Fresnel次幂", Float) = 3
        
        _float_tooltip ("提示#无光照；不支持烘培光照；透明；平面模拟球体环境反射；Fresnel效应；适用于场景窗户玻璃", float) = 1
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue"="Transparent" "RenderPipeline" = "UniversalPipeline" }
        Pass
        {
           Tags{"LightMode" = "UniversalForward"}
            Cull Back
            Blend SrcAlpha OneMinusSrcAlpha
            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
            //#pragma multi_compile_local _ _USE_RGBM
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Core.hlsl"
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Lighting.hlsl"


           struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                half2 texcoord     : TEXCOORD0;
            };
            struct Varyings
            {
                half2 uv                       : TEXCOORD0;
                float3 positionWS               : TEXCOORD1;
                float3 normalWS                 : TEXCOORD2;
                half4 tangentWS                : TEXCOORD3;
                float3 positionOS               : TEXCOORD4;
                float4 positionCS               : SV_POSITION;
            };
            
            TEXTURE2D(_Matcap);
            SAMPLER(sampler_Matcap);
            TEXTURE2D(_ReflectMap);
            SAMPLER(sampler_ReflectMap);
            TEXTURE2D(_GlassMask);
            SAMPLER(sampler_GlassMask);
            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half _Alpha;
                half3 _Center;
                half _ReflectionIntensity;
                half _FresnelScale;
                half _FresnelBias;
                half _FresnelPower;
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
                 float3 positionWS = input.positionWS;
                 float3 postionOS = input.positionOS;
                 half3 vDirWS = GetWorldSpaceNormalizeViewDir(positionWS);
                 half3 nDirWS = normalize(input.normalWS);
                 half3 sphericalPos = normalize(mul(GetObjectToWorldMatrix(),float4(postionOS - _Center,0.0)));
                 half2 cubeMapUV = (mul(UNITY_MATRIX_V,float4(sphericalPos,0.0))).xy * 0.5 + 0.5;
                 half3 Reflection = SAMPLE_TEXTURE2D(_ReflectMap,sampler_ReflectMap,cubeMapUV).xyz * _ReflectionIntensity;
              
                
                half MatCap = SAMPLE_TEXTURE2D(_Matcap,sampler_Matcap,cubeMapUV).r;
                half MatCapPow = MatCap * MatCap * MatCap * MatCap;
                half GlassMask = SAMPLE_TEXTURE2D(_GlassMask,sampler_GlassMask,uv).r;

                //Fresnel
                half ndotv = dot(nDirWS,vDirWS);
                half fresnel = pow(max(1.0 - ndotv , 0.0001),_FresnelPower) * _FresnelScale + _FresnelBias;
                
                half alpha = clamp(max(MatCapPow,GlassMask) * _Alpha  + fresnel,0.0,1.0);
                half3 baseColor = Reflection * _BaseColor.rgb  + (fresnel + GlassMask ) * (1 - Reflection * _BaseColor.rgb) ;
                baseColor = LinearToGamma22(baseColor);
                return half4(baseColor.rgb,alpha);
            }   
            ENDHLSL
        }
    }
     FallBack "Hidden/Universal Render Pipeline/FallbackError"
     CustomEditor "LWGUI.LWGUI"
}
