Shader "URP/Char/LipDissolve"
{
    Properties
    {
        [Space(10)]
        [Enum(UnityEngine.Rendering.CullMode)]_CullMode("剔除模式", Float) = 2.0
        _CutOff("裁剪阈值", Range(0.0, 1.0)) = 0.826
        
        [Space(10)]
        [Main(Base,_, off, off)] _Base ("PBR属性", Float) = 1.0
        [Space(10)]
        [Sub(Base)][MainTexture] _BaseMap("基础贴图", 2D) = "white" {}
        [Sub(Base)][MainColor] _BaseColor("皮肤颜色", Color) = (1,1,1,1)
        [Sub(Base)][MainColor] _AddColor("叠加颜色", Color) = (0,0,0,0)
        [Space(10)]
        [Sub(Base)]_LipColor("嘴唇颜色", Color) = (1,1,1,1)
        [Sub(Base)]_LipSmoothness("嘴唇光滑度", Range(-1.0, 2.0)) = 0.0
        [Sub(Base)]_SpecShininess("嘴唇高光度",Float) = 10.0
        [Sub(Base)]_SpecIntensity("嘴唇高光强度",Range(0.0, 1.0)) = 0.0
        
        [Space(10)]
        [Sub(Base)]_NormalMap("法线贴图", 2D) = "bump" {}
        [Sub(Base)]_NormalIntensity("法线强度", float) = 1.0
        
        [Space(10)]
        [Sub(Base)]_MetallicMap("金属度（R)光滑度(A)贴图", 2D) = "black" {}
        [Sub(Base)]_Metallic("金属度", Range(0.0, 1.0)) = 0.0
        [Sub(Base)]_Smoothness("光滑度", Range(-1.0, 1.0)) = 0.0
        
        [Space(10)]
        [Main(Emission, _EMISSION, off)] _Emission ("发光", Float) = 0
        [Space(10)]
        [Sub(Emission)]_EmissionMap("发光贴图", 2D) = "white" {}
        [Sub(Emission)][HDR]_EmissColor("发光颜色", Color) = (0,0,0,1)
        [Sub(Emission)]_EmissionStrength("发光强度", Range(0.0, 1.0)) =1.0
        
         [Space(10)]
         [Main(Dissolve, _, off, off)] _Dissolve ("溶解", Float) = 1
         [Space(10)]
         [Sub(Dissolve)]_DissolveAmount("溶解数量", Range( 0 , 1.5)) = 1.5
         [Sub(Dissolve)]_DissolveOffset("偏移值", Float) = -4
		 [Sub(Dissolve)]_DissolveSpread("扩散值", Float) = 1.5
         [Sub(Dissolve)]_DissloveEdgeOffset("边缘偏移", Float) = 0.94
		 [Sub(Dissolve)][HDR]_EdgeEmissColor("边缘颜色", Color) = (16.9411774,17.9450989,23.9686279,1)
         [Sub(Dissolve)]_NoiseMap("噪声图", 2D) = "white" {}
         [Sub(Dissolve)]_NoiseScale("噪声强度（XYZ)", Vector) = (1,1,1,0)
        
         [Space(10)]
         [Main(VertexDissolve, _, off, off)] _VertexDissolve ("顶点溶解", Float) = 1
         [Space(10)]
         [Sub(VertexDissolve)]_VertexDissolveSpread("扩散值", Float) = 1
		 [Sub(VertexDissolve)]_VertexDissolveOffset("扩散值", Float) = -4
		 [Sub(VertexDissolve)]_VertexOffsetIntensity("强度", Float) = 1
         [Space(10)]
         [Main(Lighting, _, off, off)] _Lighting ("光照", Float) = 1
         [Space(10)]
         [SubToggle(Lighting,_ENVIRONMENTREFLECTIONS_OFF)] _EnvironmentReflections("环境反射", Float) = 1.0
        //[Sub(Lighting)]_LightDir("直接光方向", Vector) = (0,0,0,0)
        [Sub(Lighting)]_DirLightIntensity("直接光强度", Float) = 0.2
         [Sub(Lighting)]_HightColor("直接光亮部颜色", Color) = (1,1,1,1)
         [Sub(Lighting)]_DarkColor("间接光暗部颜色", Color) = (0.5,0.5,0.5,1)
         
        
        _float_tooltip ("提示#PBR光照，支持normal图，MS图；不支持烘培光照；不透明；裁剪；环境反射；溶解；顶点偏移；局部改色", float) = 1.0
    }

    SubShader
    {
        Tags{"RenderType" = "Opaque"  "Queue"="AlphaTest-50" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit" }
        // ------------------------------------------------------------------
        //  Forward pass. Shades all light in a single pass. GI + emission + Fog
        Pass
        {
            Tags{"LightMode" = "UniversalForward"}
            Cull[_CullMode]
            HLSLPROGRAM
            #pragma target 3.0
            // -------------------------------------
            // Universal Pipeline keywords
            //#pragma multi_compile _ LIGHTMAP_ON
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
                half2 uv                       : TEXCOORD0;
                float3 positionWS               : TEXCOORD1;
                half3 normalWS                 : TEXCOORD2;
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
            TEXTURE2D(_NoiseMap);
            SAMPLER(sampler_NoiseMap);
            #if defined (_EMISSION)
            TEXTURE2D(_EmissionMap);
            SAMPLER(sampler_EmissionMap);
            #endif
            CBUFFER_START(UnityPerMaterial)
                half4 _NormalMap_ST;
                half4 _BaseColor;
                half4 _LipColor;
                half4 _HightColor;
                half4 _DarkColor;
                half _NormalIntensity;
                half _Metallic;
                half _Smoothness;
                half _LipSmoothness;
                half _DissolveAmount;
                half _DissolveOffset;
                half _DissolveSpread;
                half _DissloveEdgeOffset;
                half _VertexDissolveSpread;
                half _VertexDissolveOffset;
                half _VertexOffsetIntensity;
                half4 _EdgeEmissColor;
                half4 _NoiseScale;
                half _CutOff;
                half _EmissionStrength;
                half4 _EmissColor;
                half _SpecShininess;
                half _SpecIntensity;
            half _DirLightIntensity;
            half4 _AddColor;
            //half3 _LightDir;
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
                float3 positionWS = mul(GetObjectToWorldMatrix(), input.positionOS).xyz;
                half3 vertexOffset = max(((positionWS.y + (-3.0 + ((1-_DissolveAmount) - 1.0) * (3.5) )) - _VertexDissolveOffset) * rcp(_VertexDissolveSpread) ,0);
                half4 vertexOffset1 = half4(half3(0,1,0) * _VertexOffsetIntensity * vertexOffset + positionWS,1);
                half3 vertexOffset2 = mul(GetWorldToObjectMatrix(),vertexOffset1).xyz - input.positionOS.xyz;
                input.positionOS.xyz  +=vertexOffset2 ;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
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
                // half4 ShadowMask = float4(1.0,1.0,1.0,1.0);
                // float4 ShadowCoord = TransformWorldToShadowCoord(positionWS);
                // Light light = GetMainLight(ShadowCoord,positionWS,ShadowMask);
                // half3 lDir = light.direction;
                half3 lDir = half3(0.2,0.2,-0.1);
               //half3 lDir = half3(-4.7,-3.2,1.88);
                half3 vDir = GetWorldSpaceNormalizeViewDir(input.positionWS);
                half3 hDir = normalize(lDir + vDir);
                
                //Material Keywords
                half4 BaseColorAlpha = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,UV) ;
                half4 MRmap = SAMPLE_TEXTURE2D(_MetallicMap,sampler_MetallicMap,UV);
                half lip = MRmap.b;
                half3 BaseColor =( BaseColorAlpha.rgb * _BaseColor.rgb + _AddColor.rgb ) * (1-lip) + _LipColor * lip;
                half Metallic = MRmap.g * _Metallic;
                half smoothness = ((_Smoothness )*(1-lip) + _LipSmoothness * lip)+MRmap.a;
                half Roughness =saturate( 1 -smoothness );
                half3 nDirTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap,sampler_NormalMap,UV * _NormalMap_ST.xy + _NormalMap_ST.zw) ,_NormalIntensity) ;
                nDirWS = normalize(mul(nDirTS,TBN));
                half Occlusion = 1;
                #if defined (_EMISSION)
                    half4 EmissonMap = SAMPLE_TEXTURE2D(_EmissionMap,sampler_EmissionMap,UV);
                #else
                    half4 EmissonMap = half4(0,0,0,0);
                #endif
                half4 EmissonColor = EmissonMap* _EmissionStrength * _EmissColor;
                
                //BRDF
                half3 DiffuseColor = lerp(BaseColor,float3(0.0,0.0,0.0),Metallic);
                half3 SpecularColor = lerp(float3(0.04,0.04,0.04),BaseColor,Metallic);
                Roughness = max(Roughness,0.001f);
                
                //Dissolve
                float ObjToWorld = (1-(positionWS.y-mul( GetObjectToWorldMatrix(),float4(float3(0,0,0),1)).y)) - (-3.0 + ((1-_DissolveAmount) - 1.0) * (0.5 - -3.0) - _DissolveOffset) * rcp(_DissolveSpread);
                half dissolveStep = smoothstep(0.8,1.0,ObjToWorld);
                half dissoloveNoise = SAMPLE_TEXTURE2D(_NoiseMap,sampler_NoiseMap, positionWS * _NoiseScale.xyz).r;
                half dissolve = clamp((dissolveStep + (ObjToWorld - dissoloveNoise)),0.0,1.0);

                //EdgeColor
                half edge =pow((1- distance(ObjToWorld,_DissloveEdgeOffset)),3);
                half3 edgeColor = smoothstep(0.0,1.0,(edge - dissoloveNoise)) * _EdgeEmissColor.rgb;

                //blinPhong
                half shininess = lerp(1,_SpecShininess,smoothness);
                half NdotH = saturate(dot(nDirWS, hDir));
                half blinPhong = pow(max(0.0, NdotH),shininess *smoothness) * lip * _SpecIntensity;

                 //----directLight----
                half3 DirectLighting = half3(0,0,0);
                DirectLightingMoblie_float(DiffuseColor,SpecularColor,Roughness,_SpecShininess,nDirWS,lDir,hDir,_HightColor,_DarkColor,_DirLightIntensity,DirectLighting);
                
                //----IndirectLight----
                half3 IndirectLighting = half3(0,0,0);
                IndirectLighting_float(DiffuseColor,SpecularColor,Roughness,positionWS,nDirWS,vDirWS,Occlusion,0,IndirectLighting);
                half4 color = half4(lerp(0,0.3,DirectLighting) +lerp(0,0.7,IndirectLighting) +edgeColor +EmissonColor +blinPhong ,dissolve);
                //half4 color = half4(lerp(0,0.3,DirectLighting),1);
                clip(color.a - _CutOff);
                return color;
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor "LWGUI.LWGUI"
}
