Shader "URP/Char/KKHairDissolve"
{
    Properties
    {
        [Space(10)]
        [Enum(UnityEngine.Rendering.CullMode)]_CullMode("剔除模式", Float) = 2.0
        _CutOff("裁剪阈值", Range(0.0, 1.0)) = 0.826
        
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
		 [Sub(VertexDissolve)]_VertexDissolveOffset("偏移值", Float) = -4
		 [Sub(VertexDissolve)]_VertexOffsetIntensity("强度", Float) = 1
        
        [Space(10)]
         [Main(KK, _, off, off)] _KK ("各向异性", Float) = 1
         [Space(10)]
        [Sub(KK)]_ShiftMap ("Shift图", 2D) = "white" {}
        [Sub(KK)]_ShiftOffset("Shift偏移值",Range(-1,1)) = 0.0
        [Sub(KK)]_ShiftIntensity("Shift强度",Float) = 1.0
        [Sub(KK)]_SpecIntensity("高光强度",Float) = 1.0
        [Sub(KK)]_Shininess("高光度",Range(0.01,100)) = 1.0
      
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
                half _SpecShininess;
                half _SpecIntensity;
                half _Shininess;
                half _ShiftIntensity;
                half _ShiftOffset;
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
                half lDir = half3(0.3,-0.1,0);
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
                
                //Dissolve
                half ObjToWorld = (1-(positionWS.y-mul( GetObjectToWorldMatrix(),float4(float3(0,0,0),1)).y)) - (-3.0 + ((1-_DissolveAmount) - 1.0) * (0.5 - -3.0) - _DissolveOffset) * rcp(_DissolveSpread);
                half dissolveStep = smoothstep(0.8,1.0,ObjToWorld);
                half dissoloveNoise = SAMPLE_TEXTURE2D(_NoiseMap,sampler_NoiseMap, positionWS * _NoiseScale.xyz).r;
                half dissolve = clamp((dissolveStep + (ObjToWorld - dissoloveNoise)),0.0,1.0);

                //EdgeColor
                half edge =pow((1- distance(ObjToWorld,_DissloveEdgeOffset)),3);
                half3 edgeColor = smoothstep(0.0,1.0,(edge - dissoloveNoise)) * _EdgeEmissColor.rgb;
                
                //----IndirectLight----
                half3 IndirectLighting = half3(0,0,0);
                IndirectLightingHair_float(DiffuseColor,SpecularColor,Roughness,positionWS,nDirWS,vDirWS,Occlusion,0,IndirectLighting);
                // kk
                half2 anisoDir = 1 *2.0 -0.5 ;
                bDirWS = normalize(tDirWS * anisoDir.r + bDirWS *anisoDir.g);
                half2 uvShift = UV * _ShiftMap_ST.xy +_ShiftMap_ST.zw;
                half shiftnosie = SAMPLE_TEXTURE2D(_ShiftMap,sampler_ShiftMap,uvShift).r;
                shiftnosie = (shiftnosie * 2.0 - 1.0) * _ShiftIntensity;
                half3 bOffset = nDirWS *(_ShiftOffset + shiftnosie);
                bDirWS = normalize(bDirWS +bOffset);
                half TdotH = dot(bDirWS,hDir);
                half sinTH =sqrt(1 - TdotH * TdotH);
                half3 specColor = pow(max(0.0,sinTH),_Shininess)*_SpecIntensity;
                
                half4 color = half4(IndirectLighting +edgeColor +specColor ,dissolve);
                clip(color.a - _CutOff);
                return color;
               
            }
            ENDHLSL
        }


    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor "LWGUI.LWGUI"
}
