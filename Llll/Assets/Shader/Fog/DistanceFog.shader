Shader "URP/Fog/VolumetricFog"
{
	Properties 
	{
	    [HideInInspector]_MainTex ("Base (RGB)", 2D) = "white" {}
	    [Space(10)]
		[Toggle(DEBUG_OUTPUT)]_DEBUG ("Debug视图", Float) = 0
		[Toggle(FOG_ONLY_OUTPUT)]_FOG_ONLY ("雾效Only视图", Float) = 0
		[Space(10)]
		
		[Space(10)]
        [Main(Base,_, off, off)] _Base ("基础属性", Float) = 1.0
        [Space(10)]
		[Sub(Base)]_Presence ("浓度", Range(0, 1)) = 1 
		[Sub(Base)]_ScatteringTint ("散射颜色", Color) = (0.5,0.6,0.7)
		[Sub(Base)]_Scattering ("雾效散射度",  Range(0, 4)) = .5 
	    [Sub(Base)]_LightScatter  ("光照散射度",  Range(0, 1)) = 1 
		
		[Sub(Base)]_ExtinctionTint ("消光颜色", Color) = (0,0,0)
		[Sub(Base)]_Extinction ("消光度",  Range(0, 4)) = .5 

		[Space(10)]
        [Main(HEIGHT_FOG, _HEIGHT_FOG, off)] _HeightFog ("高度雾", Float) = 0
        [Space(10)]
        //[Sub(HEIGHT_FOG)]_HeightFogFloor ("Height Fog Floor", Range(0, 10)) = 0
		[Sub(HEIGHT_FOG)]_HeightFogDropoff ("衰减", Range(0, 10)) = 0.5

	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
            HLSLPROGRAM
            #define SHADOWS_SCREEN 1
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            #pragma shader_feature_local DEBUG_OUTPUT
            #pragma shader_feature_local FOG_ONLY_OUTPUT
            #pragma shader_feature_local _HEIGHT_FOG

            CBUFFER_START(UnityPerMaterial)
            float _Presence = 1;
            float _Extinction = 0.5f;
            float3 _ExtinctionTint = 0;
            float _Scattering = 0.5f;
            float3 _ScatteringTint = 1;
            float _LightScatter = 8;
            float _HeightFogDropoff = 1;
            int _SampleCount = 8;
            CBUFFER_END

            TEXTURE2D_X_FLOAT(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);
			#ifndef FOG_ONLY_OUTPUT
			            TEXTURE2D_X_FLOAT(_MainTex);
			            SAMPLER(sampler_MainTex);
			#endif
			#include "Fog.hlsl"
           

        struct Attributes
        {
            float4 positionOS   : POSITION;
            float2 texcoord : TEXCOORD0;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct Varyings
        {
            half4  positionCS   : SV_POSITION;
            half4  uv           : TEXCOORD0;
            UNITY_VERTEX_OUTPUT_STEREO
        };
           
        Varyings Vertex(Attributes input)
        {
            Varyings output;
            UNITY_SETUP_INSTANCE_ID(input);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
            output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
            float4 projPos = output.positionCS * 0.5;
            projPos.xy = projPos.xy + projPos.w;
            output.uv.xy = UnityStereoTransformScreenSpaceTex(input.texcoord);
            output.uv.zw = projPos.xy;
            return output;
        }

        half4 Fragment(Varyings input) : SV_Target
        {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
            float deviceDepth= SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, input.uv.xy).r;
			//#if UNITY_REVERSED_Z
        				deviceDepth =  1 - deviceDepth;
			//#endif
			           deviceDepth = 2 * deviceDepth - 1; 
            float3 vpos = ComputeViewSpacePosition(input.uv.zw, deviceDepth, unity_CameraInvProjection);
            float3 wpos = mul(unity_CameraToWorld, float4(vpos, 1)).xyz;
            float3 viewDir = wpos-_WorldSpaceCameraPos;
            float distance = length(viewDir);
            viewDir /= distance;
			#ifdef DEBUG_OUTPUT
        				return half4(distance.xxx/100.0,1);
			#else
				#ifdef FOG_ONLY_OUTPUT
				            float4 mainTex = float4(0,0,0,1);
				#else
				            float4 mainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv.xy);
				#endif
							return half4(ScatteringHeightFog(mainTex.rgb, distance, viewDir, _WorldSpaceCameraPos), mainTex.a);
			#endif
        }
            
			#pragma vertex Vertex
			#pragma fragment Fragment
			
			ENDHLSL
		}
	} 
	FallBack "Diffuse"
	CustomEditor "LWGUI.LWGUI"
}
