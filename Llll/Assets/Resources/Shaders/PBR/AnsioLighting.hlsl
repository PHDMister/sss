#ifndef ANSIO_LIGHTING_INCLUDE
    #define ANSIO_LIGHTING_INCLUDE
	#include "LightingCommon.hlsl"
	#include "Assets/Resources/Shaders/PBR/LightingCustom.hlsl"

	#include "Library/PackageCache/com.unity.render-pipelines.core@10.10.1/ShaderLibrary/BSDF.hlsl"
struct AnisoSpecularData
{
	half3 specularColor;
	half3 specularSecondaryColor;
	half specularShift;
	half specularSecondaryShift;
	half specularStrength;
	half specularSecondaryStrength;
	half specularExponent;
	half specularSecondaryExponent;
	half spread1;
	half spread2;
};


inline half3 AnisotropyDoubleSpecular(half3 SpecularColor , half2 uv, half4 tangentWS, half3 H,half3 N,half3 lDir,
	AnisoSpecularData anisoSpecularData, half4 detailNormal )
{
	half specMask = 1; // TODO ADD Mask
	//half4 detailNormal = SAMPLE_TEXTURE2D(anisoDetailMap,sampler_anisoDetailMap, uv);

	float2 jitter =(detailNormal.y-0.5) * float2(anisoSpecularData.spread1,anisoSpecularData.spread2);

	float sgn = tangentWS.w;
	float3 T = normalize(sgn * cross(N.xyz, tangentWS.xyz));
	//float3 T = normalize(tangentWS.xyz);

	float3 t1 = ShiftTangent(T, N, anisoSpecularData.specularShift + jitter.x);
	float3 t2 = ShiftTangent(T, N, anisoSpecularData.specularSecondaryShift + jitter.y);

	float3 hairSpec1 = anisoSpecularData.specularColor * anisoSpecularData.specularStrength *
		D_KajiyaKay(t1, H, anisoSpecularData.specularExponent);
	float3 hairSpec2 = anisoSpecularData.specularSecondaryColor * anisoSpecularData.specularSecondaryStrength *
		D_KajiyaKay(t2, H, anisoSpecularData.specularSecondaryExponent);

	half LdotH = saturate(dot(lDir,H));
	half NdotL = saturate(dot(N,lDir));
		
	float3 F = F_Schlick(half3(0.2,0.2,0.2), LdotH);
	half3 anisoSpecularColor = 0.25 * F * (hairSpec1 + hairSpec2) * NdotL * specMask * SpecularColor;
	return anisoSpecularColor;
}

#endif