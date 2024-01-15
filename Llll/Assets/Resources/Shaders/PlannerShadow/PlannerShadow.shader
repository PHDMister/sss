Shader "Unlit/PlannerShadow"
{
    Properties
    {
        [Header(Shadow)]
    	[Space(20)]
    	_LightDir("LightDirection",Vector) = (1,2,3,0)
        _GroundHeight("GroundHeight", Float) = 0
        _ShadowColor("ShadowColor", Color) = (0,0,0,1)
	    _ShadowFalloff("ShadowFalloff", Range(0,1)) = 0.05
        _Cutoff("_Cutoff (Alpha Cutoff)", Range(0.0, 1.0)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Name "PlanarShadow"
            Stencil
            {
                Ref 0
                Comp equal
                Pass incrWrap
                Fail keep
                ZFail keep
            }

            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite off
            Offset -1 , 0

            HLSLPROGRAM
           
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _ALPHAPREMULTIPLY_ON
            #pragma shader_feature_local _NORMALMAP

            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Core.hlsl"
            #include "Library/PackageCache/com.unity.render-pipelines.universal@10.10.1/ShaderLibrary/Lighting.hlsl"

            #pragma vertex vert
            #pragma fragment frag
            CBUFFER_START(UnityPerMaterial)
            half _GroundHeight;
            half4 _ShadowColor;
            half _ShadowFalloff; 
            half _Cutoff;
            half4 _LightDir;
            CBUFFER_END
            struct appdata
            {
                float4 vertex : POSITION;
                half2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                half4 color : COLOR;
                half2 uv : TEXCOORD0;
            };

            float3 ShadowProjectPos(float4 vertPos)
            {
                float3 shadowPos;
                float3 worldPos = mul(unity_ObjectToWorld , vertPos).xyz;
            	 float3 lightDir = normalize(_LightDir.xyz);
                shadowPos.y = min(worldPos .y , _GroundHeight);
                shadowPos.xz = worldPos .xz - lightDir.xz * max(0 , worldPos .y - _GroundHeight) / lightDir.y; 
                return shadowPos;
            }

            float GetAlpha (v2f i) {
                half alpha = _ShadowColor.a;
                alpha *= 1;
                return alpha;
            }

            v2f vert (appdata v)
            {
                v2f o;
                float3 shadowPos = ShadowProjectPos(v.vertex);
                o.vertex = TransformWorldToHClip(shadowPos);
                float3 center = float3(unity_ObjectToWorld[0].w , _GroundHeight , unity_ObjectToWorld[2].w);
                float falloff = 1-saturate(distance(shadowPos , center) * _ShadowFalloff);
                o.color = _ShadowColor;
                o.color.a *= falloff;
                o.uv =v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
            	half alpha = GetAlpha(i);
            	i.color.a *= step(_Cutoff, alpha);
            	return i.color;
            }
            ENDHLSL
        }
          
    }
}
