Shader "NRSDK/AlphaBlend "
{
	Properties
	{
		_MainTex("_MainTex", 2D) = "white" {}
		_BcakGroundTex("_BcakGroundTex", 2D) = "white" {}
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		ZTest Always
		ZWrite Off

		Pass
		{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					float4 vertex : SV_POSITION;
				};

				sampler2D _MainTex;
				sampler2D _BcakGroundTex;
				float4 _MainTex_ST;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					// sample the texture
					fixed4 col = tex2D(_MainTex, i.uv);
					fixed2 uv = fixed2(i.uv.x, 1 - i.uv.y);
					fixed4 col_bg = tex2D(_BcakGroundTex, uv);
					fixed3 col_tex = lerp(col_bg.rgb, col.rgb, col.a);
					return fixed4(col_tex,1);
				}
			ENDCG
		}
	}
}
