// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASEShaders/RimLight"
{
	Properties
	{
		_MainTexture("MainTexture", 2D) = "white" {}
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Smooth("Smooth", Range( 0 , 1)) = 0
		_Occlusion("Occlusion", 2D) = "white" {}
		_RimColor("RimColor", Color) = (0,0.990566,0.8468525,0.627451)
		_intensity("intensity", Range( 0 , 1)) = 0
		_gradualchange("gradualchange", Range( 0 , 1)) = 1
		_RimIntensity("RimIntensity", Range( 0 , 10)) = 3
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
		#pragma target 3.0
		#pragma only_renderers d3d11 glcore gles gles3 metal d3d11_9x 
		#pragma surface surf Standard keepalpha noshadow exclude_path:deferred novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			float2 uv_texcoord;
		};

		uniform float _intensity;
		uniform float4 _RimColor;
		uniform float _RimIntensity;
		uniform sampler2D _MainTexture;
		uniform float _gradualchange;
		uniform float _Metallic;
		uniform float _Smooth;
		uniform sampler2D _Occlusion;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV35 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode35 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV35, 5.0 ) );
			float temp_output_38_0 = saturate( pow( fresnelNode35 , ( 1.0 - _intensity ) ) );
			float4 tex2DNode58 = tex2D( _MainTexture, i.uv_texcoord );
			float4 lerpResult59 = lerp( ( ( temp_output_38_0 * _RimColor * _RimIntensity ) + float4(0.6320754,0.617168,0.617168,0.7686275) ) , tex2DNode58 , _gradualchange);
			o.Albedo = lerpResult59.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smooth;
			o.Occlusion = tex2D( _Occlusion, i.uv_texcoord ).r;
			float lerpResult61 = lerp( temp_output_38_0 , 1.0 , _gradualchange);
			float ifLocalVar91 = 0;
			if( 1.0 == _gradualchange )
				ifLocalVar91 = tex2DNode58.a;
			else
				ifLocalVar91 = lerpResult61;
			o.Alpha = ifLocalVar91;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15401
2054.4;28;1973;1118;1103.162;-824.7126;1.235811;True;True
Node;AmplifyShaderEditor.RangedFloatNode;40;-766.0874,1020.051;Float;False;Property;_intensity;intensity;6;0;Create;True;0;0;False;0;0;0.301;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;35;-641.2656,784.3313;Float;False;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;47;-463.9415,1028.798;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;46;-341.9415,861.7979;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;49;-254.5671,979.4703;Float;False;Property;_RimColor;RimColor;5;0;Create;True;0;0;False;0;0,0.990566,0.8468525,0.627451;0,0.990566,0.8468525,0.627451;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;38;-185.3433,648.7248;Float;True;1;0;FLOAT;1.23;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;84;-270.455,1154.059;Float;False;Property;_RimIntensity;RimIntensity;8;0;Create;True;0;0;False;0;3;4.44;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;62;306.3615,777.9197;Float;False;Constant;_Float1;Float 1;4;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;93;-409.8745,1718.204;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;60;82.47621,1519.547;Float;False;Property;_gradualchange;gradualchange;7;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;106.0585,889.7979;Float;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;41;-250.1121,1260.708;Float;False;Constant;_Color0;Color 0;0;0;Create;True;0;0;False;0;0.6320754,0.617168,0.617168,0.7686275;1,1,1,0.6431373;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;58;73.37195,1294.438;Float;True;Property;_MainTexture;MainTexture;0;0;Create;True;0;0;False;0;None;28c728e88fa51cf47bf481241268a9e1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;61;532.3615,708.9197;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;44;291.4825,1107.206;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;92;550.0776,953.1033;Float;False;Constant;_Float0;Float 0;10;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;85;78.40517,1711.177;Float;False;Property;_Smooth;Smooth;3;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;88;66.83774,1808.527;Float;True;Property;_Occlusion;Occlusion;4;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ConditionalIfNode;91;839.9777,968.7039;Float;False;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;59;752.6279,1231.464;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;86;79.40517,1633.178;Float;False;Property;_Metallic;Metallic;1;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;56;1159.857,1428.111;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;ASEShaders/RimLight;False;False;False;False;False;True;True;True;True;True;True;True;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;TransparentCutout;;Transparent;ForwardOnly;False;True;True;True;True;True;True;False;False;False;False;False;False;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;-1;False;-1;-1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;2;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;47;0;40;0
WireConnection;46;0;35;0
WireConnection;46;1;47;0
WireConnection;38;0;46;0
WireConnection;48;0;38;0
WireConnection;48;1;49;0
WireConnection;48;2;84;0
WireConnection;58;1;93;0
WireConnection;61;0;38;0
WireConnection;61;1;62;0
WireConnection;61;2;60;0
WireConnection;44;0;48;0
WireConnection;44;1;41;0
WireConnection;88;1;93;0
WireConnection;91;0;92;0
WireConnection;91;1;60;0
WireConnection;91;2;61;0
WireConnection;91;3;58;4
WireConnection;91;4;61;0
WireConnection;59;0;44;0
WireConnection;59;1;58;0
WireConnection;59;2;60;0
WireConnection;56;0;59;0
WireConnection;56;3;86;0
WireConnection;56;4;85;0
WireConnection;56;5;88;0
WireConnection;56;9;91;0
ASEEND*/
//CHKSM=F86DC582E9BDDC10179452F214ACF16DDD6EFCDA