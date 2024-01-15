Shader "URP/Char/Flow"
{
    Properties
    {
        [Space(10)]
        [Enum(UnityEngine.Rendering.CullMode)]_CullMode("剔除模式", Float) = 2.0
        
        [Space(10)]
        [Main(Base,_, off, off)] _Base ("PBR属性", Float) = 1.0
        [Space(10)]
        [Sub(Base)][MainTexture] _BaseMap("基础贴图", 2D) = "white" {}
        [Sub(Base)][MainColor] _BaseColor("基础颜色", Color) = (1,1,1,1)
        
        [Space(10)]
        [Sub(Base)]_NormalMap("法线贴图", 2D) = "bump" {}
        [Sub(Base)]_NormalIntensity("法线强度", Float) = 1.0
       
        [Space(10)]
        [Sub(Base)]_MetallicMap("金属度（R)光滑度(A)贴图", 2D) = "white" {}
        [Sub(Base)] _Metallic("金属度", Range(0.0, 1.0)) = 0.0
        [Sub(Base)]_Smoothness("光滑度", Range(0.0, 1.0)) = 0.0
        
        [Space(10)]
        [Main(Emission, _EMISSION, off)] _Emission ("发光", Float) = 0
        [Space(10)]
        [Sub(Emission)]_EmissionMap("发光贴图", 2D) = "white" {}
        [Sub(Emission)][HDR]_EmissColor("发光颜色", Color) = (0,0,0,1)
        [Sub(Emission)]_EmissionStrength("发光强度", Range(0.0, 1.0)) =1.0
        
        [Space(10)]
        [Main(Flow, _, off, off)] _Flow ("流光", Float) = 0
        [Space(10)]
        [Sub(Flow)]_FlowMap("流光图", 2D) = "black" {}
        [Sub(Flow)][HDR]_FlowColor("颜色", Color) = (0,0,0,1)
        [Sub(Flow)]_FlowSpeed ("速度(XY)", Vector) = (0,0,0,0)
        
        [Space(10)]
        [Main(Lighting, _, off, off)] _Lighting ("光照", Float) = 1
        [Space(10)]
        [SubToggle(Lighting,_ENVIRONMENTREFLECTIONS_OFF)] _EnvironmentReflections("环境反射", Float) = 1.0
        
        _float_tooltip ("提示#PBR光照，支持normal图，MS图；不支持烘培光照；不透明；环境反射；流光", float) = 1.0
    }

    SubShader
    {
        Tags{"RenderType" = "Opaque" "Queue"="AlphaTest-50" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit" }
        Pass
        {
            Tags{"LightMode" = "UniversalForward"}
            Cull[_CullMode]
            HLSLPROGRAM
            #pragma target 3.0
            // -------------------------------------
            // Universal Pipeline keywords
            #pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
            #pragma shader_feature_local _EMISSION
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Core.hlsl"
            #include "Assets/Resources/Shaders/PBR/FurnitureLighting.hlsl"
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
                float4 shadowCoord              : TEXCOORD4;
                float4 positionCS               : SV_POSITION;
            };
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            TEXTURE2D(_MetallicMap);
            SAMPLER(sampler_MetallicMap);
            #if defined (_EMISSION)
            TEXTURE2D(_EmissionMap);
            SAMPLER(sampler_EmissionMap);
            #endif
            TEXTURE2D(_FlowMap);
            SAMPLER(sampler_FlowMap);
            CBUFFER_START(UnityPerMaterial)
                half4 _NormalMap_ST;
                half4 _BaseColor;
                half _NormalIntensity;
                half _Metallic;
                half _Smoothness;
                half _EmissionStrength;
                half4 _EmissColor;
                half2 _FlowSpeed;
            half4 _FlowColor;
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
                output.shadowCoord = GetShadowCoord(vertexInput);
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
                half3 tDirWS = normalize(input.tangentWS.xyz);
                half3 bDirWS = normalize(cross(nDirWS,tDirWS) * input.tangentWS.w);
                half3x3 TBN  = half3x3(tDirWS,bDirWS,nDirWS);
                //Material Keywords
                half4 BaseColorAlpha = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,UV) ;
                half3 BaseColor = BaseColorAlpha.rgb * _BaseColor.rgb;
                half4 MetallicSmoothness = SAMPLE_TEXTURE2D(_MetallicMap,sampler_MetallicMap,UV);
                half Metallic = saturate((MetallicSmoothness).g * _Metallic);
                half Roughness = 1 - (_Smoothness * MetallicSmoothness.a);
                half3 nDirTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap,sampler_NormalMap,UV * _NormalMap_ST.xy + _NormalMap_ST.zw) ,_NormalIntensity) ;
                nDirWS = normalize(mul(nDirTS,TBN));
                half Occlusion = 1;
                //Emiss
                #if defined (_EMISSION)
                    half4 EmissonMap = SAMPLE_TEXTURE2D(_EmissionMap,sampler_EmissionMap,UV);
                #else
                    half4 EmissonMap = half4(0,0,0,0);
                #endif
                half4 EmissonColor = EmissonMap* _EmissionStrength * _EmissColor;
                
                //Flow
                half2 flowUV = UV + (_Time.y * _FlowSpeed.xy);
                half3 Flow = SAMPLE_TEXTURE2D(_FlowMap,sampler_FlowMap,flowUV).r  * (EmissonMap.g - EmissonMap.r) * _FlowColor.rgb ;
                
                //BRDF
                half3 DiffuseColor = lerp(BaseColor,float3(0.0,0.0,0.0),Metallic);
                half3 SpecularColor = lerp(float3(0.04,0.04,0.04),BaseColor,Metallic);
                Roughness = max(Roughness,0.001f);
          
                //----IndirectLight----
                half3 IndirectLighting = half3(0,0,0);
                IndirectLighting_float(DiffuseColor,SpecularColor,Roughness,positionWS,nDirWS,vDirWS,Occlusion,0,1,IndirectLighting);
                half4 color = half4(IndirectLighting + EmissonColor + Flow ,1);
                return color;
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor "LWGUI.LWGUI"
}
