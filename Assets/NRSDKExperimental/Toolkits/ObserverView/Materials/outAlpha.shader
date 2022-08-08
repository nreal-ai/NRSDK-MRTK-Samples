Shader "Nreal/AlphaBlendEffect" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}  
		_RGBTex("Base (RGB)", 2D) = "white" {}
	}
		SubShader{
		Pass{
			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
			#pragma vertex vert  
			#pragma fragment frag  

			#include "UnityCG.cginc"  

			sampler2D _MainTex;
			sampler2D _RGBTex;

			struct v2f {
				float4 pos : SV_POSITION;
				half2 uv: TEXCOORD0;
			};

			v2f vert(appdata_img v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target{
				fixed4 renderTex = tex2D(_MainTex, i.uv);
				return fixed4(renderTex.a, renderTex.a, renderTex.a, renderTex.a);
			}

		ENDCG
		}
	}
		Fallback Off
}