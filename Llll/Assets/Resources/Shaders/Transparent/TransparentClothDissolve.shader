Shader "URP/Transparent/Cloth"
{
    Properties
    {
        [Space(10)]
        [Enum(UnityEngine.Rendering.CullMode)]_CullMode("剔除模式", Float) = 2.0
        _CutOff("裁剪阈值", Range(0.0, 1.0)) = 0.7
        
        [Space(10)]
        [Main(Base,_, off, off)] _Base ("PBR属性", Float) = 1.0
        [Space(10)]
        [Sub(Base)][MainTexture] _BaseMap("基础贴图", 2D) = "white" {}
        [Sub(Base)][MainColor] _BaseColor("基础颜色", Color) = (1,1,1,1)
        [Sub(Base)]_Alpha("透明度",Range(0.0, 1.5)) = 1.0
        [Sub(Base)]_Roughness("粗糙度", Range(0.0, 1.0)) = 1.0
        [Sub(Base)]_SpecShininess("光泽度",Range(0.0, 20.0)) = 10.0
        
//         [Space(10)]
//         [Main(Dissolve, _, off, off)] _Dissolve ("溶解", Float) = 1
//         [Space(10)]
//         [Sub(Dissolve)]_DissolveAmount("溶解数量", Range( 0 , 1.5)) = 1.5
//         [Sub(Dissolve)]_DissolveOffset("偏移值", Float) = 0
//		 [Sub(Dissolve)]_DissolveSpread("扩散值", Float) = 0
//         [Sub(Dissolve)]_DissloveEdgeOffset("边缘偏移", Float) = 0
//		 [Sub(Dissolve)][HDR]_EdgeEmissColor("边缘颜色", Color) = (0.003114995,0.6603774,0.6381032,0)
//         [Sub(Dissolve)]_NoiseMap("噪声图", 2D) = "white" {}
//         [Sub(Dissolve)]_NoiseScale("噪声强度（XYZ)", Vector) = (1,1,1,0)
//        
//         [Space(10)]
//         [Main(VertexDissolve, _, off, off)] _VertexDissolve ("顶点溶解", Float) = 1
//         [Space(10)]
//         [Sub(VertexDissolve)]_VertexDissolveSpread("扩散值", Float) = 3
//		 [Sub(VertexDissolve)]_VertexDissolveOffset("偏移值", Float) = -1
//		 [Sub(VertexDissolve)]_VertexOffsetIntensity("强度", Float) = 45
        
        [Space(10)]
        [Main(Lighting, _, off, off)] _Lighting ("光照", Float) = 1
        [Sub(Lighting)]_DirLightIntensity("直接光强度", Float) = 0.2
        [Sub(Lighting)][HDR]_HightColor("直接光亮部颜色", Color) = (1,1,1,1)
        [Sub(Lighting)]_DarkColor("间接光暗部颜色", Color) = (0.5,0.5,0.5,1)
        
        _float_tooltip ("提示#PBR光照，不支持normal图，MS图；不支持烘培光照；透明；裁剪；溶解；顶点偏移", float) = 1.0 
    }

    SubShader
    {
        Tags{"RenderType" = "Transparent" "Queue"="Transparent+350" "RenderPipeline" = "UniversalPipeline" }
        Pass
        {
            Tags{"LightMode" = "UniversalForward"}
            Cull[_CullMode]
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            HLSLPROGRAM
            #pragma target 3.0
            // -------------------------------------
            // Universal Pipeline keywords
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Core.hlsl"
           #include "Assets/Resources/Shaders/PBR/LightingCustom.hlsl"
            //#include "Assets/Resources/Shaders/PBR/FurnitureLighting.hlsl"
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
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_NoiseMap);
            SAMPLER(sampler_NoiseMap);
            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half _Roughness;
                half _Alpha;
                // half _DissolveAmount;
                // half _DissolveOffset;
                // half _DissolveSpread;
                // half _DissloveEdgeOffset;
                // half _VertexDissolveSpread;
                // half _VertexDissolveOffset;
                // half _VertexOffsetIntensity;
                // half4 _EdgeEmissColor;
                // half4 _NoiseScale;
                // half _CutOff;
                half _SpecShininess;
            half4 _HightColor;
            half4 _DarkColor;
            half _DirLightIntensity;
            CBUFFER_END  

            // Used in Standard (Physically Based) shader
            Varyings LitPassVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                //VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                output.uv = input.texcoord;
                output.normalWS = normalInput.normalWS;         
                real sign = input.tangentOS.w * GetOddNegativeScale();
                half4 tangentWS = half4(normalInput.tangentWS.xyz, sign);
                output.tangentWS = tangentWS;
                // float3 positionWS = mul(GetObjectToWorldMatrix(), input.positionOS).xyz;
                // half3 vertexOffset = max(((positionWS.y + (-3.0 + ((1-_DissolveAmount) - 1.0) * (0.5 - -3.0) )) - _VertexDissolveOffset) / _VertexDissolveSpread,0);
                // half4 vertexOffset1 = half4(half3(0,1,0) * _VertexOffsetIntensity * vertexOffset + positionWS,1);
                // half3 vertexOffset2 = mul(GetWorldToObjectMatrix(),vertexOffset1).xyz - input.positionOS.xyz;
                // input.positionOS.xyz  +=vertexOffset2 ;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionWS = vertexInput.positionWS;
                output.positionCS = vertexInput.positionCS;
                return output;
            }
            
            // Used in Standard (Physically Based) shader
            half4 LitPassFragment(Varyings input) : SV_Target
            {
                //input data
                half2 UV  = input.uv;
                float3 positionWS = input.positionWS;
                half3 vDirWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
                half3 nDirWS = normalize(input.normalWS);
                half3 lDir = half3(0.2,0.2,-0.1);
                half3 hDir = normalize(lDir + vDirWS);
                
                //Material Keywords
                half4 BaseColorAlpha = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,UV) ;
                half3 BaseColor = BaseColorAlpha.rgb * _BaseColor.rgb;
                half Metallic = 0.2;
                half Roughness =  _Roughness;
                half Occlusion = 1;
                //BRDF
                float3 DiffuseColor = lerp(BaseColor,float3(0.0,0.0,0.0),Metallic);
                float3 SpecularColor = lerp(float3(0.04,0.04,0.04),BaseColor,Metallic);
                Roughness = max(Roughness,0.001f);
                //  //Dissolve
                // float ObjToWorld = (1-(positionWS.y-mul( GetObjectToWorldMatrix(),float4(float3(0,0,0),1)).y)) - (-3.0 + ((1-_DissolveAmount) - 1.0) * (0.5 - -3.0) - _DissolveOffset)/_DissolveSpread;
                // half dissolveStep = smoothstep(0.8,1.0,ObjToWorld);
                // half dissoloveNoise = SAMPLE_TEXTURE2D(_NoiseMap,sampler_NoiseMap, positionWS * half3(2,2,2)).r;
                // half dissolve = clamp((dissolveStep + (ObjToWorld - dissoloveNoise)),0.0,1.0);
                // //EdgeColor
                // half edge =pow((1- distance(ObjToWorld,_DissloveEdgeOffset)),5);
                // half3 edgeColor = smoothstep(0.0,1.0,(edge - dissoloveNoise)) * _EdgeEmissColor.rgb;


                  //----directLight----
                half3 DirectLighting = half3(0,0,0);
                DirectLightingMoblie_float(DiffuseColor,SpecularColor,Roughness,_SpecShininess,nDirWS,lDir,hDir,_HightColor,_DarkColor,_DirLightIntensity,DirectLighting);
                //----IndirectLight----
                half3 IndirectLighting = half3(0,0,0);
                IndirectLighting_float(DiffuseColor,SpecularColor,Roughness,positionWS,nDirWS,vDirWS,Occlusion,0,IndirectLighting);
                half BaseAlpha = clamp(_BaseColor.a * _Alpha * BaseColorAlpha.a,0.0,1.0);
                half4 color = half4(DirectLighting* IndirectLighting  ,BaseAlpha);
                //half4 color = half4(DirectLighting*1.1,1);
                //clip(color.a - _CutOff);
                return color;
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor "LWGUI.LWGUI"
}
