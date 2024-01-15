Shader "URP/Plants/BillBorad"
{
    Properties
    {
        [Space(10)]
        [Enum(UnityEngine.Rendering.CullMode)]_CullMode("剔除模式", Float) = 2.0
        _Cutoff("裁剪阈值", Range(0.0, 1.0)) =0.5
        
        [Space(10)]
        [Main(PBRIntput, _, off, off)] _PBRIntput ("基础", Float) = 1
        [MainTexture][Sub(PBRIntput)] _BaseMap("基础贴图", 2D) = "white" {}
        //[MainColor][Sub(PBRIntput)] _BaseColor("基础颜色", Color) = (1,1,1,1)
    	[Sub(PBRIntput)] _TopColor("顶部颜色", Color) = (1,1,1,1)
        [Sub(PBRIntput)] _ButtomColor("底部颜色", Color) = (0,0,0,0)
        [Space(10)]
        [Main(BillBoard, _BILLBOARD, off)] _BillBoard ("朝向摄像机", Float) = 1
        [Space(10)]
        [Sub(BillBoard)]_VerticalBillboarding("VerticalBillboarding", Range(0.0, 1.0)) =1.0
        
         [Space(10)]
        [Main(Wind, _WIND,off)] _Wind ("风", Float) = 1
        [Space(10)]
        [Sub(Wind)]_WindIntensity("WindIntensity", Float) = 0.2
		[Sub(Wind)]_WindSpeed("Wind Speed", Float) = 0.5
		[Sub(Wind)]_WindDirection("Wind Direction", Range( 0 , 360)) = 45
		[Sub(Wind)]_WindSizeBig("Wind Size Big", Float) = 10
		[Sub(Wind)]_WindSizeSmall("Wind Size Small", Float) = 4
		[Sub(Wind)]_WindStetch("Wind Stetch", Vector) = (0,1,0,0) 
        _float_tooltip ("提示#公告板；风力顶点动画", float) = 1
    }

    SubShader
    {
        Tags{"RenderType" = "Opaque" "Queue"="AlphaTest" "RenderPipeline" = "UniversalPipeline" }
        Pass
        {
            Tags{"LightMode" = "UniversalForward"}
            Cull[_CullMode]
            HLSLPROGRAM
            #pragma target 3.0
            
            // -------------------------------------
            // Universal Pipeline keywords
            #pragma multi_compile _ FOG_LINEAR
            //#pragma multi_compile _ LIGHTMAP_ON
             #pragma shader_feature_local _BILLBOARD
             #pragma shader_feature_local _WIND

            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Core.hlsl"
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/UnityInput.hlsl"
           // #include "Assets/Resources/Shaders/PBR/FurnitureLighting.hlsl"
            #include "Assets/Shaders/Water/GrassWind.hlsl"

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
                float4 positionCS               : SV_POSITION;
            };
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            CBUFFER_START(UnityPerMaterial)
           
            half4 _BaseMap_ST;
            half4 _EmissionMap_ST;
            half4 _DetailAlbedoMap_ST;
            half4 _NormalMap_ST;
            half4 _BaseColor;
            half4 _SpecColor;
            half4 _EmissColor;
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
            half _NormalIntensity;
            half _LightIntensitiy;
            half _VerticalBillboarding;
            float _WindLineRotate;
			float _WindLineScale;
			float _TipGradient;
			float _WindSpeed;
			float _IndirectLightIntensity;
			float _UseTexColor;
			float _FixedNormal;
			float _WindIntensity;
			float _WindSizeSmall;
			float _WindSizeBig;
			float _WindDirection;
            float2 _WindLineSpeed;
            float3 _WindStetch;
            float4 _WindLineColor;
            half3 _TopColor;
            half3 _ButtomColor;
            CBUFFER_END
           
            // Used in Standard (Physically Based) shader
            Varyings LitPassVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
            	#if defined (_BILLBOARD)
                //billborad
				float3 center = float3(0, 0, 0);
				float3 viewer = mul(unity_WorldToObject,float4(_WorldSpaceCameraPos,1));
				float3 normalDir = viewer - center;
				normalDir.y =normalDir.y * _VerticalBillboarding;
				normalDir = normalize(normalDir);
				float3 upDir = abs(normalDir.y) > 0.999 ? float3(0, 0, 1) : float3(0, 1, 0);
				float3 rightDir = normalize(cross(upDir, normalDir));     upDir = normalize(cross(normalDir, rightDir));
				float3 centerOffs = input.positionOS.xyz - center;
				float3 localPos = center + rightDir * centerOffs.x + upDir * centerOffs.y + normalDir * centerOffs.z;
            	half3 positonWSnew =  mul(GetObjectToWorldMatrix(), half4(localPos,1)).xyz;
            	#else
            	half3 positonWSnew =  mul(GetObjectToWorldMatrix(), input.positionOS).xyz;
            	float3 localPos = input.positionOS.xyz;
            	#endif
            	
            	//wind
            	#if defined (_WIND)
            	half windGradient =  input.texcoord.y;
                half windWeight = windGradient * windGradient;
            	half3 windStetch = normalize(half3(_WindStetch.x,1,_WindStetch.z));
            	half3 windPostion = half3(windStetch.x ,0,windStetch.z) * windWeight + positonWSnew;
            	half3 grassWind = GrassWind(windWeight,_WindIntensity,_WindSpeed,_WindDirection,_WindSizeBig,_WindSizeSmall,windPostion);
            	half3 wind = mul(GetWorldToObjectMatrix(),half4(grassWind + windPostion,1)).xyz  ;
				#else
            	half3 wind = input.positionOS.xyz;
            	#endif
            	#if defined (_BILLBOARD) && defined(_WIND)
            	half3 positionOS = localPos + wind;
            	#elif defined (_BILLBOARD) && !defined (_WIND)
            	half3 positionOS = localPos;
            	#elif !defined (_BILLBOARD) && defined (_WIND)
            	half3 positionOS = wind ;
            	#else
            	half3 positionOS = input.positionOS.xyz;
            	#endif
            	
            	//half3 positionOS = localPos + wind;
            	VertexPositionInputs vertexInput = GetVertexPositionInputs(positionOS.xyz);
            	VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
            	output.uv = input.texcoord;
            	output.normalWS = normalInput.normalWS;
            	output.positionWS = vertexInput.positionWS;
            	output.positionCS = vertexInput.positionCS;
                return output;
            }
            // Used in Standard (Physically Based) shader
            half4 LitPassFragment(Varyings input) : SV_Target
            {
                //Input data
                half2 UV  = input.uv;
                half4 BaseColorAlpha = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,UV* _BaseMap_ST.xy + _BaseMap_ST.zw) ;
                //fog
                float csz = input.positionCS.z * input.positionCS.w; 
                real fogFactor = ComputeFogFactor(csz);
                half4 color = half4(BaseColorAlpha.rgb * lerp(_ButtomColor,_TopColor,UV.y) ,BaseColorAlpha.a);
                color.rgb = MixFog(color.rgb,fogFactor);
                clip(BaseColorAlpha.a - _Cutoff);
                return color;
            }
             ENDHLSL
            }
//    	Pass
//        {
//            Name "Meta"
//            Tags{"LightMode" = "Meta"}
//
//            Cull Off
//
//            HLSLPROGRAM
//            #pragma exclude_renderers gles gles3 glcore
//            #pragma target 3.0
//
//            #pragma vertex UniversalVertexMeta
//            #pragma fragment UniversalFragmentMeta
//            #pragma shader_feature_local_fragment _SPECULAR_SETUP
//            #pragma shader_feature_local_fragment _EMISSION
//            #pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
//            #pragma shader_feature_local_fragment _ALPHATEST_ON
//            #pragma shader_feature_local_fragment _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
//            #pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED
//            #pragma shader_feature_local_fragment _SPECGLOSSMAP
//            #include "Assets/Resources/Shaders/PBR/LitInput.hlsl"
//            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitMetaPass.hlsl"
//            ENDHLSL
//        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor "LWGUI.LWGUI"
}
