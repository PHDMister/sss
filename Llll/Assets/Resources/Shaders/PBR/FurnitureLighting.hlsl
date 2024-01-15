#ifndef STANDARD_LIGHTING_INCLUDE
    #define STANDARD_LIGHTING_INCLUDE
	#include "LightingCommon.hlsl"
	#include "Assets/Resources/Shaders/PBR/LightingCustom.hlsl"
	
   
	void IndirectLighting_float(float3 DiffuseColor, float3 SpecularColor, float Roughness, float3 WorldPos, float3 N, float3 V,
						float Occlusion,float2 lightUV,half _LightIntensitiy, out float3 IndirectLighting)
    {
    	IndirectLighting = half3(0,0,0);
    	#ifndef SHADERGRAPH_PREVIEW
       float NoV = saturate(abs(dot(N,V))+1e-5);
    	half3 Irradiance = half3(0,0,0);
    	#if defined(LIGHTMAP_ON)
    	float4 encodedIrradiance = SAMPLE_TEXTURE2D(unity_Lightmap,samplerunity_Lightmap,lightUV);
    	Irradiance = DecodeLightmap(encodedIrradiance, float4(LIGHTMAP_HDR_MULTIPLIER, LIGHTMAP_HDR_EXPONENT, 0.0h, 0.0h)) * _LightIntensitiy ;
    	#else
    	Irradiance = SampleSH(N);
		//Irradiance = 1;
    	#endif
    	
       //SH
       float3 DiffuseAO = AOMultiBounce(DiffuseColor,Occlusion);
       float3 IndirectDiffuse = Irradiance * DiffuseColor *DiffuseAO;
       //IBL
       half3 R = reflect(-V,N);
       float3 SpecularG = GlossyEnvironmentReflection(R,Roughness,Occlusion);
       float3 SpecularDFG = EnvBRDFApprox(SpecularColor,Roughness,NoV);
       float SpecularOcclusion = GetSpecularOcclusion(NoV,Pow2(Roughness),Occlusion);
       float3 SpecularAO = AOMultiBounce(SpecularColor,SpecularOcclusion);
       float3 IndirectSpecular = SpecularG * SpecularDFG * SpecularAO;
        IndirectLighting = IndirectDiffuse + IndirectSpecular;
    	#endif
    }
	void IndirectLighting_customReflection(float3 DiffuseColor, float3 SpecularColor, float Roughness, float3 WorldPos, float3 N, float3 V,
						float Occlusion,float2 lightUV,half4  encodedIrradiance_re,half4 ReflectionCubeMap_HDR,out float3 IndirectLighting)
	{
		IndirectLighting = half3(0,0,0);
		#ifndef SHADERGRAPH_PREVIEW
		float NoV = saturate(abs(dot(N,V))+1e-5);
		half3 Irradiance = half3(0,0,0);
		#if defined(LIGHTMAP_ON)
		float4 encodedIrradiance = SAMPLE_TEXTURE2D(unity_Lightmap,samplerunity_Lightmap,lightUV);
		Irradiance = DecodeLightmap(encodedIrradiance, float4(LIGHTMAP_HDR_MULTIPLIER, LIGHTMAP_HDR_EXPONENT, 0.0h, 0.0h));
		#else
		Irradiance = SampleSH(N);
		//Irradiance = 1;
		#endif
    	
		//SH
		float3 DiffuseAO = AOMultiBounce(DiffuseColor,Occlusion);
		float3 IndirectDiffuse = Irradiance * DiffuseColor *DiffuseAO;
		//IBL
		half3 R = reflect(-V,N);
		float3 SpecularG = GlossyEnvironmentReflection_custom(R,Roughness,Occlusion,encodedIrradiance_re,ReflectionCubeMap_HDR);
		float3 SpecularDFG = EnvBRDFApprox(SpecularColor,Roughness,NoV);
		float SpecularOcclusion = GetSpecularOcclusion(NoV,Pow2(Roughness),Occlusion);
		float3 SpecularAO = AOMultiBounce(SpecularColor,SpecularOcclusion);
		float3 IndirectSpecular = SpecularG * SpecularDFG * SpecularAO;
		IndirectLighting = IndirectDiffuse + IndirectSpecular;
		#endif
	}
	void fogLinear(float Distance,float Start,float End,out float fog)
	{
		fog = (1-saturate((End - Distance) / (End - Start)));
	}

	
#endif