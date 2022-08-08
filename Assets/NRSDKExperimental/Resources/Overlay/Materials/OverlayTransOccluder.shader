/*
Code for transparent object in front of the overlay
The keepalpha option is important to let unity maintain the alpha we return from the shader
*/

Shader "NRSDK/OverlayTransOccluder" {
    Properties{
        _Color("Main Color", Color) = (1,1,1,1)
        _MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
    }
        SubShader{
        Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        ZWrite Off

        CGPROGRAM
        #pragma surface surf Lambert keepalpha
        fixed4 _Color;
        sampler2D _MainTex;
        struct Input {
            float4 color : COLOR;
            float2 uv_MainTex;
        };
        void surf(Input IN, inout SurfaceOutput o) {
            float4 texVal = tex2D (_MainTex, IN.uv_MainTex).rgba * _Color;
            o.Albedo = texVal.rgb;
            o.Alpha = texVal.a;
        }
        ENDCG
    }

    Fallback "Transparent/VertexLit"
}
