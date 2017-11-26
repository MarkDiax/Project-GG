// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Project/WaterShader"
{
	Properties
	{
		_WaterNormal("Water Normal", 2D) = "bump" {}
		_NormalScale("Normal Scale", Float) = 0
		_DeepColor("Deep Color", Color) = (0,0,0,0)
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

		_Scale ("Scale", float) = 1
		_Speed ("Speed", float) = 1
		_Frequency ("Frequency", float) = 1
	    [HideInInspector]_WaveAmplitude1 ("WaveAmplitude1", float) = 0
		[HideInInspector]_WaveAmplitude2 ("WaveAmplitude1", float) = 0
		[HideInInspector]_WaveAmplitude3 ("WaveAmplitude1", float) = 0
		[HideInInspector]_WaveAmplitude4 ("WaveAmplitude1", float) = 0
		[HideInInspector]_WaveAmplitude5 ("WaveAmplitude1", float) = 0
		[HideInInspector]_WaveAmplitude6 ("WaveAmplitude1", float) = 0
		[HideInInspector]_WaveAmplitude7 ("WaveAmplitude1", float) = 0
		[HideInInspector]_WaveAmplitude8 ("WaveAmplitude1", float) = 0
		[HideInInspector]_xImpact1 ("x Impact 1", float) = 0
		[HideInInspector]_zImpact1 ("z Impact 1", float) = 0
		[HideInInspector]_xImpact2 ("x Impact 2", float) = 0
		[HideInInspector]_zImpact2 ("z Impact 2", float) = 0
		[HideInInspector]_xImpact3 ("x Impact 3", float) = 0
		[HideInInspector]_zImpact3 ("z Impact 3", float) = 0
		[HideInInspector]_xImpact4 ("x Impact 4", float) = 0
		[HideInInspector]_zImpact4 ("z Impact 4", float) = 0
		[HideInInspector]_xImpact5 ("x Impact 5", float) = 0
		[HideInInspector]_zImpact5 ("z Impact 5", float) = 0
		[HideInInspector]_xImpact6 ("x Impact 6", float) = 0
		[HideInInspector]_zImpact6 ("z Impact 6", float) = 0
		[HideInInspector]_xImpact7 ("x Impact 7", float) = 0
		[HideInInspector]_zImpact7 ("z Impact 7", float) = 0
		[HideInInspector]_xImpact8 ("x Impact 8", float) = 0
		[HideInInspector]_zImpact8 ("z Impact 8", float) = 0
  
		[HideInInspector]_Distance1 ("Distance1", float) = 0
		[HideInInspector]_Distance2 ("Distance2", float) = 0
		[HideInInspector]_Distance3 ("Distance3", float) = 0
		[HideInInspector]_Distance4 ("Distance4", float) = 0
		[HideInInspector]_Distance5 ("Distance5", float) = 0
		[HideInInspector]_Distance6 ("Distance6", float) = 0
		[HideInInspector]_Distance7 ("Distance7", float) = 0
		[HideInInspector]_Distance8 ("Distance8", float) = 0
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Transparent+0" "ForceNoShadowCasting"="True" }
		Cull Back
		GrabPass{ }
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf StandardSpecular keepalpha vertex:vert
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
			float3 customValue;
		};

		uniform half _NormalScale;
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

		float _Scale, _Speed, _Frequency;

		float _WaveAmplitude1, _WaveAmplitude2, _WaveAmplitude3, _WaveAmplitude4, _WaveAmplitude5, _WaveAmplitude6, _WaveAmplitude7, _WaveAmplitude8;
		float _OffsetX1, _OffsetZ1, _OffsetX2, _OffsetZ2, _OffsetX3, _OffsetZ3,_OffsetX4, _OffsetZ4,_OffsetX5, _OffsetZ5,_OffsetX6, _OffsetZ6,_OffsetX7, _OffsetZ7,_OffsetX8, _OffsetZ8;
		float _Distance1, _Distance2 , _Distance3, _Distance4, _Distance5, _Distance6, _Distance7, _Distance8;
		float _xImpact1, _zImpact1, _xImpact2, _zImpact2,_xImpact3, _zImpact3,_xImpact4, _zImpact4,_xImpact5, _zImpact5,_xImpact6, _zImpact6,
		_xImpact7, _zImpact7,_xImpact8, _zImpact8;

		void vert( inout appdata_full v, out Input o)
		{
		UNITY_INITIALIZE_OUTPUT(Input, o);
		half offsetvert = ((v.vertex.x * v.vertex.x) + (v.vertex.z * v.vertex.z));
		half offsetvert2 = v.vertex.x + v.vertex.z; //diagonal waves
		//half offsetvert2 = v.vertex.x; //horizontal waves
		
		half value0 = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert2 );
		
		half value1 = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert + (v.vertex.x * _OffsetX1) + (v.vertex.z * _OffsetZ1)  );
		half value2 = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert + (v.vertex.x * _OffsetX2) + (v.vertex.z * _OffsetZ2)  );
		half value3 = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert + (v.vertex.x * _OffsetX3) + (v.vertex.z * _OffsetZ3)  );
		half value4 = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert + (v.vertex.x * _OffsetX4) + (v.vertex.z * _OffsetZ4)  );
		half value5 = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert + (v.vertex.x * _OffsetX5) + (v.vertex.z * _OffsetZ5)  );
		half value6 = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert + (v.vertex.x * _OffsetX6) + (v.vertex.z * _OffsetZ6)  );
		half value7 = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert + (v.vertex.x * _OffsetX7) + (v.vertex.z * _OffsetZ7)  );
		half value8 = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert + (v.vertex.x * _OffsetX8) + (v.vertex.z * _OffsetZ8)  );
		
		float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
		
		
		v.vertex.y += value0; //remove for no waves
		v.normal.y += value0; //remove for no waves
		o.customValue += value0  ;

		
		if (sqrt(pow(worldPos.x - _xImpact1, 2) + pow(worldPos.z - _zImpact1, 2)) < _Distance1)
		{
		v.vertex.y += value1 * _WaveAmplitude1;
		v.normal.y += value1 * _WaveAmplitude1;	
		o.customValue += value1 * _WaveAmplitude1;	

		}
		if (sqrt(pow(worldPos.x - _xImpact2, 2) + pow(worldPos.z - _zImpact2, 2)) < _Distance2)
		{
		v.vertex.y += value2 * _WaveAmplitude2;
		v.normal.y += value2 * _WaveAmplitude2;
		o.customValue +=  value2 * _WaveAmplitude2;
		}
		if (sqrt(pow(worldPos.x - _xImpact3, 2) + pow(worldPos.z - _zImpact3, 2)) < _Distance3)
		{
		v.vertex.y += value3 * _WaveAmplitude3;
		v.normal.y += value3 * _WaveAmplitude3;
		o.customValue += value3 * _WaveAmplitude3;
		}
		if (sqrt(pow(worldPos.x - _xImpact4, 2) + pow(worldPos.z - _zImpact4, 2)) < _Distance4)
		{
		v.vertex.y += value4 * _WaveAmplitude4;
		v.normal.y += value4 * _WaveAmplitude4;
		o.customValue += value4 * _WaveAmplitude4;
		}
		if (sqrt(pow(worldPos.x - _xImpact5, 2) + pow(worldPos.z - _zImpact5, 2)) < _Distance5)
		{
		v.vertex.y += value5 * _WaveAmplitude5;
		v.normal.y += value5 * _WaveAmplitude5;
		o.customValue += value5 * _WaveAmplitude5;
		}
		if (sqrt(pow(worldPos.x - _xImpact6, 2) + pow(worldPos.z - _zImpact6, 2)) < _Distance6)
		{
		v.vertex.y += value6 * _WaveAmplitude6;
		v.normal.y += value6 * _WaveAmplitude6;
		o.customValue += value6 * _WaveAmplitude6;
		}
		if (sqrt(pow(worldPos.x - _xImpact7, 2) + pow(worldPos.z - _zImpact7, 2)) < _Distance7)
		{
		v.vertex.y += value7 * _WaveAmplitude7;
		v.normal.y += value7 * _WaveAmplitude7;
		o.customValue += value7 * _WaveAmplitude7;
		}
		if (sqrt(pow(worldPos.x - _xImpact8, 2) + pow(worldPos.z - _zImpact8, 2)) < _Distance8)
		{
		v.vertex.y += value8 * _WaveAmplitude8;
		v.normal.y += value8 * _WaveAmplitude8;
		o.customValue += value8 * _WaveAmplitude8;
		}
		
		}

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 uv_WaterNormal = i.uv_texcoord * _WaterNormal_ST.xy + _WaterNormal_ST.zw;
			float2 panner22 = ( uv_WaterNormal + 1.0 * _Time.y * float2( -0.03,0 ));
			float2 panner19 = ( uv_WaterNormal + 1.0 * _Time.y * float2( 0.04,0.04 ));
			float3 temp_output_24_0 = BlendNormals( UnpackScaleNormal( tex2D( _WaterNormal, panner22 ) ,_NormalScale ) , UnpackScaleNormal( tex2D( _WaterNormal, panner19 ) ,_NormalScale ) );
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
265;141;1726;904;2696.298;1046.436;3.244686;True;True
Node;AmplifyShaderEditor.CommentaryNode;152;-2053.601,-256.6997;Float;False;828.5967;315.5001;Screen depth difference to get intersection and fading effect with terrain and obejcts;4;89;2;1;3;;1,1,1,1;0;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;2;-2046.601,-140.1996;Float;False;1;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ScreenDepthNode;1;-1781.601,-155.6997;Float;False;0;1;0;FLOAT4;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleSubtractOpNode;3;-1574.201,-110.3994;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;151;-958.7189,-1261.73;Float;False;1281.603;457.1994;Blend panning normals to fake noving ripples;7;19;23;24;21;22;17;48;;1,1,1,1;0;0
Node;AmplifyShaderEditor.AbsOpNode;89;-1335.004,-112.5834;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;21;-908.7191,-1184.431;Float;False;0;17;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.WireNode;155;-1106.507,7.515848;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;153;-843.9032,402.718;Float;False;1083.102;484.2006;Foam controls and texture;7;105;115;111;110;112;113;114;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;159;-863.7005,-467.5007;Float;False;1113.201;508.3005;Depths controls and colors;10;87;94;12;13;156;157;11;88;6;143;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;111;-862.6843,486.4808;Float;False;Property;_FoamDepth;Foam Depth;10;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.PannerNode;19;-633.7194,-1099.231;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.04,0.04;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.PannerNode;22;-636.0194,-1211.73;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.03,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;6;-858.0854,15.19771;Float;False;Property;_WaterDepth;Water Depth;4;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.WireNode;158;-1075.608,-163.0834;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.WireNode;154;-922.7065,390.316;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;48;-580.1195,-974.6314;Float;False;Property;_NormalScale;Normal Scale;1;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;17;-279.1184,-993.5303;Float;True;Property;_WaterNormal;Water Normal;0;0;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;23;-292.0191,-1203.431;Float;True;Property;_Normal2;Normal2;0;0;None;True;0;True;bump;Auto;True;Instance;17;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;10;-588.4015,137.6029;Float;False;Property;_WaterFalloff;Water Falloff;5;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;115;-671.6769,417.2115;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;112;-696.5845,670.3378;Float;False;Property;_FoamFalloff;Foam Falloff;11;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;88;-618.3488,-95.32761;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;106;-874.1786,919.3324;Float;False;0;105;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PannerNode;116;-570.1139,992.019;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.01,0.01;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.PowerNode;87;-404.5926,14.97146;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.PowerNode;110;-408.1463,453.8997;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;150;467.1957,-1501.783;Float;False;985.6011;418.6005;Get screen color for refraction and disturbe it with normals;7;96;97;98;65;149;164;165;;1,1,1,1;0;0
Node;AmplifyShaderEditor.BlendNormalsNode;24;147.884,-1058.93;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.ColorNode;11;-455.0999,-328.3;Float;False;Property;_ShalowColor;Shalow Color;3;0;1,1,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;12;-697.5002,-417.5007;Float;False;Property;_DeepColor;Deep Color;2;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SaturateNode;113;-111.301,447.8677;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.WireNode;157;-149.1077,-261.9834;Float;False;1;0;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.WireNode;156;-131.5076,-325.9835;Float;False;1;0;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SamplerNode;105;-324.3617,658.2031;Float;True;Property;_Foam;Foam;9;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SaturateNode;94;-165.2129,-178.524;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;97;710.096,-1203.183;Float;False;Property;_Distortion;Distortion;8;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.GrabScreenPosition;164;511.3026,-1442.425;Float;False;0;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.WireNode;149;487.4943,-1188.882;Float;False;1;0;FLOAT3;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;114;145.0366,604.0181;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;13;60.50008,-220.6998;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.ColorNode;108;58.99682,146.0182;Float;False;Constant;_Color0;Color 0;-1;0;1,1,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ComponentMaskNode;165;814.6503,-1385.291;Float;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;98;888.1974,-1279.783;Float;False;2;2;0;FLOAT3;0.0,0,0;False;1;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleAddOpNode;96;1041.296,-1346.683;Float;False;2;2;0;FLOAT2;0.0,0;False;1;FLOAT3;0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.LerpOp;117;339.9822,-11.10016;Float;False;3;0;COLOR;0.0,0,0,0;False;1;COLOR;0.0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.WireNode;143;95.69542,-321.0839;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.WireNode;161;660.4934,-750.6837;Float;False;1;0;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;26;827.1311,-182.0743;Float;False;Property;_WaterSmoothness;Water Smoothness;7;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;132;902.2588,130.2934;Float;False;Property;_FoamSmoothness;Foam Smoothness;13;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;104;742.9969,-676.4819;Float;False;Property;_WaterSpecular;Water Specular;6;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ScreenColorNode;65;1232.797,-1350.483;Float;False;Global;_WaterGrab;WaterGrab;-1;0;Object;-1;False;1;0;FLOAT2;0,0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;131;756.1969,-467.1806;Float;False;Property;_FoamSpecular;Foam Specular;12;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.WireNode;162;1312.293,-894.3823;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;93;1559.196,-1006.285;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;103;1644.795,-389.9843;Float;False;Constant;_Occlusion;Occlusion;-1;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;133;1323.704,-85.5688;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;130;955.7971,-465.8806;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1838.601,-748.1998;Half;False;True;2;Half;ASEMaterialInspector;0;0;StandardSpecular;ASESampleShaders/WaterSample;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;Translucent;0.5;True;False;0;False;Opaque;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0.0,0,0;False;7;FLOAT3;0.0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;11;FLOAT3;0.0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;1;0;2;0
WireConnection;3;0;1;0
WireConnection;3;1;2;4
WireConnection;89;0;3;0
WireConnection;155;0;89;0
WireConnection;19;0;21;0
WireConnection;22;0;21;0
WireConnection;158;0;89;0
WireConnection;154;0;155;0
WireConnection;17;1;19;0
WireConnection;17;5;48;0
WireConnection;23;1;22;0
WireConnection;23;5;48;0
WireConnection;115;0;154;0
WireConnection;115;1;111;0
WireConnection;88;0;158;0
WireConnection;88;1;6;0
WireConnection;116;0;106;0
WireConnection;87;0;88;0
WireConnection;87;1;10;0
WireConnection;110;0;115;0
WireConnection;110;1;112;0
WireConnection;24;0;23;0
WireConnection;24;1;17;0
WireConnection;113;0;110;0
WireConnection;157;0;11;0
WireConnection;156;0;12;0
WireConnection;105;1;116;0
WireConnection;94;0;87;0
WireConnection;149;0;24;0
WireConnection;114;0;113;0
WireConnection;114;1;105;1
WireConnection;13;0;156;0
WireConnection;13;1;157;0
WireConnection;13;2;94;0
WireConnection;165;0;164;0
WireConnection;98;0;149;0
WireConnection;98;1;97;0
WireConnection;96;0;165;0
WireConnection;96;1;98;0
WireConnection;117;0;13;0
WireConnection;117;1;108;0
WireConnection;117;2;114;0
WireConnection;143;0;94;0
WireConnection;161;0;117;0
WireConnection;65;0;96;0
WireConnection;162;0;143;0
WireConnection;93;0;161;0
WireConnection;93;1;65;0
WireConnection;93;2;162;0
WireConnection;133;0;26;0
WireConnection;133;1;132;0
WireConnection;133;2;114;0
WireConnection;130;0;104;0
WireConnection;130;1;131;0
WireConnection;130;2;114;0
WireConnection;0;0;93;0
WireConnection;0;1;24;0
WireConnection;0;3;130;0
WireConnection;0;4;133;0
WireConnection;0;5;103;0
ASEEND*/
//CHKSM=351761293D3E5C5E3AE5A1E8F9DF7602F369A73D