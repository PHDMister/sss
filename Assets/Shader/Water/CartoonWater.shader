// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "CartoonWater_Interactive"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[Toggle(_DISTORTFIX_ON)] _DistortFix("扭曲修正", Float) = 1
		[Header((Color Settings))][Space(5)]_ShallowColor("浅水颜色", Color) = (0.4862745,1,0.8588235,0)
		_DeepColor("深水颜色", Color) = (0,0.4666667,0.7450981,0)
		_WaterDeep("水深浅范围", Float) = 5
		_FresnelColor("菲涅尔颜色", Color) = (0.3686275,0.6431373,0.9137255,0)
		_FresnelIntensity("菲涅尔强度", Float) = 0.2
		_ReflectionAngle("菲涅尔反射角度", Float) = 1
		_ReflectExhance("反射度增强", Float) = 1
		[Toggle(_UNDERWATER_ON)] _UNDERWATER("水底图", Float) = 1
		_WaterDeep1("清水范围", Float) = 1.5
		_UnderWaterDark("水底压暗", Range( 0 , 1)) = 0
		_UnderWaterDistort("水底折射", Float) = 3
		_ShoreDistance("边缘透明范围", Float) = 1.5
		_Alpha("总体透明度控制", Float) = 1
		_DayIntensity("总体亮度控制", Float) = 0.75
		[Header((Water Normal))][Space(5)][KeywordEnum(LOW,MID,HIGH)] _WaterQuliaty("水动画质量", Float) = 2
		_WaterNormalSmall("细波纹法线", 2D) = "bump" {}
		_SmallNormalTiling("Small Normal Tiling", Float) = 10
		_SmallNormalSpeed("Small Normal Speed", Float) = 5
		_SmallNormalIntensity("Small Normal Intensity", Range( 0 , 0.2)) = 0.1
		_WaterNormalLarge("大波纹法线", 2D) = "bump" {}
		_LargeNormalTiling("Large Normal Tiling", Float) = 10
		_LargeNormalSpeed("Large Normal Speed", Float) = 5
		_LargeNormalIntensity("Large Normal Intensity", Range( 0 , 0.2)) = 0.1
		[Header((Reflection))][Space(5)]_ReflectCube("反射图", CUBE) = "white" {}
		_ReflectDistort("反射扭曲", Range( 0 , 1)) = 1
		_ReflectIntensity("反射强度", Float) = 1
		[Header((Caustics))][Space(5)][Toggle(_CAUSTICS_ON)] _Caustics("焦散动画", Float) = 1
		_CausticsTex("焦散图", 2D) = "white" {}
		_CausticsScale("焦散大小", Float) = 5
		_CausticsSpeed("焦散速度", Vector) = (-8,0,0,0)
		_CausticsIntensity("焦散亮度", Float) = 1
		[Header((Foam))][Space(5)][Toggle(_FOAM_ON)] _FOAM("岸边泡沫", Float) = 1
		[NoScaleOffset]_FoamNoise("泡沫Noise", 2D) = "white" {}
		_XTilling("泡沫TillingX", Float) = 10
		_YTilling("泡沫TillingY", Float) = 1
		_FoamNoiseSpeed("泡沫速度", Vector) = (0,-0.3,0,0)
		_FoamOffset("泡沫偏移", Float) = 0
		_FoamRange("泡沫范围", Float) = 1.5
		_FoamColor("泡沫颜色", Color) = (1,1,1,1)
		[Header(Sparkles)][Space(5)]_SparklesIntensity("波光亮度", Float) = 10
		_SparklesAmount("波光数量", Range( 0 , 1)) = 0.09
		[Header((Wave))][Space(5)][Toggle(_VERTEXWAVE_ON)] _VERTEXWAVE("顶点波纹动画", Float) = 1
		_Direction("水波运动方向（XY）", Vector) = (1,1,0,0)
		_WaveSpeed("水波速度", Float) = 2.4
		_WaveDistance("水波大小", Range( 0 , 1)) = 0.7
		_WaveHeight("水波高度", Float) = 0.15
		_SubWaveDirection("细节波形方向（XYZW）", Vector) = (-1,-1,-1,-1)
		_WaveNormalStr("水波法线强度", Float) = 0.16
		_WaveFadeStart("水波渐隐Start", Float) = 25
		_WaveFadeEnd("水波渐隐End", Float) = 280
		[HDR]_WaveColor("波峰颜色", Color) = (0.3686275,0.6431373,0.9137255,0)
		_Size("Size", Range( 0 , 1)) = 1
		_FallOff("FallOff", Range( 0 , 1)) = 1
		_MotionVertexDisplacement("交互顶点强度", Float) = 0.1

		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		//_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
		//_TessMin( "Tess Min Distance", Float ) = 10
		//_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25
	}

	SubShader
	{
		LOD 0

		
		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }
		
		Cull Back
		AlphaToMask Off
		HLSLINCLUDE
		#pragma target 3.0

		#pragma prefer_hlslcc gles
		#pragma only_renderers d3d9 d3d11_9x d3d11 glcore gles gles3 metal vulkan 

		#ifndef ASE_TESS_FUNCS
		#define ASE_TESS_FUNCS
		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}
		
		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		#endif //ASE_TESS_FUNCS

		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForward" }
			
			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZWrite Off
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM
			
			#define _RECEIVE_SHADOWS_OFF 1
			#define ASE_SRP_VERSION 999999
			#define REQUIRE_OPAQUE_TEXTURE 1
			#define REQUIRE_DEPTH_TEXTURE 1

			
			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#if ASE_SRP_VERSION <= 70108
			#define REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
			#endif

			#include "Assets/Shader/Water/WavesFunction.hlsl"
			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma shader_feature_local _VERTEXWAVE_ON
			#pragma shader_feature_local _UNDERWATER_ON
			#pragma shader_feature_local _DISTORTFIX_ON
			#pragma shader_feature_local _WATERQULIATY_LOW _WATERQULIATY_MID _WATERQULIATY_HIGH
			#pragma shader_feature_local _CAUSTICS_ON
			#pragma shader_feature_local _FOAM_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				#ifdef ASE_FOG
				float fogFactor : TEXCOORD2;
				#endif
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
				float4 ase_texcoord7 : TEXCOORD7;
				float4 ase_texcoord8 : TEXCOORD8;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DeepColor;
			float4 _SubWaveDirection;
			float4 _ShallowColor;
			float4 _WaveColor;
			float4 _FoamColor;
			float4 _FresnelColor;
			float2 _Direction;
			float2 _CausticsSpeed;
			float2 _FoamNoiseSpeed;
			float _ReflectionAngle;
			float _FresnelIntensity;
			float _ReflectDistort;
			float _ReflectIntensity;
			float _DayIntensity;
			float _SparklesIntensity;
			float _FoamOffset;
			float _FoamRange;
			float _XTilling;
			float _YTilling;
			float _ShoreDistance;
			float _Alpha;
			float _SparklesAmount;
			float _WaveSpeed;
			float _CausticsIntensity;
			float _ReflectExhance;
			float _WaveDistance;
			float _WaveHeight;
			float _WaveNormalStr;
			float _WaveFadeStart;
			float _WaveFadeEnd;
			float _MotionVertexDisplacement;
			float _SmallNormalSpeed;
			float _SmallNormalTiling;
			float _SmallNormalIntensity;
			float _LargeNormalSpeed;
			float _LargeNormalTiling;
			float _LargeNormalIntensity;
			float _UnderWaterDistort;
			float _UnderWaterDark;
			float _WaterDeep1;
			float _CausticsScale;
			float _Size;
			float _WaterDeep;
			float _FallOff;
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _WaterNormalSmall;
			sampler2D _WaterNormalLarge;
			uniform float4 _CameraDepthTexture_TexelSize;
			sampler2D _CausticsTex;
			samplerCUBE _ReflectCube;
			sampler2D _FoamNoise;


			float3 ReconstructWorldPos459( float2 ScreenUV, float rawdepth )
			{
				#if UNITY_REVERSED_Z
				real depth = rawdepth;
				#else
				real depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, rawdepth);
				#endif
				float3 worldPos = ComputeWorldSpacePosition(ScreenUV, depth, UNITY_MATRIX_I_VP);
				return worldPos;
			}
			
			
			VertexOutput VertexFunction ( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float localGetWaveInfo571 = ( 0.0 );
				float2 position571 = (ase_worldPos).xz;
				float2 time571 = ( _WaveSpeed * _TimeParameters.x * _Direction );
				float4 directionABCD571 = _SubWaveDirection;
				float wavedistance571 = _WaveDistance;
				float height571 = _WaveHeight;
				float normalStr571 = _WaveNormalStr;
				float fadeStart571 = _WaveFadeStart;
				float fadeEnd571 = _WaveFadeEnd;
				float3 positionWSOffset571 = float3( 0,0,0 );
				float3 normalWS571 = float3( 0,0,0 );
				GetWaveInfo( position571 , time571 , directionABCD571 , wavedistance571 , height571 , normalStr571 , fadeStart571 , fadeEnd571 , positionWSOffset571 , normalWS571 );
				float3 worldToObj583 = mul( GetWorldToObjectMatrix(), float4( ( ase_worldPos + positionWSOffset571 ), 1 ) ).xyz;
				#ifdef _VERTEXWAVE_ON
				float3 staticSwitch607 = worldToObj583;
				#else
				float3 staticSwitch607 = v.vertex.xyz;
				#endif
				float3 WaveVertexPos194 = ( staticSwitch607 + ( 0.1 * _MotionVertexDisplacement ) );
				
				float3 worldToObjDir584 = normalize( mul( GetWorldToObjectMatrix(), float4( normalWS571, 0 ) ).xyz );
				#ifdef _VERTEXWAVE_ON
				float3 staticSwitch611 = worldToObjDir584;
				#else
				float3 staticSwitch611 = v.ase_normal;
				#endif
				float3 WaveVertexNormal200 = staticSwitch611;
				
				float3 vertexToFrag739 = WaveVertexPos194;
				o.ase_texcoord4.xyz = vertexToFrag739;
				float3 ase_worldTangent = TransformObjectToWorldDir(v.ase_tangent.xyz);
				o.ase_texcoord6.xyz = ase_worldTangent;
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord7.xyz = ase_worldNormal;
				float ase_vertexTangentSign = v.ase_tangent.w * unity_WorldTransformParams.w;
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				o.ase_texcoord8.xyz = ase_worldBitangent;
				
				o.ase_texcoord3 = v.vertex;
				o.ase_texcoord5.xyz = v.ase_texcoord.xyz;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord4.w = 0;
				o.ase_texcoord5.w = 0;
				o.ase_texcoord6.w = 0;
				o.ase_texcoord7.w = 0;
				o.ase_texcoord8.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = WaveVertexPos194;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = WaveVertexNormal200;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				VertexPositionInputs vertexInput = (VertexPositionInputs)0;
				vertexInput.positionWS = positionWS;
				vertexInput.positionCS = positionCS;
				o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				#ifdef ASE_FOG
				o.fogFactor = ComputeFogFactor( positionCS.z );
				#endif
				o.clipPos = positionCS;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_tangent = v.ase_tangent;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag ( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif
				float4 color621 = IsGammaSpace() ? float4(1,1,1,1) : float4(1,1,1,1);
				float3 vertexToFrag739 = IN.ase_texcoord4.xyz;
				float4 objectToClip7_g9 = TransformWorldToHClip(TransformObjectToWorld(( IN.ase_texcoord3.xyz + vertexToFrag739 )));
				float3 objectToClip7_g9NDC = objectToClip7_g9.xyz/objectToClip7_g9.w;
				float4 appendResult13_g9 = (float4(objectToClip7_g9NDC , 1.0));
				float4 computeScreenPos10_g9 = ComputeScreenPos( appendResult13_g9 );
				float4 ScreenPos741 = computeScreenPos10_g9;
				float temp_output_40_0 = ( _SmallNormalSpeed * _TimeParameters.x * 0.1 );
				float2 texCoord314 = IN.ase_texcoord5.xyz.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_315_0 = ( texCoord314 * _SmallNormalTiling );
				float2 panner316 = ( temp_output_40_0 * float2( 0.1,0.1 ) + temp_output_315_0);
				float3 tex2DNode43 = UnpackNormalScale( tex2D( _WaterNormalSmall, panner316 ), 1.0f );
				float2 panner318 = ( temp_output_40_0 * float2( -0.1,-0.1 ) + ( temp_output_315_0 + 0.4 ));
				float3 temp_output_52_0 = BlendNormal( tex2DNode43 , UnpackNormalScale( tex2D( _WaterNormalSmall, panner318 ), 1.0f ) );
				float2 panner321 = ( temp_output_40_0 * float2( -0.1,0.1 ) + ( temp_output_315_0 + float2( 0.85,0.15 ) ));
				float2 panner324 = ( temp_output_40_0 * float2( 0.1,-0.1 ) + ( temp_output_315_0 + float2( 0.65,0.75 ) ));
				#if defined(_WATERQULIATY_LOW)
				float3 staticSwitch630 = tex2DNode43;
				#elif defined(_WATERQULIATY_MID)
				float3 staticSwitch630 = temp_output_52_0;
				#elif defined(_WATERQULIATY_HIGH)
				float3 staticSwitch630 = BlendNormal( temp_output_52_0 , BlendNormal( UnpackNormalScale( tex2D( _WaterNormalSmall, panner321 ), 1.0f ) , UnpackNormalScale( tex2D( _WaterNormalSmall, panner324 ), 1.0f ) ) );
				#else
				float3 staticSwitch630 = BlendNormal( temp_output_52_0 , BlendNormal( UnpackNormalScale( tex2D( _WaterNormalSmall, panner321 ), 1.0f ) , UnpackNormalScale( tex2D( _WaterNormalSmall, panner324 ), 1.0f ) ) );
				#endif
				float3 lerpResult327 = lerp( float3(0,0,1) , staticSwitch630 , _SmallNormalIntensity);
				float3 SmallNormal44 = lerpResult327;
				float temp_output_333_0 = ( _LargeNormalSpeed * _TimeParameters.x * 0.1 );
				float2 texCoord340 = IN.ase_texcoord5.xyz.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_337_0 = ( texCoord340 * _LargeNormalTiling );
				float2 panner350 = ( temp_output_333_0 * float2( 0.1,0.1 ) + temp_output_337_0);
				float3 tex2DNode345 = UnpackNormalScale( tex2D( _WaterNormalLarge, panner350 ), 1.0f );
				float2 panner339 = ( temp_output_333_0 * float2( -0.1,-0.1 ) + ( temp_output_337_0 + 0.4 ));
				float3 temp_output_351_0 = BlendNormal( tex2DNode345 , UnpackNormalScale( tex2D( _WaterNormalLarge, panner339 ), 1.0f ) );
				float2 panner335 = ( temp_output_333_0 * float2( -0.1,0.1 ) + ( temp_output_337_0 + float2( 0.85,0.15 ) ));
				float2 panner338 = ( temp_output_333_0 * float2( 0.1,-0.1 ) + ( temp_output_337_0 + float2( 0.65,0.75 ) ));
				#if defined(_WATERQULIATY_LOW)
				float3 staticSwitch631 = tex2DNode345;
				#elif defined(_WATERQULIATY_MID)
				float3 staticSwitch631 = temp_output_351_0;
				#elif defined(_WATERQULIATY_HIGH)
				float3 staticSwitch631 = BlendNormal( temp_output_351_0 , BlendNormal( UnpackNormalScale( tex2D( _WaterNormalLarge, panner335 ), 1.0f ) , UnpackNormalScale( tex2D( _WaterNormalLarge, panner338 ), 1.0f ) ) );
				#else
				float3 staticSwitch631 = BlendNormal( temp_output_351_0 , BlendNormal( UnpackNormalScale( tex2D( _WaterNormalLarge, panner335 ), 1.0f ) , UnpackNormalScale( tex2D( _WaterNormalLarge, panner338 ), 1.0f ) ) );
				#endif
				float3 lerpResult357 = lerp( float3(0,0,1) , staticSwitch631 , _LargeNormalIntensity);
				float3 LargeNormal358 = lerpResult357;
				float3 normalizeResult366 = normalize( BlendNormal( SmallNormal44 , LargeNormal358 ) );
				float3 SurfaceNormal361 = normalizeResult366;
				float2 ScreenDistort432 = ( (SurfaceNormal361).xy * _UnderWaterDistort * 0.01 );
				float clampDepth471 = SHADERGRAPH_SAMPLE_SCENE_DEPTH( ( ScreenPos741 + float4( ScreenDistort432, 0.0 , 0.0 ) ).xy );
				float depthToLinear472 = LinearEyeDepth(clampDepth471,_ZBufferParams);
				float depthToLinear446 = LinearEyeDepth(IN.clipPos.z,_ZBufferParams);
				#ifdef _UNDERWATER_ON
				float staticSwitch622 = step( 0.0 , ( depthToLinear472 - depthToLinear446 ) );
				#else
				float staticSwitch622 = 0.0;
				#endif
				float RefractionMask449 = staticSwitch622;
				float2 lerpResult451 = lerp( float2( 0,0 ) , ScreenDistort432 , RefractionMask449);
				#ifdef _DISTORTFIX_ON
				float2 staticSwitch738 = lerpResult451;
				#else
				float2 staticSwitch738 = ScreenDistort432;
				#endif
				float4 fetchOpaqueVal70 = float4( SHADERGRAPH_SAMPLE_SCENE_COLOR( ( ScreenPos741 + float4( staticSwitch738, 0.0 , 0.0 ) ).xy ), 1.0 );
				float4 SceneColor119 = fetchOpaqueVal70;
				#ifdef _UNDERWATER_ON
				float4 staticSwitch620 = ( SceneColor119 * ( 1.0 - _UnderWaterDark ) );
				#else
				float4 staticSwitch620 = color621;
				#endif
				float4 UnderWaterColor78 = staticSwitch620;
				float clampDepth465 = SHADERGRAPH_SAMPLE_SCENE_DEPTH( ScreenPos741.xy );
				float depthToLinear469 = LinearEyeDepth(clampDepth465,_ZBufferParams);
				float depthToLinear423 = LinearEyeDepth(ScreenPos741.z,_ZBufferParams);
				float WaterDepth492 = ( depthToLinear469 - depthToLinear423 );
				float clampResult675 = clamp( ( WaterDepth492 / max( _WaterDeep1 , 0.0 ) ) , 0.0 , 1.0 );
				float4 lerpResult671 = lerp( UnderWaterColor78 , _ShallowColor , clampResult675);
				float4 color619 = IsGammaSpace() ? float4(0,0,0,0) : float4(0,0,0,0);
				float2 ScreenUV459 = ScreenPos741.xy;
				float rawdepth459 = clampDepth465;
				float3 localReconstructWorldPos459 = ReconstructWorldPos459( ScreenUV459 , rawdepth459 );
				float3 UnderWaterPos468 = localReconstructWorldPos459;
				float2 temp_output_91_0 = ( (UnderWaterPos468).xz / _CausticsScale );
				float2 temp_output_95_0 = ( _CausticsSpeed * _TimeParameters.x * 0.01 );
				float clampResult661 = clamp( ( _ReflectExhance * 0.04 ) , 0.0 , 0.5 );
				float3 ase_worldTangent = IN.ase_texcoord6.xyz;
				float3 ase_worldNormal = IN.ase_texcoord7.xyz;
				float3 ase_worldBitangent = IN.ase_texcoord8.xyz;
				float3 tanToWorld0 = float3( ase_worldTangent.x, ase_worldBitangent.x, ase_worldNormal.x );
				float3 tanToWorld1 = float3( ase_worldTangent.y, ase_worldBitangent.y, ase_worldNormal.y );
				float3 tanToWorld2 = float3( ase_worldTangent.z, ase_worldBitangent.z, ase_worldNormal.z );
				float3 tanNormal252 = SurfaceNormal361;
				float3 worldNormal252 = float3(dot(tanToWorld0,tanNormal252), dot(tanToWorld1,tanNormal252), dot(tanToWorld2,tanNormal252));
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float dotResult253 = dot( worldNormal252 , ase_worldViewDir );
				float temp_output_255_0 = ( 1.0 - max( dotResult253 , 0.0 ) );
				float lerpResult258 = lerp( clampResult661 , 1.0 , ( temp_output_255_0 * temp_output_255_0 * temp_output_255_0 * temp_output_255_0 * temp_output_255_0 ));
				float FresnelFactor547 = lerpResult258;
				float clampResult437 = clamp( ( WaterDepth492 / _WaterDeep ) , 0.0 , 1.0 );
				float clampResult268 = clamp( max( FresnelFactor547 , ( FresnelFactor547 + clampResult437 ) ) , 0.0 , 1.0 );
				float WaterDepthRange545 = clampResult268;
				#ifdef _CAUSTICS_ON
				float4 staticSwitch618 = ( ( min( tex2D( _CausticsTex, ( temp_output_91_0 + temp_output_95_0 ) ) , tex2D( _CausticsTex, ( -temp_output_91_0 + temp_output_95_0 ) ) ) * _CausticsIntensity ) * ( 1.0 - WaterDepthRange545 ) );
				#else
				float4 staticSwitch618 = color619;
				#endif
				float4 CausticsColor101 = staticSwitch618;
				float temp_output_272_0 = pow( lerpResult258 , _ReflectionAngle );
				float temp_output_259_0 = ( temp_output_272_0 * _FresnelIntensity * 5.0 );
				float clampResult263 = clamp( ( temp_output_259_0 * temp_output_259_0 ) , 0.0 , 1.0 );
				float ColorFresnel485 = clampResult263;
				float4 lerpResult251 = lerp( _DeepColor , _FresnelColor , ColorFresnel485);
				float localGetWaveInfo571 = ( 0.0 );
				float2 position571 = (WorldPosition).xz;
				float2 time571 = ( _WaveSpeed * _TimeParameters.x * _Direction );
				float4 directionABCD571 = _SubWaveDirection;
				float wavedistance571 = _WaveDistance;
				float height571 = _WaveHeight;
				float normalStr571 = _WaveNormalStr;
				float fadeStart571 = _WaveFadeStart;
				float fadeEnd571 = _WaveFadeEnd;
				float3 positionWSOffset571 = float3( 0,0,0 );
				float3 normalWS571 = float3( 0,0,0 );
				GetWaveInfo( position571 , time571 , directionABCD571 , wavedistance571 , height571 , normalStr571 , fadeStart571 , fadeEnd571 , positionWSOffset571 , normalWS571 );
				#ifdef _VERTEXWAVE_ON
				float staticSwitch609 = (positionWSOffset571).y;
				#else
				float staticSwitch609 = 0.0;
				#endif
				float WaveY594 = staticSwitch609;
				float clampResult598 = clamp( WaveY594 , 0.0 , 1.0 );
				float4 lerpResult596 = lerp( lerpResult251 , _WaveColor , clampResult598);
				float4 lerpResult13 = lerp( ( lerpResult671 + CausticsColor101 ) , lerpResult596 , clampResult268);
				float4 WaterColor24 = lerpResult13;
				float3 lerpResult217 = lerp( float3(0,0,1) , SurfaceNormal361 , _ReflectDistort);
				float3 worldRefl236 = reflect( -ase_worldViewDir, float3( dot( tanToWorld0, lerpResult217 ), dot( tanToWorld1, lerpResult217 ), dot( tanToWorld2, lerpResult217 ) ) );
				float clampResult481 = clamp( temp_output_272_0 , 0.0 , 1.0 );
				float ReflectFresnel487 = clampResult481;
				float4 ReflectColor65 = ( texCUBE( _ReflectCube, worldRefl236 ) * _ReflectIntensity * ReflectFresnel487 );
				float Splakes537 = ( step( _SparklesAmount , (SurfaceNormal361).y ) * _SparklesIntensity );
				float temp_output_478_0 = ( WaterDepth492 / _FoamRange );
				float clampResult149 = clamp( ( _FoamOffset + temp_output_478_0 ) , 0.0 , 1.0 );
				float temp_output_150_0 = ( 1.0 - clampResult149 );
				float2 texCoord371 = IN.ase_texcoord5.xyz.xy * float2( 1,1 ) + float2( 0,0 );
				float clampResult479 = clamp( temp_output_478_0 , 0.0 , 1.0 );
				float2 appendResult384 = (float2(( _XTilling * texCoord371.x ) , ( clampResult479 * _YTilling )));
				float2 panner382 = ( 1.0 * _Time.y * _FoamNoiseSpeed + appendResult384);
				#ifdef _FOAM_ON
				float staticSwitch624 = step( 0.0 , ( depthToLinear469 - depthToLinear446 ) );
				#else
				float staticSwitch624 = 0.0;
				#endif
				float FoamMask567 = staticSwitch624;
				float clampResult411 = clamp( ( ( temp_output_150_0 - -1.0 ) * step( tex2D( _FoamNoise, panner382 ).r , temp_output_150_0 ) * FoamMask567 ) , 0.0 , 1.0 );
				#ifdef _FOAM_ON
				float4 staticSwitch615 = ( clampResult411 * _FoamColor * 2.0 );
				#else
				float4 staticSwitch615 = _FoamColor;
				#endif
				float4 FoamColor407 = staticSwitch615;
				#ifdef _FOAM_ON
				float staticSwitch614 = clampResult411;
				#else
				float staticSwitch614 = 0.0;
				#endif
				float Foam179 = staticSwitch614;
				float4 lerpResult408 = lerp( ( ( WaterColor24 + ReflectColor65 + Splakes537 ) * _DayIntensity ) , FoamColor407 , Foam179);
				
				float WaterHeight669 = (( WorldPosition - UnderWaterPos468 )).y;
				float clampResult291 = clamp( ( WaterHeight669 / max( _ShoreDistance , 0.0 ) ) , 0.0 , 1.0 );
				float WaterOpacity27 = clampResult291;
				float clampResult504 = clamp( max( 0.0 , max( max( Foam179 , ( ReflectFresnel487 * WaterOpacity27 ) ) , (CausticsColor101).r ) ) , 0.0 , 1.0 );
				float lerpResult414 = lerp( WaterOpacity27 , 1.0 , clampResult504);
				float clampResult627 = clamp( ( lerpResult414 * _Alpha ) , 0.0 , 1.0 );
				float2 texCoord681 = IN.ase_texcoord5.xyz.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_cast_5 = (_Size).xx;
				float clampResult695 = clamp( ( distance( max( ( abs( ( texCoord681 - float2( 0.5,0.5 ) ) ) - temp_cast_5 ) , float2( 0,0 ) ) , float2( 0,0 ) ) / _FallOff ) , 0.0 , 1.0 );
				float FinalAlpha497 = ( clampResult627 * ( 1.0 - clampResult695 ) );
				
				float3 BakedAlbedo = 0;
				float3 BakedEmission = 0;
				float3 Color = lerpResult408.rgb;
				float Alpha = FinalAlpha497;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;

				#ifdef _ALPHATEST_ON
					clip( Alpha - AlphaClipThreshold );
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				#ifdef ASE_FOG
					Color = MixFog( Color, IN.fogFactor );
				#endif

				return half4( Color, Alpha );
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0
			AlphaToMask Off

			HLSLPROGRAM
			
			#define _RECEIVE_SHADOWS_OFF 1
			#define ASE_SRP_VERSION 999999
			#define REQUIRE_DEPTH_TEXTURE 1

			
			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#include "Assets/Shader/Water/WavesFunction.hlsl"
			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma shader_feature_local _VERTEXWAVE_ON
			#pragma shader_feature_local _FOAM_ON
			#pragma shader_feature_local _WATERQULIATY_LOW _WATERQULIATY_MID _WATERQULIATY_HIGH
			#pragma shader_feature_local _CAUSTICS_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
				float4 ase_texcoord7 : TEXCOORD7;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DeepColor;
			float4 _SubWaveDirection;
			float4 _ShallowColor;
			float4 _WaveColor;
			float4 _FoamColor;
			float4 _FresnelColor;
			float2 _Direction;
			float2 _CausticsSpeed;
			float2 _FoamNoiseSpeed;
			float _ReflectionAngle;
			float _FresnelIntensity;
			float _ReflectDistort;
			float _ReflectIntensity;
			float _DayIntensity;
			float _SparklesIntensity;
			float _FoamOffset;
			float _FoamRange;
			float _XTilling;
			float _YTilling;
			float _ShoreDistance;
			float _Alpha;
			float _SparklesAmount;
			float _WaveSpeed;
			float _CausticsIntensity;
			float _ReflectExhance;
			float _WaveDistance;
			float _WaveHeight;
			float _WaveNormalStr;
			float _WaveFadeStart;
			float _WaveFadeEnd;
			float _MotionVertexDisplacement;
			float _SmallNormalSpeed;
			float _SmallNormalTiling;
			float _SmallNormalIntensity;
			float _LargeNormalSpeed;
			float _LargeNormalTiling;
			float _LargeNormalIntensity;
			float _UnderWaterDistort;
			float _UnderWaterDark;
			float _WaterDeep1;
			float _CausticsScale;
			float _Size;
			float _WaterDeep;
			float _FallOff;
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			uniform float4 _CameraDepthTexture_TexelSize;
			sampler2D _FoamNoise;
			sampler2D _WaterNormalSmall;
			sampler2D _WaterNormalLarge;
			sampler2D _CausticsTex;


			float3 ReconstructWorldPos459( float2 ScreenUV, float rawdepth )
			{
				#if UNITY_REVERSED_Z
				real depth = rawdepth;
				#else
				real depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, rawdepth);
				#endif
				float3 worldPos = ComputeWorldSpacePosition(ScreenUV, depth, UNITY_MATRIX_I_VP);
				return worldPos;
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float localGetWaveInfo571 = ( 0.0 );
				float2 position571 = (ase_worldPos).xz;
				float2 time571 = ( _WaveSpeed * _TimeParameters.x * _Direction );
				float4 directionABCD571 = _SubWaveDirection;
				float wavedistance571 = _WaveDistance;
				float height571 = _WaveHeight;
				float normalStr571 = _WaveNormalStr;
				float fadeStart571 = _WaveFadeStart;
				float fadeEnd571 = _WaveFadeEnd;
				float3 positionWSOffset571 = float3( 0,0,0 );
				float3 normalWS571 = float3( 0,0,0 );
				GetWaveInfo( position571 , time571 , directionABCD571 , wavedistance571 , height571 , normalStr571 , fadeStart571 , fadeEnd571 , positionWSOffset571 , normalWS571 );
				float3 worldToObj583 = mul( GetWorldToObjectMatrix(), float4( ( ase_worldPos + positionWSOffset571 ), 1 ) ).xyz;
				#ifdef _VERTEXWAVE_ON
				float3 staticSwitch607 = worldToObj583;
				#else
				float3 staticSwitch607 = v.vertex.xyz;
				#endif
				float3 WaveVertexPos194 = ( staticSwitch607 + ( 0.1 * _MotionVertexDisplacement ) );
				
				float3 worldToObjDir584 = normalize( mul( GetWorldToObjectMatrix(), float4( normalWS571, 0 ) ).xyz );
				#ifdef _VERTEXWAVE_ON
				float3 staticSwitch611 = worldToObjDir584;
				#else
				float3 staticSwitch611 = v.ase_normal;
				#endif
				float3 WaveVertexNormal200 = staticSwitch611;
				
				float3 vertexToFrag739 = WaveVertexPos194;
				o.ase_texcoord3.xyz = vertexToFrag739;
				float3 ase_worldTangent = TransformObjectToWorldDir(v.ase_tangent.xyz);
				o.ase_texcoord5.xyz = ase_worldTangent;
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord6.xyz = ase_worldNormal;
				float ase_vertexTangentSign = v.ase_tangent.w * unity_WorldTransformParams.w;
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				o.ase_texcoord7.xyz = ase_worldBitangent;
				
				o.ase_texcoord2 = v.vertex;
				o.ase_texcoord4.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord3.w = 0;
				o.ase_texcoord4.zw = 0;
				o.ase_texcoord5.w = 0;
				o.ase_texcoord6.w = 0;
				o.ase_texcoord7.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = WaveVertexPos194;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = WaveVertexNormal200;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				o.clipPos = TransformWorldToHClip( positionWS );
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = o.clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_tangent = v.ase_tangent;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float3 vertexToFrag739 = IN.ase_texcoord3.xyz;
				float4 objectToClip7_g9 = TransformWorldToHClip(TransformObjectToWorld(( IN.ase_texcoord2.xyz + vertexToFrag739 )));
				float3 objectToClip7_g9NDC = objectToClip7_g9.xyz/objectToClip7_g9.w;
				float4 appendResult13_g9 = (float4(objectToClip7_g9NDC , 1.0));
				float4 computeScreenPos10_g9 = ComputeScreenPos( appendResult13_g9 );
				float4 ScreenPos741 = computeScreenPos10_g9;
				float2 ScreenUV459 = ScreenPos741.xy;
				float clampDepth465 = SHADERGRAPH_SAMPLE_SCENE_DEPTH( ScreenPos741.xy );
				float rawdepth459 = clampDepth465;
				float3 localReconstructWorldPos459 = ReconstructWorldPos459( ScreenUV459 , rawdepth459 );
				float3 UnderWaterPos468 = localReconstructWorldPos459;
				float WaterHeight669 = (( WorldPosition - UnderWaterPos468 )).y;
				float clampResult291 = clamp( ( WaterHeight669 / max( _ShoreDistance , 0.0 ) ) , 0.0 , 1.0 );
				float WaterOpacity27 = clampResult291;
				float depthToLinear469 = LinearEyeDepth(clampDepth465,_ZBufferParams);
				float depthToLinear423 = LinearEyeDepth(ScreenPos741.z,_ZBufferParams);
				float WaterDepth492 = ( depthToLinear469 - depthToLinear423 );
				float temp_output_478_0 = ( WaterDepth492 / _FoamRange );
				float clampResult149 = clamp( ( _FoamOffset + temp_output_478_0 ) , 0.0 , 1.0 );
				float temp_output_150_0 = ( 1.0 - clampResult149 );
				float2 texCoord371 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float clampResult479 = clamp( temp_output_478_0 , 0.0 , 1.0 );
				float2 appendResult384 = (float2(( _XTilling * texCoord371.x ) , ( clampResult479 * _YTilling )));
				float2 panner382 = ( 1.0 * _Time.y * _FoamNoiseSpeed + appendResult384);
				float depthToLinear446 = LinearEyeDepth((0),_ZBufferParams);
				#ifdef _FOAM_ON
				float staticSwitch624 = step( 0.0 , ( depthToLinear469 - depthToLinear446 ) );
				#else
				float staticSwitch624 = 0.0;
				#endif
				float FoamMask567 = staticSwitch624;
				float clampResult411 = clamp( ( ( temp_output_150_0 - -1.0 ) * step( tex2D( _FoamNoise, panner382 ).r , temp_output_150_0 ) * FoamMask567 ) , 0.0 , 1.0 );
				#ifdef _FOAM_ON
				float staticSwitch614 = clampResult411;
				#else
				float staticSwitch614 = 0.0;
				#endif
				float Foam179 = staticSwitch614;
				float clampResult661 = clamp( ( _ReflectExhance * 0.04 ) , 0.0 , 0.5 );
				float temp_output_40_0 = ( _SmallNormalSpeed * _TimeParameters.x * 0.1 );
				float2 texCoord314 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_315_0 = ( texCoord314 * _SmallNormalTiling );
				float2 panner316 = ( temp_output_40_0 * float2( 0.1,0.1 ) + temp_output_315_0);
				float3 tex2DNode43 = UnpackNormalScale( tex2D( _WaterNormalSmall, panner316 ), 1.0f );
				float2 panner318 = ( temp_output_40_0 * float2( -0.1,-0.1 ) + ( temp_output_315_0 + 0.4 ));
				float3 temp_output_52_0 = BlendNormal( tex2DNode43 , UnpackNormalScale( tex2D( _WaterNormalSmall, panner318 ), 1.0f ) );
				float2 panner321 = ( temp_output_40_0 * float2( -0.1,0.1 ) + ( temp_output_315_0 + float2( 0.85,0.15 ) ));
				float2 panner324 = ( temp_output_40_0 * float2( 0.1,-0.1 ) + ( temp_output_315_0 + float2( 0.65,0.75 ) ));
				#if defined(_WATERQULIATY_LOW)
				float3 staticSwitch630 = tex2DNode43;
				#elif defined(_WATERQULIATY_MID)
				float3 staticSwitch630 = temp_output_52_0;
				#elif defined(_WATERQULIATY_HIGH)
				float3 staticSwitch630 = BlendNormal( temp_output_52_0 , BlendNormal( UnpackNormalScale( tex2D( _WaterNormalSmall, panner321 ), 1.0f ) , UnpackNormalScale( tex2D( _WaterNormalSmall, panner324 ), 1.0f ) ) );
				#else
				float3 staticSwitch630 = BlendNormal( temp_output_52_0 , BlendNormal( UnpackNormalScale( tex2D( _WaterNormalSmall, panner321 ), 1.0f ) , UnpackNormalScale( tex2D( _WaterNormalSmall, panner324 ), 1.0f ) ) );
				#endif
				float3 lerpResult327 = lerp( float3(0,0,1) , staticSwitch630 , _SmallNormalIntensity);
				float3 SmallNormal44 = lerpResult327;
				float temp_output_333_0 = ( _LargeNormalSpeed * _TimeParameters.x * 0.1 );
				float2 texCoord340 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_337_0 = ( texCoord340 * _LargeNormalTiling );
				float2 panner350 = ( temp_output_333_0 * float2( 0.1,0.1 ) + temp_output_337_0);
				float3 tex2DNode345 = UnpackNormalScale( tex2D( _WaterNormalLarge, panner350 ), 1.0f );
				float2 panner339 = ( temp_output_333_0 * float2( -0.1,-0.1 ) + ( temp_output_337_0 + 0.4 ));
				float3 temp_output_351_0 = BlendNormal( tex2DNode345 , UnpackNormalScale( tex2D( _WaterNormalLarge, panner339 ), 1.0f ) );
				float2 panner335 = ( temp_output_333_0 * float2( -0.1,0.1 ) + ( temp_output_337_0 + float2( 0.85,0.15 ) ));
				float2 panner338 = ( temp_output_333_0 * float2( 0.1,-0.1 ) + ( temp_output_337_0 + float2( 0.65,0.75 ) ));
				#if defined(_WATERQULIATY_LOW)
				float3 staticSwitch631 = tex2DNode345;
				#elif defined(_WATERQULIATY_MID)
				float3 staticSwitch631 = temp_output_351_0;
				#elif defined(_WATERQULIATY_HIGH)
				float3 staticSwitch631 = BlendNormal( temp_output_351_0 , BlendNormal( UnpackNormalScale( tex2D( _WaterNormalLarge, panner335 ), 1.0f ) , UnpackNormalScale( tex2D( _WaterNormalLarge, panner338 ), 1.0f ) ) );
				#else
				float3 staticSwitch631 = BlendNormal( temp_output_351_0 , BlendNormal( UnpackNormalScale( tex2D( _WaterNormalLarge, panner335 ), 1.0f ) , UnpackNormalScale( tex2D( _WaterNormalLarge, panner338 ), 1.0f ) ) );
				#endif
				float3 lerpResult357 = lerp( float3(0,0,1) , staticSwitch631 , _LargeNormalIntensity);
				float3 LargeNormal358 = lerpResult357;
				float3 normalizeResult366 = normalize( BlendNormal( SmallNormal44 , LargeNormal358 ) );
				float3 SurfaceNormal361 = normalizeResult366;
				float3 ase_worldTangent = IN.ase_texcoord5.xyz;
				float3 ase_worldNormal = IN.ase_texcoord6.xyz;
				float3 ase_worldBitangent = IN.ase_texcoord7.xyz;
				float3 tanToWorld0 = float3( ase_worldTangent.x, ase_worldBitangent.x, ase_worldNormal.x );
				float3 tanToWorld1 = float3( ase_worldTangent.y, ase_worldBitangent.y, ase_worldNormal.y );
				float3 tanToWorld2 = float3( ase_worldTangent.z, ase_worldBitangent.z, ase_worldNormal.z );
				float3 tanNormal252 = SurfaceNormal361;
				float3 worldNormal252 = float3(dot(tanToWorld0,tanNormal252), dot(tanToWorld1,tanNormal252), dot(tanToWorld2,tanNormal252));
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float dotResult253 = dot( worldNormal252 , ase_worldViewDir );
				float temp_output_255_0 = ( 1.0 - max( dotResult253 , 0.0 ) );
				float lerpResult258 = lerp( clampResult661 , 1.0 , ( temp_output_255_0 * temp_output_255_0 * temp_output_255_0 * temp_output_255_0 * temp_output_255_0 ));
				float temp_output_272_0 = pow( lerpResult258 , _ReflectionAngle );
				float clampResult481 = clamp( temp_output_272_0 , 0.0 , 1.0 );
				float ReflectFresnel487 = clampResult481;
				float4 color619 = IsGammaSpace() ? float4(0,0,0,0) : float4(0,0,0,0);
				float2 temp_output_91_0 = ( (UnderWaterPos468).xz / _CausticsScale );
				float2 temp_output_95_0 = ( _CausticsSpeed * _TimeParameters.x * 0.01 );
				float FresnelFactor547 = lerpResult258;
				float clampResult437 = clamp( ( WaterDepth492 / _WaterDeep ) , 0.0 , 1.0 );
				float clampResult268 = clamp( max( FresnelFactor547 , ( FresnelFactor547 + clampResult437 ) ) , 0.0 , 1.0 );
				float WaterDepthRange545 = clampResult268;
				#ifdef _CAUSTICS_ON
				float4 staticSwitch618 = ( ( min( tex2D( _CausticsTex, ( temp_output_91_0 + temp_output_95_0 ) ) , tex2D( _CausticsTex, ( -temp_output_91_0 + temp_output_95_0 ) ) ) * _CausticsIntensity ) * ( 1.0 - WaterDepthRange545 ) );
				#else
				float4 staticSwitch618 = color619;
				#endif
				float4 CausticsColor101 = staticSwitch618;
				float clampResult504 = clamp( max( 0.0 , max( max( Foam179 , ( ReflectFresnel487 * WaterOpacity27 ) ) , (CausticsColor101).r ) ) , 0.0 , 1.0 );
				float lerpResult414 = lerp( WaterOpacity27 , 1.0 , clampResult504);
				float clampResult627 = clamp( ( lerpResult414 * _Alpha ) , 0.0 , 1.0 );
				float2 texCoord681 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_cast_1 = (_Size).xx;
				float clampResult695 = clamp( ( distance( max( ( abs( ( texCoord681 - float2( 0.5,0.5 ) ) ) - temp_cast_1 ) , float2( 0,0 ) ) , float2( 0,0 ) ) / _FallOff ) , 0.0 , 1.0 );
				float FinalAlpha497 = ( clampResult627 * ( 1.0 - clampResult695 ) );
				
				float Alpha = FinalAlpha497;
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				return 0;
			}
			ENDHLSL
		}

	
	}
	CustomEditor "ASEMaterialInspector"
	Fallback "Hidden/InternalErrorShader"
	

}
/*ASEBEGIN
Version=18912
-1916.667;122.6667;1968;1041.667;-1680.191;-791.5709;1.061148;True;True
Node;AmplifyShaderEditor.CommentaryNode;53;-6757.348,1952;Inherit;False;2989.379;1116.15;Small Normals;29;322;326;323;312;321;325;310;311;324;313;44;327;52;328;330;43;45;318;316;40;319;315;42;317;320;41;34;314;630;Small Normals;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;331;-6773.348,3152;Inherit;False;3002.933;1172.903;Big Normals;29;346;354;356;359;349;342;343;338;334;335;358;357;351;352;348;345;344;339;350;336;333;353;355;337;332;347;341;340;631;Big Normals;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;314;-6645.348,2048;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;340;-6645.348,3248;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;341;-6629.348,3376;Inherit;False;Property;_LargeNormalTiling;Large Normal Tiling;21;0;Create;True;0;0;0;False;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-6629.348,2176;Inherit;False;Property;_SmallNormalTiling;Small Normal Tiling;17;0;Create;True;0;0;0;False;0;False;10;20;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;323;-6309.348,2624;Inherit;False;Constant;_Vector3;Vector 3;37;0;Create;True;0;0;0;False;0;False;0.85,0.15;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;317;-6629.348,2368;Inherit;False;Property;_SmallNormalSpeed;Small Normal Speed;18;0;Create;True;0;0;0;False;0;False;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;353;-6341.348,3472;Inherit;False;Constant;_Vector8;Vector 8;37;0;Create;True;0;0;0;False;0;False;0.4,0.35;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;356;-6405.348,4064;Inherit;False;Constant;_Vector7;Vector 7;37;0;Create;True;0;0;0;False;0;False;0.65,0.75;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleTimeNode;41;-6597.348,2496;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;354;-6309.348,3824;Inherit;False;Constant;_Vector9;Vector 9;37;0;Create;True;0;0;0;False;0;False;0.85,0.15;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;347;-6629.348,3568;Inherit;False;Property;_LargeNormalSpeed;Large Normal Speed;22;0;Create;True;0;0;0;False;0;False;5;-3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;355;-6565.348,3776;Inherit;False;Constant;_Float1;Float 1;7;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;332;-6597.348,3696;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;315;-6277.348,2048;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;326;-6405.348,2864;Inherit;False;Constant;_Vector4;Vector 4;37;0;Create;True;0;0;0;False;0;False;0.65,0.75;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;42;-6565.348,2576;Inherit;False;Constant;_Float0;Float 0;7;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;337;-6277.348,3248;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;320;-6341.348,2272;Inherit;False;Constant;_Vector2;Vector 2;37;0;Create;True;0;0;0;False;0;False;0.4,0.35;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;322;-5957.348,2608;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;592;-2044.782,5700.591;Inherit;False;2341.999;889.5058;Comment;24;607;593;594;200;583;584;582;571;585;581;586;580;587;575;573;579;574;591;572;578;608;609;611;613;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;319;-6053.348,2288;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;325;-5973.348,2848;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;334;-5973.348,4048;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;349;-5957.348,3808;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;-6341.348,2432;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;336;-6053.348,3488;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;333;-6341.348,3632;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;339;-5845.348,3488;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.1,-0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;574;-1963.782,5949.573;Inherit;False;Property;_WaveSpeed;水波速度;50;0;Create;False;0;0;0;False;0;False;2.4;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;591;-1984.761,6154.617;Inherit;True;Property;_Direction;水波运动方向（XY）;49;0;Create;False;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.WorldPosInputsNode;572;-1725.711,5750.591;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleTimeNode;578;-1994.782,6039.573;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;350;-5877.348,3248;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.1,0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;316;-5861.348,2048;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.1,0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;338;-5781.348,4048;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.1,-0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;318;-5845.348,2288;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.1,-0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;321;-5781.348,2624;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.1,0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;335;-5781.348,3824;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.1,0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;324;-5781.348,2848;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.1,-0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;587;-1562.525,6024.568;Inherit;False;Property;_SubWaveDirection;细节波形方向（XYZW）;53;0;Create;False;0;0;0;False;0;False;-1,-1,-1,-1;-1,-1,-1,-1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;586;-1737.756,6151.81;Inherit;False;Property;_WaveDistance;水波大小;51;0;Create;False;0;0;0;False;0;False;0.7;0.9;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;580;-1556.913,6402.693;Inherit;False;Property;_WaveFadeStart;水波渐隐Start;55;0;Create;False;0;0;0;False;0;False;25;25;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;585;-1574.599,6315.692;Inherit;False;Property;_WaveNormalStr;水波法线强度;54;0;Create;False;0;0;0;False;0;False;0.16;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;312;-5541.348,2592;Inherit;True;Property;_TextureSample3;Texture Sample 3;16;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Instance;43;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;581;-1566.093,6479.923;Inherit;False;Property;_WaveFadeEnd;水波渐隐End;56;0;Create;False;0;0;0;False;0;False;280;280;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;575;-1759.782,5945.573;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;343;-5541.348,3792;Inherit;True;Property;_TextureSample5;Texture Sample 5;20;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Instance;345;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;311;-5541.348,2832;Inherit;True;Property;_TextureSample2;Texture Sample 2;16;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;bump;Auto;True;Instance;43;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;45;-5541.348,2256;Inherit;True;Property;_TextureSample0;Texture Sample 0;16;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;bump;Auto;True;Instance;43;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;579;-1554.81,6224.146;Inherit;False;Property;_WaveHeight;水波高度;52;0;Create;False;0;0;0;False;0;False;0.15;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;345;-5541.348,3216;Inherit;True;Property;_WaterNormalLarge;大波纹法线;20;0;Create;False;0;0;0;False;0;False;-1;None;67b91789829782646ad0b013bf0c6a0f;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SwizzleNode;573;-1523.263,5833.189;Inherit;False;FLOAT2;0;2;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;344;-5557.348,3456;Inherit;True;Property;_TextureSample4;Texture Sample 4;20;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;bump;Auto;True;Instance;345;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;342;-5541.348,4032;Inherit;True;Property;_TextureSample6;Texture Sample 6;20;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;bump;Auto;True;Instance;345;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;43;-5541.348,2016;Inherit;True;Property;_WaterNormalSmall;细波纹法线;16;0;Create;False;0;0;0;False;0;False;-1;None;99f2b89899f133e45b6de05017ad4628;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BlendNormalsNode;313;-5141.348,2688;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CustomExpressionNode;571;-1206.188,5923.418;Inherit;False; ;7;File;10;True;position;FLOAT2;0,0;In;;Inherit;False;True;time;FLOAT2;0,0;In;;Inherit;False;True;directionABCD;FLOAT4;0,0,0,0;In;;Inherit;False;True;wavedistance;FLOAT;0;In;;Inherit;False;True;height;FLOAT;0;In;;Inherit;False;True;normalStr;FLOAT;0;In;;Inherit;False;True;fadeStart;FLOAT;0;In;;Inherit;False;True;fadeEnd;FLOAT;0;In;;Inherit;False;True;positionWSOffset;FLOAT3;0,0,0;Out;;Inherit;False;True;normalWS;FLOAT3;0,0,0;Out;;Inherit;False;GetWaveInfo;False;False;0;55772122574a8674194dae1ef64e7f84;False;11;0;FLOAT;0;False;1;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;9;FLOAT3;0,0,0;False;10;FLOAT3;0,0,0;False;3;FLOAT;0;FLOAT3;10;FLOAT3;11
Node;AmplifyShaderEditor.BlendNormalsNode;351;-5141.348,3504;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BlendNormalsNode;52;-5140.049,2335.201;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BlendNormalsNode;346;-5141.348,3888;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BlendNormalsNode;359;-4869.348,3744;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;582;-769.5663,5776.168;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BlendNormalsNode;310;-4861.158,2543.001;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;631;-4564.547,3698.371;Inherit;False;Property;_WaterQuliaty1;水动画质量;15;0;Create;False;0;0;0;False;0;False;0;2;2;True;;KeywordEnum;3;LOW;MID;HIGH;Reference;630;True;True;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector3Node;328;-4594.76,2245.401;Inherit;False;Constant;_Vector5;Vector 5;37;0;Create;True;0;0;0;False;0;False;0,0,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PosVertexDataNode;608;-517.266,5922.565;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;731;233.4654,5999.333;Inherit;False;Property;_MotionVertexDisplacement;交互顶点强度;60;0;Create;False;0;0;0;False;0;False;0.1;2.44;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;348;-4667.138,3468.325;Inherit;False;Constant;_Vector6;Vector 6;37;0;Create;True;0;0;0;False;0;False;0,0,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;732;310.4654,6078.333;Inherit;False;Constant;_Float11;Float 11;61;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TransformPositionNode;583;-522.1254,5767.264;Inherit;False;World;Object;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;352;-4596.724,3940.092;Inherit;False;Property;_LargeNormalIntensity;Large Normal Intensity;23;0;Create;True;0;0;0;False;0;False;0.1;0.1;0;0.2;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;630;-4606.198,2445.288;Inherit;False;Property;_WaterQuliaty;水动画质量;15;0;Create;False;0;0;0;False;2;Header((Water Normal));Space(5);False;0;2;2;True;;KeywordEnum;3;LOW;MID;HIGH;Create;True;True;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;330;-4674.76,2686.401;Inherit;False;Property;_SmallNormalIntensity;Small Normal Intensity;19;0;Create;True;0;0;0;False;0;False;0.1;0.1;0;0.2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;729;370.4654,5861.333;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;357;-4242.972,3681.7;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;327;-4320.56,2435.2;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;607;-172.9509,5819.234;Inherit;False;Property;_VERTEXWAVE;顶点波纹动画;48;0;Create;False;0;0;0;False;2;Header((Wave));Space(5);False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;358;-4066.972,3681.7;Inherit;False;LargeNormal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;44;-4019.961,2439.701;Inherit;False;SmallNormal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;727;502.4654,5765.333;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;628;-6735.564,5183.376;Inherit;False;44;SmallNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;194;660.6393,5766.51;Inherit;False;WaveVertexPos;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;629;-6718.437,5324.076;Inherit;False;358;LargeNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BlendNormalsNode;360;-6344.079,5203.159;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;195;2475.026,1342.448;Inherit;False;194;WaveVertexPos;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;366;-6040.372,5200.592;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.VertexToFragmentNode;739;2894.366,1520.263;Inherit;False;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;361;-5560.057,5203.564;Inherit;False;SurfaceNormal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;742;3162.366,1499.263;Inherit;False;Custom Screen Position;-1;;9;b0a29538730bccf49a848d4d334f4cfc;2,9,0,12,0;1;3;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;549;-6712.509,1103.015;Inherit;False;2584.837;577.0111;Fresnel;19;275;252;254;253;256;255;494;258;547;273;272;481;487;485;260;261;259;263;262;Fresnel;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;275;-6662.509,1168.536;Inherit;False;361;SurfaceNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;1;-2060.536,-1013.859;Inherit;False;2222.39;1211.228;Water Depth;27;667;468;669;668;666;492;422;433;472;570;447;449;622;471;425;567;624;569;566;446;455;459;469;423;465;427;650;Water Depth;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;741;3476.191,1482.382;Inherit;False;ScreenPos;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;650;-1967.905,-894.4185;Inherit;False;741;ScreenPos;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;254;-6399.772,1330.755;Inherit;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;252;-6379.772,1168.755;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.BreakToComponentsNode;427;-1555.691,-442.4845;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.DotProductOpNode;253;-6152.771,1232.755;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenDepthNode;465;-1555.32,-973.1565;Inherit;False;1;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LinearDepthNode;469;-1197.221,-960.3623;Inherit;False;0;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;660;-5969.613,896.807;Inherit;False;Property;_ReflectExhance;反射度增强;7;0;Create;False;0;0;0;False;0;False;1;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;658;-5975.613,1025.807;Inherit;False;Constant;_Float3;Float 3;54;0;Create;True;0;0;0;False;0;False;0.04;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;256;-6031.771,1264.755;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LinearDepthNode;423;-1245.642,-408.3715;Inherit;False;0;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;255;-5878.694,1266.421;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;659;-5752.613,955.807;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;422;-725.6522,-958.9788;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;661;-5604.613,986.807;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;180;-2044.752,3389.459;Inherit;False;2460.068;1106.858;Foam Color;30;407;406;179;411;181;176;381;182;372;382;384;383;385;479;386;150;149;404;405;484;371;483;478;148;477;495;568;564;614;615;Foam Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;492;-437.2292,-957.5327;Inherit;False;WaterDepth;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;459;-1426.697,-655.3661;Inherit;False;#if UNITY_REVERSED_Z$real depth = rawdepth@$#else$real depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, rawdepth)@$#endif$float3 worldPos = ComputeWorldSpacePosition(ScreenUV, depth, UNITY_MATRIX_I_VP)@$return worldPos@;3;Create;2;True;ScreenUV;FLOAT2;0,0;In;;Inherit;False;True;rawdepth;FLOAT;0;In;;Inherit;False;ReconstructWorldPos;True;False;0;;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;19;-2060.159,264.2607;Inherit;False;2361.305;1215.626;Water Color;28;675;674;673;672;437;436;16;434;271;11;671;24;596;598;10;251;13;595;597;21;486;545;268;265;264;548;677;542;Water Color;0.1020221,0.5797669,0.8161765,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;494;-5674.433,1225.052;Inherit;False;5;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;468;-816.1711,-512.7927;Inherit;False;UnderWaterPos;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;434;-2013.469,1268.49;Inherit;False;492;WaterDepth;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;477;-1989.771,3623.173;Inherit;False;492;WaterDepth;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;104;-253.7981,1592.509;Inherit;False;2422.145;803.8394;Caustics Color;22;101;618;619;112;544;102;546;116;103;100;113;93;115;95;114;91;97;98;96;92;90;89;Caustics Color;0.7379313,0,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;148;-1995.427,3739.037;Inherit;False;Property;_FoamRange;泡沫范围;38;0;Create;False;0;0;0;False;0;False;1.5;0.51;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;258;-5489.703,1178.17;Inherit;False;3;0;FLOAT;0.04;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-1997.245,1370.053;Inherit;False;Property;_WaterDeep;水深浅范围;3;0;Create;False;0;0;0;False;0;False;5;8.6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;89;-148.1402,1642.656;Inherit;False;468;UnderWaterPos;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TemplateFragmentDataNode;455;-1557.298,-74.32535;Inherit;False;0;1;clipPos;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;436;-1765.469,1318.49;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;478;-1818.771,3710.173;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;547;-5189.772,1169.425;Inherit;False;FresnelFactor;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;437;-1601.349,1331.76;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;548;-1555.088,1197.582;Inherit;False;547;FresnelFactor;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;92;48.15664,1763.509;Inherit;False;Property;_CausticsScale;焦散大小;29;0;Create;False;0;0;0;False;0;False;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;386;-1649.322,4010.193;Inherit;False;Property;_YTilling;泡沫TillingY;35;0;Create;False;0;0;0;False;0;False;1;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;371;-1934.347,4204.643;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;483;-1872.732,4113.379;Inherit;False;Property;_XTilling;泡沫TillingX;34;0;Create;False;0;0;0;False;0;False;10;20;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;90;89.15666,1642.509;Inherit;False;FLOAT2;0;2;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ClampOpNode;479;-1599.946,3872.685;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LinearDepthNode;446;-1245.01,-45.71001;Inherit;False;0;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;98;9.154695,1919.922;Inherit;False;Property;_CausticsSpeed;焦散速度;30;0;Create;False;0;0;0;False;0;False;-8,0;-8,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;264;-1260.339,1309.634;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;484;-1699.885,4169.742;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;405;-1849.141,3534.893;Inherit;False;Property;_FoamOffset;泡沫偏移;37;0;Create;False;0;0;0;False;0;False;0;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;667;-771.272,-717.9165;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;97;36.92546,2124.296;Inherit;False;Constant;_Float6;Float 6;15;0;Create;True;0;0;0;False;0;False;0.01;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;96;18.4784,2047.778;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;385;-1435.643,3962.207;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;91;308.1565,1676.509;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;566;-808.8323,-333.9212;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;666;-562.878,-642.1665;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;404;-1644.151,3624.933;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;569;-633.3572,-352.0144;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;114;423.9215,1933.072;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;265;-1093.105,1227.297;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;255.2105,2020.349;Inherit;False;3;3;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;383;-1296.356,4301.239;Inherit;False;Property;_FoamNoiseSpeed;泡沫速度;36;0;Create;False;0;0;0;False;0;False;0,-0.3;0,-0.3;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.DynamicAppendNode;384;-1248.196,4157.228;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ClampOpNode;268;-719.5121,1226.616;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;668;-317.0122,-641.9453;Inherit;False;FLOAT;1;1;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;93;632.1563,1675.509;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ClampOpNode;149;-1527.25,3711.502;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;496;1043.174,2646.558;Inherit;False;1746.843;758.4836;Alpha;22;497;627;625;414;626;504;213;563;27;501;291;562;376;499;502;439;276;653;654;657;670;676;Alpha;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;115;619.263,2003.638;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;624;-426.6443,-321.5014;Inherit;False;Property;_UNDERWATER2;水底;32;0;Create;False;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Reference;614;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;382;-1040.845,4152.915;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;669;-147.0123,-641.9453;Inherit;False;WaterHeight;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;372;-856.3123,4123.585;Inherit;True;Property;_FoamNoise;泡沫Noise;33;1;[NoScaleOffset];Create;False;0;0;0;False;0;False;-1;None;b8850342fb8b1e846ac3807e35ec1685;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;276;1104.161,2863.372;Inherit;False;Property;_ShoreDistance;边缘透明范围;12;0;Create;False;0;0;0;False;0;False;1.5;1.35;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;150;-1336.053,3707.048;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;100;793.1563,1647.509;Inherit;True;Property;_CausticsTex;焦散图;28;0;Create;False;0;0;0;False;0;False;-1;None;f7eca5a8da71d274a935b7ae77479b8d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;545;-295.8197,1247.74;Inherit;False;WaterDepthRange;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;113;775.4913,1974.393;Inherit;True;Property;_TextureSample1;Texture Sample 1;28;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;100;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;567;-168.0306,-358.2496;Inherit;False;FoamMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;495;-960.7318,3800.277;Inherit;False;Constant;_Float2;Float 2;40;0;Create;True;0;0;0;False;0;False;-1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;273;-5333.585,1366.491;Inherit;False;Property;_ReflectionAngle;菲涅尔反射角度;6;0;Create;False;0;0;0;False;0;False;1;1.36;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;546;1099.4,2167.614;Inherit;False;545;WaterDepthRange;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;381;-585.9371,3859.194;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;103;1118.791,1981.297;Inherit;False;Property;_CausticsIntensity;焦散亮度;31;0;Create;False;0;0;0;False;0;False;1;1.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;568;-439.0929,3943.669;Inherit;False;567;FoamMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;676;1288.152,2942.787;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;670;1068.506,2767.854;Inherit;False;669;WaterHeight;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMinOpNode;116;1106.922,1840.069;Inherit;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;176;-680.44,3654.798;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;544;1350.796,2128.302;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;272;-5114.413,1316.691;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;439;1381.174,2742.559;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;181;-360.6551,3639.804;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;102;1337.791,1856.296;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;619;1475.112,1673.594;Inherit;False;Constant;_Color0;Color 0;51;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;291;1619.81,2739.712;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;617;-86.07019,3482.873;Inherit;False;Constant;_Float8;Float 8;50;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;411;-71.17676,3644.594;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;112;1533.757,1925.655;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;481;-4848.977,1153.015;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;618;1692.413,1890.945;Inherit;False;Property;_Caustics;焦散动画;27;0;Create;False;0;0;0;False;2;Header((Caustics));Space(5);False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;487;-4681.069,1153.728;Inherit;False;ReflectFresnel;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;27;1877.196,2723.507;Inherit;False;WaterOpacity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;614;76.59351,3476.401;Inherit;False;Property;_FOAM;岸边泡沫;32;0;Create;False;0;0;0;False;2;Header((Foam));Space(5);False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;179;205.9429,3656.28;Inherit;False;Foam;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;681;1072.768,3557.726;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;101;1898.681,1890.374;Inherit;False;CausticsColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;502;1093.972,3156.754;Inherit;False;487;ReflectFresnel;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;688;1031.105,3712.218;Inherit;False;Constant;_Vector1;Vector 1;57;0;Create;True;0;0;0;False;0;False;0.5,0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;653;1101.805,3235.383;Inherit;False;27;WaterOpacity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;654;1305.089,3187.806;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;686;1343.105,3650.218;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;376;1149.643,3083.896;Inherit;False;179;Foam;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;499;1119.802,3317.775;Inherit;False;101;CausticsColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;562;1443.843,3132.297;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;501;1345.998,3318.448;Inherit;False;FLOAT;0;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;692;1472.105,3877.218;Inherit;False;Property;_Size;Size;58;0;Create;True;0;0;0;False;0;False;1;0.435;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;683;1589.185,3599.302;Inherit;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;691;1860.105,3648.218;Inherit;True;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;563;1540.721,3253.581;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;657;1702.476,3304.452;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;689;2067.105,3581.218;Inherit;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;694;2430.105,3786.218;Inherit;False;Property;_FallOff;FallOff;59;0;Create;True;0;0;0;False;0;False;1;0.05;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;504;1776.761,3137.705;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;690;2398.105,3493.218;Inherit;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;213;1489.201,2997.237;Inherit;False;27;WaterOpacity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;626;1943.163,3219.537;Inherit;False;Property;_Alpha;总体透明度控制;13;0;Create;False;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;693;2659.105,3575.218;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;414;1920.285,3017.159;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;695;2836.105,3510.218;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;625;2149.163,3085.537;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;613;-480.2349,6420.522;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;627;2394.46,3110.537;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TransformDirectionNode;584;-537.0981,6234.761;Inherit;False;World;Object;True;Fast;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.OneMinusNode;697;3081.66,3543.184;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;611;-226.2349,6266.522;Inherit;False;Property;_VERTEXWAVE;VERTEXWAVE;48;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;607;True;True;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;696;2581.131,3224.057;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;550;-6777.69,4437.351;Inherit;False;1483.719;522.8035;Splakes;7;561;559;537;560;540;536;535;Splakes;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;193;-2040.182,4705.584;Inherit;False;2604.553;905.533;Wave Vertex Animation ;21;243;240;197;241;202;199;203;198;237;189;191;196;244;188;239;190;204;242;245;238;192;Wave Vertex Animation ;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;30;-2047.93,2573.62;Inherit;False;1793.607;564.0559;ReflectColor;10;274;65;217;236;87;364;488;216;218;233;ReflectColor;0,0.006896734,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;497;2592.132,3101.233;Inherit;False;FinalAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;200;31.67461,6221.042;Inherit;False;WaveVertexNormal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;79;-2053.536,1593.248;Inherit;False;1753.72;842.3602;UnderWater Color;19;78;76;75;432;363;70;431;450;77;451;72;119;620;621;651;662;663;665;738;UnderWater Color;1,0.6827586,0,1;0;0
Node;AmplifyShaderEditor.WorldReflectionVector;236;-1358.104,2676.476;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;409;2149.342,1122.008;Inherit;False;407;FoamColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.CustomExpressionNode;203;-884.27,4959.316;Inherit;False;float steepness = wave.z * 0.01@$float wavelength = wave.w@$float k = 2 * PI / wavelength@$float c = sqrt(9.8 / k)@$float2 d = normalize(wave.xy)@$float f = k * (dot(d, position.xz) - c * time)@$float a = steepness / k@$			$$tangent += float3($-d.x * d.x * (steepness * sin(f)),$d.x * (steepness * cos(f)),$-d.x * d.y * (steepness * sin(f))$)@$$binormal += float3($-d.x * d.y * (steepness * sin(f)),$d.y * (steepness * cos(f)),$-d.y * d.y * (steepness * sin(f))$)@$$return float3($d.x * (a * cos(f)),$a * sin(f),$d.y * (a * cos(f))$)@;3;Create;5;True;position;FLOAT3;0,0,0;In;;Inherit;False;True;tangent;FLOAT3;1,0,0;InOut;;Inherit;False;True;binormal;FLOAT3;0,0,1;InOut;;Inherit;False;True;wave;FLOAT4;0,0,0,0;In;;Inherit;False;True;time;FLOAT;0;In;;Inherit;False;GerstnerWave;True;False;0;;False;5;0;FLOAT3;0,0,0;False;1;FLOAT3;1,0,0;False;2;FLOAT3;0,0,1;False;3;FLOAT4;0,0,0,0;False;4;FLOAT;0;False;3;FLOAT3;0;FLOAT3;2;FLOAT3;3
Node;AmplifyShaderEditor.ColorNode;621;-753.6715,2063.62;Inherit;False;Constant;_Color1;Color 1;52;0;Create;True;0;0;0;False;0;False;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;488;-875.064,2961.739;Inherit;False;487;ReflectFresnel;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;262;-4645.998,1298.026;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;242;-1501.589,5328.503;Inherit;False;Property;_WaveBSpeed;WaveBSpeed;45;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;78;-478.9516,2060.387;Inherit;False;UnderWaterColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;615;71.92981,3969.873;Inherit;False;Property;_FOAM1;岸边泡沫;32;0;Create;False;0;0;0;False;2;Header((Foam));Space(5);False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;614;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;561;-6168.991,4653.935;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;662;-1096.917,1951.506;Inherit;False;Property;_UnderWaterDark;水底压暗;10;0;Create;False;0;0;0;False;0;False;0;0.1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;233;-628.6934,2665.169;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector4Node;204;-1129.721,5174.523;Inherit;False;Property;_WaveC;WaveC;46;0;Create;True;0;0;0;False;0;False;0.5,-0.5,3,2;0.5,-0.5,3,2;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;432;-1539.522,1968.142;Inherit;False;ScreenDistort;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;620;-510.697,1883.61;Inherit;False;Property;_UNDERWATER;水底图;8;0;Create;False;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;610;-344.2349,6016.522;Inherit;False;Constant;_Float7;Float 7;49;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;191;-528.7634,4796.667;Inherit;False;4;4;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;119;-813.2425,1707.84;Inherit;False;SceneColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;65;-489.438,2651.563;Inherit;False;ReflectColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;11;-1283.071,406.1161;Inherit;False;Property;_ShallowColor;浅水颜色;1;0;Create;False;0;0;0;False;2;Header((Color Settings));Space(5);False;0.4862745,1,0.8588235,0;0.3040227,0.7924528,0.4800847,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;218;-1981.133,2872.425;Inherit;False;Property;_ReflectDistort;反射扭曲;25;0;Create;False;0;0;0;False;0;False;1;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TransformPositionNode;192;-155.216,4796.961;Inherit;False;World;Object;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SwizzleNode;593;-486.3947,6083.433;Inherit;False;FLOAT;1;1;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;433;-1968.526,-83.25314;Inherit;False;432;ScreenDistort;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;245;-1038.589,5353.503;Inherit;False;Property;_WaveCSpeed;WaveCSpeed;47;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;75;-1705.536,1948.248;Inherit;False;3;3;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;730;160.4654,5917.333;Inherit;False;-1;;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;66;1555.003,973.8758;Inherit;False;65;ReflectColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;450;-1540.526,2133.999;Inherit;False;449;RefractionMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;24;-197.8573,566.0661;Inherit;False;WaterColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;308;1867.276,1178.116;Inherit;False;Property;_DayIntensity;总体亮度控制;14;0;Create;False;0;0;0;False;0;False;0.75;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;239;-1698.256,5321.836;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;263;-4499.997,1296.026;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;188;-1560.584,4984.038;Inherit;False;float steepness = wave.z * 0.01@$float wavelength = wave.w@$float k = 2 * PI / wavelength@$float c = sqrt(9.8 / k)@$float2 d = normalize(wave.xy)@$float f = k * (dot(d, position.xz) - c * time)@$float a = steepness / k@$			$$tangent += float3($-d.x * d.x * (steepness * sin(f)),$d.x * (steepness * cos(f)),$-d.x * d.y * (steepness * sin(f))$)@$$binormal += float3($-d.x * d.y * (steepness * sin(f)),$d.y * (steepness * cos(f)),$-d.y * d.y * (steepness * sin(f))$)@$$return float3($d.x * (a * cos(f)),$a * sin(f),$d.y * (a * cos(f))$)@;3;Create;5;True;position;FLOAT3;0,0,0;In;;Inherit;False;True;tangent;FLOAT3;1,0,0;InOut;;Inherit;False;True;binormal;FLOAT3;0,0,1;InOut;;Inherit;False;True;wave;FLOAT4;0,0,0,0;In;;Inherit;False;True;time;FLOAT;0;In;;Inherit;False;GerstnerWave;True;False;0;;False;5;0;FLOAT3;0,0,0;False;1;FLOAT3;1,0,0;False;2;FLOAT3;0,0,1;False;3;FLOAT4;0,0,0,0;False;4;FLOAT;0;False;3;FLOAT3;0;FLOAT3;2;FLOAT3;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;406;-17.11719,4101.44;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;594;14.50423,6023.158;Inherit;False;WaveY;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;83;1857.875,998.5416;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;685;1839.105,3860.218;Inherit;False;BoxMask;-1;;10;9dce4093ad5a42b4aa255f0153c4f209;0;4;1;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;10;FLOAT3;0,0,0;False;17;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;189;-1919.758,4787.31;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;410;2175.447,1210.744;Inherit;False;179;Foam;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;201;2515.237,1469.376;Inherit;False;200;WaveVertexNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleTimeNode;240;-1501.712,5433.785;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;202;-1548.529,5157.735;Inherit;False;Property;_WaveB;WaveB;44;0;Create;True;0;0;0;False;0;False;0.6,0.8,1.5,10;0.6,0.8,1.5,10;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;680;-675.8921,522.4221;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;596;-931.7859,943.1348;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;535;-5791.277,4701.186;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CrossProductOpNode;197;-549.0609,5231.915;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;241;-1277.589,5355.503;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;673;-1494.594,700.3927;Inherit;False;Property;_WaterDeep1;清水范围;9;0;Create;False;0;0;0;False;0;False;1.5;7.32;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;238;-1962.256,5391.836;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;182;-390.5433,4116.394;Inherit;False;Property;_FoamColor;泡沫颜色;39;0;Create;False;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;216;-1909.132,2631.424;Inherit;False;Constant;_Vector0;Vector 0;32;0;Create;True;0;0;0;False;0;False;0,0,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;538;1574.626,1183.497;Inherit;False;537;Splakes;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;447;-913.5317,-166.437;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;408;2407.244,1006.853;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;25;1560.139,872.8199;Inherit;False;24;WaterColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;271;-1285.834,309.6893;Inherit;False;78;UnderWaterColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;498;2524.855,1240.989;Inherit;False;497;FinalAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;486;-1835.876,1010.793;Inherit;False;485;ColorFresnel;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;244;-814.5888,5380.503;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;77;-1997.536,2076.249;Inherit;False;Constant;_Float5;Float 5;11;0;Create;True;0;0;0;False;0;False;0.01;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;259;-4809.998,1306.026;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;598;-1247.422,1104.224;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;536;-5978.269,4808.187;Inherit;False;Property;_SparklesIntensity;波光亮度;40;0;Create;False;0;0;0;False;2;Header(Sparkles);Space(5);False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;431;-1811.616,1855.386;Inherit;False;FLOAT2;0;1;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;663;-799.9181,1955.506;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenColorNode;70;-1014.476,1710.472;Inherit;False;Global;_GrabScreen0;Grab Screen 0;10;0;Create;True;0;0;0;False;0;False;Object;-1;False;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;485;-4351.672,1298.572;Inherit;False;ColorFresnel;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;651;-1491.639,1706.661;Inherit;False;741;ScreenPos;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StaticSwitch;609;-210.266,6043.266;Inherit;False;Property;_VERTEXWAVE;VERTEXWAVE;48;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;607;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;261;-5000.998,1564.026;Inherit;False;Constant;_Float9;Float 9;33;0;Create;True;0;0;0;False;0;False;5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;564;-323.22,4308.273;Inherit;False;Constant;_Float4;Float 4;40;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;570;-644.0327,-150.0048;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;542;-906.358,668.8412;Inherit;False;101;CausticsColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.CustomExpressionNode;196;-1224.601,4984.125;Inherit;False;float steepness = wave.z * 0.01@$float wavelength = wave.w@$float k = 2 * PI / wavelength@$float c = sqrt(9.8 / k)@$float2 d = normalize(wave.xy)@$float f = k * (dot(d, position.xz) - c * time)@$float a = steepness / k@$			$$tangent += float3($-d.x * d.x * (steepness * sin(f)),$d.x * (steepness * cos(f)),$-d.x * d.y * (steepness * sin(f))$)@$$binormal += float3($-d.x * d.y * (steepness * sin(f)),$d.y * (steepness * cos(f)),$-d.y * d.y * (steepness * sin(f))$)@$$return float3($d.x * (a * cos(f)),$a * sin(f),$d.y * (a * cos(f))$)@;3;Create;5;True;position;FLOAT3;0,0,0;In;;Inherit;False;True;tangent;FLOAT3;1,0,0;InOut;;Inherit;False;True;binormal;FLOAT3;0,0,1;InOut;;Inherit;False;True;wave;FLOAT4;0,0,0,0;In;;Inherit;False;True;time;FLOAT;0;In;;Inherit;False;GerstnerWave;True;False;0;;False;5;0;FLOAT3;0,0,0;False;1;FLOAT3;1,0,0;False;2;FLOAT3;0,0,1;False;3;FLOAT4;0,0,0,0;False;4;FLOAT;0;False;3;FLOAT3;0;FLOAT3;2;FLOAT3;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;407;205.8506,4094.498;Inherit;False;FoamColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;451;-1274.921,1927.926;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;190;-1961.758,5085.31;Inherit;False;Property;_WaveADirSteepnesswavelength;WaveA(Dir,Steepness,wavelength);42;0;Create;True;0;0;0;False;2;Header((Wave));Space(5);False;0.7,0.8,1,50;0.7,0.8,1,50;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;307;2023.814,1003.797;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalizeNode;198;-350.0591,5265.915;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TransformDirectionNode;199;-140.2974,5267.096;Inherit;False;World;Object;False;Fast;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LerpOp;13;-508.5121,860.5709;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;87;-901.649,2848.399;Inherit;False;Property;_ReflectIntensity;反射强度;26;0;Create;False;0;0;0;False;0;False;1;0.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;363;-2003.053,1852.959;Inherit;False;361;SurfaceNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;274;-1000.808,2641.209;Inherit;True;Property;_ReflectCube;反射图;24;0;Create;False;0;0;0;False;2;Header((Reflection));Space(5);False;-1;None;cebf816f31a6d3a41832b558b280fa15;True;0;False;white;LockedToCube;False;Object;-1;Auto;Cube;8;0;SAMPLERCUBE;;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;425;-1723.222,-233.4845;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ScreenDepthNode;471;-1577.717,-244.042;Inherit;False;1;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;76;-2002.536,1974.248;Inherit;False;Property;_UnderWaterDistort;水底折射;11;0;Create;False;0;0;0;False;0;False;3;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;738;-1387.278,1811.7;Inherit;False;Property;_DistortFix;扭曲修正;0;0;Create;False;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;537;-5651.755,4701.517;Inherit;False;Splakes;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;622;-451.6099,-149.51;Inherit;False;Property;_UNDERWATER1;水底;8;0;Create;False;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Reference;620;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;665;-595.8502,1731.896;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;449;-185.9583,-157.5467;Inherit;False;RefractionMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;10;-1859.18,651.0201;Inherit;False;Property;_DeepColor;深水颜色;2;0;Create;False;0;0;0;False;0;False;0,0.4666667,0.7450981,0;0,0.4292973,0.509434,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;243;-1020.218,5460.838;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;21;-1865.72,824.627;Inherit;False;Property;_FresnelColor;菲涅尔颜色;4;0;Create;False;0;0;0;False;0;False;0.3686275,0.6431373,0.9137255,0;0.3668992,0.6433284,0.9150943,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;72;-1175.488,1714.849;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;217;-1577.133,2679.424;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LinearDepthNode;472;-1248.448,-248.9217;Inherit;False;0;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;560;-6441.147,4646.007;Inherit;False;FLOAT;1;1;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;595;-1397.417,955.5938;Inherit;False;Property;_WaveColor;波峰颜色;57;1;[HDR];Create;False;0;0;0;False;0;False;0.3686275,0.6431373,0.9137255,0;0.3686275,0.6431373,0.9137255,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;597;-1532.46,1104.751;Inherit;False;594;WaveY;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;251;-1530.422,830.7002;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;674;-1240.042,624.963;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;671;-876.2928,476.9991;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;559;-6669.639,4634.313;Inherit;False;361;SurfaceNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ClampOpNode;675;-1118.953,604.2272;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;540;-6580.513,4756.486;Inherit;False;Property;_SparklesAmount;波光数量;41;0;Create;False;0;0;0;False;0;False;0.09;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;260;-5062.997,1475.026;Inherit;False;Property;_FresnelIntensity;菲涅尔强度;5;0;Create;False;0;0;0;False;0;False;0.2;0.54;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;677;-1331.83,772.4922;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;237;-1922.256,5294.836;Inherit;False;Property;_WaveASpeed;WaveASpeed;43;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;672;-1488.042,574.963;Inherit;False;492;WaterDepth;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;364;-1951.72,2773.793;Inherit;False;361;SurfaceNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;632;2837.034,1030.68;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;3;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;0;False;False;0;Hidden/InternalErrorShader;4;=;=;=;=;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;634;2837.034,1030.68;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;3;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=ShadowCaster;False;False;0;Hidden/InternalErrorShader;4;=;=;=;=;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;635;2837.034,1030.68;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;3;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;True;False;False;False;False;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;False;False;True;1;LightMode=DepthOnly;False;False;0;Hidden/InternalErrorShader;4;=;=;=;=;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;633;2858.086,1003.241;Float;False;True;-1;2;ASEMaterialInspector;0;3;CartoonWater_Interactive;2992e84f91cbeb14eab234972e07ea9d;True;Forward;0;1;Forward;8;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;2;True;8;d3d9;d3d11_9x;d3d11;glcore;gles;gles3;metal;vulkan;0;False;True;1;5;False;-1;10;False;-1;1;1;False;-1;10;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;2;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=UniversalForward;False;False;0;Hidden/InternalErrorShader;4;=;=;=;=;0;Standard;22;Surface;1;  Blend;0;Two Sided;1;Cast Shadows;0;  Use Shadow Threshold;0;Receive Shadows;0;GPU Instancing;0;LOD CrossFade;0;Built-in Fog;0;DOTS Instancing;0;Meta Pass;0;Extra Pre Pass;0;Tessellation;0;  Phong;0;  Strength;0.5,False,-1;  Type;0;  Tess;16,False,-1;  Min;10,False,-1;  Max;25,False,-1;  Edge Length;16,False,-1;  Max Displacement;25,False,-1;Vertex Position,InvertActionOnDeselection;1;0;5;False;True;False;True;False;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;636;2837.034,1030.68;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;3;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;Hidden/InternalErrorShader;4;=;=;=;=;0;Standard;0;False;0
WireConnection;315;0;314;0
WireConnection;315;1;34;0
WireConnection;337;0;340;0
WireConnection;337;1;341;0
WireConnection;322;0;315;0
WireConnection;322;1;323;0
WireConnection;319;0;315;0
WireConnection;319;1;320;1
WireConnection;325;0;315;0
WireConnection;325;1;326;0
WireConnection;334;0;337;0
WireConnection;334;1;356;0
WireConnection;349;0;337;0
WireConnection;349;1;354;0
WireConnection;40;0;317;0
WireConnection;40;1;41;0
WireConnection;40;2;42;0
WireConnection;336;0;337;0
WireConnection;336;1;353;1
WireConnection;333;0;347;0
WireConnection;333;1;332;0
WireConnection;333;2;355;0
WireConnection;339;0;336;0
WireConnection;339;1;333;0
WireConnection;350;0;337;0
WireConnection;350;1;333;0
WireConnection;316;0;315;0
WireConnection;316;1;40;0
WireConnection;338;0;334;0
WireConnection;338;1;333;0
WireConnection;318;0;319;0
WireConnection;318;1;40;0
WireConnection;321;0;322;0
WireConnection;321;1;40;0
WireConnection;335;0;349;0
WireConnection;335;1;333;0
WireConnection;324;0;325;0
WireConnection;324;1;40;0
WireConnection;312;1;321;0
WireConnection;575;0;574;0
WireConnection;575;1;578;0
WireConnection;575;2;591;0
WireConnection;343;1;335;0
WireConnection;311;1;324;0
WireConnection;45;1;318;0
WireConnection;345;1;350;0
WireConnection;573;0;572;0
WireConnection;344;1;339;0
WireConnection;342;1;338;0
WireConnection;43;1;316;0
WireConnection;313;0;312;0
WireConnection;313;1;311;0
WireConnection;571;1;573;0
WireConnection;571;2;575;0
WireConnection;571;3;587;0
WireConnection;571;4;586;0
WireConnection;571;5;579;0
WireConnection;571;6;585;0
WireConnection;571;7;580;0
WireConnection;571;8;581;0
WireConnection;351;0;345;0
WireConnection;351;1;344;0
WireConnection;52;0;43;0
WireConnection;52;1;45;0
WireConnection;346;0;343;0
WireConnection;346;1;342;0
WireConnection;359;0;351;0
WireConnection;359;1;346;0
WireConnection;582;0;572;0
WireConnection;582;1;571;10
WireConnection;310;0;52;0
WireConnection;310;1;313;0
WireConnection;631;1;345;0
WireConnection;631;0;351;0
WireConnection;631;2;359;0
WireConnection;583;0;582;0
WireConnection;630;1;43;0
WireConnection;630;0;52;0
WireConnection;630;2;310;0
WireConnection;729;0;732;0
WireConnection;729;1;731;0
WireConnection;357;0;348;0
WireConnection;357;1;631;0
WireConnection;357;2;352;0
WireConnection;327;0;328;0
WireConnection;327;1;630;0
WireConnection;327;2;330;0
WireConnection;607;1;608;0
WireConnection;607;0;583;0
WireConnection;358;0;357;0
WireConnection;44;0;327;0
WireConnection;727;0;607;0
WireConnection;727;1;729;0
WireConnection;194;0;727;0
WireConnection;360;0;628;0
WireConnection;360;1;629;0
WireConnection;366;0;360;0
WireConnection;739;0;195;0
WireConnection;361;0;366;0
WireConnection;742;3;739;0
WireConnection;741;0;742;0
WireConnection;252;0;275;0
WireConnection;427;0;650;0
WireConnection;253;0;252;0
WireConnection;253;1;254;0
WireConnection;465;0;650;0
WireConnection;469;0;465;0
WireConnection;256;0;253;0
WireConnection;423;0;427;2
WireConnection;255;0;256;0
WireConnection;659;0;660;0
WireConnection;659;1;658;0
WireConnection;422;0;469;0
WireConnection;422;1;423;0
WireConnection;661;0;659;0
WireConnection;492;0;422;0
WireConnection;459;0;650;0
WireConnection;459;1;465;0
WireConnection;494;0;255;0
WireConnection;494;1;255;0
WireConnection;494;2;255;0
WireConnection;494;3;255;0
WireConnection;494;4;255;0
WireConnection;468;0;459;0
WireConnection;258;0;661;0
WireConnection;258;2;494;0
WireConnection;436;0;434;0
WireConnection;436;1;16;0
WireConnection;478;0;477;0
WireConnection;478;1;148;0
WireConnection;547;0;258;0
WireConnection;437;0;436;0
WireConnection;90;0;89;0
WireConnection;479;0;478;0
WireConnection;446;0;455;3
WireConnection;264;0;548;0
WireConnection;264;1;437;0
WireConnection;484;0;483;0
WireConnection;484;1;371;1
WireConnection;385;0;479;0
WireConnection;385;1;386;0
WireConnection;91;0;90;0
WireConnection;91;1;92;0
WireConnection;566;0;469;0
WireConnection;566;1;446;0
WireConnection;666;0;667;0
WireConnection;666;1;468;0
WireConnection;404;0;405;0
WireConnection;404;1;478;0
WireConnection;569;1;566;0
WireConnection;114;0;91;0
WireConnection;265;0;548;0
WireConnection;265;1;264;0
WireConnection;95;0;98;0
WireConnection;95;1;96;0
WireConnection;95;2;97;0
WireConnection;384;0;484;0
WireConnection;384;1;385;0
WireConnection;268;0;265;0
WireConnection;668;0;666;0
WireConnection;93;0;91;0
WireConnection;93;1;95;0
WireConnection;149;0;404;0
WireConnection;115;0;114;0
WireConnection;115;1;95;0
WireConnection;624;0;569;0
WireConnection;382;0;384;0
WireConnection;382;2;383;0
WireConnection;669;0;668;0
WireConnection;372;1;382;0
WireConnection;150;0;149;0
WireConnection;100;1;93;0
WireConnection;545;0;268;0
WireConnection;113;1;115;0
WireConnection;567;0;624;0
WireConnection;381;0;372;1
WireConnection;381;1;150;0
WireConnection;676;0;276;0
WireConnection;116;0;100;0
WireConnection;116;1;113;0
WireConnection;176;0;150;0
WireConnection;176;1;495;0
WireConnection;544;0;546;0
WireConnection;272;0;258;0
WireConnection;272;1;273;0
WireConnection;439;0;670;0
WireConnection;439;1;676;0
WireConnection;181;0;176;0
WireConnection;181;1;381;0
WireConnection;181;2;568;0
WireConnection;102;0;116;0
WireConnection;102;1;103;0
WireConnection;291;0;439;0
WireConnection;411;0;181;0
WireConnection;112;0;102;0
WireConnection;112;1;544;0
WireConnection;481;0;272;0
WireConnection;618;1;619;0
WireConnection;618;0;112;0
WireConnection;487;0;481;0
WireConnection;27;0;291;0
WireConnection;614;1;617;0
WireConnection;614;0;411;0
WireConnection;179;0;614;0
WireConnection;101;0;618;0
WireConnection;654;0;502;0
WireConnection;654;1;653;0
WireConnection;686;0;681;0
WireConnection;686;1;688;0
WireConnection;562;0;376;0
WireConnection;562;1;654;0
WireConnection;501;0;499;0
WireConnection;683;0;686;0
WireConnection;691;0;683;0
WireConnection;691;1;692;0
WireConnection;563;0;562;0
WireConnection;563;1;501;0
WireConnection;657;1;563;0
WireConnection;689;0;691;0
WireConnection;504;0;657;0
WireConnection;690;0;689;0
WireConnection;693;0;690;0
WireConnection;693;1;694;0
WireConnection;414;0;213;0
WireConnection;414;2;504;0
WireConnection;695;0;693;0
WireConnection;625;0;414;0
WireConnection;625;1;626;0
WireConnection;627;0;625;0
WireConnection;584;0;571;11
WireConnection;697;0;695;0
WireConnection;611;1;613;0
WireConnection;611;0;584;0
WireConnection;696;0;627;0
WireConnection;696;1;697;0
WireConnection;497;0;696;0
WireConnection;200;0;611;0
WireConnection;236;0;217;0
WireConnection;203;0;189;0
WireConnection;203;1;196;2
WireConnection;203;2;196;3
WireConnection;203;3;204;0
WireConnection;203;4;244;0
WireConnection;262;0;259;0
WireConnection;262;1;259;0
WireConnection;78;0;620;0
WireConnection;615;1;182;0
WireConnection;615;0;406;0
WireConnection;561;0;540;0
WireConnection;561;1;560;0
WireConnection;233;0;274;0
WireConnection;233;1;87;0
WireConnection;233;2;488;0
WireConnection;432;0;75;0
WireConnection;620;1;621;0
WireConnection;620;0;665;0
WireConnection;191;0;189;0
WireConnection;191;1;188;0
WireConnection;191;2;196;0
WireConnection;191;3;203;0
WireConnection;119;0;70;0
WireConnection;65;0;233;0
WireConnection;192;0;191;0
WireConnection;593;0;571;10
WireConnection;75;0;431;0
WireConnection;75;1;76;0
WireConnection;75;2;77;0
WireConnection;24;0;13;0
WireConnection;239;0;237;0
WireConnection;239;1;238;0
WireConnection;263;0;262;0
WireConnection;188;0;189;0
WireConnection;188;3;190;0
WireConnection;188;4;239;0
WireConnection;406;0;411;0
WireConnection;406;1;182;0
WireConnection;406;2;564;0
WireConnection;594;0;609;0
WireConnection;83;0;25;0
WireConnection;83;1;66;0
WireConnection;83;2;538;0
WireConnection;680;0;671;0
WireConnection;680;1;542;0
WireConnection;596;0;251;0
WireConnection;596;1;595;0
WireConnection;596;2;598;0
WireConnection;535;0;561;0
WireConnection;535;1;536;0
WireConnection;197;0;203;3
WireConnection;197;1;203;2
WireConnection;241;0;242;0
WireConnection;241;1;240;0
WireConnection;447;0;472;0
WireConnection;447;1;446;0
WireConnection;408;0;307;0
WireConnection;408;1;409;0
WireConnection;408;2;410;0
WireConnection;244;0;245;0
WireConnection;244;1;243;0
WireConnection;259;0;272;0
WireConnection;259;1;260;0
WireConnection;259;2;261;0
WireConnection;598;0;597;0
WireConnection;431;0;363;0
WireConnection;663;0;662;0
WireConnection;70;0;72;0
WireConnection;485;0;263;0
WireConnection;609;1;610;0
WireConnection;609;0;593;0
WireConnection;570;1;447;0
WireConnection;196;0;189;0
WireConnection;196;1;188;2
WireConnection;196;2;188;3
WireConnection;196;3;202;0
WireConnection;196;4;241;0
WireConnection;407;0;615;0
WireConnection;451;1;432;0
WireConnection;451;2;450;0
WireConnection;307;0;83;0
WireConnection;307;1;308;0
WireConnection;198;0;197;0
WireConnection;199;0;198;0
WireConnection;13;0;680;0
WireConnection;13;1;596;0
WireConnection;13;2;268;0
WireConnection;274;1;236;0
WireConnection;425;0;650;0
WireConnection;425;1;433;0
WireConnection;471;0;425;0
WireConnection;738;1;432;0
WireConnection;738;0;451;0
WireConnection;537;0;535;0
WireConnection;622;0;570;0
WireConnection;665;0;119;0
WireConnection;665;1;663;0
WireConnection;449;0;622;0
WireConnection;72;0;651;0
WireConnection;72;1;738;0
WireConnection;217;0;216;0
WireConnection;217;1;364;0
WireConnection;217;2;218;0
WireConnection;472;0;471;0
WireConnection;560;0;559;0
WireConnection;251;0;10;0
WireConnection;251;1;21;0
WireConnection;251;2;486;0
WireConnection;674;0;672;0
WireConnection;674;1;677;0
WireConnection;671;0;271;0
WireConnection;671;1;11;0
WireConnection;671;2;675;0
WireConnection;675;0;674;0
WireConnection;677;0;673;0
WireConnection;633;2;408;0
WireConnection;633;3;498;0
WireConnection;633;5;195;0
WireConnection;633;6;201;0
ASEEND*/
//CHKSM=E261D5F7A4DB30EDEC8790CFD7D60EB176D9AA42