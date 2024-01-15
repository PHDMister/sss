Shader "URP/Furniture/General"
{
    Properties
    {
        [Space(10)]
        [Enum(UnityEngine.Rendering.CullMode)]_CullMode("剔除模式", Float) = 2.0
        [Space(10)]
        [Main(PBRIntput, _, off, off)] _PBRIntput ("PBR 属性", Float) = 1
        [Sub(PBRIntput)][MainTexture] _BaseMap("Albedo 贴图", 2D) = "white" {}
        [Sub(PBRIntput)][MainColor] _BaseColor("Albedo 颜色", Color) = (1,1,1,1)
        [Sub(PBRIntput)]_Metallic("金属度",Range(0.0, 1.5)) = 0.0
        [Sub(PBRIntput)]_Smoothness("光滑度", Range(0.0, 2.0)) = 0.0
        
        [Space(10)]
        [Main(Normal, _NORMAL, off)] _normal ("法线", Float) = 0
        [Space(10)]
        [Sub(Normal)]_NormalMap("法线贴图", 2D) = "bump" {}
        [Sub(Normal)]_NormalIntensity("法线强度", Float) = 1.0
        
        [Space(10)]
        [Main(Emission, _EMISSION, off)] _Emission ("发光", Float) = 0
        [Space(10)]
        [Sub(Emission)]_EmissionMap("发光贴图", 2D) = "white" {}
        [Sub(Emission)][HDR]_EmissColor("发光颜色", Color) = (0,0,0,1)
        [Sub(Emission)]_EmissionStrength("发光强度", Range(0.0, 1.0)) =1.0
        
        [Space(10)]
        [Main(Lighting, _, off, off)] _Lighting ("光照", Float) = 1
        [Space(10)]
        [Sub(Lighting)]_LightIntensitiy("烘培光照强度",Range(0.0, 2.0)) = 1.0
        [SubToggle(Lighting,_ENVIRONMENTREFLECTIONS_OFF)]_EnvironmentReflections("环境反射", Float) = 0.0
        [Space(10)]
        
        [Space(10)]
        [Main(Stencil, _, off, off)] _Stencil ("模版测试（Artist不需要关注）", Float) = 1
        [Space(10)]
        [IntRange][Sub(Stencil)]_StencilRef("Stencil Ref",Range(0,255)) = 0
        [SubEnum(Stencil, UnityEngine.Rendering.CompareFunction)]_Comp ("Stencil Comp", Float) = 8
        [SubEnum(Stencil, UnityEngine.Rendering.StencilOp)]_Op ("Stencil Op", Float) = 2
        [IntRange][Sub(Stencil)]_ReadMask("ReadMask",Range(0,255)) = 255
        [IntRange][Sub(Stencil)]_WriteMask("WriteMask",Range(0,255)) = 255
        
        [Space(10)]
        _float_tooltip ("提示#PBR光照；仅支持烘培光照；不透明；发光；环境反射；支持系统雾效", float) = 1
    }

    SubShader
    {
        Tags{"RenderType" = "Opaque" "Queue"="Geometry +100" "RenderPipeline" = "UniversalPipeline" }
        
        stencil
        {
            Ref [_StencilRef]
            //ReadMask 1
            //writeMask 1
            Comp [_Comp]
            Pass [_Op]
            Fail Keep
            ZFail Keep
            ReadMask [_ReadMask]
		    WriteMask [_WriteMask]
        }
        Pass
        {
            Tags{"LightMode" = "UniversalForward"}
            Cull[_CullMode]
            //ZWrite Off
            //ZTest Off
            HLSLPROGRAM
            #pragma target 3.0
            
            // -------------------------------------
            // Universal Pipeline keywords
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
            #pragma shader_feature_local _EMISSION
            #pragma shader_feature_local _NORMAL
            #pragma multi_compile _ FOG_LINEAR

            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Core.hlsl"
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/UnityInput.hlsl"
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
                half2 staticLightmapUV : TEXCOORD5;
                float4 positionCS               : SV_POSITION;
            };
            
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            #if defined (_NORMAL)
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            #endif
            #if defined (_EMISSION)
            TEXTURE2D(_EmissionMap);
            SAMPLER(sampler_EmissionMap);
            #endif
             CBUFFER_START(UnityPerMaterial)
                
            //float4 _ReflectionCubeMap_HDR;
            half4 _BaseMap_ST;
            half4 _DetailAlbedoMap_ST;
            half4 _NormalMap_ST;
            half4 _BaseColor;
            half4 _SpecColor;
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
            half4 _EmissColor;
            half _NormalIntensity;
            half _LightIntensitiy;
           
            CBUFFER_END  

            // Used in Standard (Physically Based) shader
            Varyings LitPassVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                output.uv = input.texcoord;
                output.normalWS = normalInput.normalWS;         
                real sign = input.tangentOS.w * GetOddNegativeScale();
                half4 tangentWS = half4(normalInput.tangentWS.xyz, sign);
                output.tangentWS = tangentWS;
                output.positionWS = vertexInput.positionWS;
                output.positionCS = vertexInput.positionCS;
                output.staticLightmapUV = input.staticLightmapUV * unity_LightmapST.xy + unity_LightmapST.zw;
                return output;
            }
            
            // Used in Standard (Physically Based) shader
            half4 LitPassFragment(Varyings input) : SV_Target
            {
                //Input data
                half2 UV  = input.uv;
                float3 positionWS = input.positionWS;
                half3 vDirWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
                
                //Material Keywords
               // #if defined (_BASEMAP)
                    half4 BaseColorAlpha = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,UV* _BaseMap_ST.xy + _BaseMap_ST.zw) ;
                //#else
                   // half4 BaseColorAlpha = half4(1,1,1,1)* _BaseColor;
                //#endif
                half3 nDirWS = normalize(input.normalWS);
                half3 tDirWS = normalize(input.tangentWS.xyz);
                half3 bDirWS = normalize(cross(nDirWS,tDirWS) * input.tangentWS.w);
                half3x3 TBN  = half3x3(tDirWS,bDirWS,nDirWS);
                #if defined (_NORMAL)
                    half3 nDirTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap,sampler_NormalMap,UV * _NormalMap_ST.xy + _NormalMap_ST.zw) ,_NormalIntensity) ;
                    nDirWS = normalize(mul(nDirTS,TBN));
                #else
                    nDirWS = normalize(input.normalWS);
                #endif
                half3 BaseColor = BaseColorAlpha.rgb * _BaseColor.rgb;
                #if defined (_EMISSION)
                    half4 EmissonMap = SAMPLE_TEXTURE2D(_EmissionMap,sampler_EmissionMap,UV);
                #else
                    half4 EmissonMap = half4(1,1,1,1);
                #endif
                half4 EmissonColor = EmissonMap* _EmissionStrength * _EmissColor;
                half Metallic = _Metallic;
                half Roughness = saturate(1 - _Smoothness);
                half Occlusion = 1;
                 //fog
                float csz = input.positionCS.z * input.positionCS.w; 
                real fogFactor = ComputeFogFactor(csz);
                //BRDF
                half3 DiffuseColor = lerp(BaseColor,float3(0.0,0.0,0.0),Metallic);
                half3 SpecularColor = lerp(float3(0.04,0.04,0.04),BaseColor,Metallic);
                Roughness = max(Roughness,0.001f);
                //IndirectLight
                half3 IndirectLighting = half3(0,0,0);
                IndirectLighting_float(DiffuseColor,SpecularColor,Roughness,positionWS,nDirWS,vDirWS,Occlusion,input.staticLightmapUV,_LightIntensitiy,IndirectLighting);
                half4 color = half4(IndirectLighting +  EmissonColor,1);
                color.rgb = MixFog(color.rgb,fogFactor);
                return color;
            }
             ENDHLSL
            }

          Pass
        {
            Name "Meta"
            Tags{"LightMode" = "Meta"}

            Cull Off

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 3.0

            #pragma vertex UniversalVertexMeta
            #pragma fragment UniversalFragmentMeta
            #pragma shader_feature_local_fragment _SPECULAR_SETUP
            #pragma shader_feature_local_fragment _EMISSION
            #pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED
            #pragma shader_feature_local_fragment _SPECGLOSSMAP
            #include "Assets/Resources/Shaders/PBR/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitMetaPass.hlsl"
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor "LWGUI.LWGUI"
}
