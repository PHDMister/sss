Shader "URP/Char/DoubleKKHair"
{
    Properties
    {
        [Space(10)]
        [Enum(UnityEngine.Rendering.CullMode)]_CullMode("剔除模式", Float) = 2.0
        [Space(10)]
        [Main(Base,_, off, off)] _Base ("PBR属性", Float) = 1.0
        [Space(10)]
        [Sub(Base)][MainColor] _BaseColor("基础颜色", Color) = (1,1,1,1)
        [Space(10)]
        [Sub(Base)]_NormalMap("法线贴图", 2D) = "bump" {}
        [Sub(Base)]_NormalIntensity("法线强度", float) = 1.0
        [Space(10)]
        [Sub(Base)]_Smoothness("光滑度", Range(-1.0, 1.0)) = 0.0
        [Sub(Base)]_Metallic("金属度", Range(0.0, 1.0)) = 0.0
        
         [Space(10)]
         [Main(KK, _, off, off)] _KK ("各向异性", Float) = 1
         [Space(10)]
         [Sub(KK)]_ShiftMap ("Aniso  Shift Map", 2D) = "white" {}
         [Sub(KK)]_AnisoShiftScale("Scale", Range(1, 50)) = 10
         [Sub(KK)]_AnisoSpecularColor("Layer1 Color", Color) = (1,1,1,1)
         [Sub(KK)]_AnisoSpread1("Layer1 Spread", Range(-1,1)) = 0.0
         [Sub(KK)]_AnsioSpeularShift("Layer1 Shift", Range(-3,3)) = 1.0
         [Sub(KK)]_AnsioSpeularStrength("Layer1 Strength", Range(0, 64)) = 1.0
         [Sub(KK)]_AnsioSpeularExponent("Layer1 Exponent ", Range(1,1024)) = 1.0
        
         [Sub(KK)] _AnisoSecondarySpecularColor("Layer2 Color", Color) = (0.5,0.5,0.5,1)
         [Sub(KK)] _AnisoSpread2("Layer2 Spread", Range(-1,1)) = 0.0
         [Sub(KK)] _AnsioSecondarySpeularShift("Layer2 Shift", Range(-3,3)) = 1.0
         [Sub(KK)] _AnsioSecondarySpeularStrength("Layer2 Strength", Range(0, 64)) = 1.0
         [Sub(KK)] _AnsioSecondarySpeularExponent("Layer2 Exponent",Range(1,1024)) = 1.0
      
        [Space(10)]
         [Main(Lighting, _, off, off)] _Lighting ("光照", Float) = 1
         [Space(10)]
         [SubToggle(Lighting,_ENVIRONMENTREFLECTIONS_OFF)] _EnvironmentReflections("环境反射", Float) = 1.0
        
        _float_tooltip ("提示#PBR光照；不支持烘培光照；不透明；裁剪；环境反射；溶解；顶点偏移；各向异性高光", float) = 1.0
    }

    SubShader
    {
        Tags{"RenderType" = "Opaque"  "Queue"="AlphaTest" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit" }

        Pass
        {
            Tags{"LightMode" = "UniversalForward"}
            Cull[_CullMode]
            HLSLPROGRAM
            #pragma target 3.0
            // -------------------------------------
            // Universal Pipeline keywords
            #pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Core.hlsl"
            #include "Assets/Resources/Shaders/PBR/LightingCustom.hlsl"
            #include "Assets/Resources/Shaders/PBR/StandardLighting.hlsl"
             #include "Assets/Resources/Shaders/PBR/AnsioLighting.hlsl"

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
                float3 bDir : TEXCOORD5;
            };
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            TEXTURE2D(_NoiseMap);
            SAMPLER(sampler_NoiseMap);
            TEXTURE2D(_ShiftMap);
            SAMPLER(sampler_ShiftMap);
            
            CBUFFER_START(UnityPerMaterial)
                half4 _NormalMap_ST;
                half4 _ShiftMap_ST;
                half4 _BaseColor;
                half _NormalIntensity;
                half _Metallic;
                half _Smoothness;
                half _AnisoShiftScale;
                half4 _AnisoSpecularColor;
                half _AnisoSpread1;
                half _AnsioSpeularShift;
                half _AnsioSpeularStrength;
                half _AnsioSpeularExponent;
                half4 _AnisoSecondarySpecularColor;
                half _AnisoSpread2;
                half _AnsioSecondarySpeularShift;
                half _AnsioSecondarySpeularStrength;
                half _AnsioSecondarySpeularExponent;
            CBUFFER_END  
            inline void InitAnisoSpecularData(out AnisoSpecularData anisoSpecularData)
            {
	        anisoSpecularData.specularColor = _AnisoSpecularColor.rgb;
	        anisoSpecularData.specularSecondaryColor = _AnisoSecondarySpecularColor.rgb;
	        anisoSpecularData.specularShift = _AnsioSpeularShift;
	        anisoSpecularData.specularSecondaryShift  = _AnsioSecondarySpeularShift;
	        anisoSpecularData.specularStrength = _AnsioSpeularStrength;
	        anisoSpecularData.specularSecondaryStrength = _AnsioSecondarySpeularStrength;
	        anisoSpecularData.specularExponent = _AnsioSpeularExponent;
	        anisoSpecularData.specularSecondaryExponent = _AnsioSecondarySpeularExponent;
	        anisoSpecularData.spread1 = _AnisoSpread1;
	        anisoSpecularData.spread2 = _AnisoSpread2;
            }
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
                output.bDir = normalize(cross(output.normalWS.xyz,output.tangentWS.xyz)) * sign;
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
                half3 tDirWS = normalize(input.tangentWS.xyz);
                half3 bDirWS = input.bDir;
                half3x3 TBN  = half3x3(tDirWS,bDirWS,nDirWS);
                half3 lDir = half3(0.2,0.2,-0.1);
                half3 hDir = normalize(lDir + vDirWS);
                
                //Material Keywords
                half3 BaseColor =  _BaseColor.rgb ;
                
                half Metallic =  _Metallic;
                half smoothness = (_Smoothness );
                half Roughness =saturate( 1 -smoothness );
                half3 nDirTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap,sampler_NormalMap,UV * _NormalMap_ST.xy + _NormalMap_ST.zw) ,_NormalIntensity) ;
                nDirWS = normalize(mul(nDirTS,TBN));
                half Occlusion = 1;
                
                //BRDF
                half3 DiffuseColor = lerp(BaseColor,float3(0.0,0.0,0.0),Metallic);
                half3 SpecularColor = lerp(float3(0.04,0.04,0.04),BaseColor,Metallic);
                Roughness = max(Roughness,0.001f);
                
                //----IndirectLight----
                half3 IndirectLighting = half3(0,0,0);
                IndirectLightingHair_float(DiffuseColor,SpecularColor,Roughness,positionWS,nDirWS,vDirWS,Occlusion,0,IndirectLighting);
                // // kk
                half3 specColor = half3(0,0,0);
                 half2 anisoUV = UV.xy * _AnisoShiftScale;
                AnisoSpecularData anisoSpecularData;
                InitAnisoSpecularData(anisoSpecularData);
                half4 detailNormal = SAMPLE_TEXTURE2D(_ShiftMap,sampler_ShiftMap, anisoUV);
                specColor = AnisotropyDoubleSpecular(SpecularColor, anisoUV, input.tangentWS,hDir,nDirWS,lDir, anisoSpecularData,
                detailNormal);
                
                half4 color = half4(IndirectLighting +specColor ,1);
                return color;
               
            }
            ENDHLSL
        }


    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor "LWGUI.LWGUI"
}
