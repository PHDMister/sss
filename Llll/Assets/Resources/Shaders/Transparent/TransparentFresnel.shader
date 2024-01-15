Shader "URP/Transparent/Fresnel"
{
    Properties
    {
        [Space(10)]
        [Enum(UnityEngine.Rendering.CullMode)]_CullMode("剔除模式", Float) = 2.0
        
        [Space(10)]
        [Main(PBRIntput, _, off, off)] _PBRIntput ("PBR 属性", Float) = 1
        [Sub(PBRIntput)][_MainTex]_BaseMap("基础贴图", 2D) = "white" {}
        [Sub(PBRIntput)][HDR]_BaseColor("基础颜色", Color) = (1,1,1,1)
        [Sub(PBRIntput)]_Alpha("透明度", Range( 0 , 1)) = 0
        [Sub(PBRIntput)]_Metallic("金属度",Range(0.0, 1.5)) = 1.0
        [Sub(PBRIntput)]_Smoothness("光滑度", Range(0.0, 2.0)) = 1.0
        
        [Space(10)]
        [Main(Emission, _EMISSION, off)] _Emission ("发光", Float) = 0
        [Space(10)]
        [Sub(Emission)]_EmissMask("发光贴图", 2D) = "white" {}
        [Sub(Emission)][HDR]_EmissColor("发光颜色", Color) = (0,0,0,0)
        [Sub(Emission)]_EmissIntensity("发光强度", Range( 0 , 5)) = 0
        
        [Space(10)]
        [Main(Fresnel, _, off, off)] _Fresnel ("Fresnel 效应", Float) = 1
        [Space(10)]
        [Sub(Fresnel)][HDR]_FresnelColor("Fresnel 颜色", Color) = (0,0,0,0)
        [Sub(Fresnel)]_FresnelScale("Fresnel强度", Float) = 0
		[Sub(Fresnel)]_FresnelBias("Fresnel偏差", Float) = 0
		[Sub(Fresnel)]_FresnelPower("Fresnel次幂", Range(0.0, 4.0)) = 3
        
        [Space(10)]
        [Main(Lighting, _, off, off)] _Lighting ("光照", Float) = 1
        [Space(10)]
        [SubToggle(Lighting, _CUSTOMREFLECTION)] _CustomReflection ("自定义环境反射", float) = 0
        [Space(10)]
		[Tex(Lighting_CUSTOMREFLECTION)] _ReflectionCubeMap ("反射cubeMap", Cube) = "white" { }
        
        _float_tooltip ("提示#PBR光照；不支持烘培光照；透明；发光；永远开启环境反射；支持自定义环境反射；Fresnel效应", float) = 1

        
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+200" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit"}
        Pass
        {
           Tags{"LightMode" = "UniversalForward"}
            Cull[_CullMode]
            Blend SrcAlpha OneMinusSrcAlpha
            HLSLPROGRAM
            #pragma target 3.0
            #pragma shader_feature_local _EMISSION
            #pragma shader_feature_local _CUSTOMREFLECTION
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Core.hlsl"
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Lighting.hlsl"
            #include "Assets/Resources/Shaders/PBR/FurnitureLighting.hlsl"
           struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                half2 texcoord     : TEXCOORD0;
                half2 staticLightmapUV : TEXCOORD1;
            };
            struct Varyings
            {
                half2 uv                       : TEXCOORD0;
                float3 positionWS               : TEXCOORD1;
                float3 normalWS                 : TEXCOORD2;
                half4 tangentWS                : TEXCOORD3;
                float3 positionOS               : TEXCOORD4;
                float4 positionCS               : SV_POSITION;
                half2 staticLightmapUV : TEXCOORD5;
            };
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_EmissMask);
            SAMPLER(sampler_EmissMask);
             #if defined (_CUSTOMREFLECTION)
            TEXTURECUBE(_ReflectionCubeMap);
            SAMPLER(sampler_ReflectionCubeMap);
             #endif
            CBUFFER_START(UnityPerMaterial)
                float4 _ReflectionCubeMap_HDR;
                half4 _BaseColor;
                half4 _FresnelColor;
                half _Alpha;
                half4 _EmissColor;
                half _EmissIntensity;
                half _Smoothness;
                half _Metallic;
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
                output.staticLightmapUV = input.staticLightmapUV * unity_LightmapST.xy + unity_LightmapST.zw;
                return output;
            }

            half4 LitPassFragment (Varyings input) : SV_Target
            {
                half2 uv  = input.uv;
                float3 positionWS = input.positionWS;
                //float3 postionOS = input.positionOS;
                half3 vDirWS = GetWorldSpaceNormalizeViewDir(positionWS);
                half3 nDirWS = normalize(input.normalWS);
                half4 baseMap = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,uv);
                half3 baseColor = baseMap.rgb * _BaseColor.rgb;
                half alpha = baseMap.a;
                half Metallic = _Metallic;
                half Roughness = saturate(1 - _Smoothness);
                half Occlusion = 1;
                half3 R = reflect(-vDirWS,nDirWS);
                //BRDF
                half3 DiffuseColor = lerp(baseColor,float3(0.0,0.0,0.0),Metallic);
                half3 SpecularColor = lerp(float3(0.04,0.04,0.04),baseColor,Metallic);
                Roughness = max(Roughness,0.001f);
                half3 IndirectLighting = half3(0,0,0);
                #if defined (_CUSTOMREFLECTION)
                half mip = PerceptualRoughnessToMipmapLevel(Roughness);
	            half4 encodedIrradiance = SAMPLE_TEXTURECUBE_LOD(_ReflectionCubeMap, sampler_ReflectionCubeMap, R,mip);
                IndirectLighting_customReflection(DiffuseColor,SpecularColor,Roughness,positionWS,nDirWS,vDirWS,Occlusion,input.staticLightmapUV,encodedIrradiance,_ReflectionCubeMap_HDR,IndirectLighting);
                #else
                IndirectLighting_float(DiffuseColor,SpecularColor,Roughness,positionWS,nDirWS,vDirWS,Occlusion,input.staticLightmapUV,1,IndirectLighting);
                #endif
                
                #if defined (_EMISSION)
                half4 EmissonMap = SAMPLE_TEXTURE2D(_EmissMask,sampler_EmissMask,uv)* alpha ;
                #else
                    half4 EmissonMap = half4(0,0,0,0);
                #endif
                half4 EmissonColor = EmissonMap* _EmissIntensity * _EmissColor;
                //Fresnel
                half ndotv = dot(nDirWS,vDirWS);
                half fresnel = pow(max(1.0 - ndotv , 0.0001),_FresnelPower) * _FresnelScale + _FresnelBias;
                half3 fianlfresnel = fresnel * _FresnelColor;
                //half3 finalColor = baseColor * _BaseColor + emissColor;
                half finalAlpha = alpha * _Alpha;
                return half4(IndirectLighting + EmissonColor + fianlfresnel,finalAlpha);
            }   
            ENDHLSL
        }
    }
     FallBack "Hidden/Universal Render Pipeline/FallbackError"
     CustomEditor "LWGUI.LWGUI"
}
