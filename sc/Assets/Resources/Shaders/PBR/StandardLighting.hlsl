#ifndef STANDARD_LIGHTING_INCLUDE
    #define STANDARD_LIGHTING_INCLUDE
	#include "LightingCommon.hlsl"
	#include "Assets/Resources/Shaders/PBR/LightingCustom.hlsl"
   
    float3 StandardBRDF(float3 DiffuseColor, float3 SpecularColor, float Roughness, float3 L , float3 V,float3 N,float3 LightColor,float Shadow )
    {
	    float a2 = Pow4( Roughness );
	    float3 H = normalize(V + L);
        float NoH = saturate(dot(N,H));
        float NoV = saturate(abs(dot(N,V))+1e-5); // Both side 
        float NoL = saturate(dot(N,L));
        float VoH = saturate(dot(V,H));
        half3 Radiance =   NoL *LightColor * Shadow * PI;

        float3 DiffuseTerm = Diffuse_Lambert(DiffuseColor) * Radiance ;
	    // Generalized microfacet specular
	    float D = D_GGX_UE4( a2, NoH ) ;
	    float Vis = Vis_SmithJointApprox( a2, NoV, NoL );
	    float3 F = F_Schlick_UE4( SpecularColor, VoH );
        half3 SpecularTerm =(( D * Vis )* F)* Radiance;
        float3 DirectLighting = (DiffuseTerm + SpecularTerm) ;

	    return DirectLighting;
    }
	void IndirectLighting_float(float3 DiffuseColor, float3 SpecularColor, float Roughness, float3 WorldPos, float3 N, float3 V,
						float Occlusion,float EnvRotation,out float3 IndirectLighting)
    {
    	IndirectLighting = half3(0,0,0);
    	#ifndef SHADERGRAPH_PREVIEW
       float NoV = saturate(abs(dot(N,V))+1e-5);
       //SH
       float3 DiffuseAO = AOMultiBounce(DiffuseColor,Occlusion);
	   float3 RadianceSH = SampleSH(N);
       float3 IndirectDiffuse = RadianceSH * DiffuseColor *DiffuseAO;
       //IBL
       half3 R = reflect(-V,N);
       R = RotateDirection(R,EnvRotation);
       float3 SpecularG = GlossyEnvironmentReflection(R,Roughness,Occlusion);
       float3 SpecularDFG = EnvBRDFApprox(SpecularColor,Roughness,NoV);
       float SpecularOcclusion = GetSpecularOcclusion(NoV,Pow2(Roughness),Occlusion);
       float3 SpecularAO = AOMultiBounce(SpecularColor,SpecularOcclusion);
       float3 IndirectSpecular = SpecularG * SpecularDFG * SpecularAO;
        IndirectLighting = IndirectDiffuse + IndirectSpecular;
    	#endif
    }

