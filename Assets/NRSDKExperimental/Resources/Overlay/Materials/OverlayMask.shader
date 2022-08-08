/*
This shader is used to punch the hole while clearing out the alpha of the eye buffer
The important details are:
- Cull is Off to make sure it still work from back
- Queue is Background, in consideration of efficiency
*/

Shader "NRSDK/OverlayMask" {
	SubShader
	{
		Tags{ "Queue" = "Background" "RenderType" = "Opaque" }
        // Tags { "Queue" = "Geometry+1" "RenderType" = "Opaque" }
		Lighting Off
		Cull Off
		ZWrite On
		ZTest LEqual
		Pass
		{
			Color(0,0,0,0)
		}
	}
	FallBack "Diffuse"
}
