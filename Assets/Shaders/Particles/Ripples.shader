// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Ripples"
{
	Properties
	{
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
	}

	Category 
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane"  }
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask RGB
		Cull Off Lighting Off ZWrite Off
		
		SubShader
		{
			

			Pass {
			
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile_particles
				#pragma multi_compile_fog
				#include "UnityShaderVariables.cginc"


				#include "UnityCG.cginc"

				struct appdata_t 
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
				};

				struct v2f 
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					#ifdef SOFTPARTICLES_ON
					float4 projPos : TEXCOORD2;
					#endif
				};
				
				uniform sampler2D _MainTex;
				uniform fixed4 _TintColor;
				uniform float4 _MainTex_ST;
				uniform sampler2D_float _CameraDepthTexture;
				uniform float _InvFade;
				uniform sampler2D _TextureSample0;

				v2f vert ( appdata_t v  )
				{
					v2f o;

					v.vertex.xyz +=  float3( 0, 0, 0 ) ;
					o.vertex = UnityObjectToClipPos(v.vertex);
					#ifdef SOFTPARTICLES_ON
						o.projPos = ComputeScreenPos (o.vertex);
						COMPUTE_EYEDEPTH(o.projPos.z);
					#endif
					o.color = v.color;
					o.texcoord = v.texcoord;
					o.texcoord.xy = TRANSFORM_TEX(v.texcoord,_MainTex);
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag ( v2f i  ) : SV_Target
				{
					#ifdef SOFTPARTICLES_ON
						float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
						float partZ = i.projPos.z;
						float fade = saturate (_InvFade * (sceneZ-partZ));
						i.color.a *= fade;
					#endif

					float2 appendResult5 = (float2(-0.5 , -0.5));
					float2 uv2 = i.texcoord * float2( 1,1 ) + float2( 0,0 );
					float temp_output_9_0 = ( 2.0 * ( appendResult5 + uv2 ).x );
					float temp_output_10_0 = ( 2.0 * ( appendResult5 + uv2 ).y );
					float clampResult15 = clamp( ( ( temp_output_9_0 * temp_output_9_0 ) + ( temp_output_10_0 * temp_output_10_0 ) ) , 0.0 , 1.0 );
					float2 temp_cast_0 = (clampResult15).xx;
					float2 panner22 = ( temp_cast_0 + 1.0 * _Time.y * float2( 0,-1 ));
					

					fixed4 col = tex2D( _TextureSample0, panner22 );
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG 
			}
		}	
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13801
577;259;1726;904;-193.1561;269.4926;1.030086;True;True
Node;AmplifyShaderEditor.RangedFloatNode;3;-980.793,-104.0499;Float;False;Constant;_Float0;Float 0;0;0;-0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-1045.793,10.35001;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;5;-796.1928,-104.05;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleAddOpNode;6;-624.5931,28.55003;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.BreakToComponentsNode;7;-468.5934,51.94992;Float;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;8;-274.8936,-126.15;Float;False;Constant;_Float1;Float 1;0;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-25.29358,-57.25;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-39.59363,84.44998;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;146.3064,101.35;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;133.3064,-45.54994;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;13;299.7067,32.45005;Float;True;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;15;528.1093,28.46894;Float;True;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.PannerNode;22;844.8866,-13.56876;Float;False;3;0;FLOAT2;0,-1;False;2;FLOAT2;0,-1;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SamplerNode;18;1040.022,44.65074;Float;True;Property;_TextureSample0;Texture Sample 0;0;0;Resources/unity_builtin_extra;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TemplateMasterNode;21;1695.767,-1398.6;Float;False;True;2;Float;ASEMaterialInspector;0;5;Ripples;0b6a9f8b4f707c74ca64c0be8e590de0;Particles Alpha Blended;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;5;0;3;0
WireConnection;5;1;3;0
WireConnection;6;0;5;0
WireConnection;6;1;2;0
WireConnection;7;0;6;0
WireConnection;9;0;8;0
WireConnection;9;1;7;0
WireConnection;10;0;8;0
WireConnection;10;1;7;1
WireConnection;11;0;10;0
WireConnection;11;1;10;0
WireConnection;12;0;9;0
WireConnection;12;1;9;0
WireConnection;13;0;12;0
WireConnection;13;1;11;0
WireConnection;15;0;13;0
WireConnection;22;0;15;0
WireConnection;18;1;22;0
WireConnection;21;0;18;0
ASEEND*/
//CHKSM=9E1E90792E21BD1C136064C0857B80548592E9A2