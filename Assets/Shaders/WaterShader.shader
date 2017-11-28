// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Project/WaterShader"
{
	Properties
	{
		_Normal2("Normal2", 2D) = "bump" {}
		_TessValue( "Max Tessellation", Range( 1, 32 ) ) = 20
		_TessMin( "Tess Min Distance", Float ) = 10
		_TessMax( "Tess Max Distance", Float ) = 25
		_TessPhongStrength( "Phong Tess Strength", Range( 0, 1 ) ) = 0.5
		_WaterNormal("Water Normal", 2D) = "bump" {}
		_NormalScale("Normal Scale", Float) = 0
		_Ripples("Ripples", 2D) = "white" {}
		_DeepColor("Deep Color", Color) = (0,0,0,0)
		_Float0("Float 0", Range( 0 , 5)) = 0
		_ShalowColor("Shalow Color", Color) = (1,1,1,0)
		_WaterDepth("Water Depth", Float) = 0
		_WaterFalloff("Water Falloff", Float) = 0
		_WaterSpecular("Water Specular", Float) = 0
		_WaterSmoothness("Water Smoothness", Float) = 0
		_Distortion("Distortion", Float) = 0.5
		_Foam("Foam", 2D) = "white" {}
		_FoamDepth("Foam Depth", Float) = 0
		_FoamFalloff("Foam Falloff", Float) = 0
		_FoamSpecular("Foam Specular", Float) = 0
		_FoamSmoothness("Foam Smoothness", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Transparent-1" "IgnoreProjector" = "True" }
		Cull Off
		GrabPass{ }
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Tessellation.cginc"
		#pragma target 5.0
		#pragma surface surf StandardSpecular keepalpha vertex:vertexDataFunc tessellate:tessFunction tessphong:_TessPhongStrength 
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
		};

		struct appdata
		{
			float4 vertex : POSITION;
			float4 tangent : TANGENT;
			float3 normal : NORMAL;
			float4 texcoord : TEXCOORD0;
			float4 texcoord1 : TEXCOORD1;
			float4 texcoord2 : TEXCOORD2;
			float4 texcoord3 : TEXCOORD3;
			fixed4 color : COLOR;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		uniform half _NormalScale;
		uniform sampler2D _Normal2;
		uniform sampler2D _WaterNormal;
		uniform float4 _WaterNormal_ST;
		uniform half4 _DeepColor;
		uniform half4 _ShalowColor;
		uniform sampler2D _CameraDepthTexture;
		uniform half _WaterDepth;
		uniform half _WaterFalloff;
		uniform half _FoamDepth;
		uniform half _FoamFalloff;
		uniform sampler2D _Foam;
		uniform float4 _Foam_ST;
		uniform sampler2D _GrabTexture;
		uniform half _Distortion;
		uniform half _WaterSpecular;
		uniform half _FoamSpecular;
		uniform half _WaterSmoothness;
		uniform half _FoamSmoothness;
		uniform sampler2D _Ripples;
		uniform float4 _Ripples_ST;
		uniform half _Float0;
		uniform half _TessValue;
		uniform half _TessMin;
		uniform half _TessMax;
		uniform half _TessPhongStrength;

		float4 tessFunction( appdata v0, appdata v1, appdata v2 )
		{
			return UnityDistanceBasedTess( v0.vertex, v1.vertex, v2.vertex, _TessMin, _TessMax, _TessValue );
		}

		void vertexDataFunc( inout appdata v )
		{
			float2 uv_Ripples = v.texcoord * _Ripples_ST.xy + _Ripples_ST.zw;
			float temp_output_195_0 = ( tex2Dlod( _Ripples, half4( uv_Ripples, 0, 1.0) ).r * 0.1 );
			float3 ase_vertexNormal = v.normal.xyz;
			v.vertex.xyz += ( temp_output_195_0 * ( ase_vertexNormal * _Float0 ) );
		}

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 uv_WaterNormal = i.uv_texcoord * _WaterNormal_ST.xy + _WaterNormal_ST.zw;
			float2 panner22 = ( uv_WaterNormal + 1.0 * _Time.y * float2( -0.03,0 ));
			float2 panner19 = ( uv_WaterNormal + 1.0 * _Time.y * float2( 0.04,0.04 ));
			float3 temp_output_24_0 = BlendNormals( UnpackScaleNormal( tex2D( _Normal2, panner22 ) ,_NormalScale ) , UnpackScaleNormal( tex2D( _WaterNormal, panner19 ) ,_NormalScale ) );
			o.Normal = temp_output_24_0;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float eyeDepth1 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float temp_output_89_0 = abs( ( eyeDepth1 - ase_screenPos.w ) );
			float temp_output_94_0 = saturate( pow( ( temp_output_89_0 + _WaterDepth ) , _WaterFalloff ) );
			float4 lerpResult13 = lerp( _DeepColor , _ShalowColor , temp_output_94_0);
			float2 uv_Foam = i.uv_texcoord * _Foam_ST.xy + _Foam_ST.zw;
			float2 panner116 = ( uv_Foam + 1.0 * _Time.y * float2( -0.01,0.01 ));
			float temp_output_114_0 = ( saturate( pow( ( temp_output_89_0 + _FoamDepth ) , _FoamFalloff ) ) * tex2D( _Foam, panner116 ).r );
			float4 lerpResult117 = lerp( lerpResult13 , half4(1,1,1,0) , temp_output_114_0);
			float4 ase_screenPos164 = ase_screenPos;
			#if UNITY_UV_STARTS_AT_TOP
			float scale164 = -1.0;
			#else
			float scale164 = 1.0;
			#endif
			float halfPosW164 = ase_screenPos164.w * 0.5;
			ase_screenPos164.y = ( ase_screenPos164.y - halfPosW164 ) * _ProjectionParams.x* scale164 + halfPosW164;
			ase_screenPos164.xyzw /= ase_screenPos164.w;
			float4 screenColor65 = tex2D( _GrabTexture, ( half3( (ase_screenPos164).xy ,  0.0 ) + ( temp_output_24_0 * _Distortion ) ).xy );
			float4 lerpResult93 = lerp( lerpResult117 , screenColor65 , temp_output_94_0);
			o.Albedo = lerpResult93.rgb;
			float lerpResult130 = lerp( _WaterSpecular , _FoamSpecular , temp_output_114_0);
			half3 temp_cast_3 = (lerpResult130).xxx;
			o.Specular = temp_cast_3;
			float lerpResult133 = lerp( _WaterSmoothness , _FoamSmoothness , temp_output_114_0);
			o.Smoothness = lerpResult133;
			o.Occlusion = 0.0;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13801
89;641;1726;904;186.7867;-692.7466;1.462187;True;True
Node;AmplifyShaderEditor.CommentaryNode;152;-2053.601,-256.6997;Float;False;828.5967;315.5001;Screen depth difference to get intersection and fading effect with terrain and obejcts;4;89;2;1;3;;1,1,1,1;0;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;2;-2046.601,-140.1996;Float;False;1;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ScreenDepthNode;1;-1781.601,-155.6997;Float;False;0;1;0;FLOAT4;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleSubtractOpNode;3;-1574.201,-110.3994;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.AbsOpNode;89;-1335.004,-112.5834;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;151;-958.7189,-1261.73;Float;False;1281.603;457.1994;Blend panning normals to fake noving ripples;7;19;23;24;21;22;17;48;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;153;-843.9032,402.718;Float;False;1083.102;484.2006;Foam controls and texture;7;105;115;111;110;112;113;114;;1,1,1,1;0;0
Node;AmplifyShaderEditor.WireNode;155;-1106.507,7.515848;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;21;-908.7191,-1184.431;Float;False;0;17;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;159;-863.7005,-467.5007;Float;False;1113.201;508.3005;Depths controls and colors;10;87;94;12;13;156;157;11;88;6;143;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-580.1195,-974.6314;Float;False;Property;_NormalScale;Normal Scale;6;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;111;-862.6843,486.4808;Float;False;Property;_FoamDepth;Foam Depth;17;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.WireNode;154;-922.7065,390.316;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.PannerNode;19;-633.7194,-1099.231;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.04,0.04;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;6;-858.0854,15.19771;Float;False;Property;_WaterDepth;Water Depth;11;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.WireNode;158;-1075.608,-163.0834;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.PannerNode;22;-636.0194,-1211.73;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.03,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SamplerNode;23;-292.0191,-1203.431;Float;True;Property;_Normal2;Normal2;0;0;None;True;0;True;bump;Auto;True;Instance;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;112;-696.5845,670.3378;Float;False;Property;_FoamFalloff;Foam Falloff;18;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;10;-588.4015,137.6029;Float;False;Property;_WaterFalloff;Water Falloff;12;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;17;-279.1184,-993.5303;Float;True;Property;_WaterNormal;Water Normal;5;0;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;106;-874.1786,919.3324;Float;False;0;105;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;115;-671.6769,417.2115;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;88;-618.3488,-95.32761;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.BlendNormalsNode;24;147.884,-1058.93;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.ColorNode;11;-455.0999,-328.3;Float;False;Property;_ShalowColor;Shalow Color;10;0;1,1,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PowerNode;87;-404.5926,14.97146;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;150;467.1957,-1501.783;Float;False;985.6011;418.6005;Get screen color for refraction and disturbe it with normals;7;96;97;98;65;149;164;165;;1,1,1,1;0;0
Node;AmplifyShaderEditor.ColorNode;12;-697.5002,-417.5007;Float;False;Property;_DeepColor;Deep Color;8;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PowerNode;110;-408.1463,453.8997;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.PannerNode;116;-570.1139,992.019;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.01,0.01;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SamplerNode;105;-324.3617,658.2031;Float;True;Property;_Foam;Foam;16;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SaturateNode;113;-111.301,447.8677;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.WireNode;156;-131.5076,-325.9835;Float;False;1;0;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.GrabScreenPosition;164;511.3026,-1442.425;Float;False;0;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;97;710.096,-1203.183;Float;False;Property;_Distortion;Distortion;15;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;94;-165.2129,-178.524;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.WireNode;157;-149.1077,-261.9834;Float;False;1;0;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.WireNode;149;487.4943,-1188.882;Float;False;1;0;FLOAT3;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.ComponentMaskNode;165;814.6503,-1385.291;Float;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.LerpOp;13;60.50008,-220.6998;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;98;888.1974,-1279.783;Float;False;2;2;0;FLOAT3;0.0,0,0;False;1;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.ColorNode;108;58.99682,146.0182;Float;False;Constant;_Color0;Color 0;-1;0;1,1,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;114;145.0366,604.0181;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.NormalVertexDataNode;191;716.7854,1597.906;Float;False;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;190;433.5634,1376.751;Float;True;Property;_Ripples;Ripples;7;0;Assets/Textures/Effects/Render Texture/Ripples.asset;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;96;1041.296,-1346.683;Float;False;2;2;0;FLOAT2;0.0,0;False;1;FLOAT3;0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.LerpOp;117;339.9822,-11.10016;Float;False;3;0;COLOR;0.0,0,0,0;False;1;COLOR;0.0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;189;672.4694,1753.716;Float;False;Property;_Float0;Float 0;9;0;0;0;5;0;1;FLOAT
Node;AmplifyShaderEditor.WireNode;143;95.69542,-321.0839;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;195;885.8868,1410.072;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.1;False;1;FLOAT
Node;AmplifyShaderEditor.WireNode;161;660.4934,-750.6837;Float;False;1;0;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;131;756.1969,-467.1806;Float;False;Property;_FoamSpecular;Foam Specular;19;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;104;742.9969,-676.4819;Float;False;Property;_WaterSpecular;Water Specular;13;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;26;827.1311,-182.0743;Float;False;Property;_WaterSmoothness;Water Smoothness;14;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ScreenColorNode;65;1232.797,-1350.483;Float;False;Global;_WaterGrab;WaterGrab;-1;0;Object;-1;False;1;0;FLOAT2;0,0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;132;902.2588,130.2934;Float;False;Property;_FoamSmoothness;Foam Smoothness;20;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.WireNode;162;1312.293,-894.3823;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;193;1043.839,1678.987;Float;False;2;2;0;FLOAT3;1.0,0,0;False;1;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;192;1116.144,1441.322;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.5;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;103;1605.795,-501.9843;Float;False;Constant;_Occlusion;Occlusion;-1;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;133;1323.704,-85.5688;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;93;1559.196,-1006.285;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;194;1211.149,1657.852;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT3;0;False;1;FLOAT3
Node;AmplifyShaderEditor.LerpOp;130;955.7971,-465.8806;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1853.451,-414.1358;Half;False;True;7;Half;ASEMaterialInspector;0;0;StandardSpecular;Project/WaterShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Off;0;3;False;0;0;Translucent;0.5;True;False;-1;False;Opaque;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;True;0;20;10;25;True;0.5;True;0;SrcAlpha;OneMinusSrcAlpha;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;0;0;0;0;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0.0,0,0;False;7;FLOAT3;0.0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;11;FLOAT3;0.0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;1;0;2;0
WireConnection;3;0;1;0
WireConnection;3;1;2;4
WireConnection;89;0;3;0
WireConnection;155;0;89;0
WireConnection;154;0;155;0
WireConnection;19;0;21;0
WireConnection;158;0;89;0
WireConnection;22;0;21;0
WireConnection;23;1;22;0
WireConnection;23;5;48;0
WireConnection;17;1;19;0
WireConnection;17;5;48;0
WireConnection;115;0;154;0
WireConnection;115;1;111;0
WireConnection;88;0;158;0
WireConnection;88;1;6;0
WireConnection;24;0;23;0
WireConnection;24;1;17;0
WireConnection;87;0;88;0
WireConnection;87;1;10;0
WireConnection;110;0;115;0
WireConnection;110;1;112;0
WireConnection;116;0;106;0
WireConnection;105;1;116;0
WireConnection;113;0;110;0
WireConnection;156;0;12;0
WireConnection;94;0;87;0
WireConnection;157;0;11;0
WireConnection;149;0;24;0
WireConnection;165;0;164;0
WireConnection;13;0;156;0
WireConnection;13;1;157;0
WireConnection;13;2;94;0
WireConnection;98;0;149;0
WireConnection;98;1;97;0
WireConnection;114;0;113;0
WireConnection;114;1;105;1
WireConnection;96;0;165;0
WireConnection;96;1;98;0
WireConnection;117;0;13;0
WireConnection;117;1;108;0
WireConnection;117;2;114;0
WireConnection;143;0;94;0
WireConnection;195;0;190;1
WireConnection;161;0;117;0
WireConnection;65;0;96;0
WireConnection;162;0;143;0
WireConnection;193;0;191;0
WireConnection;193;1;189;0
WireConnection;192;0;195;0
WireConnection;133;0;26;0
WireConnection;133;1;132;0
WireConnection;133;2;114;0
WireConnection;93;0;161;0
WireConnection;93;1;65;0
WireConnection;93;2;162;0
WireConnection;194;0;195;0
WireConnection;194;1;193;0
WireConnection;130;0;104;0
WireConnection;130;1;131;0
WireConnection;130;2;114;0
WireConnection;0;0;93;0
WireConnection;0;1;24;0
WireConnection;0;3;130;0
WireConnection;0;4;133;0
WireConnection;0;5;103;0
WireConnection;0;11;194;0
ASEEND*/
//CHKSM=9EB0B373C7C1404378573DA9FAF844F101094CDD