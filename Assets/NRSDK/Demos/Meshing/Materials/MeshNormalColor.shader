Shader "NRSDK/NormalColor"
{
    SubShader
    {
        Tags { "Queue" = "Geometry" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag
         
            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normal: NORMAL;
            };

            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal) * 0.5 + 0.5;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(i.normal, 1);
            }
            ENDCG
        }
    } 
}