void IndirectLightingHair_float(float3 DiffuseColor, float3 SpecularColor, float Roughness, float3 WorldPos, float3 N, float3 V,
						float Occlusion,float EnvRotation,out float3 IndirectLighting)
    {
    	IndirectLighting = half3(0,0,0);
    	#ifndef SHADERGRAPH_PREVIEW
    	//float NoV = saturate(abs(dot(N,V))+1e-5);
    	//SH
    	float3 DiffuseAO = AOMultiBounce(DiffuseColor,Occlusion);
    	float3 RadianceSH = SampleSH(N);
    	float3 IndirectDiffuse = RadianceSH * DiffuseColor *DiffuseAO ;
    	
    	//IBL
    	// half3 R = reflect(-V,N);
    	// R = RotateDirection(R,EnvRotation);
    	// float3 SpecularG = GlossyEnvironmentReflection(R,Roughness,Occlusion);
    	// float3 SpecularDFG = EnvBRDFApprox(SpecularColor,Roughness,NoV);
    	// float SpecularOcclusion = GetSpecularOcclusion(NoV,Pow2(Roughness),Occlusion);
    	// float3 SpecularAO = AOMultiBounce(SpecularColor,SpecularOcclusion);
    	// float3 IndirectSpecular = SpecularG * SpecularDFG * SpecularAO;
    	IndirectLighting = IndirectDiffuse ;
    	#endif
    }
	void IndirectLightingSSS_float(float3 DiffuseColor, float3 SpecularColor, float Roughness, float3 WorldPos, float3 N, float3 V,
						float Occlusion,float EnvRotation,half3 lutMap,half4 MetallicSmoothness, out float3 IndirectLightingSSS)
	    {
    		IndirectLightingSSS = half3(0,0,0);
    		half NoV = saturate(abs(dot(N,V))+1e-5);
    		//SH
    		half3 DiffuseAO = AOMultiBounce(DiffuseColor,Occlusion);
    		half3 RadianceSH = SampleSH(N);
    		half3 IndirectDiffuse = RadianceSH * DiffuseColor *DiffuseAO;
    		half3 IndirectDiffuseSSS = lutMap * IndirectDiffuse;
    		half skin = 1- MetallicSmoothness.a;
    		half3 Diffuse = lerp(IndirectDiffuse,IndirectDiffuseSSS * 0.5 + 0.5,skin);
    		//IBL
    		half3 R = reflect(-V,N);
    		R = RotateDirection(R,EnvRotation);
    		half3 SpecularG = GlossyEnvironmentReflection(R,Roughness,Occlusion);
    		half3 SpecularDFG = EnvBRDFApprox(SpecularColor,Roughness,NoV);
    		half SpecularOcclusion = GetSpecularOcclusion(NoV,Pow2(Roughness),Occlusion);
    		half3 SpecularAO = AOMultiBounce(SpecularColor,SpecularOcclusion);
    		half3 IndirectSpecular = SpecularG * SpecularDFG * SpecularAO;
    		IndirectLightingSSS = Diffuse + IndirectSpecular;
	    }
		void SSSuv_float(float3 DiffuseColor, float3 N, float3 WorldPos,
							float Occlusion,half LutOffset,half sssPower,out float2 uvLut)
		    {
    			uvLut = half2(0,0);
    			//SH
    			half3 DiffuseAO = AOMultiBounce(DiffuseColor,Occlusion);
    			half3 RadianceSH = SampleSH(N);
    			half3 IndirectDiffuse = RadianceSH * DiffuseColor *DiffuseAO;
    			half IndirectSSS = IndirectDiffuse.x;
    			half Curvature = 0;
    			GetCurvature_float(LutOffset,sssPower,N,WorldPos,Curvature);
    			uvLut = half2(IndirectSSS,Curvature);
		    }

	void DirectLighting_float(float3 DiffuseColor, float3 SpecularColor, float Roughness,float3 WorldPos, float3 N, float3 V,
								out float3 DirectLighting)
    {
    	DirectLighting = half3(0,0,0);
    	#ifndef SHADERGRAPH_PREVIEW
    	#if defined(_MAIN_LIGHT_SHADOWS_SCREEN) && !defined(_SURFACE_TYPE_TRANSPARENT)
    	float4 positionCS = TransformWorldToHClip(WorldPos);
    	float4 ShadowCoord = ComputeScreenPos(positionCS);
    	#else
    	float4 ShadowCoord = TransformWorldToShadowCoord(WorldPos);
    	#endif
    	float4 ShadowMask = float4(1.0,1.0,1.0,1.0);
    	//mainLight
    	half3 DirectLight_MainLight = half3(0,0,0);
	    {
    		Light light = GetMainLight(ShadowCoord,WorldPos,ShadowMask);
    		half3 L = light.direction;
    		half3 LightColor = light.color;
    		half Shadow = light.shadowAttenuation;
    		DirectLight_MainLight = StandardBRDF(DiffuseColor,SpecularColor,Roughness,L,V,N,LightColor,Shadow) ;
	    }
    	
    	DirectLighting = DirectLight_MainLight ;
    	#endif
    }

void DirectLightingMoblie_float(float3 DiffuseColor, float3 SpecularColor, float Roughness,float SpecShininess, float3 N, float3 L,float3 H,half3 hightColor,half3 darkColor,half dirIntensity,
							out float3 DirectLighting)
    {
    	DirectLighting = half3(0,0,0);
    	#ifndef SHADERGRAPH_PREVIEW
    	//direct diffuse
    	half lambert = max(0.0,dot(N, L))  ;
    	half3 diffuse = lerp(darkColor,hightColor,lambert);
    	half3 DirectDiffuse = diffuse + DiffuseColor;
    		
    	//direct specular
    	half NdotH = dot(N,  H);
    	half smoothness = 1.0 - Roughness;
    	half shininess = lerp(1,SpecShininess,smoothness);
    	half binPhong = pow(max(0.0, NdotH),shininess *smoothness);
    	half3 DirectSpec = binPhong * SpecularColor;

    	DirectLighting = (DirectDiffuse+ DirectSpec)  * dirIntensity;
    	#endif
    }


#endif