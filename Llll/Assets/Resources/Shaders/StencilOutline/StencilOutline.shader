Shader "StencilOutLine"
{
    Properties
    {
        _Color("outline color",color) = (1,1,1,1)
        _Width("outline width",range(0,1)) = 0.2
    }
    Subshader
    {
        Pass
        {
        Tags {"LightMode" = "LightweightForward" "RenderType" = "Opaque" "Queue" = "Geometry + 10"}
        colormask 0 
        ZWrite Off
        ZTest Off
        Stencil
        {
            Ref 1
            Comp Always
            Pass replace
        }
        HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
 
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
 
            struct appdata
            {
                float4 vertex: POSITION;
                float3 normal:NORMAL;
            };
 
            struct v2f
            {
                float4 vertex: SV_POSITION;
            };
            half4 _Color;
            half _Width;
            v2f vert(appdata v)
            {
                v2f o;
                                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                return o;
            }
 
            half4 frag(v2f i) : SV_Target
            {
                return half4 (0.5h, 0.0h, 0.0h, 1.0h);
            }
            ENDHLSL
        }
        Pass
        {
                Tags {"RenderType" = "Opaque" "Queue" = "Geometry + 20"}
                ZTest off
 
            Stencil {
                Ref 1
                Comp notEqual
                Pass keep
            }
                HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            struct appdata
            {
                float4 positionOS: POSITION;
                float3 normalOS:NORMAL;
                float4 tangentOS : TANGENT;
                float3 smoothNormal : TEXCOORD2;
            };
 
            struct v2f
            {
                float4 positionCS: SV_POSITION;
            };
 
            half4 _Color;
            half _Width;
             float3 OctahedronToUnitVector(float2 oct)
            {
                float3 unitVec = float3(oct, 1 - dot(float2(1, 1), abs(oct)));

                if (unitVec.z < 0)
                {
                    unitVec.xy = (1 - abs(unitVec.yx)) * float2(unitVec.x >= 0 ? 1 : -1, unitVec.y >= 0 ? 1 : -1);
                }
                
                return normalize(unitVec);
            }
            v2f vert(appdata v)
            {
               // v2f o;
                // float3 normal = any(v.smoothNormal) ? v.smoothNormal : v.normal;
                // v.vertex.xyz += _Width * normalize(normal);
                // o.vertex = TransformObjectToHClip(v.vertex.xyz);
                // return o;
               //  v2f o;
               // float4 pos = TransformObjectToHClip(v.vertex);
               //  float3 normal = any(v.smoothNormal) ? v.smoothNormal : v.normal;
               //  float3 viewNormal = mul((float3x3)UNITY_MATRIX_IT_MV, normal.xyz);
               //  float3 ndcNormal = normalize(mul((float3x3)UNITY_MATRIX_P, viewNormal.xyz)) * pos.w;
               //  float4 nearUpperRight = mul(unity_CameraInvProjection, float4(1, 1, UNITY_NEAR_CLIP_VALUE, _ProjectionParams.y));
               //  float aspect = abs(nearUpperRight.y / nearUpperRight.x);
               //  ndcNormal.x *= aspect;
               //  pos.xy += 0.01 * _Width * ndcNormal.xy;
               //  o.vertex = pos;
               //  return o;
                v2f output = (v2f)0;
                output.positionCS = TransformObjectToHClip(v.positionOS);
                float3 normalTS = OctahedronToUnitVector(v.smoothNormal);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(v.normalOS, v.tangentOS);
                float3x3 tangentToWorld = float3x3(normalInputs.tangentWS, normalInputs.bitangentWS, normalInputs.normalWS);
                float3 normalWS = TransformTangentToWorld(normalTS, tangentToWorld);
                float3 normalCS = TransformWorldToHClipDir(normalWS);
                float2 offset = normalize(normalCS.xy) / _ScreenParams.xy * _Width * output.positionCS.w * 2*10;
                output.positionCS.xy += offset;
                return output;
            }
 
            half4 frag(v2f i) : SV_Target
            {
                return _Color;
            }
            ENDHLSL
        }
    }
}