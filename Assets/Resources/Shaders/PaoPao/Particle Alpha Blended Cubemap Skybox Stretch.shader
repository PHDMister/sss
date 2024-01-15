
Shader "Particles/Alpha Blended Cubemap Skybox Stretch" {
	Properties {
		[HDR]_TintColor ("Tint Color", Color) = (0.16, 0.16, 0.16, 1)
		_MainTex ("Base (RGB) Mask (A)", 2D) = "white" {}
		_CubemapPow ("Cubemap Intensity", Range(0, 2)) = 0.7//Opacity of cubemap.
		_Stretch ("Circular Stretch", Range(-10,10)) = -0.3//How much the cubemap is stretched in (positive) or out (negative) in roughly the circular shape.
		_CubemapOffset ("Cubemap Offset", Range(-2, 2)) = -0.15//How much the cubemap is offset corresponding to the view position of each vertex.
		_DLightPow ("Dir Light Power", Range(0,100)) = 0.5//How much the main directional light affects the material.
		_Glow ("Intensity", Range(0, 100)) = 0//Only increase the _Glow/Intensity when HDR mode is enabled in your rendering camera.
		
		 [Header(Fresnel)]
        [Space(10)]
        _FresnelScale("FresnelScale", Float) = 1
		_FresnelBias("FresnelBias", Float) = 0
		_FresnelPower("FresnelPower", Float) = 3
	}
	SubShader {
		Tags { "Queue"="Transparent+1" "RenderType"="Transparent" "PreviewType" = "Plane" "RenderPipeline" = "UniversalPipeline"}
		Cull Back
		ZTest On
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			HLSLPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 3.0
				#pragma multi_compile_particles
				
				//#include "UnityCG.cginc"
				//#include "UnityLightingCommon.cginc"
				 #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Core.hlsl"
				 #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Lighting.hlsl"
				struct appdata {
					float4 vertex : POSITION;
					half2 uv : TEXCOORD0;
					half4 color : COLOR;
					half3 normal : NORMAL;
					half4 tangentOS    : TANGENT;
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					half2 uv : TEXCOORD0;
					half3 normal : TEXCOORD1;
					half2 viewVertex : TEXCOORD2;
					half3 normalWS : TEXCOORD3;
					half4 color : COLOR;
				};
				TEXTURE2D(_MainTex);
				SAMPLER(sampler_MainTex);
				half4 _TintColor;
				half4 _MainTex_ST;
				half _CubemapPow;
				half _DLightPow;
				half _Glow;
				half _Stretch;
				half _CubemapOffset;
				half _FresnelScale;
                half _FresnelBias;
                half _FresnelPower;
				
				v2f vert (appdata v) {
					v2f o;
					VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);
					o.vertex = vertexInput.positionCS;
					o.viewVertex = mul(o.vertex, UNITY_MATRIX_P).xy * _CubemapOffset;
					o.uv = v.uv;
					o.normal = v.normal;
					o.color = v.color;
					VertexNormalInputs normalInput = GetVertexNormalInputs(v.normal, v.tangentOS);
					o.normalWS = normalInput.normalWS;  
					//o.color.rgb += _LightColor0.rgb * _DLightPow;
					//o.color.rgb += _Glow;
					return o;
				}
				
				half4 frag (v2f i) : SV_Target {
					half2 uvRepos = i.uv * 2 - 1;
					half xSq = uvRepos.x * uvRepos.x;
					half ySq = uvRepos.y * uvRepos.y;
					half zSq = xSq + ySq;
					// half3 vDirWS = GetWorldSpaceNormalizeViewDir(i.vertex);
					float3 worldNormal = i.normalWS;
					float3 worldRefl = reflect(float3(
					uvRepos.x * pow(abs(uvRepos.x / xSq/zSq), _Stretch) * 0.5 + i.viewVertex.x,
					uvRepos.y * pow(abs(uvRepos.y / ySq/zSq), _Stretch) * 0.5 + i.viewVertex.y,
					0.5
					), worldNormal);
					 //Fresnel
	                // half ndotv = dot(i.normal,vDirWS);
	                // half fresnel = pow(max(1.0 - ndotv , 0.0001),_FresnelPower) * _FresnelScale +
					half4 cubemap = SAMPLE_TEXTURECUBE(unity_SpecCube0,samplerunity_SpecCube0, worldRefl);
					half4 maintex2d = SAMPLE_TEXTURE2D (_MainTex,sampler_MainTex, i.uv);
					half4 col;
					col.rgb = lerp(maintex2d.rgb * _TintColor.rgb, cubemap.rgb * maintex2d, _CubemapPow) * i.color.rgb;
					col.a = (cubemap.a+0.1) * maintex2d.a * i.color.a * _TintColor.a;
					//col.a = 1;
					return col;
				}
			ENDHLSL
		}
	}
}