#ifndef LIGHTING_COMMON_INCLUDE
    #define LIGHTING_COMMON_INCLUDE

#include "Library\PackageCache\com.unity.render-pipelines.core@10.10.1\ShaderLibrary\EntityLighting.hlsl"

void GetSSAO_float(float2 ScreenUV,out float SSAO)
{
    SSAO = 1.0f;
    #ifndef SHADERGRAPH_PREVIEW
    #if defined(_SCREEN_SPACE_OCCLUSION)
	    AmbientOcclusionFactor aoFactor = GetScreenSpaceAmbientOcclusion(ScreenUV);
	    SSAO = aoFactor.indirectAmbientOcclusion;
    #endif
    #endif
}

inline half Pow2(half x)
{
    return x*x ;
}
// inline half Pow4(half x)
// {
//     return x*x * x*x ;
// }
    
inline half Pow5(half x)
{
    return x*x * x*x *x;
}
float3 Diffuse_Lambert( float3 DiffuseColor )
{
    return DiffuseColor * (1 / PI);
}

float D_GGX_UE4( float a2, float NoH )
{
    float d = ( NoH * a2 - NoH ) * NoH + 1;	// 2 mad
    return a2 / ( PI*d*d );					// 4 mul, 1 rcp
}
float Vis_Implicit()
{
    return 0.25;
}
float3 F_None( float3 SpecularColor )
    {
        return SpecularColor;
    }
    
    
    float Vis_SmithJointApprox( float a2, float NoV, float NoL )
    {
	    float a = sqrt(a2);
	    float Vis_SmithV = NoL * ( NoV * ( 1 - a ) + a );
	    float Vis_SmithL = NoV * ( NoL * ( 1 - a ) + a );
	    return 0.5 * rcp( Vis_SmithV + Vis_SmithL );
    }
    float3 F_Schlick_UE4( float3 SpecularColor, float VoH )
    {
	    float Fc = Pow5( 1 - VoH );					// 1 sub, 3 mul
	    //return Fc + (1 - Fc) * SpecularColor;		// 1 add, 3 mad
	
	    // Anything less than 2% is physically impossible and is instead considered to be shadowing
	    return saturate( 50.0 * SpecularColor.g ) * Fc + (1 - Fc) * SpecularColor;
	
    }
    half DisneyDiffuse(half NdotV, half NdotL, half LdotH, half perceptualRoughness)
    {
        half fd90 = 0.5 + 2 * LdotH * LdotH * perceptualRoughness;
        // Two schlick fresnel term
        half lightScatter   = (1 + (fd90 - 1) * Pow5(1 - NdotL));
        half viewScatter    = (1 + (fd90 - 1) * Pow5(1 - NdotV));

        return lightScatter * viewScatter;
    }
    inline float GGXTerm_Unity (float NdotH, float Alpha)
    {
        float a2 = Alpha * Alpha;
        float d = (NdotH * a2 - NdotH) * NdotH + 1.0f; // 2 mad
        return INV_PI * a2 / (d * d + 1e-7f); // This function is not intended to be running on Mobile,
                                                // therefore epsilon is smaller than what can be represented by half
    }
    half3 EnvBRDFApprox( half3 SpecularColor, half Roughness, half NoV )
    {
	    // [ Lazarov 2013, "Getting More Physical in Call of Duty: Black Ops II" ]
	    // Adaptation to fit our G term.
	    const half4 c0 = { -1, -0.0275, -0.572, 0.022 };
	    const half4 c1 = { 1, 0.0425, 1.04, -0.04 };
	    half4 r = Roughness * c0 + c1;
	    half a004 = min( r.x * r.x, exp2( -9.28 * NoV ) ) * r.x + r.y;
	    half2 AB = half2( -1.04, 1.04 ) * a004 + r.zw;

	    // Anything less than 2% is physically impossible and is instead considered to be shadowing
	    // Note: this is needed for the 'specular' show flag to work, since it uses a SpecularColor of 0
	    AB.y *= saturate( 50.0 * SpecularColor.g );

	    return SpecularColor * AB.x + AB.y;
    }

// half3 EnvironmentBRDFSpecular(half3 SpecularColor, half Roughness, half fresnelTerm)
// {
// 	float surfaceReduction = 1.0 / (Roughness + 1.0);
// 	return surfaceReduction * lerp(SpecularColor, brdfData.grazingTerm, fresnelTerm);
// }
//
// half3 EnvironmentBRDF( half3 indirectDiffuse, half3 indirectSpecular,half Roughness, half fresnelTerm)
// {
// 	half3 c = indirectDiffuse ;
// 	c += indirectSpecular * EnvironmentBRDFSpecular(indirectSpecular,Roughness, fresnelTerm);
// 	return c;
// }

    inline half3 RotateDirection(half3 R, half degrees)
    {
	    float3 reflUVW = R;
	    half theta = degrees * PI / 180.0f;
	    half costha = cos(theta);
	    half sintha = sin(theta);
	    reflUVW = half3(reflUVW.x * costha - reflUVW.z * sintha, reflUVW.y, reflUVW.x * sintha + reflUVW.z * costha);
	    return reflUVW;
    }
    float GetSpecularOcclusion(float NoV, float RoughnessSq, float AO)
    {
	    return saturate( pow( NoV + AO, RoughnessSq ) - 1 + AO );
    }

    float3 AOMultiBounce( float3 BaseColor, float AO )
    {
	    float3 a =  2.0404 * BaseColor - 0.3324;
	    float3 b = -4.7951 * BaseColor + 0.6417;
	    float3 c =  2.7552 * BaseColor + 0.6903;
	    return max( AO, ( ( AO * a + b ) * AO + c ) * AO );
    }
	void GetCurvature_float(float SSSRange,float SSSPower,float3 WorldNormal,float3 WorldPos,out float Curvature)
	{
		Curvature = 1.0;
	
		float deltaWorldNormal = length(fwidth(WorldNormal));
		float deltaWorldPosition = length(fwidth(WorldPos));
		Curvature = saturate(SSSRange + deltaWorldNormal/deltaWorldPosition * SSSPower);
	}
half3 GlossyEnvironmentReflection_custom(half3 reflectVector, half perceptualRoughness, half occlusion,half4 encodedIrradiance ,half4 ReflectionCubeMap_HDR )
{
	#if defined(_CUSTOMREFLECTION)
	//half mip = PerceptualRoughnessToMipmapLevel(perceptualRoughness);
	//half4 encodedIrradiance = SAMPLE_TEXTURECUBE(ReflectionCubeMap, sampler_ReflectionCubeMap, reflectVector);

	 #if defined(UNITY_USE_NATIVE_HDR)
	half3 irradiance = encodedIrradiance.rgb;
	 #else
	 half3 irradiance = DecodeHDREnvironment(encodedIrradiance, ReflectionCubeMap_HDR);
	 #endif

	return irradiance * occlusion;
	#endif // GLOSSY_REFLECTIONS

	return _GlossyEnvironmentColor.rgb * occlusion;
}


#endif