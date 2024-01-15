Shader "URP/Plants/VCwind"
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
             #pragma shader_feature_local _BILLBOARD
             #pragma shader_feature_local _WIND

            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Core.hlsl"
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/UnityInput.hlsl"
           // #include "Assets/Resources/Shaders/PBR/FurnitureLighting.hlsl"
            #include "Assets/Resources/Shaders/Water/GrassWind.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                half2 texcoord     : TEXCOORD0;
            	half4 vectexColor : COLOR;
            };
            struct Varyings
            {
                half2 uv                       : TEXCOORD0;
                float3 positionWS               : TEXCOORD1;
                float3 normalWS                 : TEXCOORD2;
            	half4 vectexColor : TEXCOORD3;
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
            	half3 positonWSnew =  mul(GetObjectToWorldMatrix(), input.positionOS).xyz;
            	float3 localPos = input.positionOS.xyz;
            	//wind
            	#if defined (_WIND)
            	half windGradient =  input.vectexColor.r;
                half windWeight = windGradient * windGradient;
            	half3 windStetch = normalize(half3(_WindStetch.x,1,_WindStetch.z));
            	half3 windPostion = half3(windStetch.x ,0,windStetch.z) * windWeight + positonWSnew;
            	half3 grassWind = GrassWind(windWeight,_WindIntensity,_WindSpeed,_WindDirection,_WindSizeBig,_WindSizeSmall,windPostion);
            	half3 wind = mul(GetWorldToObjectMatrix(),half4(grassWind + windPostion,1)).xyz  ;
				#else
            	half3 wind = input.positionOS.xyz;
            	#endif
            	VertexPositionInputs vertexInput = GetVertexPositionInputs(wind.xyz);
            	VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
            	output.uv = input.texcoord;
            	output.normalWS = normalInput.normalWS;
            	output.positionWS = vertexInput.positionWS;
            	output.positionCS = vertexInput.positionCS;
            	output.vectexColor = input.vectexColor;
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
                half4 color = half4(BaseColorAlpha.rgb * lerp(_ButtomColor,_TopColor,input.vectexColor.r) ,BaseColorAlpha.a);
                color.rgb = MixFog(color.rgb,fogFactor);
                clip(BaseColorAlpha.a - _Cutoff);
                return color;
            }
             ENDHLSL
            }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor "LWGUI.LWGUI"
}
