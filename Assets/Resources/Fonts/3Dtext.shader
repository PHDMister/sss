
Shader "Custom/3DText" {
    Properties {
        [Space(10)]
        _FontTex ("字体贴图", 2D) = "white" {}
        _Color ("字体颜色", Color) = (1,1,1,1)
    }
    SubShader {
        Tags{ "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Lighting Off Cull Off ZWrite On Fog { Mode Off }
        Blend SrcAlpha OneMinusSrcAlpha
        Pass {
            Color [_Color]
            SetTexture [_FontTex] {
                combine primary, texture * primary
            }
        }
    }
    CustomEditor "LWGUI.LWGUI"
    }
