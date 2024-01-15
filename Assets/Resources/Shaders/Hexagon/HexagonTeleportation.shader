Shader "URP/Hexagon Teleportation"
{
    Properties
    {
    	[Header(Rim)]
        [Space(10)]
        //_RimPower("RimPower", Float) = 2
		_RimScale("RimScale", Float) = 1
		_RimBias("RimBias", Float) = 0
    	
    	[Header(Emiss)]
        [Space(10)]
		[HDR]_EmissColor("EmissColor", Color) = (0,0,0,0)
		_EmissIntensity("EmissIntensity", Float) = 1
    	
    	[Header(Line)]
        [Space(10)]
    	_LineEmissMask("LineEmissMask", 2D) = "white" {}
        //_HexagonLineIntensity("HexagonLineIntensity", Float) = 1
        _LineEmissIntensity("LineEmissIntensity", Float) = 1
		_LineMaskSpeed("LineMaskSpeed", Vector) = (0.1,0.1,0,0)
    	
//    	[Header(Aura)]
//        [Space(10)]
//		_AuraTex("AuraTex", 2D) = "white" {}
//		_AuraIntensity("AuraIntensity", Float) = 1
//		_AuraSpeed("AuraSpeed", Vector) = (0.02,0.035,0,0)
//		_AuraTexMask("AuraTexMask", 2D) = "white" {}
//    	
    	[Header(Disslve)]
        [Space(10)]
		_DisslvePoint("DisslvePoint", Vector) = (0,0,0,0)
		//_DepthFade("DepthFade", Float) = 1
		_DissolveAmount("DissolveAmount", Range( 0 , 1)) = 1
		_DissolveSpread("DissolveSpread", Float) = 1
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent+450"}
        Pass
        {
           Tags{"LightMode" = "UniversalForward"}
            Cull Back
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        	ZWrite Off
			
            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Core.hlsl"
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Lighting.hlsl"
           struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                half2 texcoord1     : TEXCOORD0;
           		half2 texcoord4     : TEXCOORD3;
           		half2 texcoord3     : TEXCOORD2;
           	
            };
            struct Varyings
            {
                half2 uv1                       : TEXCOORD0;
            	half2 uv4                      : TEXCOORD1;
            	half2 uv3                       : TEXCOORD2;
                float3 positionWS               : TEXCOORD3;
                float3 normalWS                 : TEXCOORD4;
            	half dissolve					 : TEXCOORD5;
                float4 positionCS               : SV_POSITION;
            };
            
            TEXTURE2D(_LineEmissMask);
            SAMPLER(sampler_LineEmissMask);
            // TEXTURE2D(_AuraTex);
            // SAMPLER(sampler_AuraTex);
            // TEXTURE2D(_AuraTexMask);
            // SAMPLER(sampler_AuraTexMask);
           
            CBUFFER_START(UnityPerMaterial)
				half4 _EmissColor;
				half3 _DisslvePoint;
				half2 _LineMaskSpeed;
				half _RimScale;
				half _RimBias;
				half _EmissIntensity;
				half _DissolveSpread;
				half _LineEmissIntensity;
				half _DissolveAmount;
				//half2 _AuraSpeed;
				//float _RimPower;
				//half _HexagonLineIntensity;
				//half _AuraIntensity;
				//half _DepthFade;
			CBUFFER_END
            
           Varyings LitPassVertex(Attributes input)
         {
				Varyings o = (Varyings)0;
				//input data
				half3 normalWS = TransformObjectToWorldNormal(input.normalOS);
				half3 normalOS = mul( GetWorldToObjectMatrix(), float4( normalWS, 0 ) ).xyz;
				float3 positionWS = mul(GetObjectToWorldMatrix(),  input.positionOS).xyz;
				//dissolve
				half3 worldOriginDir = mul( GetObjectToWorldMatrix(), half4( half3( 0,0,0 ), 1 ) ).xyz;
				float3 posDir = ( positionWS - worldOriginDir );
				half posDotn = dot( posDir , normalWS );
				half nDotn = dot( normalWS , normalWS );
				half3 PointToCenterDir = -( posDir - ( ( posDotn * rcp(nDotn) ) * normalWS ) );
				half3 HexagonCenter = ( positionWS + PointToCenterDir );
				half dissolve = clamp( ( ( distance( _DisslvePoint , ( HexagonCenter - worldOriginDir ) ) - (-0.5 + (_DissolveAmount - 1.0) * (3.5) * rcp(-1.0)) ) * rcp(_DissolveSpread)  ) , 0.0 , 1.0 );
				half dissolveSpread = ( 1.0 - dissolve );
				//offset
				float3 worldToObj = mul( GetWorldToObjectMatrix(), float4( ( positionWS + ( PointToCenterDir * dissolveSpread ) ), 1 ) ).xyz;
				float3 vertexValue = ( ( ( 0.1 * normalOS ) * dissolveSpread ) + worldToObj );
				float3 positionWS1 = TransformObjectToWorld( vertexValue );
				float4 positionCS = TransformWorldToHClip( positionWS1 );
				o.positionWS = positionWS1;
				o.positionCS = positionCS;
				o.normalWS = normalWS;
				o.dissolve  = dissolve;
				o.uv1 = input.texcoord1;
				o.uv4 = input.texcoord4;
				o.uv3 = input.texcoord3;
				return o;
            }

            half4 LitPassFragment (Varyings input) : SV_Target
            {
            	//input data
                half2 uv1  = input.uv1;
            	half2 uv4  = input.uv4;
            	half2 uv3  = input.uv3;
            	half3 positionWS = input.positionWS;
            	half3 vDirWS = GetWorldSpaceNormalizeViewDir(positionWS);
            	half3 nDirWS = normalize(input.normalWS);
            	
            	//fresnel
            	half fresnelTerm = 1.0 - dot(vDirWS,nDirWS);
            	half fresnel =(fresnelTerm * fresnelTerm * fresnelTerm) * _RimScale + _RimBias;
            	
            	//line
            	half LineEmiss = SAMPLE_TEXTURE2D(_LineEmissMask,sampler_LineEmissMask,uv1).r;
            	half LineEmissMask = SAMPLE_TEXTURE2D(_LineEmissMask,sampler_LineEmissMask,uv3 + _Time.y * _LineMaskSpeed).r;
            	half HexagonLine = LineEmissMask * LineEmiss ;
            	half LineColor = HexagonLine * _LineEmissIntensity;
            	
            	//flow
				// half AuraMap = SAMPLE_TEXTURE2D(_AuraTex,sampler_AuraTex,uv4 * half2(0.5,0.5) + _Time.y * _AuraSpeed * half2(1.5,2)).r;
				// half AuraMap1 = SAMPLE_TEXTURE2D(_AuraTex,sampler_AuraTex,(uv4 + AuraMap * 0.5) + _Time.y * _AuraSpeed);
				// half AuraMask = SAMPLE_TEXTURE2D(_AuraTex,sampler_AuraTex,uv4 + _Time.y * half2(0,0.02)).r * _AuraIntensity * 2* 0.01 * AuraMap1;
				// half flow = AuraMap1 + AuraMask;

     			//dissolve
            	half dissolveAlpha = step(0.01,input.dissolve);
            	half dissolveEmiss = dissolveAlpha * (1- input.dissolve) * 2.0;
            	
            	//final
            	half mask = fresnel  + LineColor +  dissolveEmiss + dissolveEmiss ;
            	half3 finalColor = _EmissColor * _EmissIntensity * mask;
            	half finalAplha = saturate(mask) * dissolveAlpha;
            	return half4(finalColor,finalAplha);
            }   
            ENDHLSL
        }
    }
     FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
