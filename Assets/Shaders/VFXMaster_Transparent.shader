// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "VFX/VFXMaster_Transparent"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[ASEBegin][Enum(UnityEngine.Rendering.CullMode)]_Culling("Culling", Float) = 2
		[Enum(UnityEngine.Rendering.CompareFunction)]_ZTest("ZTest", Float) = 4
		[Toggle(_POLARUV_ON)] _PolarUV("Polar UV", Float) = 0
		[NoScaleOffset]_GradientMap("GradientMap", 2D) = "white" {}
		[Space(20)]_MainTex("MainTex", 2D) = "white" {}
		[Toggle(_CUSTOMFRAMERATE_ON)] _CustomFramerate("Custom Framerate", Float) = 0
		_Framerate("Framerate", Float) = 5
		_PanningSpeedXYmaintexZWdisplacementtex("Panning Speed (XY main tex - ZW displacement tex)", Vector) = (0,0,0,0)
		[Space(20)][Toggle(_SECONDARYTEXTURE_ON)] _SecondaryTexture("Secondary Texture", Float) = 0
		_SecondaryTex("Secondary Tex", 2D) = "white" {}
		_Secondarypanningspeed("Secondary panning speed", Vector) = (0,0,0,0)
		[HDR][Space(20)]_Color("Color", Color) = (1,1,1,1)
		_Contrast("Contrast", Float) = 1
		_Power("Power", Float) = 1
		[Space(20)]_Cutoff("Cutoff", Range( 0 , 1)) = 0
		_Cutoffsoftness("Cutoff softness", Range( 0 , 1)) = 0
		[Space(20)][Toggle(_SOFTFADE_ON)] _SoftFade("Soft Fade", Float) = 0
		_IntersectionThresholdMax("IntersectionThresholdMax", Float) = 1
		[HDR]_BurnColor("Burn Color", Color) = (1,1,1,1)
		_BurnSize("Burn Size", Float) = 0
		[Toggle(_FRESNELALPHA_ON)] _FresnelAlpha("Fresnel Alpha", Float) = 0
		_FresnelBias("Fresnel Bias", Float) = 0
		_FresnelScale("Fresnel Scale", Float) = 1
		_FresnelPower("Fresnel Power", Float) = 5
		[Space(20)]_DisplacementAmount("Displacement Amount", Float) = 0
		_DisplacementGuide("Displacement Guide", 2D) = "white" {}
		[Space(20)][Toggle(_VERTEXOFFSET_ON)] _VertexOffset("Vertex Offset", Float) = 0
		_VertexOffsetAmount("Vertex Offset Amount", Float) = 0
		[Space(20)][Toggle(_BANDING_ON)] _Banding("Banding", Float) = 0
		_Numberofbands("Number of bands", Float) = 3
		[Space(20)][Toggle(_CIRCLEMASK_ON)] _CircleMask("Circle Mask", Float) = 0
		_InnerRadius("Inner Radius", Range( -1 , 1)) = 0
		_OuterRadius("Outer Radius", Range( 0 , 1)) = 0.5
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.2
		[Space(20)][Toggle(_RECTMASK_ON)] _RectMask("Rect Mask", Float) = 0
		_RectWidth("RectWidth", Float) = 0
		_RectHeight("RectHeight", Float) = 0
		_RectMaskCutoff("RectMaskCutoff", Range( 0 , 1)) = 0
		_RectMaskSmoothness("RectMaskSmoothness", Range( 0 , 1)) = 0
		[Toggle(_VERTEXALPHACUTOFF_ON)] _VertexAlphaCutoff("Vertex Alpha Cutoff", Float) = 1
		[ASEEnd][Header(TexCoord.z)][Toggle(_PARTICLECONTROLSDISPLACEMENT_ON)] _ParticleControlsDisplacement("ParticleControlsDisplacement", Float) = 0


		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		//_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
		//_TessMin( "Tess Min Distance", Float ) = 10
		//_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25

		[HideInInspector] _QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector] _QueueControl("_QueueControl", Float) = -1

        [HideInInspector][NoScaleOffset] unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" "UniversalMaterialType"="Unlit" }

		Cull [_Culling]
		AlphaToMask Off

		

		HLSLINCLUDE
		#pragma target 3.5
		#pragma prefer_hlslcc gles
		// ensure rendering platforms toggle list is visible

		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"

		#ifndef ASE_TESS_FUNCS
		#define ASE_TESS_FUNCS
		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}

		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		#endif //ASE_TESS_FUNCS
		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForwardOnly" }

			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZWrite Off
			ZTest [_ZTest]
			Offset 0 , 0
			ColorMask RGBA

			

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_SRP_VERSION 140007
			#define REQUIRE_DEPTH_TEXTURE 1


			#pragma instancing_options renderinglayer

			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma shader_feature _ _SAMPLE_GI
			#pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        	#pragma multi_compile_fragment _ DEBUG_DISPLAY
        	#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
        	#pragma multi_compile_fragment _ _WRITE_RENDERING_LAYERS

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_UNLIT

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging3D.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceData.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_COLOR
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma shader_feature_local _VERTEXOFFSET_ON
			#pragma shader_feature_local _SECONDARYTEXTURE_ON
			#pragma shader_feature_local _CUSTOMFRAMERATE_ON
			#pragma shader_feature_local _BANDING_ON
			#pragma shader_feature_local _RECTMASK_ON
			#pragma shader_feature_local _CIRCLEMASK_ON
			#pragma shader_feature_local _POLARUV_ON
			#pragma shader_feature_local _PARTICLECONTROLSDISPLACEMENT_ON
			#pragma shader_feature_local _VERTEXALPHACUTOFF_ON
			#pragma shader_feature_local _FRESNELALPHA_ON
			#pragma shader_feature_local _SOFTFADE_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD1;
				#endif
				#ifdef ASE_FOG
					float fogFactor : TEXCOORD2;
				#endif
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_color : COLOR;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _MainTex_ST;
			float4 _PanningSpeedXYmaintexZWdisplacementtex;
			float4 _SecondaryTex_ST;
			float4 _DisplacementGuide_ST;
			float4 _BurnColor;
			float4 _Color;
			float2 _Secondarypanningspeed;
			float _Culling;
			float _FresnelBias;
			float _IntersectionThresholdMax;
			float _BurnSize;
			float _Cutoffsoftness;
			float _Cutoff;
			float _Numberofbands;
			float _RectHeight;
			float _RectWidth;
			float _InnerRadius;
			float _RectMaskCutoff;
			float _FresnelScale;
			float _Smoothness;
			float _OuterRadius;
			float _Power;
			float _Contrast;
			float _DisplacementAmount;
			float _VertexOffsetAmount;
			float _Framerate;
			float _ZTest;
			float _RectMaskSmoothness;
			float _FresnelPower;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _MainTex;
			sampler2D _SecondaryTex;
			sampler2D _GradientMap;
			sampler2D _DisplacementGuide;
			uniform float4 _CameraDepthTexture_TexelSize;


			
			VertexOutput VertexFunction ( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float2 uv_MainTex = v.ase_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
				#ifdef _CUSTOMFRAMERATE_ON
				float staticSwitch334_g57 = ( round( ( _TimeParameters.x * _Framerate ) ) / _Framerate );
				#else
				float staticSwitch334_g57 = _TimeParameters.x;
				#endif
				float4 temp_output_17_0_g57 = ( _PanningSpeedXYmaintexZWdisplacementtex * staticSwitch334_g57 );
				float2 _PanningSpeedXY69_g57 = (temp_output_17_0_g57).xy;
				float4 tex2DNode210_g57 = tex2Dlod( _MainTex, float4( ( uv_MainTex + _PanningSpeedXY69_g57 ), 0, 1.0) );
				float2 uv2_SecondaryTex = v.ase_texcoord1.xy * _SecondaryTex_ST.xy + _SecondaryTex_ST.zw;
				float2 _SecondaryPanningSpeed67_g57 = ( _Secondarypanningspeed * staticSwitch334_g57 );
				#ifdef _SECONDARYTEXTURE_ON
				float staticSwitch158_g57 = ( tex2DNode210_g57.r * tex2Dlod( _SecondaryTex, float4( ( uv2_SecondaryTex + _SecondaryPanningSpeed67_g57 ), 0, 1.0) ).r * 2.0 );
				#else
				float staticSwitch158_g57 = tex2DNode210_g57.r;
				#endif
				float _VertexOffsetAmount162_g57 = _VertexOffsetAmount;
				#ifdef _VERTEXOFFSET_ON
				float3 staticSwitch185_g57 = ( ( (-1.0 + (staticSwitch158_g57 - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) * _VertexOffsetAmount162_g57 ) * v.ase_normal );
				#else
				float3 staticSwitch185_g57 = float3( 0,0,0 );
				#endif
				float3 vertexToFrag188_g57 = staticSwitch185_g57;
				float3 __VertexOffset191_g57 = vertexToFrag188_g57;
				
				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord5 = screenPos;
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord6.xyz = ase_worldNormal;
				
				o.ase_texcoord3 = v.ase_texcoord;
				o.ase_texcoord4.xy = v.ase_texcoord1.xy;
				o.ase_color = v.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord4.zw = 0;
				o.ase_texcoord6.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = __VertexOffset191_g57;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.worldPos = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				#ifdef ASE_FOG
					o.fogFactor = ComputeFogFactor( positionCS.z );
				#endif

				o.clipPos = positionCS;

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord1 = v.ase_texcoord1;
				o.ase_color = v.ase_color;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag ( VertexOutput IN
				#ifdef _WRITE_RENDERING_LAYERS
				, out float4 outRenderingLayers : SV_Target1
				#endif
				 ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = IN.worldPos;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 uv_MainTex = IN.ase_texcoord3.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float2 texCoord294_g57 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_21_0_g57 = (float2( -1,-1 ) + (texCoord294_g57 - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 )));
				float2 break24_g57 = temp_output_21_0_g57;
				float2 appendResult54_g57 = (float2(( ( ( atan2( break24_g57.y , break24_g57.x ) / PI ) / 2.0 ) + 0.5 ) , length( temp_output_21_0_g57 )));
				#ifdef _POLARUV_ON
				float2 staticSwitch63_g57 = ( ( appendResult54_g57 * _MainTex_ST.xy ) + _MainTex_ST.zw );
				#else
				float2 staticSwitch63_g57 = uv_MainTex;
				#endif
				float2 _UVMain66_g57 = staticSwitch63_g57;
				#ifdef _CUSTOMFRAMERATE_ON
				float staticSwitch334_g57 = ( round( ( _TimeParameters.x * _Framerate ) ) / _Framerate );
				#else
				float staticSwitch334_g57 = _TimeParameters.x;
				#endif
				float4 temp_output_17_0_g57 = ( _PanningSpeedXYmaintexZWdisplacementtex * staticSwitch334_g57 );
				float2 _PanningSpeedXY69_g57 = (temp_output_17_0_g57).xy;
				float2 uv_DisplacementGuide = IN.ase_texcoord3.xy * _DisplacementGuide_ST.xy + _DisplacementGuide_ST.zw;
				float2 texCoord301_g57 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_3_0_g57 = (float2( -1,-1 ) + (texCoord301_g57 - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 )));
				float2 break4_g57 = temp_output_3_0_g57;
				float2 appendResult18_g57 = (float2(( ( ( atan2( break4_g57.y , break4_g57.x ) / PI ) / 2.0 ) + 0.5 ) , length( temp_output_3_0_g57 )));
				#ifdef _POLARUV_ON
				float2 staticSwitch195_g57 = ( ( appendResult18_g57 * _DisplacementGuide_ST.xy ) + _DisplacementGuide_ST.zw );
				#else
				float2 staticSwitch195_g57 = uv_DisplacementGuide;
				#endif
				float2 _UVDisplacement33_g57 = staticSwitch195_g57;
				float2 _PanningSpeedZW31_g57 = (temp_output_17_0_g57).zw;
				float4 appendResult22_g57 = (float4(ddx( uv_DisplacementGuide ) , ddy( uv_DisplacementGuide )));
				float4 _DDDisplacement29_g57 = appendResult22_g57;
				#ifdef _PARTICLECONTROLSDISPLACEMENT_ON
				float staticSwitch335_g57 = IN.ase_texcoord3.z;
				#else
				float staticSwitch335_g57 = _DisplacementAmount;
				#endif
				float2 __Displacement77_g57 = ( (float2( -1,-1 ) + ((tex2D( _DisplacementGuide, ( _UVDisplacement33_g57 + _PanningSpeedZW31_g57 ), (_DDDisplacement29_g57).xy, (_DDDisplacement29_g57).zw )).rg - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 ))) * staticSwitch335_g57 );
				float4 appendResult68_g57 = (float4(ddx( uv_MainTex ) , ddy( uv_MainTex )));
				float4 _DDMainTex76_g57 = appendResult68_g57;
				float4 tex2DNode96_g57 = tex2D( _MainTex, ( ( _UVMain66_g57 + _PanningSpeedXY69_g57 ) + __Displacement77_g57 ), (_DDMainTex76_g57).xy, (_DDMainTex76_g57).zw );
				float _Contrast90_g57 = _Contrast;
				float lerpResult98_g57 = lerp( 0.5 , tex2DNode96_g57.r , _Contrast90_g57);
				float _Power97_g57 = _Power;
				float temp_output_106_0_g57 = pow( saturate( lerpResult98_g57 ) , _Power97_g57 );
				float2 uv_SecondaryTex = IN.ase_texcoord3.xy * _SecondaryTex_ST.xy + _SecondaryTex_ST.zw;
				float2 texCoord302_g57 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_20_0_g57 = (float2( -1,-1 ) + (texCoord302_g57 - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 )));
				float2 break26_g57 = temp_output_20_0_g57;
				float2 appendResult53_g57 = (float2(( ( ( atan2( break26_g57.y , break26_g57.x ) / PI ) / 2.0 ) + 0.5 ) , length( temp_output_20_0_g57 )));
				#ifdef _POLARUV_ON
				float2 staticSwitch197_g57 = ( ( appendResult53_g57 * _SecondaryTex_ST.xy ) + _SecondaryTex_ST.zw );
				#else
				float2 staticSwitch197_g57 = uv_SecondaryTex;
				#endif
				float2 _UVSecondary65_g57 = staticSwitch197_g57;
				float2 _SecondaryPanningSpeed67_g57 = ( _Secondarypanningspeed * staticSwitch334_g57 );
				float4 appendResult71_g57 = (float4(ddx( uv_SecondaryTex ) , ddy( uv_SecondaryTex )));
				float4 _DDSecondary75_g57 = appendResult71_g57;
				float lerpResult99_g57 = lerp( 0.5 , tex2D( _SecondaryTex, ( ( _UVSecondary65_g57 + _SecondaryPanningSpeed67_g57 ) + __Displacement77_g57 ), (_DDSecondary75_g57).xy, (_DDSecondary75_g57).zw ).r , _Contrast90_g57);
				#ifdef _SECONDARYTEXTURE_ON
				float staticSwitch198_g57 = ( temp_output_106_0_g57 * pow( saturate( lerpResult99_g57 ) , _Power97_g57 ) * 2.0 );
				#else
				float staticSwitch198_g57 = temp_output_106_0_g57;
				#endif
				float2 texCoord102_g57 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_110_0_g57 = distance( texCoord102_g57 , float2( 0.5,0.5 ) );
				float smoothstepResult113_g57 = smoothstep( _OuterRadius , ( _OuterRadius + _Smoothness ) , temp_output_110_0_g57);
				float smoothstepResult115_g57 = smoothstep( _InnerRadius , ( _InnerRadius + _Smoothness ) , temp_output_110_0_g57);
				#ifdef _CIRCLEMASK_ON
				float staticSwitch119_g57 = ( staticSwitch198_g57 * ( 1.0 - smoothstepResult113_g57 ) * smoothstepResult115_g57 );
				#else
				float staticSwitch119_g57 = staticSwitch198_g57;
				#endif
				float2 texCoord212_g57 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 break217_g57 = (float2( -1,-1 ) + (texCoord212_g57 - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 )));
				float smoothstepResult222_g57 = smoothstep( _RectMaskCutoff , ( _RectMaskCutoff + _RectMaskSmoothness ) , max( abs( ( break217_g57.x / _RectWidth ) ) , abs( ( break217_g57.y / _RectHeight ) ) ));
				#ifdef _RECTMASK_ON
				float staticSwitch211_g57 = ( staticSwitch119_g57 * ( 1.0 - smoothstepResult222_g57 ) );
				#else
				float staticSwitch211_g57 = staticSwitch119_g57;
				#endif
				float __orgCol124_g57 = staticSwitch211_g57;
				#ifdef _BANDING_ON
				float staticSwitch127_g57 = ( round( ( staticSwitch211_g57 * _Numberofbands ) ) / _Numberofbands );
				#else
				float staticSwitch127_g57 = __orgCol124_g57;
				#endif
				float __Col132_g57 = staticSwitch127_g57;
				float2 appendResult141_g57 = (float2(__Col132_g57 , 0.0));
				float4 __RampColor151_g57 = tex2D( _GradientMap, appendResult141_g57 );
				float4 _Color161_g57 = ( IN.ase_color * _Color );
				float4 _BurnCol173_g57 = _BurnColor;
				float _Cutoff130_g57 = _Cutoff;
				#ifdef _VERTEXALPHACUTOFF_ON
				float staticSwitch318_g57 = ( 1.0 - IN.ase_color.a );
				#else
				float staticSwitch318_g57 = 0.0;
				#endif
				float temp_output_150_0_g57 = saturate( ( _Cutoff130_g57 + staticSwitch318_g57 ) );
				float __Cutout237_g57 = temp_output_150_0_g57;
				float temp_output_240_0_g57 = ( __orgCol124_g57 - __Cutout237_g57 );
				float _CutoffSoftness201_g57 = _Cutoffsoftness;
				float _BurnSize168_g57 = _BurnSize;
				float smoothstepResult236_g57 = smoothstep( temp_output_240_0_g57 , ( temp_output_240_0_g57 + _CutoffSoftness201_g57 ) , _BurnSize168_g57);
				float smoothstepResult253_g57 = smoothstep( 0.001 , 0.5 , __Cutout237_g57);
				float3 temp_output_261_0_g57 = (( ( ( ( float4( (__RampColor151_g57).rgb , 0.0 ) * _Color161_g57 ) + ( _BurnCol173_g57 * smoothstepResult236_g57 * smoothstepResult253_g57 ) ) * _Color161_g57 ) * (__RampColor151_g57).a )).rgb;
				float smoothstepResult231_g57 = smoothstep( temp_output_150_0_g57 , ( temp_output_150_0_g57 + _CutoffSoftness201_g57 ) , __orgCol124_g57);
				float _Alpha235_g57 = smoothstepResult231_g57;
				float _MainTexAlpha266_g57 = tex2DNode96_g57.a;
				float temp_output_267_0_g57 = ( _Alpha235_g57 * _MainTexAlpha266_g57 * (_Color161_g57).a );
				float4 screenPos = IN.ase_texcoord5;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float _IntersectionThresholdMax203_g57 = _IntersectionThresholdMax;
				float screenDepth277_g57 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth277_g57 = saturate( abs( ( screenDepth277_g57 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _IntersectionThresholdMax203_g57 ) ) );
				#ifdef _SOFTFADE_ON
				float staticSwitch279_g57 = ( temp_output_267_0_g57 * distanceDepth277_g57 );
				#else
				float staticSwitch279_g57 = temp_output_267_0_g57;
				#endif
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 ase_worldNormal = IN.ase_texcoord6.xyz;
				float fresnelNdotV306_g57 = dot( ase_worldNormal, ase_worldViewDir );
				float fresnelNode306_g57 = ( _FresnelBias + _FresnelScale * pow( 1.0 - fresnelNdotV306_g57, _FresnelPower ) );
				#ifdef _FRESNELALPHA_ON
				float staticSwitch307_g57 = ( staticSwitch279_g57 * fresnelNode306_g57 );
				#else
				float staticSwitch307_g57 = staticSwitch279_g57;
				#endif
				float4 appendResult260_g57 = (float4(temp_output_261_0_g57 , staticSwitch307_g57));
				float4 temp_output_71_0 = appendResult260_g57;
				
				float3 BakedAlbedo = 0;
				float3 BakedEmission = 0;
				float3 Color = temp_output_71_0.xyz;
				float Alpha = saturate( (temp_output_71_0).w );
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;

				#ifdef _ALPHATEST_ON
					clip( Alpha - AlphaClipThreshold );
				#endif

				#if defined(_DBUFFER)
					ApplyDecalToBaseColor(IN.clipPos, Color);
				#endif

				#if defined(_ALPHAPREMULTIPLY_ON)
				Color *= Alpha;
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODFadeCrossFade( IN.clipPos );
				#endif

				#ifdef ASE_FOG
					Color = MixFog( Color, IN.fogFactor );
				#endif

				#ifdef _WRITE_RENDERING_LAYERS
					uint renderingLayers = GetMeshRenderingLayer();
					outRenderingLayers = float4( EncodeMeshRenderingLayer( renderingLayers ), 0, 0, 0 );
				#endif

				return half4( Color, Alpha );
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }

			ZWrite On
			ZTest LEqual
			AlphaToMask Off
			ColorMask 0

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_SRP_VERSION 140007
			#define REQUIRE_DEPTH_TEXTURE 1


			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile _ _CASTING_PUNCTUAL_LIGHT_SHADOW

			#define SHADERPASS SHADERPASS_SHADOWCASTER

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_COLOR
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma shader_feature_local _VERTEXOFFSET_ON
			#pragma shader_feature_local _SECONDARYTEXTURE_ON
			#pragma shader_feature_local _CUSTOMFRAMERATE_ON
			#pragma shader_feature_local _BANDING_ON
			#pragma shader_feature_local _RECTMASK_ON
			#pragma shader_feature_local _CIRCLEMASK_ON
			#pragma shader_feature_local _POLARUV_ON
			#pragma shader_feature_local _PARTICLECONTROLSDISPLACEMENT_ON
			#pragma shader_feature_local _VERTEXALPHACUTOFF_ON
			#pragma shader_feature_local _FRESNELALPHA_ON
			#pragma shader_feature_local _SOFTFADE_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_color : COLOR;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _MainTex_ST;
			float4 _PanningSpeedXYmaintexZWdisplacementtex;
			float4 _SecondaryTex_ST;
			float4 _DisplacementGuide_ST;
			float4 _BurnColor;
			float4 _Color;
			float2 _Secondarypanningspeed;
			float _Culling;
			float _FresnelBias;
			float _IntersectionThresholdMax;
			float _BurnSize;
			float _Cutoffsoftness;
			float _Cutoff;
			float _Numberofbands;
			float _RectHeight;
			float _RectWidth;
			float _InnerRadius;
			float _RectMaskCutoff;
			float _FresnelScale;
			float _Smoothness;
			float _OuterRadius;
			float _Power;
			float _Contrast;
			float _DisplacementAmount;
			float _VertexOffsetAmount;
			float _Framerate;
			float _ZTest;
			float _RectMaskSmoothness;
			float _FresnelPower;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _MainTex;
			sampler2D _SecondaryTex;
			sampler2D _GradientMap;
			sampler2D _DisplacementGuide;
			uniform float4 _CameraDepthTexture_TexelSize;


			
			float3 _LightDirection;
			float3 _LightPosition;

			VertexOutput VertexFunction( VertexInput v )
			{
				VertexOutput o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				float2 uv_MainTex = v.ase_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
				#ifdef _CUSTOMFRAMERATE_ON
				float staticSwitch334_g57 = ( round( ( _TimeParameters.x * _Framerate ) ) / _Framerate );
				#else
				float staticSwitch334_g57 = _TimeParameters.x;
				#endif
				float4 temp_output_17_0_g57 = ( _PanningSpeedXYmaintexZWdisplacementtex * staticSwitch334_g57 );
				float2 _PanningSpeedXY69_g57 = (temp_output_17_0_g57).xy;
				float4 tex2DNode210_g57 = tex2Dlod( _MainTex, float4( ( uv_MainTex + _PanningSpeedXY69_g57 ), 0, 1.0) );
				float2 uv2_SecondaryTex = v.ase_texcoord1.xy * _SecondaryTex_ST.xy + _SecondaryTex_ST.zw;
				float2 _SecondaryPanningSpeed67_g57 = ( _Secondarypanningspeed * staticSwitch334_g57 );
				#ifdef _SECONDARYTEXTURE_ON
				float staticSwitch158_g57 = ( tex2DNode210_g57.r * tex2Dlod( _SecondaryTex, float4( ( uv2_SecondaryTex + _SecondaryPanningSpeed67_g57 ), 0, 1.0) ).r * 2.0 );
				#else
				float staticSwitch158_g57 = tex2DNode210_g57.r;
				#endif
				float _VertexOffsetAmount162_g57 = _VertexOffsetAmount;
				#ifdef _VERTEXOFFSET_ON
				float3 staticSwitch185_g57 = ( ( (-1.0 + (staticSwitch158_g57 - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) * _VertexOffsetAmount162_g57 ) * v.ase_normal );
				#else
				float3 staticSwitch185_g57 = float3( 0,0,0 );
				#endif
				float3 vertexToFrag188_g57 = staticSwitch185_g57;
				float3 __VertexOffset191_g57 = vertexToFrag188_g57;
				
				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord4 = screenPos;
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord5.xyz = ase_worldNormal;
				
				o.ase_texcoord2 = v.ase_texcoord;
				o.ase_texcoord3.xy = v.ase_texcoord1.xy;
				o.ase_color = v.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord3.zw = 0;
				o.ase_texcoord5.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = __VertexOffset191_g57;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.worldPos = positionWS;
				#endif

				float3 normalWS = TransformObjectToWorldDir( v.ase_normal );

				#if _CASTING_PUNCTUAL_LIGHT_SHADOW
					float3 lightDirectionWS = normalize(_LightPosition - positionWS);
				#else
					float3 lightDirectionWS = _LightDirection;
				#endif

				float4 clipPos = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));

				#if UNITY_REVERSED_Z
					clipPos.z = min(clipPos.z, UNITY_NEAR_CLIP_VALUE);
				#else
					clipPos.z = max(clipPos.z, UNITY_NEAR_CLIP_VALUE);
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				o.clipPos = clipPos;

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord1 = v.ase_texcoord1;
				o.ase_color = v.ase_color;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = IN.worldPos;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 uv_MainTex = IN.ase_texcoord2.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float2 texCoord294_g57 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_21_0_g57 = (float2( -1,-1 ) + (texCoord294_g57 - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 )));
				float2 break24_g57 = temp_output_21_0_g57;
				float2 appendResult54_g57 = (float2(( ( ( atan2( break24_g57.y , break24_g57.x ) / PI ) / 2.0 ) + 0.5 ) , length( temp_output_21_0_g57 )));
				#ifdef _POLARUV_ON
				float2 staticSwitch63_g57 = ( ( appendResult54_g57 * _MainTex_ST.xy ) + _MainTex_ST.zw );
				#else
				float2 staticSwitch63_g57 = uv_MainTex;
				#endif
				float2 _UVMain66_g57 = staticSwitch63_g57;
				#ifdef _CUSTOMFRAMERATE_ON
				float staticSwitch334_g57 = ( round( ( _TimeParameters.x * _Framerate ) ) / _Framerate );
				#else
				float staticSwitch334_g57 = _TimeParameters.x;
				#endif
				float4 temp_output_17_0_g57 = ( _PanningSpeedXYmaintexZWdisplacementtex * staticSwitch334_g57 );
				float2 _PanningSpeedXY69_g57 = (temp_output_17_0_g57).xy;
				float2 uv_DisplacementGuide = IN.ase_texcoord2.xy * _DisplacementGuide_ST.xy + _DisplacementGuide_ST.zw;
				float2 texCoord301_g57 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_3_0_g57 = (float2( -1,-1 ) + (texCoord301_g57 - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 )));
				float2 break4_g57 = temp_output_3_0_g57;
				float2 appendResult18_g57 = (float2(( ( ( atan2( break4_g57.y , break4_g57.x ) / PI ) / 2.0 ) + 0.5 ) , length( temp_output_3_0_g57 )));
				#ifdef _POLARUV_ON
				float2 staticSwitch195_g57 = ( ( appendResult18_g57 * _DisplacementGuide_ST.xy ) + _DisplacementGuide_ST.zw );
				#else
				float2 staticSwitch195_g57 = uv_DisplacementGuide;
				#endif
				float2 _UVDisplacement33_g57 = staticSwitch195_g57;
				float2 _PanningSpeedZW31_g57 = (temp_output_17_0_g57).zw;
				float4 appendResult22_g57 = (float4(ddx( uv_DisplacementGuide ) , ddy( uv_DisplacementGuide )));
				float4 _DDDisplacement29_g57 = appendResult22_g57;
				#ifdef _PARTICLECONTROLSDISPLACEMENT_ON
				float staticSwitch335_g57 = IN.ase_texcoord2.z;
				#else
				float staticSwitch335_g57 = _DisplacementAmount;
				#endif
				float2 __Displacement77_g57 = ( (float2( -1,-1 ) + ((tex2D( _DisplacementGuide, ( _UVDisplacement33_g57 + _PanningSpeedZW31_g57 ), (_DDDisplacement29_g57).xy, (_DDDisplacement29_g57).zw )).rg - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 ))) * staticSwitch335_g57 );
				float4 appendResult68_g57 = (float4(ddx( uv_MainTex ) , ddy( uv_MainTex )));
				float4 _DDMainTex76_g57 = appendResult68_g57;
				float4 tex2DNode96_g57 = tex2D( _MainTex, ( ( _UVMain66_g57 + _PanningSpeedXY69_g57 ) + __Displacement77_g57 ), (_DDMainTex76_g57).xy, (_DDMainTex76_g57).zw );
				float _Contrast90_g57 = _Contrast;
				float lerpResult98_g57 = lerp( 0.5 , tex2DNode96_g57.r , _Contrast90_g57);
				float _Power97_g57 = _Power;
				float temp_output_106_0_g57 = pow( saturate( lerpResult98_g57 ) , _Power97_g57 );
				float2 uv_SecondaryTex = IN.ase_texcoord2.xy * _SecondaryTex_ST.xy + _SecondaryTex_ST.zw;
				float2 texCoord302_g57 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_20_0_g57 = (float2( -1,-1 ) + (texCoord302_g57 - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 )));
				float2 break26_g57 = temp_output_20_0_g57;
				float2 appendResult53_g57 = (float2(( ( ( atan2( break26_g57.y , break26_g57.x ) / PI ) / 2.0 ) + 0.5 ) , length( temp_output_20_0_g57 )));
				#ifdef _POLARUV_ON
				float2 staticSwitch197_g57 = ( ( appendResult53_g57 * _SecondaryTex_ST.xy ) + _SecondaryTex_ST.zw );
				#else
				float2 staticSwitch197_g57 = uv_SecondaryTex;
				#endif
				float2 _UVSecondary65_g57 = staticSwitch197_g57;
				float2 _SecondaryPanningSpeed67_g57 = ( _Secondarypanningspeed * staticSwitch334_g57 );
				float4 appendResult71_g57 = (float4(ddx( uv_SecondaryTex ) , ddy( uv_SecondaryTex )));
				float4 _DDSecondary75_g57 = appendResult71_g57;
				float lerpResult99_g57 = lerp( 0.5 , tex2D( _SecondaryTex, ( ( _UVSecondary65_g57 + _SecondaryPanningSpeed67_g57 ) + __Displacement77_g57 ), (_DDSecondary75_g57).xy, (_DDSecondary75_g57).zw ).r , _Contrast90_g57);
				#ifdef _SECONDARYTEXTURE_ON
				float staticSwitch198_g57 = ( temp_output_106_0_g57 * pow( saturate( lerpResult99_g57 ) , _Power97_g57 ) * 2.0 );
				#else
				float staticSwitch198_g57 = temp_output_106_0_g57;
				#endif
				float2 texCoord102_g57 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_110_0_g57 = distance( texCoord102_g57 , float2( 0.5,0.5 ) );
				float smoothstepResult113_g57 = smoothstep( _OuterRadius , ( _OuterRadius + _Smoothness ) , temp_output_110_0_g57);
				float smoothstepResult115_g57 = smoothstep( _InnerRadius , ( _InnerRadius + _Smoothness ) , temp_output_110_0_g57);
				#ifdef _CIRCLEMASK_ON
				float staticSwitch119_g57 = ( staticSwitch198_g57 * ( 1.0 - smoothstepResult113_g57 ) * smoothstepResult115_g57 );
				#else
				float staticSwitch119_g57 = staticSwitch198_g57;
				#endif
				float2 texCoord212_g57 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 break217_g57 = (float2( -1,-1 ) + (texCoord212_g57 - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 )));
				float smoothstepResult222_g57 = smoothstep( _RectMaskCutoff , ( _RectMaskCutoff + _RectMaskSmoothness ) , max( abs( ( break217_g57.x / _RectWidth ) ) , abs( ( break217_g57.y / _RectHeight ) ) ));
				#ifdef _RECTMASK_ON
				float staticSwitch211_g57 = ( staticSwitch119_g57 * ( 1.0 - smoothstepResult222_g57 ) );
				#else
				float staticSwitch211_g57 = staticSwitch119_g57;
				#endif
				float __orgCol124_g57 = staticSwitch211_g57;
				#ifdef _BANDING_ON
				float staticSwitch127_g57 = ( round( ( staticSwitch211_g57 * _Numberofbands ) ) / _Numberofbands );
				#else
				float staticSwitch127_g57 = __orgCol124_g57;
				#endif
				float __Col132_g57 = staticSwitch127_g57;
				float2 appendResult141_g57 = (float2(__Col132_g57 , 0.0));
				float4 __RampColor151_g57 = tex2D( _GradientMap, appendResult141_g57 );
				float4 _Color161_g57 = ( IN.ase_color * _Color );
				float4 _BurnCol173_g57 = _BurnColor;
				float _Cutoff130_g57 = _Cutoff;
				#ifdef _VERTEXALPHACUTOFF_ON
				float staticSwitch318_g57 = ( 1.0 - IN.ase_color.a );
				#else
				float staticSwitch318_g57 = 0.0;
				#endif
				float temp_output_150_0_g57 = saturate( ( _Cutoff130_g57 + staticSwitch318_g57 ) );
				float __Cutout237_g57 = temp_output_150_0_g57;
				float temp_output_240_0_g57 = ( __orgCol124_g57 - __Cutout237_g57 );
				float _CutoffSoftness201_g57 = _Cutoffsoftness;
				float _BurnSize168_g57 = _BurnSize;
				float smoothstepResult236_g57 = smoothstep( temp_output_240_0_g57 , ( temp_output_240_0_g57 + _CutoffSoftness201_g57 ) , _BurnSize168_g57);
				float smoothstepResult253_g57 = smoothstep( 0.001 , 0.5 , __Cutout237_g57);
				float3 temp_output_261_0_g57 = (( ( ( ( float4( (__RampColor151_g57).rgb , 0.0 ) * _Color161_g57 ) + ( _BurnCol173_g57 * smoothstepResult236_g57 * smoothstepResult253_g57 ) ) * _Color161_g57 ) * (__RampColor151_g57).a )).rgb;
				float smoothstepResult231_g57 = smoothstep( temp_output_150_0_g57 , ( temp_output_150_0_g57 + _CutoffSoftness201_g57 ) , __orgCol124_g57);
				float _Alpha235_g57 = smoothstepResult231_g57;
				float _MainTexAlpha266_g57 = tex2DNode96_g57.a;
				float temp_output_267_0_g57 = ( _Alpha235_g57 * _MainTexAlpha266_g57 * (_Color161_g57).a );
				float4 screenPos = IN.ase_texcoord4;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float _IntersectionThresholdMax203_g57 = _IntersectionThresholdMax;
				float screenDepth277_g57 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth277_g57 = saturate( abs( ( screenDepth277_g57 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _IntersectionThresholdMax203_g57 ) ) );
				#ifdef _SOFTFADE_ON
				float staticSwitch279_g57 = ( temp_output_267_0_g57 * distanceDepth277_g57 );
				#else
				float staticSwitch279_g57 = temp_output_267_0_g57;
				#endif
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 ase_worldNormal = IN.ase_texcoord5.xyz;
				float fresnelNdotV306_g57 = dot( ase_worldNormal, ase_worldViewDir );
				float fresnelNode306_g57 = ( _FresnelBias + _FresnelScale * pow( 1.0 - fresnelNdotV306_g57, _FresnelPower ) );
				#ifdef _FRESNELALPHA_ON
				float staticSwitch307_g57 = ( staticSwitch279_g57 * fresnelNode306_g57 );
				#else
				float staticSwitch307_g57 = staticSwitch279_g57;
				#endif
				float4 appendResult260_g57 = (float4(temp_output_261_0_g57 , staticSwitch307_g57));
				float4 temp_output_71_0 = appendResult260_g57;
				

				float Alpha = saturate( (temp_output_71_0).w );
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;

				#ifdef _ALPHATEST_ON
					#ifdef _ALPHATEST_SHADOW_ON
						clip(Alpha - AlphaClipThresholdShadow);
					#else
						clip(Alpha - AlphaClipThreshold);
					#endif
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODFadeCrossFade( IN.clipPos );
				#endif
				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0
			AlphaToMask Off

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_SRP_VERSION 140007
			#define REQUIRE_DEPTH_TEXTURE 1


			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_COLOR
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma shader_feature_local _VERTEXOFFSET_ON
			#pragma shader_feature_local _SECONDARYTEXTURE_ON
			#pragma shader_feature_local _CUSTOMFRAMERATE_ON
			#pragma shader_feature_local _BANDING_ON
			#pragma shader_feature_local _RECTMASK_ON
			#pragma shader_feature_local _CIRCLEMASK_ON
			#pragma shader_feature_local _POLARUV_ON
			#pragma shader_feature_local _PARTICLECONTROLSDISPLACEMENT_ON
			#pragma shader_feature_local _VERTEXALPHACUTOFF_ON
			#pragma shader_feature_local _FRESNELALPHA_ON
			#pragma shader_feature_local _SOFTFADE_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_color : COLOR;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _MainTex_ST;
			float4 _PanningSpeedXYmaintexZWdisplacementtex;
			float4 _SecondaryTex_ST;
			float4 _DisplacementGuide_ST;
			float4 _BurnColor;
			float4 _Color;
			float2 _Secondarypanningspeed;
			float _Culling;
			float _FresnelBias;
			float _IntersectionThresholdMax;
			float _BurnSize;
			float _Cutoffsoftness;
			float _Cutoff;
			float _Numberofbands;
			float _RectHeight;
			float _RectWidth;
			float _InnerRadius;
			float _RectMaskCutoff;
			float _FresnelScale;
			float _Smoothness;
			float _OuterRadius;
			float _Power;
			float _Contrast;
			float _DisplacementAmount;
			float _VertexOffsetAmount;
			float _Framerate;
			float _ZTest;
			float _RectMaskSmoothness;
			float _FresnelPower;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _MainTex;
			sampler2D _SecondaryTex;
			sampler2D _GradientMap;
			sampler2D _DisplacementGuide;
			uniform float4 _CameraDepthTexture_TexelSize;


			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float2 uv_MainTex = v.ase_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
				#ifdef _CUSTOMFRAMERATE_ON
				float staticSwitch334_g57 = ( round( ( _TimeParameters.x * _Framerate ) ) / _Framerate );
				#else
				float staticSwitch334_g57 = _TimeParameters.x;
				#endif
				float4 temp_output_17_0_g57 = ( _PanningSpeedXYmaintexZWdisplacementtex * staticSwitch334_g57 );
				float2 _PanningSpeedXY69_g57 = (temp_output_17_0_g57).xy;
				float4 tex2DNode210_g57 = tex2Dlod( _MainTex, float4( ( uv_MainTex + _PanningSpeedXY69_g57 ), 0, 1.0) );
				float2 uv2_SecondaryTex = v.ase_texcoord1.xy * _SecondaryTex_ST.xy + _SecondaryTex_ST.zw;
				float2 _SecondaryPanningSpeed67_g57 = ( _Secondarypanningspeed * staticSwitch334_g57 );
				#ifdef _SECONDARYTEXTURE_ON
				float staticSwitch158_g57 = ( tex2DNode210_g57.r * tex2Dlod( _SecondaryTex, float4( ( uv2_SecondaryTex + _SecondaryPanningSpeed67_g57 ), 0, 1.0) ).r * 2.0 );
				#else
				float staticSwitch158_g57 = tex2DNode210_g57.r;
				#endif
				float _VertexOffsetAmount162_g57 = _VertexOffsetAmount;
				#ifdef _VERTEXOFFSET_ON
				float3 staticSwitch185_g57 = ( ( (-1.0 + (staticSwitch158_g57 - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) * _VertexOffsetAmount162_g57 ) * v.ase_normal );
				#else
				float3 staticSwitch185_g57 = float3( 0,0,0 );
				#endif
				float3 vertexToFrag188_g57 = staticSwitch185_g57;
				float3 __VertexOffset191_g57 = vertexToFrag188_g57;
				
				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord4 = screenPos;
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord5.xyz = ase_worldNormal;
				
				o.ase_texcoord2 = v.ase_texcoord;
				o.ase_texcoord3.xy = v.ase_texcoord1.xy;
				o.ase_color = v.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord3.zw = 0;
				o.ase_texcoord5.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = __VertexOffset191_g57;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.worldPos = positionWS;
				#endif

				o.clipPos = TransformWorldToHClip( positionWS );
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = o.clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord1 = v.ase_texcoord1;
				o.ase_color = v.ase_color;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = IN.worldPos;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 uv_MainTex = IN.ase_texcoord2.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float2 texCoord294_g57 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_21_0_g57 = (float2( -1,-1 ) + (texCoord294_g57 - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 )));
				float2 break24_g57 = temp_output_21_0_g57;
				float2 appendResult54_g57 = (float2(( ( ( atan2( break24_g57.y , break24_g57.x ) / PI ) / 2.0 ) + 0.5 ) , length( temp_output_21_0_g57 )));
				#ifdef _POLARUV_ON
				float2 staticSwitch63_g57 = ( ( appendResult54_g57 * _MainTex_ST.xy ) + _MainTex_ST.zw );
				#else
				float2 staticSwitch63_g57 = uv_MainTex;
				#endif
				float2 _UVMain66_g57 = staticSwitch63_g57;
				#ifdef _CUSTOMFRAMERATE_ON
				float staticSwitch334_g57 = ( round( ( _TimeParameters.x * _Framerate ) ) / _Framerate );
				#else
				float staticSwitch334_g57 = _TimeParameters.x;
				#endif
				float4 temp_output_17_0_g57 = ( _PanningSpeedXYmaintexZWdisplacementtex * staticSwitch334_g57 );
				float2 _PanningSpeedXY69_g57 = (temp_output_17_0_g57).xy;
				float2 uv_DisplacementGuide = IN.ase_texcoord2.xy * _DisplacementGuide_ST.xy + _DisplacementGuide_ST.zw;
				float2 texCoord301_g57 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_3_0_g57 = (float2( -1,-1 ) + (texCoord301_g57 - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 )));
				float2 break4_g57 = temp_output_3_0_g57;
				float2 appendResult18_g57 = (float2(( ( ( atan2( break4_g57.y , break4_g57.x ) / PI ) / 2.0 ) + 0.5 ) , length( temp_output_3_0_g57 )));
				#ifdef _POLARUV_ON
				float2 staticSwitch195_g57 = ( ( appendResult18_g57 * _DisplacementGuide_ST.xy ) + _DisplacementGuide_ST.zw );
				#else
				float2 staticSwitch195_g57 = uv_DisplacementGuide;
				#endif
				float2 _UVDisplacement33_g57 = staticSwitch195_g57;
				float2 _PanningSpeedZW31_g57 = (temp_output_17_0_g57).zw;
				float4 appendResult22_g57 = (float4(ddx( uv_DisplacementGuide ) , ddy( uv_DisplacementGuide )));
				float4 _DDDisplacement29_g57 = appendResult22_g57;
				#ifdef _PARTICLECONTROLSDISPLACEMENT_ON
				float staticSwitch335_g57 = IN.ase_texcoord2.z;
				#else
				float staticSwitch335_g57 = _DisplacementAmount;
				#endif
				float2 __Displacement77_g57 = ( (float2( -1,-1 ) + ((tex2D( _DisplacementGuide, ( _UVDisplacement33_g57 + _PanningSpeedZW31_g57 ), (_DDDisplacement29_g57).xy, (_DDDisplacement29_g57).zw )).rg - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 ))) * staticSwitch335_g57 );
				float4 appendResult68_g57 = (float4(ddx( uv_MainTex ) , ddy( uv_MainTex )));
				float4 _DDMainTex76_g57 = appendResult68_g57;
				float4 tex2DNode96_g57 = tex2D( _MainTex, ( ( _UVMain66_g57 + _PanningSpeedXY69_g57 ) + __Displacement77_g57 ), (_DDMainTex76_g57).xy, (_DDMainTex76_g57).zw );
				float _Contrast90_g57 = _Contrast;
				float lerpResult98_g57 = lerp( 0.5 , tex2DNode96_g57.r , _Contrast90_g57);
				float _Power97_g57 = _Power;
				float temp_output_106_0_g57 = pow( saturate( lerpResult98_g57 ) , _Power97_g57 );
				float2 uv_SecondaryTex = IN.ase_texcoord2.xy * _SecondaryTex_ST.xy + _SecondaryTex_ST.zw;
				float2 texCoord302_g57 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_20_0_g57 = (float2( -1,-1 ) + (texCoord302_g57 - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 )));
				float2 break26_g57 = temp_output_20_0_g57;
				float2 appendResult53_g57 = (float2(( ( ( atan2( break26_g57.y , break26_g57.x ) / PI ) / 2.0 ) + 0.5 ) , length( temp_output_20_0_g57 )));
				#ifdef _POLARUV_ON
				float2 staticSwitch197_g57 = ( ( appendResult53_g57 * _SecondaryTex_ST.xy ) + _SecondaryTex_ST.zw );
				#else
				float2 staticSwitch197_g57 = uv_SecondaryTex;
				#endif
				float2 _UVSecondary65_g57 = staticSwitch197_g57;
				float2 _SecondaryPanningSpeed67_g57 = ( _Secondarypanningspeed * staticSwitch334_g57 );
				float4 appendResult71_g57 = (float4(ddx( uv_SecondaryTex ) , ddy( uv_SecondaryTex )));
				float4 _DDSecondary75_g57 = appendResult71_g57;
				float lerpResult99_g57 = lerp( 0.5 , tex2D( _SecondaryTex, ( ( _UVSecondary65_g57 + _SecondaryPanningSpeed67_g57 ) + __Displacement77_g57 ), (_DDSecondary75_g57).xy, (_DDSecondary75_g57).zw ).r , _Contrast90_g57);
				#ifdef _SECONDARYTEXTURE_ON
				float staticSwitch198_g57 = ( temp_output_106_0_g57 * pow( saturate( lerpResult99_g57 ) , _Power97_g57 ) * 2.0 );
				#else
				float staticSwitch198_g57 = temp_output_106_0_g57;
				#endif
				float2 texCoord102_g57 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_110_0_g57 = distance( texCoord102_g57 , float2( 0.5,0.5 ) );
				float smoothstepResult113_g57 = smoothstep( _OuterRadius , ( _OuterRadius + _Smoothness ) , temp_output_110_0_g57);
				float smoothstepResult115_g57 = smoothstep( _InnerRadius , ( _InnerRadius + _Smoothness ) , temp_output_110_0_g57);
				#ifdef _CIRCLEMASK_ON
				float staticSwitch119_g57 = ( staticSwitch198_g57 * ( 1.0 - smoothstepResult113_g57 ) * smoothstepResult115_g57 );
				#else
				float staticSwitch119_g57 = staticSwitch198_g57;
				#endif
				float2 texCoord212_g57 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 break217_g57 = (float2( -1,-1 ) + (texCoord212_g57 - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 )));
				float smoothstepResult222_g57 = smoothstep( _RectMaskCutoff , ( _RectMaskCutoff + _RectMaskSmoothness ) , max( abs( ( break217_g57.x / _RectWidth ) ) , abs( ( break217_g57.y / _RectHeight ) ) ));
				#ifdef _RECTMASK_ON
				float staticSwitch211_g57 = ( staticSwitch119_g57 * ( 1.0 - smoothstepResult222_g57 ) );
				#else
				float staticSwitch211_g57 = staticSwitch119_g57;
				#endif
				float __orgCol124_g57 = staticSwitch211_g57;
				#ifdef _BANDING_ON
				float staticSwitch127_g57 = ( round( ( staticSwitch211_g57 * _Numberofbands ) ) / _Numberofbands );
				#else
				float staticSwitch127_g57 = __orgCol124_g57;
				#endif
				float __Col132_g57 = staticSwitch127_g57;
				float2 appendResult141_g57 = (float2(__Col132_g57 , 0.0));
				float4 __RampColor151_g57 = tex2D( _GradientMap, appendResult141_g57 );
				float4 _Color161_g57 = ( IN.ase_color * _Color );
				float4 _BurnCol173_g57 = _BurnColor;
				float _Cutoff130_g57 = _Cutoff;
				#ifdef _VERTEXALPHACUTOFF_ON
				float staticSwitch318_g57 = ( 1.0 - IN.ase_color.a );
				#else
				float staticSwitch318_g57 = 0.0;
				#endif
				float temp_output_150_0_g57 = saturate( ( _Cutoff130_g57 + staticSwitch318_g57 ) );
				float __Cutout237_g57 = temp_output_150_0_g57;
				float temp_output_240_0_g57 = ( __orgCol124_g57 - __Cutout237_g57 );
				float _CutoffSoftness201_g57 = _Cutoffsoftness;
				float _BurnSize168_g57 = _BurnSize;
				float smoothstepResult236_g57 = smoothstep( temp_output_240_0_g57 , ( temp_output_240_0_g57 + _CutoffSoftness201_g57 ) , _BurnSize168_g57);
				float smoothstepResult253_g57 = smoothstep( 0.001 , 0.5 , __Cutout237_g57);
				float3 temp_output_261_0_g57 = (( ( ( ( float4( (__RampColor151_g57).rgb , 0.0 ) * _Color161_g57 ) + ( _BurnCol173_g57 * smoothstepResult236_g57 * smoothstepResult253_g57 ) ) * _Color161_g57 ) * (__RampColor151_g57).a )).rgb;
				float smoothstepResult231_g57 = smoothstep( temp_output_150_0_g57 , ( temp_output_150_0_g57 + _CutoffSoftness201_g57 ) , __orgCol124_g57);
				float _Alpha235_g57 = smoothstepResult231_g57;
				float _MainTexAlpha266_g57 = tex2DNode96_g57.a;
				float temp_output_267_0_g57 = ( _Alpha235_g57 * _MainTexAlpha266_g57 * (_Color161_g57).a );
				float4 screenPos = IN.ase_texcoord4;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float _IntersectionThresholdMax203_g57 = _IntersectionThresholdMax;
				float screenDepth277_g57 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth277_g57 = saturate( abs( ( screenDepth277_g57 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _IntersectionThresholdMax203_g57 ) ) );
				#ifdef _SOFTFADE_ON
				float staticSwitch279_g57 = ( temp_output_267_0_g57 * distanceDepth277_g57 );
				#else
				float staticSwitch279_g57 = temp_output_267_0_g57;
				#endif
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 ase_worldNormal = IN.ase_texcoord5.xyz;
				float fresnelNdotV306_g57 = dot( ase_worldNormal, ase_worldViewDir );
				float fresnelNode306_g57 = ( _FresnelBias + _FresnelScale * pow( 1.0 - fresnelNdotV306_g57, _FresnelPower ) );
				#ifdef _FRESNELALPHA_ON
				float staticSwitch307_g57 = ( staticSwitch279_g57 * fresnelNode306_g57 );
				#else
				float staticSwitch307_g57 = staticSwitch279_g57;
				#endif
				float4 appendResult260_g57 = (float4(temp_output_261_0_g57 , staticSwitch307_g57));
				float4 temp_output_71_0 = appendResult260_g57;
				

				float Alpha = saturate( (temp_output_71_0).w );
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODFadeCrossFade( IN.clipPos );
				#endif
				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
            Name "SceneSelectionPass"
            Tags { "LightMode"="SceneSelectionPass" }

			Cull Off

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_SRP_VERSION 140007
			#define REQUIRE_DEPTH_TEXTURE 1


			#pragma vertex vert
			#pragma fragment frag

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_COLOR
			#pragma shader_feature_local _VERTEXOFFSET_ON
			#pragma shader_feature_local _SECONDARYTEXTURE_ON
			#pragma shader_feature_local _CUSTOMFRAMERATE_ON
			#pragma shader_feature_local _BANDING_ON
			#pragma shader_feature_local _RECTMASK_ON
			#pragma shader_feature_local _CIRCLEMASK_ON
			#pragma shader_feature_local _POLARUV_ON
			#pragma shader_feature_local _PARTICLECONTROLSDISPLACEMENT_ON
			#pragma shader_feature_local _VERTEXALPHACUTOFF_ON
			#pragma shader_feature_local _FRESNELALPHA_ON
			#pragma shader_feature_local _SOFTFADE_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _MainTex_ST;
			float4 _PanningSpeedXYmaintexZWdisplacementtex;
			float4 _SecondaryTex_ST;
			float4 _DisplacementGuide_ST;
			float4 _BurnColor;
			float4 _Color;
			float2 _Secondarypanningspeed;
			float _Culling;
			float _FresnelBias;
			float _IntersectionThresholdMax;
			float _BurnSize;
			float _Cutoffsoftness;
			float _Cutoff;
			float _Numberofbands;
			float _RectHeight;
			float _RectWidth;
			float _InnerRadius;
			float _RectMaskCutoff;
			float _FresnelScale;
			float _Smoothness;
			float _OuterRadius;
			float _Power;
			float _Contrast;
			float _DisplacementAmount;
			float _VertexOffsetAmount;
			float _Framerate;
			float _ZTest;
			float _RectMaskSmoothness;
			float _FresnelPower;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _MainTex;
			sampler2D _SecondaryTex;
			sampler2D _GradientMap;
			sampler2D _DisplacementGuide;
			uniform float4 _CameraDepthTexture_TexelSize;


			
			int _ObjectId;
			int _PassValue;

			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			VertexOutput VertexFunction(VertexInput v  )
			{
				VertexOutput o;
				ZERO_INITIALIZE(VertexOutput, o);

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float2 uv_MainTex = v.ase_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
				#ifdef _CUSTOMFRAMERATE_ON
				float staticSwitch334_g57 = ( round( ( _TimeParameters.x * _Framerate ) ) / _Framerate );
				#else
				float staticSwitch334_g57 = _TimeParameters.x;
				#endif
				float4 temp_output_17_0_g57 = ( _PanningSpeedXYmaintexZWdisplacementtex * staticSwitch334_g57 );
				float2 _PanningSpeedXY69_g57 = (temp_output_17_0_g57).xy;
				float4 tex2DNode210_g57 = tex2Dlod( _MainTex, float4( ( uv_MainTex + _PanningSpeedXY69_g57 ), 0, 1.0) );
				float2 uv2_SecondaryTex = v.ase_texcoord1.xy * _SecondaryTex_ST.xy + _SecondaryTex_ST.zw;
				float2 _SecondaryPanningSpeed67_g57 = ( _Secondarypanningspeed * staticSwitch334_g57 );
				#ifdef _SECONDARYTEXTURE_ON
				float staticSwitch158_g57 = ( tex2DNode210_g57.r * tex2Dlod( _SecondaryTex, float4( ( uv2_SecondaryTex + _SecondaryPanningSpeed67_g57 ), 0, 1.0) ).r * 2.0 );
				#else
				float staticSwitch158_g57 = tex2DNode210_g57.r;
				#endif
				float _VertexOffsetAmount162_g57 = _VertexOffsetAmount;
				#ifdef _VERTEXOFFSET_ON
				float3 staticSwitch185_g57 = ( ( (-1.0 + (staticSwitch158_g57 - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) * _VertexOffsetAmount162_g57 ) * v.ase_normal );
				#else
				float3 staticSwitch185_g57 = float3( 0,0,0 );
				#endif
				float3 vertexToFrag188_g57 = staticSwitch185_g57;
				float3 __VertexOffset191_g57 = vertexToFrag188_g57;
				
				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord2 = screenPos;
				float3 ase_worldPos = TransformObjectToWorld( (v.vertex).xyz );
				o.ase_texcoord3.xyz = ase_worldPos;
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord4.xyz = ase_worldNormal;
				
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord1.xy = v.ase_texcoord1.xy;
				o.ase_color = v.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.zw = 0;
				o.ase_texcoord3.w = 0;
				o.ase_texcoord4.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = __VertexOffset191_g57;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				o.clipPos = TransformWorldToHClip(positionWS);

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord1 = v.ase_texcoord1;
				o.ase_color = v.ase_color;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN ) : SV_TARGET
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float2 uv_MainTex = IN.ase_texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float2 texCoord294_g57 = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_21_0_g57 = (float2( -1,-1 ) + (texCoord294_g57 - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 )));
				float2 break24_g57 = temp_output_21_0_g57;
				float2 appendResult54_g57 = (float2(( ( ( atan2( break24_g57.y , break24_g57.x ) / PI ) / 2.0 ) + 0.5 ) , length( temp_output_21_0_g57 )));
				#ifdef _POLARUV_ON
				float2 staticSwitch63_g57 = ( ( appendResult54_g57 * _MainTex_ST.xy ) + _MainTex_ST.zw );
				#else
				float2 staticSwitch63_g57 = uv_MainTex;
				#endif
				float2 _UVMain66_g57 = staticSwitch63_g57;
				#ifdef _CUSTOMFRAMERATE_ON
				float staticSwitch334_g57 = ( round( ( _TimeParameters.x * _Framerate ) ) / _Framerate );
				#else
				float staticSwitch334_g57 = _TimeParameters.x;
				#endif
				float4 temp_output_17_0_g57 = ( _PanningSpeedXYmaintexZWdisplacementtex * staticSwitch334_g57 );
				float2 _PanningSpeedXY69_g57 = (temp_output_17_0_g57).xy;
				float2 uv_DisplacementGuide = IN.ase_texcoord.xy * _DisplacementGuide_ST.xy + _DisplacementGuide_ST.zw;
				float2 texCoord301_g57 = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_3_0_g57 = (float2( -1,-1 ) + (texCoord301_g57 - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 )));
				float2 break4_g57 = temp_output_3_0_g57;
				float2 appendResult18_g57 = (float2(( ( ( atan2( break4_g57.y , break4_g57.x ) / PI ) / 2.0 ) + 0.5 ) , length( temp_output_3_0_g57 )));
				#ifdef _POLARUV_ON
				float2 staticSwitch195_g57 = ( ( appendResult18_g57 * _DisplacementGuide_ST.xy ) + _DisplacementGuide_ST.zw );
				#else
				float2 staticSwitch195_g57 = uv_DisplacementGuide;
				#endif
				float2 _UVDisplacement33_g57 = staticSwitch195_g57;
				float2 _PanningSpeedZW31_g57 = (temp_output_17_0_g57).zw;
				float4 appendResult22_g57 = (float4(ddx( uv_DisplacementGuide ) , ddy( uv_DisplacementGuide )));
				float4 _DDDisplacement29_g57 = appendResult22_g57;
				#ifdef _PARTICLECONTROLSDISPLACEMENT_ON
				float staticSwitch335_g57 = IN.ase_texcoord.z;
				#else
				float staticSwitch335_g57 = _DisplacementAmount;
				#endif
				float2 __Displacement77_g57 = ( (float2( -1,-1 ) + ((tex2D( _DisplacementGuide, ( _UVDisplacement33_g57 + _PanningSpeedZW31_g57 ), (_DDDisplacement29_g57).xy, (_DDDisplacement29_g57).zw )).rg - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 ))) * staticSwitch335_g57 );
				float4 appendResult68_g57 = (float4(ddx( uv_MainTex ) , ddy( uv_MainTex )));
				float4 _DDMainTex76_g57 = appendResult68_g57;
				float4 tex2DNode96_g57 = tex2D( _MainTex, ( ( _UVMain66_g57 + _PanningSpeedXY69_g57 ) + __Displacement77_g57 ), (_DDMainTex76_g57).xy, (_DDMainTex76_g57).zw );
				float _Contrast90_g57 = _Contrast;
				float lerpResult98_g57 = lerp( 0.5 , tex2DNode96_g57.r , _Contrast90_g57);
				float _Power97_g57 = _Power;
				float temp_output_106_0_g57 = pow( saturate( lerpResult98_g57 ) , _Power97_g57 );
				float2 uv_SecondaryTex = IN.ase_texcoord.xy * _SecondaryTex_ST.xy + _SecondaryTex_ST.zw;
				float2 texCoord302_g57 = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_20_0_g57 = (float2( -1,-1 ) + (texCoord302_g57 - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 )));
				float2 break26_g57 = temp_output_20_0_g57;
				float2 appendResult53_g57 = (float2(( ( ( atan2( break26_g57.y , break26_g57.x ) / PI ) / 2.0 ) + 0.5 ) , length( temp_output_20_0_g57 )));
				#ifdef _POLARUV_ON
				float2 staticSwitch197_g57 = ( ( appendResult53_g57 * _SecondaryTex_ST.xy ) + _SecondaryTex_ST.zw );
				#else
				float2 staticSwitch197_g57 = uv_SecondaryTex;
				#endif
				float2 _UVSecondary65_g57 = staticSwitch197_g57;
				float2 _SecondaryPanningSpeed67_g57 = ( _Secondarypanningspeed * staticSwitch334_g57 );
				float4 appendResult71_g57 = (float4(ddx( uv_SecondaryTex ) , ddy( uv_SecondaryTex )));
				float4 _DDSecondary75_g57 = appendResult71_g57;
				float lerpResult99_g57 = lerp( 0.5 , tex2D( _SecondaryTex, ( ( _UVSecondary65_g57 + _SecondaryPanningSpeed67_g57 ) + __Displacement77_g57 ), (_DDSecondary75_g57).xy, (_DDSecondary75_g57).zw ).r , _Contrast90_g57);
				#ifdef _SECONDARYTEXTURE_ON
				float staticSwitch198_g57 = ( temp_output_106_0_g57 * pow( saturate( lerpResult99_g57 ) , _Power97_g57 ) * 2.0 );
				#else
				float staticSwitch198_g57 = temp_output_106_0_g57;
				#endif
				float2 texCoord102_g57 = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_110_0_g57 = distance( texCoord102_g57 , float2( 0.5,0.5 ) );
				float smoothstepResult113_g57 = smoothstep( _OuterRadius , ( _OuterRadius + _Smoothness ) , temp_output_110_0_g57);
				float smoothstepResult115_g57 = smoothstep( _InnerRadius , ( _InnerRadius + _Smoothness ) , temp_output_110_0_g57);
				#ifdef _CIRCLEMASK_ON
				float staticSwitch119_g57 = ( staticSwitch198_g57 * ( 1.0 - smoothstepResult113_g57 ) * smoothstepResult115_g57 );
				#else
				float staticSwitch119_g57 = staticSwitch198_g57;
				#endif
				float2 texCoord212_g57 = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 break217_g57 = (float2( -1,-1 ) + (texCoord212_g57 - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 )));
				float smoothstepResult222_g57 = smoothstep( _RectMaskCutoff , ( _RectMaskCutoff + _RectMaskSmoothness ) , max( abs( ( break217_g57.x / _RectWidth ) ) , abs( ( break217_g57.y / _RectHeight ) ) ));
				#ifdef _RECTMASK_ON
				float staticSwitch211_g57 = ( staticSwitch119_g57 * ( 1.0 - smoothstepResult222_g57 ) );
				#else
				float staticSwitch211_g57 = staticSwitch119_g57;
				#endif
				float __orgCol124_g57 = staticSwitch211_g57;
				#ifdef _BANDING_ON
				float staticSwitch127_g57 = ( round( ( staticSwitch211_g57 * _Numberofbands ) ) / _Numberofbands );
				#else
				float staticSwitch127_g57 = __orgCol124_g57;
				#endif
				float __Col132_g57 = staticSwitch127_g57;
				float2 appendResult141_g57 = (float2(__Col132_g57 , 0.0));
				float4 __RampColor151_g57 = tex2D( _GradientMap, appendResult141_g57 );
				float4 _Color161_g57 = ( IN.ase_color * _Color );
				float4 _BurnCol173_g57 = _BurnColor;
				float _Cutoff130_g57 = _Cutoff;
				#ifdef _VERTEXALPHACUTOFF_ON
				float staticSwitch318_g57 = ( 1.0 - IN.ase_color.a );
				#else
				float staticSwitch318_g57 = 0.0;
				#endif
				float temp_output_150_0_g57 = saturate( ( _Cutoff130_g57 + staticSwitch318_g57 ) );
				float __Cutout237_g57 = temp_output_150_0_g57;
				float temp_output_240_0_g57 = ( __orgCol124_g57 - __Cutout237_g57 );
				float _CutoffSoftness201_g57 = _Cutoffsoftness;
				float _BurnSize168_g57 = _BurnSize;
				float smoothstepResult236_g57 = smoothstep( temp_output_240_0_g57 , ( temp_output_240_0_g57 + _CutoffSoftness201_g57 ) , _BurnSize168_g57);
				float smoothstepResult253_g57 = smoothstep( 0.001 , 0.5 , __Cutout237_g57);
				float3 temp_output_261_0_g57 = (( ( ( ( float4( (__RampColor151_g57).rgb , 0.0 ) * _Color161_g57 ) + ( _BurnCol173_g57 * smoothstepResult236_g57 * smoothstepResult253_g57 ) ) * _Color161_g57 ) * (__RampColor151_g57).a )).rgb;
				float smoothstepResult231_g57 = smoothstep( temp_output_150_0_g57 , ( temp_output_150_0_g57 + _CutoffSoftness201_g57 ) , __orgCol124_g57);
				float _Alpha235_g57 = smoothstepResult231_g57;
				float _MainTexAlpha266_g57 = tex2DNode96_g57.a;
				float temp_output_267_0_g57 = ( _Alpha235_g57 * _MainTexAlpha266_g57 * (_Color161_g57).a );
				float4 screenPos = IN.ase_texcoord2;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float _IntersectionThresholdMax203_g57 = _IntersectionThresholdMax;
				float screenDepth277_g57 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth277_g57 = saturate( abs( ( screenDepth277_g57 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _IntersectionThresholdMax203_g57 ) ) );
				#ifdef _SOFTFADE_ON
				float staticSwitch279_g57 = ( temp_output_267_0_g57 * distanceDepth277_g57 );
				#else
				float staticSwitch279_g57 = temp_output_267_0_g57;
				#endif
				float3 ase_worldPos = IN.ase_texcoord3.xyz;
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - ase_worldPos );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 ase_worldNormal = IN.ase_texcoord4.xyz;
				float fresnelNdotV306_g57 = dot( ase_worldNormal, ase_worldViewDir );
				float fresnelNode306_g57 = ( _FresnelBias + _FresnelScale * pow( 1.0 - fresnelNdotV306_g57, _FresnelPower ) );
				#ifdef _FRESNELALPHA_ON
				float staticSwitch307_g57 = ( staticSwitch279_g57 * fresnelNode306_g57 );
				#else
				float staticSwitch307_g57 = staticSwitch279_g57;
				#endif
				float4 appendResult260_g57 = (float4(temp_output_261_0_g57 , staticSwitch307_g57));
				float4 temp_output_71_0 = appendResult260_g57;
				

				surfaceDescription.Alpha = saturate( (temp_output_71_0).w );
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
					clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				half4 outColor = half4(_ObjectId, _PassValue, 1.0, 1.0);
				return outColor;
			}
			ENDHLSL
		}

		
		Pass
		{
			
            Name "ScenePickingPass"
            Tags { "LightMode"="Picking" }

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_SRP_VERSION 140007
			#define REQUIRE_DEPTH_TEXTURE 1


			#pragma vertex vert
			#pragma fragment frag

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_COLOR
			#pragma shader_feature_local _VERTEXOFFSET_ON
			#pragma shader_feature_local _SECONDARYTEXTURE_ON
			#pragma shader_feature_local _CUSTOMFRAMERATE_ON
			#pragma shader_feature_local _BANDING_ON
			#pragma shader_feature_local _RECTMASK_ON
			#pragma shader_feature_local _CIRCLEMASK_ON
			#pragma shader_feature_local _POLARUV_ON
			#pragma shader_feature_local _PARTICLECONTROLSDISPLACEMENT_ON
			#pragma shader_feature_local _VERTEXALPHACUTOFF_ON
			#pragma shader_feature_local _FRESNELALPHA_ON
			#pragma shader_feature_local _SOFTFADE_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _MainTex_ST;
			float4 _PanningSpeedXYmaintexZWdisplacementtex;
			float4 _SecondaryTex_ST;
			float4 _DisplacementGuide_ST;
			float4 _BurnColor;
			float4 _Color;
			float2 _Secondarypanningspeed;
			float _Culling;
			float _FresnelBias;
			float _IntersectionThresholdMax;
			float _BurnSize;
			float _Cutoffsoftness;
			float _Cutoff;
			float _Numberofbands;
			float _RectHeight;
			float _RectWidth;
			float _InnerRadius;
			float _RectMaskCutoff;
			float _FresnelScale;
			float _Smoothness;
			float _OuterRadius;
			float _Power;
			float _Contrast;
			float _DisplacementAmount;
			float _VertexOffsetAmount;
			float _Framerate;
			float _ZTest;
			float _RectMaskSmoothness;
			float _FresnelPower;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _MainTex;
			sampler2D _SecondaryTex;
			sampler2D _GradientMap;
			sampler2D _DisplacementGuide;
			uniform float4 _CameraDepthTexture_TexelSize;


			
			float4 _SelectionID;


			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			VertexOutput VertexFunction(VertexInput v  )
			{
				VertexOutput o;
				ZERO_INITIALIZE(VertexOutput, o);

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float2 uv_MainTex = v.ase_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
				#ifdef _CUSTOMFRAMERATE_ON
				float staticSwitch334_g57 = ( round( ( _TimeParameters.x * _Framerate ) ) / _Framerate );
				#else
				float staticSwitch334_g57 = _TimeParameters.x;
				#endif
				float4 temp_output_17_0_g57 = ( _PanningSpeedXYmaintexZWdisplacementtex * staticSwitch334_g57 );
				float2 _PanningSpeedXY69_g57 = (temp_output_17_0_g57).xy;
				float4 tex2DNode210_g57 = tex2Dlod( _MainTex, float4( ( uv_MainTex + _PanningSpeedXY69_g57 ), 0, 1.0) );
				float2 uv2_SecondaryTex = v.ase_texcoord1.xy * _SecondaryTex_ST.xy + _SecondaryTex_ST.zw;
				float2 _SecondaryPanningSpeed67_g57 = ( _Secondarypanningspeed * staticSwitch334_g57 );
				#ifdef _SECONDARYTEXTURE_ON
				float staticSwitch158_g57 = ( tex2DNode210_g57.r * tex2Dlod( _SecondaryTex, float4( ( uv2_SecondaryTex + _SecondaryPanningSpeed67_g57 ), 0, 1.0) ).r * 2.0 );
				#else
				float staticSwitch158_g57 = tex2DNode210_g57.r;
				#endif
				float _VertexOffsetAmount162_g57 = _VertexOffsetAmount;
				#ifdef _VERTEXOFFSET_ON
				float3 staticSwitch185_g57 = ( ( (-1.0 + (staticSwitch158_g57 - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) * _VertexOffsetAmount162_g57 ) * v.ase_normal );
				#else
				float3 staticSwitch185_g57 = float3( 0,0,0 );
				#endif
				float3 vertexToFrag188_g57 = staticSwitch185_g57;
				float3 __VertexOffset191_g57 = vertexToFrag188_g57;
				
				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord2 = screenPos;
				float3 ase_worldPos = TransformObjectToWorld( (v.vertex).xyz );
				o.ase_texcoord3.xyz = ase_worldPos;
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord4.xyz = ase_worldNormal;
				
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord1.xy = v.ase_texcoord1.xy;
				o.ase_color = v.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.zw = 0;
				o.ase_texcoord3.w = 0;
				o.ase_texcoord4.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = __VertexOffset191_g57;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				o.clipPos = TransformWorldToHClip(positionWS);
				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord1 = v.ase_texcoord1;
				o.ase_color = v.ase_color;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN ) : SV_TARGET
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float2 uv_MainTex = IN.ase_texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float2 texCoord294_g57 = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_21_0_g57 = (float2( -1,-1 ) + (texCoord294_g57 - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 )));
				float2 break24_g57 = temp_output_21_0_g57;
				float2 appendResult54_g57 = (float2(( ( ( atan2( break24_g57.y , break24_g57.x ) / PI ) / 2.0 ) + 0.5 ) , length( temp_output_21_0_g57 )));
				#ifdef _POLARUV_ON
				float2 staticSwitch63_g57 = ( ( appendResult54_g57 * _MainTex_ST.xy ) + _MainTex_ST.zw );
				#else
				float2 staticSwitch63_g57 = uv_MainTex;
				#endif
				float2 _UVMain66_g57 = staticSwitch63_g57;
				#ifdef _CUSTOMFRAMERATE_ON
				float staticSwitch334_g57 = ( round( ( _TimeParameters.x * _Framerate ) ) / _Framerate );
				#else
				float staticSwitch334_g57 = _TimeParameters.x;
				#endif
				float4 temp_output_17_0_g57 = ( _PanningSpeedXYmaintexZWdisplacementtex * staticSwitch334_g57 );
				float2 _PanningSpeedXY69_g57 = (temp_output_17_0_g57).xy;
				float2 uv_DisplacementGuide = IN.ase_texcoord.xy * _DisplacementGuide_ST.xy + _DisplacementGuide_ST.zw;
				float2 texCoord301_g57 = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_3_0_g57 = (float2( -1,-1 ) + (texCoord301_g57 - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 )));
				float2 break4_g57 = temp_output_3_0_g57;
				float2 appendResult18_g57 = (float2(( ( ( atan2( break4_g57.y , break4_g57.x ) / PI ) / 2.0 ) + 0.5 ) , length( temp_output_3_0_g57 )));
				#ifdef _POLARUV_ON
				float2 staticSwitch195_g57 = ( ( appendResult18_g57 * _DisplacementGuide_ST.xy ) + _DisplacementGuide_ST.zw );
				#else
				float2 staticSwitch195_g57 = uv_DisplacementGuide;
				#endif
				float2 _UVDisplacement33_g57 = staticSwitch195_g57;
				float2 _PanningSpeedZW31_g57 = (temp_output_17_0_g57).zw;
				float4 appendResult22_g57 = (float4(ddx( uv_DisplacementGuide ) , ddy( uv_DisplacementGuide )));
				float4 _DDDisplacement29_g57 = appendResult22_g57;
				#ifdef _PARTICLECONTROLSDISPLACEMENT_ON
				float staticSwitch335_g57 = IN.ase_texcoord.z;
				#else
				float staticSwitch335_g57 = _DisplacementAmount;
				#endif
				float2 __Displacement77_g57 = ( (float2( -1,-1 ) + ((tex2D( _DisplacementGuide, ( _UVDisplacement33_g57 + _PanningSpeedZW31_g57 ), (_DDDisplacement29_g57).xy, (_DDDisplacement29_g57).zw )).rg - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 ))) * staticSwitch335_g57 );
				float4 appendResult68_g57 = (float4(ddx( uv_MainTex ) , ddy( uv_MainTex )));
				float4 _DDMainTex76_g57 = appendResult68_g57;
				float4 tex2DNode96_g57 = tex2D( _MainTex, ( ( _UVMain66_g57 + _PanningSpeedXY69_g57 ) + __Displacement77_g57 ), (_DDMainTex76_g57).xy, (_DDMainTex76_g57).zw );
				float _Contrast90_g57 = _Contrast;
				float lerpResult98_g57 = lerp( 0.5 , tex2DNode96_g57.r , _Contrast90_g57);
				float _Power97_g57 = _Power;
				float temp_output_106_0_g57 = pow( saturate( lerpResult98_g57 ) , _Power97_g57 );
				float2 uv_SecondaryTex = IN.ase_texcoord.xy * _SecondaryTex_ST.xy + _SecondaryTex_ST.zw;
				float2 texCoord302_g57 = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_20_0_g57 = (float2( -1,-1 ) + (texCoord302_g57 - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 )));
				float2 break26_g57 = temp_output_20_0_g57;
				float2 appendResult53_g57 = (float2(( ( ( atan2( break26_g57.y , break26_g57.x ) / PI ) / 2.0 ) + 0.5 ) , length( temp_output_20_0_g57 )));
				#ifdef _POLARUV_ON
				float2 staticSwitch197_g57 = ( ( appendResult53_g57 * _SecondaryTex_ST.xy ) + _SecondaryTex_ST.zw );
				#else
				float2 staticSwitch197_g57 = uv_SecondaryTex;
				#endif
				float2 _UVSecondary65_g57 = staticSwitch197_g57;
				float2 _SecondaryPanningSpeed67_g57 = ( _Secondarypanningspeed * staticSwitch334_g57 );
				float4 appendResult71_g57 = (float4(ddx( uv_SecondaryTex ) , ddy( uv_SecondaryTex )));
				float4 _DDSecondary75_g57 = appendResult71_g57;
				float lerpResult99_g57 = lerp( 0.5 , tex2D( _SecondaryTex, ( ( _UVSecondary65_g57 + _SecondaryPanningSpeed67_g57 ) + __Displacement77_g57 ), (_DDSecondary75_g57).xy, (_DDSecondary75_g57).zw ).r , _Contrast90_g57);
				#ifdef _SECONDARYTEXTURE_ON
				float staticSwitch198_g57 = ( temp_output_106_0_g57 * pow( saturate( lerpResult99_g57 ) , _Power97_g57 ) * 2.0 );
				#else
				float staticSwitch198_g57 = temp_output_106_0_g57;
				#endif
				float2 texCoord102_g57 = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_110_0_g57 = distance( texCoord102_g57 , float2( 0.5,0.5 ) );
				float smoothstepResult113_g57 = smoothstep( _OuterRadius , ( _OuterRadius + _Smoothness ) , temp_output_110_0_g57);
				float smoothstepResult115_g57 = smoothstep( _InnerRadius , ( _InnerRadius + _Smoothness ) , temp_output_110_0_g57);
				#ifdef _CIRCLEMASK_ON
				float staticSwitch119_g57 = ( staticSwitch198_g57 * ( 1.0 - smoothstepResult113_g57 ) * smoothstepResult115_g57 );
				#else
				float staticSwitch119_g57 = staticSwitch198_g57;
				#endif
				float2 texCoord212_g57 = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 break217_g57 = (float2( -1,-1 ) + (texCoord212_g57 - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 )));
				float smoothstepResult222_g57 = smoothstep( _RectMaskCutoff , ( _RectMaskCutoff + _RectMaskSmoothness ) , max( abs( ( break217_g57.x / _RectWidth ) ) , abs( ( break217_g57.y / _RectHeight ) ) ));
				#ifdef _RECTMASK_ON
				float staticSwitch211_g57 = ( staticSwitch119_g57 * ( 1.0 - smoothstepResult222_g57 ) );
				#else
				float staticSwitch211_g57 = staticSwitch119_g57;
				#endif
				float __orgCol124_g57 = staticSwitch211_g57;
				#ifdef _BANDING_ON
				float staticSwitch127_g57 = ( round( ( staticSwitch211_g57 * _Numberofbands ) ) / _Numberofbands );
				#else
				float staticSwitch127_g57 = __orgCol124_g57;
				#endif
				float __Col132_g57 = staticSwitch127_g57;
				float2 appendResult141_g57 = (float2(__Col132_g57 , 0.0));
				float4 __RampColor151_g57 = tex2D( _GradientMap, appendResult141_g57 );
				float4 _Color161_g57 = ( IN.ase_color * _Color );
				float4 _BurnCol173_g57 = _BurnColor;
				float _Cutoff130_g57 = _Cutoff;
				#ifdef _VERTEXALPHACUTOFF_ON
				float staticSwitch318_g57 = ( 1.0 - IN.ase_color.a );
				#else
				float staticSwitch318_g57 = 0.0;
				#endif
				float temp_output_150_0_g57 = saturate( ( _Cutoff130_g57 + staticSwitch318_g57 ) );
				float __Cutout237_g57 = temp_output_150_0_g57;
				float temp_output_240_0_g57 = ( __orgCol124_g57 - __Cutout237_g57 );
				float _CutoffSoftness201_g57 = _Cutoffsoftness;
				float _BurnSize168_g57 = _BurnSize;
				float smoothstepResult236_g57 = smoothstep( temp_output_240_0_g57 , ( temp_output_240_0_g57 + _CutoffSoftness201_g57 ) , _BurnSize168_g57);
				float smoothstepResult253_g57 = smoothstep( 0.001 , 0.5 , __Cutout237_g57);
				float3 temp_output_261_0_g57 = (( ( ( ( float4( (__RampColor151_g57).rgb , 0.0 ) * _Color161_g57 ) + ( _BurnCol173_g57 * smoothstepResult236_g57 * smoothstepResult253_g57 ) ) * _Color161_g57 ) * (__RampColor151_g57).a )).rgb;
				float smoothstepResult231_g57 = smoothstep( temp_output_150_0_g57 , ( temp_output_150_0_g57 + _CutoffSoftness201_g57 ) , __orgCol124_g57);
				float _Alpha235_g57 = smoothstepResult231_g57;
				float _MainTexAlpha266_g57 = tex2DNode96_g57.a;
				float temp_output_267_0_g57 = ( _Alpha235_g57 * _MainTexAlpha266_g57 * (_Color161_g57).a );
				float4 screenPos = IN.ase_texcoord2;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float _IntersectionThresholdMax203_g57 = _IntersectionThresholdMax;
				float screenDepth277_g57 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth277_g57 = saturate( abs( ( screenDepth277_g57 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _IntersectionThresholdMax203_g57 ) ) );
				#ifdef _SOFTFADE_ON
				float staticSwitch279_g57 = ( temp_output_267_0_g57 * distanceDepth277_g57 );
				#else
				float staticSwitch279_g57 = temp_output_267_0_g57;
				#endif
				float3 ase_worldPos = IN.ase_texcoord3.xyz;
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - ase_worldPos );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 ase_worldNormal = IN.ase_texcoord4.xyz;
				float fresnelNdotV306_g57 = dot( ase_worldNormal, ase_worldViewDir );
				float fresnelNode306_g57 = ( _FresnelBias + _FresnelScale * pow( 1.0 - fresnelNdotV306_g57, _FresnelPower ) );
				#ifdef _FRESNELALPHA_ON
				float staticSwitch307_g57 = ( staticSwitch279_g57 * fresnelNode306_g57 );
				#else
				float staticSwitch307_g57 = staticSwitch279_g57;
				#endif
				float4 appendResult260_g57 = (float4(temp_output_261_0_g57 , staticSwitch307_g57));
				float4 temp_output_71_0 = appendResult260_g57;
				

				surfaceDescription.Alpha = saturate( (temp_output_71_0).w );
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
					clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				half4 outColor = 0;
				outColor = _SelectionID;

				return outColor;
			}

			ENDHLSL
		}

		
		Pass
		{
			
            Name "DepthNormals"
            Tags { "LightMode"="DepthNormalsOnly" }

			ZTest LEqual
			ZWrite On


			HLSLPROGRAM

			#pragma multi_compile_instancing
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_SRP_VERSION 140007
			#define REQUIRE_DEPTH_TEXTURE 1


			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile_fragment _ _WRITE_RENDERING_LAYERS
        	#pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define VARYINGS_NEED_NORMAL_WS

			#define SHADERPASS SHADERPASS_DEPTHNORMALSONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_COLOR
			#pragma shader_feature_local _VERTEXOFFSET_ON
			#pragma shader_feature_local _SECONDARYTEXTURE_ON
			#pragma shader_feature_local _CUSTOMFRAMERATE_ON
			#pragma shader_feature_local _BANDING_ON
			#pragma shader_feature_local _RECTMASK_ON
			#pragma shader_feature_local _CIRCLEMASK_ON
			#pragma shader_feature_local _POLARUV_ON
			#pragma shader_feature_local _PARTICLECONTROLSDISPLACEMENT_ON
			#pragma shader_feature_local _VERTEXALPHACUTOFF_ON
			#pragma shader_feature_local _FRESNELALPHA_ON
			#pragma shader_feature_local _SOFTFADE_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float3 normalWS : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_color : COLOR;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _MainTex_ST;
			float4 _PanningSpeedXYmaintexZWdisplacementtex;
			float4 _SecondaryTex_ST;
			float4 _DisplacementGuide_ST;
			float4 _BurnColor;
			float4 _Color;
			float2 _Secondarypanningspeed;
			float _Culling;
			float _FresnelBias;
			float _IntersectionThresholdMax;
			float _BurnSize;
			float _Cutoffsoftness;
			float _Cutoff;
			float _Numberofbands;
			float _RectHeight;
			float _RectWidth;
			float _InnerRadius;
			float _RectMaskCutoff;
			float _FresnelScale;
			float _Smoothness;
			float _OuterRadius;
			float _Power;
			float _Contrast;
			float _DisplacementAmount;
			float _VertexOffsetAmount;
			float _Framerate;
			float _ZTest;
			float _RectMaskSmoothness;
			float _FresnelPower;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _MainTex;
			sampler2D _SecondaryTex;
			sampler2D _GradientMap;
			sampler2D _DisplacementGuide;
			uniform float4 _CameraDepthTexture_TexelSize;


			
			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			VertexOutput VertexFunction(VertexInput v  )
			{
				VertexOutput o;
				ZERO_INITIALIZE(VertexOutput, o);

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float2 uv_MainTex = v.ase_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
				#ifdef _CUSTOMFRAMERATE_ON
				float staticSwitch334_g57 = ( round( ( _TimeParameters.x * _Framerate ) ) / _Framerate );
				#else
				float staticSwitch334_g57 = _TimeParameters.x;
				#endif
				float4 temp_output_17_0_g57 = ( _PanningSpeedXYmaintexZWdisplacementtex * staticSwitch334_g57 );
				float2 _PanningSpeedXY69_g57 = (temp_output_17_0_g57).xy;
				float4 tex2DNode210_g57 = tex2Dlod( _MainTex, float4( ( uv_MainTex + _PanningSpeedXY69_g57 ), 0, 1.0) );
				float2 uv2_SecondaryTex = v.ase_texcoord1.xy * _SecondaryTex_ST.xy + _SecondaryTex_ST.zw;
				float2 _SecondaryPanningSpeed67_g57 = ( _Secondarypanningspeed * staticSwitch334_g57 );
				#ifdef _SECONDARYTEXTURE_ON
				float staticSwitch158_g57 = ( tex2DNode210_g57.r * tex2Dlod( _SecondaryTex, float4( ( uv2_SecondaryTex + _SecondaryPanningSpeed67_g57 ), 0, 1.0) ).r * 2.0 );
				#else
				float staticSwitch158_g57 = tex2DNode210_g57.r;
				#endif
				float _VertexOffsetAmount162_g57 = _VertexOffsetAmount;
				#ifdef _VERTEXOFFSET_ON
				float3 staticSwitch185_g57 = ( ( (-1.0 + (staticSwitch158_g57 - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) * _VertexOffsetAmount162_g57 ) * v.ase_normal );
				#else
				float3 staticSwitch185_g57 = float3( 0,0,0 );
				#endif
				float3 vertexToFrag188_g57 = staticSwitch185_g57;
				float3 __VertexOffset191_g57 = vertexToFrag188_g57;
				
				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord3 = screenPos;
				float3 ase_worldPos = TransformObjectToWorld( (v.vertex).xyz );
				o.ase_texcoord4.xyz = ase_worldPos;
				
				o.ase_texcoord1 = v.ase_texcoord;
				o.ase_texcoord2.xy = v.ase_texcoord1.xy;
				o.ase_color = v.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.zw = 0;
				o.ase_texcoord4.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = __VertexOffset191_g57;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 normalWS = TransformObjectToWorldNormal(v.ase_normal);

				o.clipPos = TransformWorldToHClip(positionWS);
				o.normalWS.xyz =  normalWS;

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord1 = v.ase_texcoord1;
				o.ase_color = v.ase_color;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			void frag( VertexOutput IN
				, out half4 outNormalWS : SV_Target0
			#ifdef _WRITE_RENDERING_LAYERS
				, out float4 outRenderingLayers : SV_Target1
			#endif
				 )
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float2 uv_MainTex = IN.ase_texcoord1.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float2 texCoord294_g57 = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_21_0_g57 = (float2( -1,-1 ) + (texCoord294_g57 - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 )));
				float2 break24_g57 = temp_output_21_0_g57;
				float2 appendResult54_g57 = (float2(( ( ( atan2( break24_g57.y , break24_g57.x ) / PI ) / 2.0 ) + 0.5 ) , length( temp_output_21_0_g57 )));
				#ifdef _POLARUV_ON
				float2 staticSwitch63_g57 = ( ( appendResult54_g57 * _MainTex_ST.xy ) + _MainTex_ST.zw );
				#else
				float2 staticSwitch63_g57 = uv_MainTex;
				#endif
				float2 _UVMain66_g57 = staticSwitch63_g57;
				#ifdef _CUSTOMFRAMERATE_ON
				float staticSwitch334_g57 = ( round( ( _TimeParameters.x * _Framerate ) ) / _Framerate );
				#else
				float staticSwitch334_g57 = _TimeParameters.x;
				#endif
				float4 temp_output_17_0_g57 = ( _PanningSpeedXYmaintexZWdisplacementtex * staticSwitch334_g57 );
				float2 _PanningSpeedXY69_g57 = (temp_output_17_0_g57).xy;
				float2 uv_DisplacementGuide = IN.ase_texcoord1.xy * _DisplacementGuide_ST.xy + _DisplacementGuide_ST.zw;
				float2 texCoord301_g57 = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_3_0_g57 = (float2( -1,-1 ) + (texCoord301_g57 - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 )));
				float2 break4_g57 = temp_output_3_0_g57;
				float2 appendResult18_g57 = (float2(( ( ( atan2( break4_g57.y , break4_g57.x ) / PI ) / 2.0 ) + 0.5 ) , length( temp_output_3_0_g57 )));
				#ifdef _POLARUV_ON
				float2 staticSwitch195_g57 = ( ( appendResult18_g57 * _DisplacementGuide_ST.xy ) + _DisplacementGuide_ST.zw );
				#else
				float2 staticSwitch195_g57 = uv_DisplacementGuide;
				#endif
				float2 _UVDisplacement33_g57 = staticSwitch195_g57;
				float2 _PanningSpeedZW31_g57 = (temp_output_17_0_g57).zw;
				float4 appendResult22_g57 = (float4(ddx( uv_DisplacementGuide ) , ddy( uv_DisplacementGuide )));
				float4 _DDDisplacement29_g57 = appendResult22_g57;
				#ifdef _PARTICLECONTROLSDISPLACEMENT_ON
				float staticSwitch335_g57 = IN.ase_texcoord1.z;
				#else
				float staticSwitch335_g57 = _DisplacementAmount;
				#endif
				float2 __Displacement77_g57 = ( (float2( -1,-1 ) + ((tex2D( _DisplacementGuide, ( _UVDisplacement33_g57 + _PanningSpeedZW31_g57 ), (_DDDisplacement29_g57).xy, (_DDDisplacement29_g57).zw )).rg - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 ))) * staticSwitch335_g57 );
				float4 appendResult68_g57 = (float4(ddx( uv_MainTex ) , ddy( uv_MainTex )));
				float4 _DDMainTex76_g57 = appendResult68_g57;
				float4 tex2DNode96_g57 = tex2D( _MainTex, ( ( _UVMain66_g57 + _PanningSpeedXY69_g57 ) + __Displacement77_g57 ), (_DDMainTex76_g57).xy, (_DDMainTex76_g57).zw );
				float _Contrast90_g57 = _Contrast;
				float lerpResult98_g57 = lerp( 0.5 , tex2DNode96_g57.r , _Contrast90_g57);
				float _Power97_g57 = _Power;
				float temp_output_106_0_g57 = pow( saturate( lerpResult98_g57 ) , _Power97_g57 );
				float2 uv_SecondaryTex = IN.ase_texcoord1.xy * _SecondaryTex_ST.xy + _SecondaryTex_ST.zw;
				float2 texCoord302_g57 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_20_0_g57 = (float2( -1,-1 ) + (texCoord302_g57 - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 )));
				float2 break26_g57 = temp_output_20_0_g57;
				float2 appendResult53_g57 = (float2(( ( ( atan2( break26_g57.y , break26_g57.x ) / PI ) / 2.0 ) + 0.5 ) , length( temp_output_20_0_g57 )));
				#ifdef _POLARUV_ON
				float2 staticSwitch197_g57 = ( ( appendResult53_g57 * _SecondaryTex_ST.xy ) + _SecondaryTex_ST.zw );
				#else
				float2 staticSwitch197_g57 = uv_SecondaryTex;
				#endif
				float2 _UVSecondary65_g57 = staticSwitch197_g57;
				float2 _SecondaryPanningSpeed67_g57 = ( _Secondarypanningspeed * staticSwitch334_g57 );
				float4 appendResult71_g57 = (float4(ddx( uv_SecondaryTex ) , ddy( uv_SecondaryTex )));
				float4 _DDSecondary75_g57 = appendResult71_g57;
				float lerpResult99_g57 = lerp( 0.5 , tex2D( _SecondaryTex, ( ( _UVSecondary65_g57 + _SecondaryPanningSpeed67_g57 ) + __Displacement77_g57 ), (_DDSecondary75_g57).xy, (_DDSecondary75_g57).zw ).r , _Contrast90_g57);
				#ifdef _SECONDARYTEXTURE_ON
				float staticSwitch198_g57 = ( temp_output_106_0_g57 * pow( saturate( lerpResult99_g57 ) , _Power97_g57 ) * 2.0 );
				#else
				float staticSwitch198_g57 = temp_output_106_0_g57;
				#endif
				float2 texCoord102_g57 = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_110_0_g57 = distance( texCoord102_g57 , float2( 0.5,0.5 ) );
				float smoothstepResult113_g57 = smoothstep( _OuterRadius , ( _OuterRadius + _Smoothness ) , temp_output_110_0_g57);
				float smoothstepResult115_g57 = smoothstep( _InnerRadius , ( _InnerRadius + _Smoothness ) , temp_output_110_0_g57);
				#ifdef _CIRCLEMASK_ON
				float staticSwitch119_g57 = ( staticSwitch198_g57 * ( 1.0 - smoothstepResult113_g57 ) * smoothstepResult115_g57 );
				#else
				float staticSwitch119_g57 = staticSwitch198_g57;
				#endif
				float2 texCoord212_g57 = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 break217_g57 = (float2( -1,-1 ) + (texCoord212_g57 - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 )));
				float smoothstepResult222_g57 = smoothstep( _RectMaskCutoff , ( _RectMaskCutoff + _RectMaskSmoothness ) , max( abs( ( break217_g57.x / _RectWidth ) ) , abs( ( break217_g57.y / _RectHeight ) ) ));
				#ifdef _RECTMASK_ON
				float staticSwitch211_g57 = ( staticSwitch119_g57 * ( 1.0 - smoothstepResult222_g57 ) );
				#else
				float staticSwitch211_g57 = staticSwitch119_g57;
				#endif
				float __orgCol124_g57 = staticSwitch211_g57;
				#ifdef _BANDING_ON
				float staticSwitch127_g57 = ( round( ( staticSwitch211_g57 * _Numberofbands ) ) / _Numberofbands );
				#else
				float staticSwitch127_g57 = __orgCol124_g57;
				#endif
				float __Col132_g57 = staticSwitch127_g57;
				float2 appendResult141_g57 = (float2(__Col132_g57 , 0.0));
				float4 __RampColor151_g57 = tex2D( _GradientMap, appendResult141_g57 );
				float4 _Color161_g57 = ( IN.ase_color * _Color );
				float4 _BurnCol173_g57 = _BurnColor;
				float _Cutoff130_g57 = _Cutoff;
				#ifdef _VERTEXALPHACUTOFF_ON
				float staticSwitch318_g57 = ( 1.0 - IN.ase_color.a );
				#else
				float staticSwitch318_g57 = 0.0;
				#endif
				float temp_output_150_0_g57 = saturate( ( _Cutoff130_g57 + staticSwitch318_g57 ) );
				float __Cutout237_g57 = temp_output_150_0_g57;
				float temp_output_240_0_g57 = ( __orgCol124_g57 - __Cutout237_g57 );
				float _CutoffSoftness201_g57 = _Cutoffsoftness;
				float _BurnSize168_g57 = _BurnSize;
				float smoothstepResult236_g57 = smoothstep( temp_output_240_0_g57 , ( temp_output_240_0_g57 + _CutoffSoftness201_g57 ) , _BurnSize168_g57);
				float smoothstepResult253_g57 = smoothstep( 0.001 , 0.5 , __Cutout237_g57);
				float3 temp_output_261_0_g57 = (( ( ( ( float4( (__RampColor151_g57).rgb , 0.0 ) * _Color161_g57 ) + ( _BurnCol173_g57 * smoothstepResult236_g57 * smoothstepResult253_g57 ) ) * _Color161_g57 ) * (__RampColor151_g57).a )).rgb;
				float smoothstepResult231_g57 = smoothstep( temp_output_150_0_g57 , ( temp_output_150_0_g57 + _CutoffSoftness201_g57 ) , __orgCol124_g57);
				float _Alpha235_g57 = smoothstepResult231_g57;
				float _MainTexAlpha266_g57 = tex2DNode96_g57.a;
				float temp_output_267_0_g57 = ( _Alpha235_g57 * _MainTexAlpha266_g57 * (_Color161_g57).a );
				float4 screenPos = IN.ase_texcoord3;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float _IntersectionThresholdMax203_g57 = _IntersectionThresholdMax;
				float screenDepth277_g57 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth277_g57 = saturate( abs( ( screenDepth277_g57 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _IntersectionThresholdMax203_g57 ) ) );
				#ifdef _SOFTFADE_ON
				float staticSwitch279_g57 = ( temp_output_267_0_g57 * distanceDepth277_g57 );
				#else
				float staticSwitch279_g57 = temp_output_267_0_g57;
				#endif
				float3 ase_worldPos = IN.ase_texcoord4.xyz;
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - ase_worldPos );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float fresnelNdotV306_g57 = dot( IN.normalWS, ase_worldViewDir );
				float fresnelNode306_g57 = ( _FresnelBias + _FresnelScale * pow( 1.0 - fresnelNdotV306_g57, _FresnelPower ) );
				#ifdef _FRESNELALPHA_ON
				float staticSwitch307_g57 = ( staticSwitch279_g57 * fresnelNode306_g57 );
				#else
				float staticSwitch307_g57 = staticSwitch279_g57;
				#endif
				float4 appendResult260_g57 = (float4(temp_output_261_0_g57 , staticSwitch307_g57));
				float4 temp_output_71_0 = appendResult260_g57;
				

				surfaceDescription.Alpha = saturate( (temp_output_71_0).w );
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					clip(surfaceDescription.Alpha - surfaceDescription.AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODFadeCrossFade( IN.clipPos );
				#endif

				#if defined(_GBUFFER_NORMALS_OCT)
					float3 normalWS = normalize(IN.normalWS);
					float2 octNormalWS = PackNormalOctQuadEncode(normalWS);           // values between [-1, +1], must use fp32 on some platforms
					float2 remappedOctNormalWS = saturate(octNormalWS * 0.5 + 0.5);   // values between [ 0,  1]
					half3 packedNormalWS = PackFloat2To888(remappedOctNormalWS);      // values between [ 0,  1]
					outNormalWS = half4(packedNormalWS, 0.0);
				#else
					float3 normalWS = IN.normalWS;
					outNormalWS = half4(NormalizeNormalPerPixel(normalWS), 0.0);
				#endif

				#ifdef _WRITE_RENDERING_LAYERS
					uint renderingLayers = GetMeshRenderingLayer();
					outRenderingLayers = float4(EncodeMeshRenderingLayer(renderingLayers), 0, 0, 0);
				#endif
			}

			ENDHLSL
		}

	
	}
	
	CustomEditor "UnityEditor.ShaderGraphUnlitGUI"
	FallBack "Hidden/Shader Graph/FallbackError"
	
	Fallback Off
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.CommentaryNode;9;-348.1069,-80.45557;Inherit;False;214;166;Culling;1;10;;1,1,1,1;0;0
Node;AmplifyShaderEditor.ComponentMaskNode;23;-85.93408,150.8398;Inherit;False;False;False;False;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;45;153.028,152.3606;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;61;390,-13;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;0;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;62;390,-13;Float;False;True;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;VFX/VFXMaster_Transparent;2992e84f91cbeb14eab234972e07ea9d;True;Forward;0;1;Forward;8;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;True;True;1;True;_Culling;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;True;1;5;False;;10;False;;1;1;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;False;;True;3;True;_ZTest;True;True;0;False;;0;False;;True;1;LightMode=UniversalForwardOnly;False;False;0;;0;0;Standard;23;Surface;1;638088936110755809;  Blend;0;638181128815698246;Two Sided;1;0;Forward Only;0;0;Cast Shadows;1;0;  Use Shadow Threshold;0;0;Receive Shadows;1;0;GPU Instancing;1;0;LOD CrossFade;0;0;Built-in Fog;0;0;DOTS Instancing;0;0;Meta Pass;0;0;Extra Pre Pass;0;0;Tessellation;0;0;  Phong;0;0;  Strength;0.5,False,;0;  Type;0;0;  Tess;16,False,;0;  Min;10,False,;0;  Max;25,False,;0;  Edge Length;16,False,;0;  Max Displacement;25,False,;0;Vertex Position,InvertActionOnDeselection;1;0;0;10;False;True;True;True;False;False;True;True;True;False;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;63;390,-13;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=ShadowCaster;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;64;390,-13;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;False;False;True;1;LightMode=DepthOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;65;390,-13;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;66;390,-13;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Universal2D;0;5;Universal2D;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=Universal2D;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;67;390,-13;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;SceneSelectionPass;0;6;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;68;390,-13;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ScenePickingPass;0;7;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Picking;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;69;390,-13;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthNormals;0;8;DepthNormals;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormalsOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;70;390,-13;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthNormalsOnly;0;9;DepthNormalsOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormalsOnly;False;True;9;d3d11;metal;vulkan;xboxone;xboxseries;playstation;ps4;ps5;switch;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.FunctionNode;71;-364.0702,126.412;Inherit;False;VFXMaster;2;;57;eee9e2fc950ab3840ab42c823f5502fd;3,280,0,273,0,282,0;0;2;FLOAT4;0;FLOAT3;194
Node;AmplifyShaderEditor.RangedFloatNode;10;-298.1069,-30.45557;Inherit;False;Property;_Culling;Culling;0;0;Create;True;0;0;0;True;1;Enum(UnityEngine.Rendering.CullMode);False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;73;-84.5437,-32.26974;Inherit;False;Property;_ZTest;ZTest;1;0;Create;True;0;0;0;True;1;Enum(UnityEngine.Rendering.CompareFunction);False;4;4;0;0;0;1;FLOAT;0
WireConnection;23;0;71;0
WireConnection;45;0;23;0
WireConnection;62;2;71;0
WireConnection;62;3;45;0
WireConnection;62;5;71;194
ASEEND*/
//CHKSM=BE40B49FEE09CEC8D0CA4E91A9EA1D44608F97D3