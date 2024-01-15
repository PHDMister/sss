Shader "URP/Furniture/PanoramaMirror"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.CullMode)]_CullMode("Cull Mode", Float) = 2.0
        [Header(Surface Inputs)]
        [Space(10)]
        //[Toggle(_BASEMAP)] _Base("BaseMap Toggle", Float) = 1.0
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        [MainColor] _BaseColor("Base Color", Color) = (1,1,1,1)
       // _Metallic("Metallic",Range(0.0, 1.5)) = 0.0
        //_Smoothness("Smoothness", Range(0.0, 2.0)) = 0.0
        
         [Space(10)]
        [Main(Reflection, _, off, off)] _Reflection ("反射", Float) = 1
        [Space(10)]
        //[Sub(Reflection)]_Matcap("Matcap", 2D) = "white" {}
        [Sub(Reflection)]_ReflectMap("反射Matcap", 2D) = "white" {}
        [Sub(Reflection)]_Center("反射中心坐标（XYZ)", Vector) = (0,0,0,0)
        [Sub(Reflection)]_RV ("Rotate Vector" , Vector) = (1, 0, 0, 0)
        [Sub(Reflection)]_ReflectionIntensity("反射强度", Float) = 1
        
        [Space(10)]
        [Main(Fresnel, _, off, off)] _Fresnel ("Fresnel 效应", Float) = 1
        [Space(10)]
        [Sub(Fresnel)]_FresnelScale("Fresnel强度", Float) = 1
		[Sub(Fresnel)]_FresnelBias("Fresnel偏差", Float) = 0
		[Sub(Fresnel)]_FresnelPower("Fresnel次幂", Float) = 3
    }

    SubShader
    {
        Tags{"RenderType" = "Opaque" "Queue"="Geometry +100" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit" }
        Pass
        {
            Tags{"LightMode" = "UniversalForward"}
            Cull[_CullMode]
            HLSLPROGRAM
            #pragma target 3.0
            
            // -------------------------------------
            // Universal Pipeline keywords
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
            #pragma multi_compile _ LIGHTMAP_ON
            
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
                 float3 positionOS               : TEXCOORD4;
                half2 staticLightmapUV : TEXCOORD5;
                float4 positionCS               : SV_POSITION;
            };
            //#if defined (_BASEMAP)
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            //#endif
            #if defined (_NORMAL)
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            #endif
            #if defined (_EMISSION)
            TEXTURE2D(_EmissionMap);
            SAMPLER(sampler_EmissionMap);
            #endif
             #if defined (_CUSTOMREFLECTION)
            TEXTURECUBE(_ReflectionCubeMap);
            SAMPLER(sampler_ReflectionCubeMap);
             #endif
            TEXTURE2D(_ReflectMap);
            SAMPLER(sampler_ReflectMap);
             CBUFFER_START(UnityPerMaterial)
                
            float4 _ReflectionCubeMap_HDR;
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
            half3 _Center;
            half _ReflectionIntensity;
                half _FresnelScale;
                half _FresnelBias;
                half _FresnelPower;
            float4 _RV;
           
            CBUFFER_END  

            // Used in Standard (Physically Based) shader
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
            
            // Used in Standard (Physically Based) shader
            half4 LitPassFragment(Varyings input) : SV_Target
            {
                 float3 positionWS = input.positionWS;
                 float3 postionOS = input.positionOS;
                 half3 vDirWS = GetWorldSpaceNormalizeViewDir(positionWS);
                 half3 nDirWS = normalize(input.normalWS);

                //Rotation
                float4 RV = float4(normalize(_RV.xyz) , _RV.w);
				float sinR = sin(_RV.w);
				float cosR = cos(_RV.w);
				float FcosR = 1 - cosR;
				float3 R1 = float3(RV.x * RV.x * FcosR + cosR , RV.x * RV.y * FcosR - RV.z * sinR , RV.x * RV.z * FcosR + RV.y * sinR );
				float3 R2 = float3(RV.x * RV.y * FcosR + RV.z * sinR , RV.y * RV.y * FcosR + cosR , RV.y * RV.z * FcosR - RV.x * sinR );
				float3 R3 = float3(RV.x * RV.z * FcosR - RV.y * sinR , RV.y * RV.z * FcosR + RV.x * sinR , RV.z * RV.z * FcosR + cosR );
                positionWS -= postionOS;
				positionWS = float3 (dot(positionWS, R1), dot(positionWS, R2), dot(positionWS, R3));
				positionWS += postionOS;
                 half3 sphericalPos = normalize(mul(GetObjectToWorldMatrix(),float4(postionOS - positionWS - _Center,0)));
                 half2 cubeMapUV = (mul(UNITY_MATRIX_V,float4(sphericalPos,0.0))).xy * 0.5 + 0.5;
                 half3 Reflection = SAMPLE_TEXTURE2D(_ReflectMap,sampler_ReflectMap,cubeMapUV).xyz * _ReflectionIntensity;
              
                
                //half MatCap = SAMPLE_TEXTURE2D(_Matcap,sampler_Matcap,cubeMapUV).r;
                //half MatCapPow = MatCap * MatCap * MatCap * MatCap;
               // half GlassMask = SAMPLE_TEXTURE2D(_GlassMask,sampler_GlassMask,uv).r;

                //Fresnel
                half ndotv = dot(nDirWS,vDirWS);
                half fresnel = pow(max(1.0 - ndotv , 0.0001),_FresnelPower) * _FresnelScale + _FresnelBias;
                
                //half alpha = clamp(max(MatCapPow,GlassMask) * _Alpha  + fresnel,0.0,1.0);
                half3 baseColor = Reflection * _BaseColor.rgb  + (fresnel ) * (1 - Reflection * _BaseColor.rgb) ;
                //baseColor = LinearToGamma22(baseColor);
                return half4(baseColor.rgb,1);
               //return color;
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
