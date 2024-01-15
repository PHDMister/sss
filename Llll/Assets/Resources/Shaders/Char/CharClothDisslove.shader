Shader "URP/Char/Cloth"
{
    Properties
    {
        [Space(10)]
        [Enum(UnityEngine.Rendering.CullMode)]_CullMode("剔除模式", Float) = 2.0
        [Space(10)]
        [Main(Base,_, off, off)] _Base ("PBR属性", Float) = 1.0
        [SubToggle(Base, _BASEMAP)] _CustomReflection ("使用基础贴图", float) = 1.0
        [Sub(Base_BASEMAP)][MainTexture] _BaseMap ("基础贴图", 2D) = "white" { }
        [Sub(Base)][MainColor] _BaseColor("基础颜色", Color) = (1,1,1,1)
        [Sub(Base)]_Metallic("金属度", Range(0.0, 1.0)) = 0.0
        [Sub(Base)]_Smoothness("光滑度", Range(0.2, 1.0)) = 0.2
        [Sub(Base)]_SpecShininess("光泽度",Range(0, 20.0)) = 10.0
        
        [Space(10)]
        [Main(Lighting, _, off, off)] _Lighting ("光照", Float) = 1
        [Sub(Lighting)]_DirLightIntensity("直接光强度", Float) = 0.3
        [Sub(Lighting)][HDR]_HightColor("直接光亮部颜色", Color) = (1,1,1,1)
        [Sub(Lighting)]_DarkColor("直接光暗部颜色", Color) = (0.5,0.5,0.5,1)
        
        [Space(10)]
        [SubToggle(Lighting,_ENVIRONMENTREFLECTIONS_OFF)] _EnvironmentReflections("环境反射", Float) = 1.0
        
        _float_tooltip ("提示#PBR光照，但不支持normal图，MS图；不支持烘培光照；不透明；裁剪；环境反射", float) = 1
        
    }

    SubShader
    {
        Tags{"RenderType" = "Opaque"  "Queue"="AlphaTest-200" "RenderPipeline" = "UniversalPipeline" }
        Pass
        {
            Tags{"LightMode" = "UniversalForward"}
            Cull[_CullMode]
            HLSLPROGRAM
            #pragma target 3.0
            #pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
            #pragma shader_feature_local _BASEMAP
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Core.hlsl"
            #include "Assets/Resources/Shaders/PBR/LightingCustom.hlsl"
            #include "Assets/Resources/Shaders/PBR/StandardLighting.hlsl"

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
                half4 tangentWS                : TEXCOORD3;    
                float4 positionCS               : SV_POSITION;
            };
            #if defined (_BASEMAP)
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            #endif
            TEXTURE2D(_NoiseMap);
            SAMPLER(sampler_NoiseMap);
            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half _Metallic;
                half _Smoothness;
                half _SpecShininess;
                half4 _HightColor;
                half4 _DarkColor;
            
            half _DirLightIntensity;
            CBUFFER_END  
            
            // Used in Standard (Physically Based) shader
            Varyings LitPassVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                output.uv = input.texcoord;
                output.normalWS = normalInput.normalWS;         
                real sign = input.tangentOS.w * GetOddNegativeScale();
                half4 tangentWS = half4(normalInput.tangentWS.xyz, sign);
                output.tangentWS = tangentWS;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionWS = vertexInput.positionWS;
                output.positionCS = vertexInput.positionCS;
                return output;
            }
            // Used in Standard (Physically Based) shader
            half4 LitPassFragment(Varyings input) : SV_Target
            {
                //Input data
                half2 UV  = input.uv;
                float3 positionWS = input.positionWS;
                half3 vDirWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
                half3 nDirWS = normalize(input.normalWS);
                half3 lDir = half3(0.2,0.2,-0.1);
                half3 hDir = normalize(lDir + vDirWS);
               
                //Material Keywords
                #if defined (_BASEMAP)
                half4 BaseColorAlpha = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,UV) ;
                #else
                half4 BaseColorAlpha = half4(1,1,1,1);
                #endif
                half3 BaseColor = BaseColorAlpha.rgb * _BaseColor.rgb ;
                
                half Metallic =  _Metallic;
                half smoothness = (_Smoothness );
                half Roughness =saturate( 1 -smoothness );
                half Occlusion = 1;
                
                //BRDF
                half3 DiffuseColor = lerp(BaseColor,float3(0.0,0.0,0.0),Metallic);
                half3 SpecularColor = lerp(float3(0.04,0.04,0.04),BaseColor,Metallic);
                Roughness = max(Roughness,0.001f);

                //----directLight----
                half3 DirectLighting = half3(0,0,0);
                DirectLightingMoblie_float(DiffuseColor,SpecularColor,Roughness,_SpecShininess,nDirWS,lDir,hDir,_HightColor,_DarkColor,_DirLightIntensity,DirectLighting);
                //----IndirectLight----
                half3 IndirectLighting = half3(0,0,0);
                IndirectLighting_float(DiffuseColor,SpecularColor,Roughness,positionWS,nDirWS,vDirWS,Occlusion,0,IndirectLighting);
                half4 color = half4(DirectLighting* IndirectLighting  ,1);
                return color;
               
            }
            ENDHLSL
        }

    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor "LWGUI.LWGUI"
}
