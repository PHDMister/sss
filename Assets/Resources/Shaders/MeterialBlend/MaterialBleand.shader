Shader "URP/MaterialBlend"
{
    Properties
    {
        
        [Space(10)]
        [Enum(UnityEngine.Rendering.CullMode)]_CullMode("剔除模式", Float) = 2.0
        _BlendContrast("混合强度", Range( 0 , 1)) = 0.1
        [Space(10)]
        
        [Main(Layer1, _, off, off)] _Layer1 ("Layer1", Float) = 1
        [Sub(Layer1)]_Layer1_Tilling("Tilling", Float) = 1
        [Sub(Layer1)]_Layer1_BaseMap("基础贴图", 2D) = "black" {}
        [Sub(Layer1)]_Layer1_BaseColor("Albedo 颜色", Color) = (1,1,1,1)
        [Sub(Layer1)]_Layer1_addColor("叠加颜色", Color) = (0,0,0,0)
        [Sub(Layer1)]_Layer1_Normal("法线", 2D) = "bump" {}
        [Sub(Layer1)]_Layer1_NormalIntensity("法线强度", Float) = 1.0
        [Sub(Layer1)]_Layer1_Height("高度图", 2D) = "black" {}
        [Sub(Layer1)]_Layer1_HeightContrast("高度对比", Range( 0 , 1)) = 0
        [Sub(Layer1)]_Layer1_MS("金属度（R）光滑度(A)贴图", 2D) = "black" {}
        
        [Space(10)]
        
        [Main(Layer2, _, off, off)] _Layer2 ("Layer2", Float) = 1
        [Sub(Layer2)]_Layer2_Tilling("Tilling", Float) = 1
        [Sub(Layer2)]_Layer2_BaseMap("基础贴图", 2D) = "black" {}
        [Sub(Layer2)]_Layer2_BaseColor("Albedo 颜色", Color) = (1,1,1,1)
        [Sub(Layer2)]_Layer2_addColor("叠加颜色", Color) = (0,0,0,0)
        [Sub(Layer2)]_Layer2_Normal("法线", 2D) = "bump" {}
        [Sub(Layer2)]_Layer2_NormalIntensity("法线强度", Float) = 1.0
        [Sub(Layer2)]_Layer2_Height("高度图", 2D) = "black" {}
        [Sub(Layer2)]_Layer2_HeightContrast("高度对比", Range( 0 , 1)) = 0
        [Sub(Layer2)]_Layer2_MS("金属度（R）光滑度(A)贴图", 2D) = "black" {}
        
        [Space(10)]
        [Main(Layer3,_, off, off)] _Layer3 ("Layer3", Float) = 0
        [Sub(Layer3)]_Layer3_Tilling("Tilling", Float) = 1
        [Sub(Layer3)]_Layer3_BaseMap("基础贴图", 2D) ="black" {}
        [Sub(Layer3)]_Layer3_BaseColor("Albedo 颜色", Color) = (1,1,1,1)
        [Sub(Layer3)]_Layer3_addColor("叠加颜色", Color) = (0,0,0,0)
        [Sub(Layer3)]_Layer3_Normal("法线", 2D) = "bump" {}
        [Sub(Layer3)]_Layer3_NormalIntensity("法线强度", Float) = 1.0
        [Sub(Layer3)]_Layer3_Height("高度图", 2D) = "black" {}
        [Sub(Layer3)]_Layer3_HeightContrast("高度对比", Range( 0 , 1)) = 0
        [Sub(Layer3)]_Layer3_MS("金属度（R）光滑度(A)贴图", 2D) = "black" {}
        
        
        [Space(10)]
        [Main(Lighting, _, off, off)] _Lighting ("光照", Float) = 1
        [Space(10)]
        [Sub(Lighting)]_LightIntensitiy("烘培光照强度",Range(0.0, 2.0)) = 1.0
        [SubToggle(Lighting,_ENVIRONMENTREFLECTIONS_OFF)]_EnvironmentReflections("环境反射", Float) = 0.0
       
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "RenderPipeline" = "UniversalPipeline"}
        Pass
        {
           Tags{"LightMode" = "UniversalForward"}
            Cull[_CullMode]
            ZWrite On
           HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
           //#pragma shader_feature_local _LAYER3
           #pragma multi_compile _ FOG_LINEAR
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Core.hlsl"
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Lighting.hlsl"
            #include "Assets/Resources/Shaders/PBR/FurnitureLighting.hlsl"
           struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                float2 texcoord     : TEXCOORD0;
            half2 staticLightmapUV : TEXCOORD1;
                half4 vertexColor :  COLOR;
            };
            struct Varyings
            {
                float2 uv                       : TEXCOORD0;
                float3 positionWS               : TEXCOORD1;
                float3 normalWS                 : TEXCOORD2;
                half4 tangentWS                : TEXCOORD3;
                float3 positionOS               : TEXCOORD4;
                float4 positionCS               : SV_POSITION;
              half2 staticLightmapUV : TEXCOORD5;
                half4 vertexColor :  TEXCOORD6;
            };
            
            TEXTURE2D(_Layer1_BaseMap);
            SAMPLER(sampler_Layer1_BaseMap);
            TEXTURE2D(_Layer1_Height);
            SAMPLER(sampler_Layer1_Height);
            TEXTURE2D(_Layer1_MS);
            SAMPLER(sampler_Layer1_MS);
           TEXTURE2D(_Layer1_Normal);
            SAMPLER(sampler_Layer1_Normal);

            TEXTURE2D(_Layer2_BaseMap);
            SAMPLER(sampler_Layer2_BaseMap);
            TEXTURE2D(_Layer2_Height);
            SAMPLER(sampler_Layer2_Height);
            TEXTURE2D(_Layer2_MS);
            SAMPLER(sampler_Layer2_MS);
           TEXTURE2D(_Layer2_Normal);
            SAMPLER(sampler_Layer2_Normal);

            TEXTURE2D(_Layer3_BaseMap);
            SAMPLER(sampler_Layer3_BaseMap);
            TEXTURE2D(_Layer3_Height);
            SAMPLER(sampler_Layer3_Height);
            TEXTURE2D(_Layer3_MS);
            SAMPLER(sampler_Layer3_MS);
           TEXTURE2D(_Layer3_Normal);
            SAMPLER(sampler_Layer3_Normal);
           
            CBUFFER_START(UnityPerMaterial)
                half _Layer1_Tilling;
			             half _Layer1_HeightContrast;
			             half _Layer2_HeightContrast;
			             half _Layer2_Tilling;
			             half _Layer3_HeightContrast;
			             half _Layer3_Tilling;
			             half _BlendContrast;
                       half _Layer1_NormalIntensity;
                       half _Layer2_NormalIntensity;
                       half _Layer3_NormalIntensity;
                       half _LightIntensitiy;
                        half4 _Layer1_addColor;
                        half4 _Layer1_BaseColor;
                         half4 _Layer2_addColor;
                        half4 _Layer2_BaseColor;
                        half4 _Layer3_addColor;
                        half4 _Layer3_BaseColor;
            CBUFFER_END  
            
            float CheapContrast( float BlendContrast, float Input )
            {
	                float a =clamp(lerp(( 0 -  BlendContrast), (BlendContrast + 0 ) ,Input),0,1);
	                
	                return a;
            }
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
                output.positionOS = input.positionOS;
                output.vertexColor = input.vertexColor;
                output.staticLightmapUV = input.staticLightmapUV * unity_LightmapST.xy + unity_LightmapST.zw;
                return output;
            }

            half4 LitPassFragment (Varyings input) : SV_Target
            {

                float3 positionWS = input.positionWS;
                half3 vDirWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
                half3 nDirWS = normalize(input.normalWS);
                half3 tDirWS = normalize(input.tangentWS.xyz);
                half3 bDirWS = normalize(cross(nDirWS,tDirWS) * input.tangentWS.w);
                half3x3 TBN  = half3x3(tDirWS,bDirWS,nDirWS);
                //uv
                half2 uv  = input.uv;
                half2 layer1uv = uv * _Layer1_Tilling;
                half2 layer2uv = uv * _Layer2_Tilling;
                half2 layer3uv = uv * _Layer3_Tilling; 

                //layer1
                half4 layer1BaseColor = SAMPLE_TEXTURE2D(_Layer1_BaseMap,sampler_Layer1_BaseMap,layer1uv) * _Layer1_BaseColor + _Layer1_addColor;
                half layer1Height = SAMPLE_TEXTURE2D(_Layer1_Height,sampler_Layer1_Height,layer1uv).r;
                layer1Height = CheapContrast(_Layer1_HeightContrast,layer1Height);
                half layer1Smoothness   = SAMPLE_TEXTURE2D(_Layer1_MS,sampler_Layer1_MS,layer1uv).a;
                half3 layer1nDirTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_Layer1_Normal,sampler_Layer1_Normal,layer1uv) ,_Layer1_NormalIntensity) ;
                layer1nDirTS = normalize(mul(layer1nDirTS,TBN));

                 //layer2
                half4 layer2BaseColor = SAMPLE_TEXTURE2D(_Layer2_BaseMap,sampler_Layer2_BaseMap,layer2uv)* _Layer2_BaseColor + _Layer2_addColor;
                half layer2Height = SAMPLE_TEXTURE2D(_Layer2_Height,sampler_Layer2_Height,layer2uv).r;
                layer2Height = CheapContrast(_Layer2_HeightContrast,layer2Height);
                half layer2Smoothness   = SAMPLE_TEXTURE2D(_Layer2_MS,sampler_Layer2_MS,layer2uv).a;
                half3 layer2nDirTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_Layer2_Normal,sampler_Layer2_Normal,layer2uv) ,_Layer2_NormalIntensity) ;
                layer2nDirTS = normalize(mul(layer2nDirTS,TBN));

                 //layer1
                half4 layer3BaseColor = SAMPLE_TEXTURE2D(_Layer3_BaseMap,sampler_Layer3_BaseMap,layer3uv)* _Layer3_BaseColor + _Layer3_addColor;
                half layer3Height = SAMPLE_TEXTURE2D(_Layer3_Height,sampler_Layer3_Height,layer3uv).r;
                layer3Height = CheapContrast(_Layer3_HeightContrast,layer3Height);
                half layer3Smoothness   = SAMPLE_TEXTURE2D(_Layer3_MS,sampler_Layer3_MS,layer3uv).a;
                half3 layer3nDirTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_Layer3_Normal,sampler_Layer3_Normal,layer3uv) ,_Layer3_NormalIntensity) ;
                layer3nDirTS = normalize(mul(layer3nDirTS,TBN));
                
                //BlendFactor
                 half layer1Factor = input.vertexColor.r + layer1Height;
                 half layer2Factor = input.vertexColor.g + layer2Height;
                 half layer3Factor = input.vertexColor.b + layer3Height;
                 half subtractFactor =  max(max(layer1Factor,layer2Factor),layer3Factor) - _BlendContrast;
                 half layer1 = max((layer1Factor - subtractFactor),0);
                 half layer2 = max((layer2Factor - subtractFactor),0);
                 half layer3 = max((layer3Factor - subtractFactor),0);
                 half3 blendFactor = half3(layer1,layer2,layer3) / (layer1+layer2+layer3);
             
                //Material Keywords
                //#if defined (_LAYER3)
                half4 BaseColor = layer1BaseColor * blendFactor.x + layer2BaseColor * blendFactor.y + layer3BaseColor * blendFactor.z;
               // #else
                //half4 BaseColor = layer1BaseColor * blendFactor.x + layer2BaseColor * blendFactor.y + blendFactor.z;
                //#endif
                //#if defined (_LAYER3)
                half Roughness = saturate(1-(layer1Smoothness * blendFactor.x + layer2Smoothness * blendFactor.y + layer3Smoothness * blendFactor.z));
                // #else
                // half Roughness = saturate(1-(layer1Smoothness * blendFactor.x + layer2Smoothness * blendFactor.y ));
                // #endif

                //#if defined (_LAYER3)
                half3 normal = layer1nDirTS * blendFactor.x  + layer2nDirTS  * blendFactor.y+ layer3nDirTS *  blendFactor.z ;
                // #else
                // half3 normal = layer1nDirTS * blendFactor.x  + layer2nDirTS  * blendFactor.y ;
                // #endif
                half Occlusion = 1;
                //half Metallic = 0;
                 //BRDF
                half3 DiffuseColor = BaseColor;
                half3 SpecularColor = float3(0.04,0.04,0.04);
                Roughness = max(Roughness,0.001f);

                //fog
                float csz = input.positionCS.z * input.positionCS.w; 
                real fogFactor = ComputeFogFactor(csz);
               //IndirectLight
                half3 IndirectLighting = half3(0,0,0);
                IndirectLighting_float(DiffuseColor,SpecularColor,Roughness,positionWS,normal,vDirWS,Occlusion,input.staticLightmapUV,_LightIntensitiy,IndirectLighting);
                half4 color = half4(IndirectLighting,1);
                color.rgb = MixFog(color.rgb,fogFactor);
                return color;
            }   
            ENDHLSL
        }
     Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma target 2.0

            //--------------------------------------
            // GPU Instancing
            //#pragma multi_compile_instancing

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            //#pragma shader_feature_local_fragment _ALPHATEST_ON
            //#pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            #include "Assets/Resources/Shaders/PBR/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
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