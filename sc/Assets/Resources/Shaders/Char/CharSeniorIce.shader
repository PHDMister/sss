Shader "URP/Char/Ice"
{
    Properties
    {
        [Space(10)]
        [Enum(UnityEngine.Rendering.CullMode)]_CullMode("剔除模式", Float) = 2.0
        //_CutOff("裁剪阈值", Range(0.0, 1.0)) = 0.826
        
        [Space(10)]
        [Main(Base,_, off, off)] _Base ("PBR属性", Float) = 1.0
        [Sub(Base)][MainTexture] _BaseMap ("基础贴图", 2D) = "white" { }
        [Sub(Base)][MainColor] _BaseColor("基础颜色", Color) = (1,1,1,1)
        [Space(10)]
        [Sub(Base)]_NormalMap("法线贴图", 2D) = "bump" {}
        [Sub(Base)]_NormalIntensity("法线强度", Float) = 1.0
        [Space(10)]
        [Sub(Base)]_MetallicMap("金属度（R)AO(G)光滑度(A)贴图", 2D) = "white" {}
        [Sub(Base)]_Metallic("金属度", Range(0.0, 1.0)) = 0.0
        [Sub(Base)]_Smoothness("光滑度", Range(0, 2.0)) = 0.2
        [Sub(Base)]_SpecShininess("光泽度",Float) = 10.0
        [Sub(Base)]_AO("遮蔽强度",Range(0.0, 2.0)) =1.0
        
        [Space(10)]
        [Main(Emission, _EMISSION, off)] _Emission ("发光", Float) = 0
        [Space(10)]
        [Sub(Emission)]_EmissionMap("发光贴图", 2D) = "white" {}
        [Sub(Emission)][HDR]_EmissColor("发光颜色", Color) = (0,0,0,1)
        [Sub(Emission)]_EmissionStrength("发光强度", Range(0.0, 1.0)) =1.0
        
        [Space(10)]
        [Main(Ice, _, off, off)] _Ice ("Ice", Float) = 1
        [Sub(Ice)]_IceIntensity("强度", Range(0.0, 1.0)) =0.0
        [Sub(Ice)]_SnowMap ("冰雪贴图", 2D) = "white" { }
        [Sub(Ice)]_SnowNormal ("法线贴图", 2D) = "bump" { }
        [Sub(Ice)]_IceNormalIntensity("法线强度", Range(0.0, 1.0)) =1.0
        //[Sub(Ice)]_SnowColor("冰雪颜色", Color) = (0,0,0,0)
        [Sub(Ice)]_IceMask ("冰雪贴图", 2D) = "black" { }
        [Sub(Ice)]_IceMaskTilling("MaskTilling", Range(0.0, 2.0)) =1.0
        [Sub(Ice)]_IceLength("长度", Range(0.0, 1.0)) =1.0
        
        [Space(10)]
        [Main(Lighting, _, off, off)] _Lighting ("光照", Float) = 1
        [Sub(Lighting)]_DirLightIntensity("直接光强度", Float) = 1
        [Sub(Lighting)][HDR]_HightColor("直接光亮部颜色", Color) = (1,1,1,1)
        [Sub(Lighting)]_DarkColor("间接光暗部颜色", Color) = (0.3,0.3,0.3,1)
        
        [Space(10)]
        [SubToggle(Lighting,_ENVIRONMENTREFLECTIONS_OFF)] _EnvironmentReflections("环境反射", Float) = 1.0
        
        _float_tooltip ("提示#PBR光照，支持normal图，MS图；不支持烘培光照，但模拟了一个实时光照，暂不支持更改光照方向，仅适用动态对象；不透明；环境反射；", float) = 1
        
    }

    SubShader
    {
        Tags{"RenderType" = "Opaque"  "Queue"="AlphaTest-10" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit" }
        Pass
        {
            Tags{"LightMode" = "UniversalForward"}
            Cull[_CullMode]
            HLSLPROGRAM
            #pragma target 3.0
            #pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
            #pragma shader_feature_local _EMISSION
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
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
                half3 uv                       : TEXCOORD0;
                float3 positionWS               : TEXCOORD1;
                half3 normalWS                 : TEXCOORD2;
                half4 tangentWS                : TEXCOORD3;    
                float4 positionCS               : SV_POSITION;
                
            };
            
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_SnowMap);
            SAMPLER(sampler_SnowMap);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            TEXTURE2D(_SnowNormal);
            SAMPLER(sampler_SnowNormal);
            sampler2D _IceMask;
           // SAMPLER(sampler_IceMask);
            TEXTURE2D(_MetallicMap);
            SAMPLER(sampler_MetallicMap);
            #if defined (_EMISSION)
            TEXTURE2D(_EmissionMap);
            SAMPLER(sampler_EmissionMap);
            #endif
            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                //half4 _SnowColor;
                half4 _NormalMap_ST;
            half4 _SnowNormal_ST;
         
            half4 _SnowMap_ST;
                half _NormalIntensity;
                half _Metallic;
                half _Smoothness;
                half _SpecShininess;
                half _EmissionStrength;
                half4 _EmissColor;
                half4 _HightColor;
                half4 _DarkColor;
                //half _DirLightIntensity;
                half _DirLightIntensity;
                half _AO;
                half _IceIntensity;
                half _IceNormalIntensity;
                half _IceMaskTilling;
                half _IceLength;
            CBUFFER_END  
            
            // Used in Standard (Physically Based) shader
            Varyings LitPassVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                half4 offsetMask = tex2Dlod(_IceMask,half4((normalInput.normalWS.xz * _IceMaskTilling)+0.5,0,0));
                half mask = saturate(normalInput.normalWS.y * (-0.3) * _IceIntensity);
                half maskTop  = saturate(normalInput.normalWS.y * (0.3));
                half4 vertexOffset  = (offsetMask * mask * _IceLength)*float4(input.normalOS,0) +( maskTop * (_IceIntensity * 0.01)) ;
                output.uv = half3(input.texcoord,mask);
                output.normalWS = normalInput.normalWS;         
                real sign = input.tangentOS.w * GetOddNegativeScale();
                half4 tangentWS = half4(normalInput.tangentWS.xyz, sign);
                output.tangentWS = tangentWS;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz +vertexOffset.xyz);
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
                // half4 ShadowMask = float4(1.0,1.0,1.0,1.0);
                // float4 ShadowCoord = TransformWorldToShadowCoord(positionWS);
                // Light light = GetMainLight(ShadowCoord,positionWS,ShadowMask);
                // half3 lDir = light.direction;
                half3 lDir = half3(0.2,0.2,-0.1);
                //half3 lDir = half3(4.7,-3.2,1.88);
                half3 hDir = normalize(lDir + vDirWS);
                half3 tDirWS = normalize(input.tangentWS.xyz);
                half3 bDirWS = normalize(cross(nDirWS,tDirWS) * input.tangentWS.w);
                half3x3 TBN  = half3x3(tDirWS,bDirWS,nDirWS);
                
                //Material Keywords
                half4 BaseColorAlpha = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,UV) ;
                half3 BaseColor = BaseColorAlpha.rgb * _BaseColor.rgb ;
                half4 SnowMap = SAMPLE_TEXTURE2D(_SnowMap,sampler_SnowMap,UV * _SnowMap_ST.xy + _SnowMap_ST.zw);
                half3 finalColor = lerp(lerp(BaseColor,SnowMap.rgb,saturate(input.uv.z * 8)),SnowMap,_IceIntensity);
                half4 MetallicSmoothness = SAMPLE_TEXTURE2D(_MetallicMap,sampler_MetallicMap,UV);
                half Metallic = saturate((MetallicSmoothness).r * _Metallic);
                half Roughness = 1 - (lerp(_Smoothness * MetallicSmoothness.a,1,_IceIntensity));
                half3 nDirTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap,sampler_NormalMap,UV * _NormalMap_ST.xy + _NormalMap_ST.zw) ,_NormalIntensity) ;
                half3 SnownDirTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_SnowNormal,sampler_SnowNormal,UV * _SnowNormal_ST.xy + _SnowNormal_ST.zw) ,_IceNormalIntensity) ;
                half3 BasenDirWS = normalize(mul(nDirTS,TBN));
                half3 SnownDirWS = normalize(mul(SnownDirTS,TBN));
                nDirWS = lerp(BasenDirWS,SnownDirWS,_IceIntensity);
                float Occlusion = clamp(pow(MetallicSmoothness.g,_AO),0,1);
                
                //Emiss
                #if defined (_EMISSION)
                    half4 EmissonMap = SAMPLE_TEXTURE2D(_EmissionMap,sampler_EmissionMap,UV);
                #else
                    half4 EmissonMap = half4(0,0,0,0);
                #endif
                float fresnel = pow((1 - dot( nDirWS, vDirWS)),2.5) *3 ;
                half4 snowEmission = SnowMap * fresnel * _IceIntensity;
                half4 EmissonColor = EmissonMap* _EmissionStrength * _EmissColor +snowEmission;
                
                //BRDF
                half3 DiffuseColor = lerp(finalColor,float3(0.0,0.0,0.0),Metallic);
                half3 SpecularColor = lerp(float3(0.04,0.04,0.04),finalColor,Metallic);
                Roughness = max(Roughness,0.001f);
                
                //----directLight----
                half3 DirectLighting = half3(0,0,0);
                DirectLightingMoblie_float(DiffuseColor,SpecularColor,Roughness,_SpecShininess,nDirWS,lDir,hDir,_HightColor,_DarkColor,_DirLightIntensity,DirectLighting);
                //----IndirectLight----
                half3 IndirectLighting = half3(0,0,0);
                IndirectLighting_float(DiffuseColor,SpecularColor,Roughness,positionWS,nDirWS,vDirWS,Occlusion,0,IndirectLighting);
                //half4 color = half4(pow( DirectLighting,5)+IndirectLighting+ EmissonColor ,1);
                half4 color = half4(DirectLighting* IndirectLighting+ EmissonColor ,1);
                return color;
               
            }
            ENDHLSL
        }

    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor "LWGUI.LWGUI"
}
