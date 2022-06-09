// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "KumaBeer/URP_Anime_water"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[ASEBegin]_Maincolor("Main color", Color) = (0.2117647,0.5529412,0.5843138,1)
		_Wavestime("Waves time", Range( 0 , 1)) = 1
		_Wavestile("Waves tile", Float) = 1
		_WavesNormal("Waves Normal", 2D) = "bump" {}
		_WavesNormalscale("Waves Normal scale", Range( 0.1 , 1)) = 0.3
		_Distortion("Distortion", Float) = 0.5
		_DepthColor("Depth Color", Color) = (0,0.04313726,0.4039216,1)
		_WaterDepth("Water Depth", Float) = 0
		_WaterFalloff("Water Falloff", Float) = -3.5
		_Specular("Specular", Range( -1 , 0)) = -0.624
		[HDR]_RimColor("Rim Color", Color) = (0.1333333,0.3058824,0.3098039,0)
		_RimOffset("Rim Offset", Range( 0 , 1)) = 0.564
		_RimStr("Rim Str", Range( 0.01 , 1)) = 0.95
		_Sparklestile("Sparkles tile", Float) = 25
		_CausticSparklesStr("Caustic/Sparkles Str", Float) = 1
		_Caustictile("Caustic tile", Float) = 15
		_Causticalignmin("Caustic align min", Float) = -3
		_Causticalignmax("Caustic align max", Float) = 1
		_Foamtile("Foam tile", Float) = 10
		_Foamcolor("Foam color", Color) = (0.7372549,0.7372549,0.7372549,0)
		_FoamFalloff("Foam Falloff", Float) = 1.15
		_FoamDepth("Foam Depth", Float) = 1.5
		_WaterMask("Water Mask", 2D) = "white" {}
		_Surf("Surf", Float) = 2
		_Surfclamp("Surf clamp", Range( 0 , 1)) = 0.05
		_Reflectstr("Reflect str", Range( 0 , 1)) = 0.4
		[ASEEnd][Toggle(_USEREFLECTION_ON)] _Usereflection("Use reflection", Float) = 0

		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		_TessValue( "Max Tessellation", Range( 1, 32 ) ) = 16
		_TessMin( "Tess Min Distance", Float ) = 10
		_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25
	}

	SubShader
	{
		LOD 0

		
		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }
		
		Cull Back
		AlphaToMask Off
		HLSLINCLUDE
		#pragma target 4.0

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
			Tags { "LightMode"="UniversalForward" }
			
			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZWrite Off
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGB
			

			HLSLPROGRAM
			#define _RECEIVE_SHADOWS_OFF 1
			#pragma multi_compile_instancing
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#pragma multi_compile _ DOTS_INSTANCING_ON
			#define TESSELLATION_ON 1
			#pragma require tessellation tessHW
			#pragma hull HullFunction
			#pragma domain DomainFunction
			#define ASE_DISTANCE_TESSELLATION
			#define ASE_SRP_VERSION 70501
			#define REQUIRE_DEPTH_TEXTURE 1

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#if ASE_SRP_VERSION <= 70108
			#define REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
			#endif

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma shader_feature_local _USEREFLECTION_ON
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;
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
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
				float4 ase_texcoord7 : TEXCOORD7;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Foamcolor;
			float4 _DepthColor;
			float4 _Maincolor;
			float4 _RimColor;
			float _Reflectstr;
			float _CausticSparklesStr;
			float _Causticalignmax;
			float _Causticalignmin;
			float _Caustictile;
			float _Sparklestile;
			float _Specular;
			float _Distortion;
			float _Wavestime;
			float _Surf;
			float _FoamFalloff;
			float _Foamtile;
			float _FoamDepth;
			float _RimOffset;
			float _RimStr;
			float _Surfclamp;
			float _WavesNormalscale;
			float _Wavestile;
			float _WaterDepth;
			float _WaterFalloff;
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _WavesNormal;
			uniform float4 _CameraDepthTexture_TexelSize;
			sampler2D _WaterMask;
			sampler2D _ReflectionTex;


			float SHADERGRAPH_SAMPLE_SCENE_DEPTH_LOD(float2 uv)
			{
				#if defined(REQUIRE_DEPTH_TEXTURE)
				#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
				 	float rawDepth = SAMPLE_TEXTURE2D_ARRAY_LOD(_CameraDepthTexture, sampler_CameraDepthTexture, uv, unity_StereoEyeIndex, 0).r;
				#else
				 	float rawDepth = SAMPLE_DEPTH_TEXTURE_LOD(_CameraDepthTexture, sampler_CameraDepthTexture, uv, 0);
				#endif
				return rawDepth;
				#endif // REQUIRE_DEPTH_TEXTURE
				return 0;
			}
			
			float3 AdditionalLightsFlat( float3 WorldPosition )
			{
				float3 Color = 0;
				#ifdef _ADDITIONAL_LIGHTS
				int numLights = GetAdditionalLightsCount();
				for(int i = 0; i<numLights;i++)
				{
					Light light = GetAdditionalLight(i, WorldPosition);
					Color += light.color *(light.distanceAttenuation * light.shadowAttenuation);
					
				}
				#endif
				return Color;
			}
			
			
			VertexOutput VertexFunction ( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float mulTime896 = _TimeParameters.x * _Wavestime;
				float2 temp_cast_0 = (_Wavestile).xx;
				float2 texCoord477 = v.ase_texcoord.xy * temp_cast_0 + float2( 0,0 );
				float2 panner475 = ( mulTime896 * float2( 0.04,0.03 ) + texCoord477);
				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float eyeDepth1 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH_LOD( ase_screenPosNorm.xy ),_ZBufferParams);
				float temp_output_89_0 = abs( ( eyeDepth1 - screenPos.w ) );
				float temp_output_794_0 = ( 1.0 - temp_output_89_0 );
				float lerpResult814 = lerp( _WavesNormalscale , _Surfclamp , saturate( temp_output_794_0 ));
				float3 unpack23 = UnpackNormalScale( tex2Dlod( _WavesNormal, float4( panner475, 0, 1.0) ), lerpResult814 );
				unpack23.z = lerp( 1, unpack23.z, saturate(lerpResult814) );
				float2 panner829 = ( mulTime896 * float2( -0.03,0.04 ) + texCoord477);
				float3 unpack17 = UnpackNormalScale( tex2Dlod( _WavesNormal, float4( panner829, 0, 1.0) ), lerpResult814 );
				unpack17.z = lerp( 1, unpack17.z, saturate(lerpResult814) );
				float3 temp_output_826_0 = BlendNormal( unpack23 , unpack17 );
				float3 temp_cast_1 = (temp_output_826_0.y).xxx;
				
				o.ase_texcoord4 = screenPos;
				float3 ase_worldTangent = TransformObjectToWorldDir(v.ase_tangent.xyz);
				o.ase_texcoord5.xyz = ase_worldTangent;
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord6.xyz = ase_worldNormal;
				float ase_vertexTangentSign = v.ase_tangent.w * unity_WorldTransformParams.w;
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				o.ase_texcoord7.xyz = ase_worldBitangent;
				
				o.ase_texcoord3.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord3.zw = 0;
				o.ase_texcoord5.w = 0;
				o.ase_texcoord6.w = 0;
				o.ase_texcoord7.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = temp_cast_1;
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

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;

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
				o.ase_tangent = v.ase_tangent;
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
				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
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

			half4 frag ( VertexOutput IN  ) : SV_Target
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
				float mulTime896 = _TimeParameters.x * _Wavestime;
				float2 temp_cast_0 = (_Wavestile).xx;
				float2 texCoord477 = IN.ase_texcoord3.xy * temp_cast_0 + float2( 0,0 );
				float2 panner475 = ( mulTime896 * float2( 0.04,0.03 ) + texCoord477);
				float4 screenPos = IN.ase_texcoord4;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float eyeDepth1 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float temp_output_89_0 = abs( ( eyeDepth1 - screenPos.w ) );
				float temp_output_794_0 = ( 1.0 - temp_output_89_0 );
				float lerpResult814 = lerp( _WavesNormalscale , _Surfclamp , saturate( temp_output_794_0 ));
				float3 unpack23 = UnpackNormalScale( tex2D( _WavesNormal, panner475 ), lerpResult814 );
				unpack23.z = lerp( 1, unpack23.z, saturate(lerpResult814) );
				float2 panner829 = ( mulTime896 * float2( -0.03,0.04 ) + texCoord477);
				float3 unpack17 = UnpackNormalScale( tex2D( _WavesNormal, panner829 ), lerpResult814 );
				unpack17.z = lerp( 1, unpack17.z, saturate(lerpResult814) );
				float3 temp_output_826_0 = BlendNormal( unpack23 , unpack17 );
				float3 ase_worldTangent = IN.ase_texcoord5.xyz;
				float3 ase_worldNormal = IN.ase_texcoord6.xyz;
				float3 ase_worldBitangent = IN.ase_texcoord7.xyz;
				float3 tanToWorld0 = float3( ase_worldTangent.x, ase_worldBitangent.x, ase_worldNormal.x );
				float3 tanToWorld1 = float3( ase_worldTangent.y, ase_worldBitangent.y, ase_worldNormal.y );
				float3 tanToWorld2 = float3( ase_worldTangent.z, ase_worldBitangent.z, ase_worldNormal.z );
				float3 tanNormal261 = temp_output_826_0;
				float3 worldNormal261 = float3(dot(tanToWorld0,tanNormal261), dot(tanToWorld1,tanNormal261), dot(tanToWorld2,tanNormal261));
				float dotResult240 = dot( worldNormal261 , _MainLightPosition.xyz );
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float dotResult242 = dot( worldNormal261 , ase_worldViewDir );
				float temp_output_244_0 = ( dotResult242 + _RimOffset );
				float smoothstepResult265 = smoothstep( _RimStr , _RimOffset , temp_output_244_0);
				float temp_output_255_0 = saturate( ( dotResult240 * smoothstepResult265 ) );
				float4 Lightcolor233 = _MainLightColor;
				float temp_output_364_0 = ( sin( _TimeParameters.x ) * ( 1.0 - dotResult242 ) );
				float2 temp_cast_2 = (_Foamtile).xx;
				float2 texCoord106 = IN.ase_texcoord3.xy * temp_cast_2 + float2( 0,0 );
				float2 panner116 = ( temp_output_364_0 * float2( 0.5,0.5 ) + texCoord106);
				float4 tex2DNode668 = tex2D( _WaterMask, panner116 );
				float smoothstepResult335 = smoothstep( 1.2 , 1.0 , ( ( temp_output_89_0 * _FoamDepth * tex2DNode668.b ) + _FoamFalloff ));
				float temp_output_688_0 = sin( ( ( (1.0 + (temp_output_89_0 - 0.0) * (0.0 - 1.0) / (1.0 - 0.0)) - ( _TimeParameters.x * 0.4 ) ) * 6.0 ) );
				float lerpResult712 = lerp( temp_output_688_0 , 2.0 , _Surf);
				float Foam338 = saturate( ( smoothstepResult335 * lerpResult712 ) );
				float4 temp_output_453_0 = ( Foam338 * _Foamcolor );
				float2 appendResult770 = (float2(ase_screenPosNorm.x , ase_screenPosNorm.y));
				float4 tex2DNode227 = tex2D( _ReflectionTex, ( float3( appendResult770 ,  0.0 ) + ( float3( (temp_output_826_0).xy ,  0.0 ) * ( temp_output_826_0 * _Distortion ) ) ).xy );
				float4 lerpResult841 = lerp( Lightcolor233 , tex2DNode227 , 0.6);
				float lerpResult758 = lerp( _Specular , temp_output_255_0 , 0.45);
				float clampResult750 = clamp( sign( lerpResult758 ) , 0.0 , 0.8 );
				float2 temp_cast_6 = (_Sparklestile).xx;
				float2 texCoord860 = IN.ase_texcoord3.xy * temp_cast_6 + ( screenPos * dotResult242 ).xy;
				float2 temp_cast_8 = (_Caustictile).xx;
				float2 texCoord737 = IN.ase_texcoord3.xy * temp_cast_8 + float2( 0,0 );
				float clampResult893 = clamp( temp_output_794_0 , _Causticalignmin , _Causticalignmax );
				float2 panner22 = ( 1.0 * _Time.y * float2( -0.03,0 ) + ( texCoord737 + clampResult893 ));
				float4 tex2DNode105 = tex2D( _WaterMask, panner22 );
				float4 lerpResult836 = lerp( float4( 0,0,0,0 ) , lerpResult841 , ( clampResult750 + saturate( (( UnpackNormalScale( tex2D( _WaterMask, texCoord860 ), 1.0f ).g * tex2DNode668.b * tex2DNode105.r * _CausticSparklesStr )*20.0 + -0.5) ) ));
				float clampResult853 = clamp( temp_output_794_0 , 0.05 , 1.0 );
				float clampResult724 = clamp( ( tex2DNode105.r * _CausticSparklesStr * clampResult853 * ( 1.0 - Foam338 ) * _MainLightColor.a ) , 0.0 , Lightcolor233.r );
				float4 temp_cast_10 = (0.0).xxxx;
				float4 lerpResult210 = lerp( _Maincolor , tex2DNode227 , ( temp_output_244_0 * _Reflectstr * saturate( temp_output_89_0 ) ));
				#ifdef _USEREFLECTION_ON
				float4 staticSwitch986 = lerpResult210;
				#else
				float4 staticSwitch986 = temp_cast_10;
				#endif
				float4 lerpResult93 = lerp( _DepthColor , float4( 0,0,0,0 ) , saturate( pow( ( temp_output_89_0 + _WaterDepth ) , _WaterFalloff ) ));
				float3 worldPosValue44_g1 = WorldPosition;
				float3 WorldPosition8_g1 = worldPosValue44_g1;
				float3 localAdditionalLightsFlat8_g1 = AdditionalLightsFlat( WorldPosition8_g1 );
				float3 FlatResult29_g1 = localAdditionalLightsFlat8_g1;
				
				float3 BakedAlbedo = 0;
				float3 BakedEmission = 0;
				float3 Color = ( ( ( temp_output_255_0 * float4( (_RimColor).rgb , 0.0 ) * Lightcolor233 ) + temp_output_453_0 + lerpResult836 + clampResult724 + staticSwitch986 ) + ( lerpResult93 * _MainLightColor ) + float4( FlatResult29_g1 , 0.0 ) ).rgb;
				float Alpha = ( saturate( temp_output_89_0 ) + temp_output_453_0 ).r;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;

				#ifdef _ALPHATEST_ON
					clip( Alpha - AlphaClipThreshold );
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				#ifdef ASE_FOG
					Color = MixFog( Color, IN.fogFactor );
				#endif

				return half4( Color, Alpha );
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
			#define _RECEIVE_SHADOWS_OFF 1
			#pragma multi_compile_instancing
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#pragma multi_compile _ DOTS_INSTANCING_ON
			#define TESSELLATION_ON 1
			#pragma require tessellation tessHW
			#pragma hull HullFunction
			#pragma domain DomainFunction
			#define ASE_DISTANCE_TESSELLATION
			#define ASE_SRP_VERSION 70501
			#define REQUIRE_DEPTH_TEXTURE 1

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;
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
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Foamcolor;
			float4 _DepthColor;
			float4 _Maincolor;
			float4 _RimColor;
			float _Reflectstr;
			float _CausticSparklesStr;
			float _Causticalignmax;
			float _Causticalignmin;
			float _Caustictile;
			float _Sparklestile;
			float _Specular;
			float _Distortion;
			float _Wavestime;
			float _Surf;
			float _FoamFalloff;
			float _Foamtile;
			float _FoamDepth;
			float _RimOffset;
			float _RimStr;
			float _Surfclamp;
			float _WavesNormalscale;
			float _Wavestile;
			float _WaterDepth;
			float _WaterFalloff;
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _WavesNormal;
			uniform float4 _CameraDepthTexture_TexelSize;
			sampler2D _WaterMask;


			float SHADERGRAPH_SAMPLE_SCENE_DEPTH_LOD(float2 uv)
			{
				#if defined(REQUIRE_DEPTH_TEXTURE)
				#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
				 	float rawDepth = SAMPLE_TEXTURE2D_ARRAY_LOD(_CameraDepthTexture, sampler_CameraDepthTexture, uv, unity_StereoEyeIndex, 0).r;
				#else
				 	float rawDepth = SAMPLE_DEPTH_TEXTURE_LOD(_CameraDepthTexture, sampler_CameraDepthTexture, uv, 0);
				#endif
				return rawDepth;
				#endif // REQUIRE_DEPTH_TEXTURE
				return 0;
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float mulTime896 = _TimeParameters.x * _Wavestime;
				float2 temp_cast_0 = (_Wavestile).xx;
				float2 texCoord477 = v.ase_texcoord.xy * temp_cast_0 + float2( 0,0 );
				float2 panner475 = ( mulTime896 * float2( 0.04,0.03 ) + texCoord477);
				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float eyeDepth1 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH_LOD( ase_screenPosNorm.xy ),_ZBufferParams);
				float temp_output_89_0 = abs( ( eyeDepth1 - screenPos.w ) );
				float temp_output_794_0 = ( 1.0 - temp_output_89_0 );
				float lerpResult814 = lerp( _WavesNormalscale , _Surfclamp , saturate( temp_output_794_0 ));
				float3 unpack23 = UnpackNormalScale( tex2Dlod( _WavesNormal, float4( panner475, 0, 1.0) ), lerpResult814 );
				unpack23.z = lerp( 1, unpack23.z, saturate(lerpResult814) );
				float2 panner829 = ( mulTime896 * float2( -0.03,0.04 ) + texCoord477);
				float3 unpack17 = UnpackNormalScale( tex2Dlod( _WavesNormal, float4( panner829, 0, 1.0) ), lerpResult814 );
				unpack17.z = lerp( 1, unpack17.z, saturate(lerpResult814) );
				float3 temp_output_826_0 = BlendNormal( unpack23 , unpack17 );
				float3 temp_cast_1 = (temp_output_826_0.y).xxx;
				
				o.ase_texcoord2 = screenPos;
				float3 ase_worldTangent = TransformObjectToWorldDir(v.ase_tangent.xyz);
				o.ase_texcoord4.xyz = ase_worldTangent;
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord5.xyz = ase_worldNormal;
				float ase_vertexTangentSign = v.ase_tangent.w * unity_WorldTransformParams.w;
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				o.ase_texcoord6.xyz = ase_worldBitangent;
				
				o.ase_texcoord3.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord3.zw = 0;
				o.ase_texcoord4.w = 0;
				o.ase_texcoord5.w = 0;
				o.ase_texcoord6.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = temp_cast_1;
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

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;

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
				o.ase_tangent = v.ase_tangent;
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
				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
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

				float4 screenPos = IN.ase_texcoord2;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float eyeDepth1 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float temp_output_89_0 = abs( ( eyeDepth1 - screenPos.w ) );
				float mulTime896 = _TimeParameters.x * _Wavestime;
				float2 temp_cast_0 = (_Wavestile).xx;
				float2 texCoord477 = IN.ase_texcoord3.xy * temp_cast_0 + float2( 0,0 );
				float2 panner475 = ( mulTime896 * float2( 0.04,0.03 ) + texCoord477);
				float temp_output_794_0 = ( 1.0 - temp_output_89_0 );
				float lerpResult814 = lerp( _WavesNormalscale , _Surfclamp , saturate( temp_output_794_0 ));
				float3 unpack23 = UnpackNormalScale( tex2D( _WavesNormal, panner475 ), lerpResult814 );
				unpack23.z = lerp( 1, unpack23.z, saturate(lerpResult814) );
				float2 panner829 = ( mulTime896 * float2( -0.03,0.04 ) + texCoord477);
				float3 unpack17 = UnpackNormalScale( tex2D( _WavesNormal, panner829 ), lerpResult814 );
				unpack17.z = lerp( 1, unpack17.z, saturate(lerpResult814) );
				float3 temp_output_826_0 = BlendNormal( unpack23 , unpack17 );
				float3 ase_worldTangent = IN.ase_texcoord4.xyz;
				float3 ase_worldNormal = IN.ase_texcoord5.xyz;
				float3 ase_worldBitangent = IN.ase_texcoord6.xyz;
				float3 tanToWorld0 = float3( ase_worldTangent.x, ase_worldBitangent.x, ase_worldNormal.x );
				float3 tanToWorld1 = float3( ase_worldTangent.y, ase_worldBitangent.y, ase_worldNormal.y );
				float3 tanToWorld2 = float3( ase_worldTangent.z, ase_worldBitangent.z, ase_worldNormal.z );
				float3 tanNormal261 = temp_output_826_0;
				float3 worldNormal261 = float3(dot(tanToWorld0,tanNormal261), dot(tanToWorld1,tanNormal261), dot(tanToWorld2,tanNormal261));
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float dotResult242 = dot( worldNormal261 , ase_worldViewDir );
				float temp_output_364_0 = ( sin( _TimeParameters.x ) * ( 1.0 - dotResult242 ) );
				float2 temp_cast_1 = (_Foamtile).xx;
				float2 texCoord106 = IN.ase_texcoord3.xy * temp_cast_1 + float2( 0,0 );
				float2 panner116 = ( temp_output_364_0 * float2( 0.5,0.5 ) + texCoord106);
				float4 tex2DNode668 = tex2D( _WaterMask, panner116 );
				float smoothstepResult335 = smoothstep( 1.2 , 1.0 , ( ( temp_output_89_0 * _FoamDepth * tex2DNode668.b ) + _FoamFalloff ));
				float temp_output_688_0 = sin( ( ( (1.0 + (temp_output_89_0 - 0.0) * (0.0 - 1.0) / (1.0 - 0.0)) - ( _TimeParameters.x * 0.4 ) ) * 6.0 ) );
				float lerpResult712 = lerp( temp_output_688_0 , 2.0 , _Surf);
				float Foam338 = saturate( ( smoothstepResult335 * lerpResult712 ) );
				float4 temp_output_453_0 = ( Foam338 * _Foamcolor );
				
				float Alpha = ( saturate( temp_output_89_0 ) + temp_output_453_0 ).r;
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				return 0;
			}
			ENDHLSL
		}

	
	}
	CustomEditor "UnityEditor.ShaderGraph.PBRMasterGUI"
	Fallback "0"
	
}
/*ASEBEGIN
Version=18909
333;356;1297;593;2372.011;25.09799;1.818871;True;True
Node;AmplifyShaderEditor.CommentaryNode;768;1160.254,-55.59366;Inherit;False;1363.002;554.5168;Reflections;14;166;834;97;98;400;770;224;225;228;848;226;210;840;227;Reflections;0.2311321,1,0.8462238,1;0;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;166;1182.145,-0.3448472;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;152;-2489.1,-299.3656;Inherit;False;679.924;291.6787;Depth Fade;5;1;3;89;999;1031;Depth;1,1,1,1;0;0
Node;AmplifyShaderEditor.WireNode;1000;1328.629,-154.4981;Inherit;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.WireNode;999;-2464.025,-150.3575;Inherit;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;1031;-2285.955,-179.2633;Float;False;1;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScreenDepthNode;1;-2458.499,-254.9639;Inherit;False;0;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;3;-2083.757,-250.9547;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;769;-625.9565,-453.1382;Inherit;False;1036.188;456.9082;Waves;11;818;829;814;475;477;813;721;17;23;896;897;Waves;1,1,1,1;0;0
Node;AmplifyShaderEditor.AbsOpNode;89;-1934.762,-242.9418;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;897;-618.0594,-108.7512;Inherit;False;Property;_Wavestime;Waves time;1;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;794;-1580.626,-79.66352;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;721;-612.8724,-393.1046;Inherit;False;Property;_Wavestile;Waves tile;2;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;816;-1340.244,-243.628;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;818;-618.0889,-201.4229;Inherit;False;Property;_Surfclamp;Surf clamp;24;0;Create;True;0;0;0;False;0;False;0.05;0.05;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;896;-338.0594,-103.7512;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;477;-433.4601,-411.6447;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;813;-616.8032,-282.5838;Inherit;False;Property;_WavesNormalscale;Waves Normal scale;4;0;Create;True;0;0;0;False;0;False;0.3;0.3;0.1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;814;-122.1481,-281.8014;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;475;-128.5865,-405.6331;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.04,0.03;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;829;-134.0423,-149.5408;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.03,0.04;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;17;65.53008,-203.7247;Inherit;True;Property;_WavesNormal;Waves Normal;3;0;Create;True;0;0;0;False;0;False;-1;None;0e2840867afdcbc4a88cb532e35985a7;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;23;67.34294,-418.2462;Inherit;True;Property;_WavesNormal2;Waves Normal 2;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Instance;17;Auto;Texture2D;8;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BlendNormalsNode;826;464.5795,-282.0019;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;234;-531.8716,-2591.205;Inherit;False;3028.814;623.0454;Rimlight;24;750;753;758;760;253;251;247;246;255;252;240;238;265;250;244;254;242;261;249;836;841;843;886;888;Rimlight+Specular;1,1,1,1;0;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;249;-474.011,-2242.57;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;261;-435.4382,-2530.584;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;153;-2831.974,159.5769;Inherit;False;2570.978;742.7954;Foam;26;338;113;438;712;335;713;115;688;690;735;333;686;111;706;684;116;364;106;704;375;365;363;800;797;793;668;Foam;1,1,1,1;0;0
Node;AmplifyShaderEditor.DotProductOpNode;242;-154.0119,-2306.569;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;505;-2846.487,-23.73637;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;363;-2809.215,495.5995;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;704;-2544.448,277.7944;Inherit;False;Property;_Foamtile;Foam tile;18;0;Create;True;0;0;0;False;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;375;-2802.045,583.6836;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;365;-2529.285,495.5934;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;364;-2371.919,559.4222;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;106;-2318.988,220.5851;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;10,10;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;706;-2546.576,727.6694;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.4;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;684;-1961.96,529.1949;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;116;-2065.225,225.2435;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.5,0.5;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;668;-1840.211,207.9578;Inherit;True;Property;_WaterMask;Water Mask;22;0;Create;True;0;0;0;False;0;False;-1;None;f8d49aff2ed9bac4c825ad2dfac48383;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;686;-1662.034,682.9836;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;111;-1518.495,342.7661;Float;False;Property;_FoamDepth;Foam Depth;21;0;Create;True;0;0;0;False;0;False;1.5;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;735;-1326.646,348.3019;Float;False;Property;_FoamFalloff;Foam Falloff;20;0;Create;True;0;0;0;False;0;False;1.15;1.06;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;690;-1423.684,683.6314;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;6;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;333;-1322.886,210.5276;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;115;-1160.813,220.1345;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1.15;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;713;-1207.122,573.4477;Inherit;False;Property;_Surf;Surf;23;0;Create;True;0;0;0;False;0;False;2;0.8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;688;-1096.591,683.305;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;712;-954.7236,533.95;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;2;False;2;FLOAT;0.6;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;335;-1022.108,223.6589;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1.2;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;438;-820.7691,221.6705;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;113;-653.8522,224.8192;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;338;-490.0742,218.7641;Inherit;False;Foam;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;447;1994.328,-639.1523;Inherit;False;Property;_Foamcolor;Foam color;19;0;Create;True;0;0;0;False;0;False;0.7372549,0.7372549,0.7372549,0;0.9056604,0.9056604,0.9056604,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;339;2228.906,-709.9649;Inherit;False;338;Foam;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;926;2822.175,-242.5689;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;159;-782.3449,-949.2329;Inherit;False;790.0168;382.0366;Depth control;6;6;94;87;10;88;12;Depth coloring;0.245105,0.3316393,0.6415094,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;895;532.0017,-1554.597;Inherit;False;1813.997;650.1569;Caustics and sparkles;20;890;885;729;722;724;105;22;894;893;737;736;862;860;891;872;898;899;2;979;980;Caustics and sparkles;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;453;2408.411,-657.6507;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TexturePropertyNode;225;1702.892,30.45972;Inherit;True;Global;_ReflectionTex;_ReflectionTex;27;0;Create;True;0;0;0;False;0;False;None;;False;white;LockedToTexture2D;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;254;-243.9667,-2088.857;Float;False;Property;_RimOffset;Rim Offset;11;0;Create;True;0;0;0;False;0;False;0.564;0.416;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;898;635.2087,-1080.908;Inherit;False;Property;_Causticalignmin;Caustic align min;16;0;Create;True;0;0;0;False;0;False;-3;-33.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;899;633.2087,-1007.908;Inherit;False;Property;_Causticalignmax;Caustic align max;17;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;736;559.1447,-1188.314;Inherit;False;Property;_Caustictile;Caustic tile;15;0;Create;True;0;0;0;False;0;False;15;15;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;238;113.4446,-2454.887;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TextureCoordinatesNode;737;751.1449,-1220.314;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;97;1185.11,290.6603;Float;False;Property;_Distortion;Distortion;5;0;Create;True;0;0;0;False;0;False;0.5;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;87;-347.5934,-880.2133;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;246;620.1282,-2159.205;Float;False;Property;_RimColor;Rim Color;10;1;[HDR];Create;True;0;0;0;False;0;False;0.1333333,0.3058824,0.3098039,0;0.1333333,0.3058824,0.3098039,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;227;2036.589,174.1663;Inherit;True;Global;_ReflectionTexinp;_ReflectionTexinp;26;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScreenPosInputsNode;2;537.4302,-1402.207;Float;False;1;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;891;640.7505,-1489.203;Inherit;False;Property;_Sparklestile;Sparkles tile;13;0;Create;True;0;0;0;False;0;False;25;20;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;872;718.733,-1398.606;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;894;1007.201,-1213.656;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SmoothstepOpNode;265;422.1218,-2289.372;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.42;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;893;842.1371,-1073.62;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;-3;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;848;1663.62,421.6656;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;250;99.21306,-2066.926;Float;False;Property;_RimStr;Rim Str;12;0;Create;True;0;0;0;False;0;False;0.95;0.85;0.01;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;758;1482.963,-2506.949;Inherit;False;3;0;FLOAT;-0.7;False;1;FLOAT;1.25;False;2;FLOAT;0.45;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;224;1720.125,226.3984;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;834;1318.885,181.5711;Inherit;False;True;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;760;1188,-2514.831;Inherit;False;Property;_Specular;Specular;9;0;Create;True;0;0;0;False;0;False;-0.624;-0.624;-1;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;729;1609.5,-1342.491;Inherit;False;Property;_CausticSparklesStr;Caustic/Sparkles Str;14;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;105;1361.156,-1241.477;Inherit;True;Property;_RCaustic;(R) Caustic;22;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;668;Auto;Texture2D;8;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;255;1006.677,-2478.089;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;400;1544.801,248.6609;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;770;1481.225,28.26142;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;88;-529.6058,-879.7777;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;885;1870.824,-1495.166;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;232;1021.043,-712.0812;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;6;-747.7183,-723.6888;Float;False;Property;_WaterDepth;Water Depth;7;0;Create;True;0;0;0;False;0;False;0;0.63;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;98;1372.417,272.2092;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;226;1853.283,374.213;Inherit;False;Property;_Reflectstr;Reflect str;25;0;Create;True;0;0;0;False;0;False;0.4;0.4;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;244;68.10379,-2290.569;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SignOpNode;753;1678.485,-2498.858;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-535.0668,-748.8902;Float;False;Property;_WaterFalloff;Water Falloff;8;0;Create;True;0;0;0;False;0;False;-3.5;-3.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;890;2030.824,-1495.166;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;20;False;2;FLOAT;-0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;252;780.1287,-2479.205;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;841;1947.877,-2084.813;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.1415094;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;233;1365.867,-875.7365;Inherit;False;Lightcolor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;251;892.1288,-2159.205;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;722;1905.271,-1308.674;Inherit;True;5;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;792;2404.819,-539.7777;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;985;2254.039,-346.5457;Inherit;False;Constant;_Float1;Float 1;29;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;210;2366.834,156.5884;Inherit;False;3;0;COLOR;1,1,1,0;False;1;COLOR;1,1,1,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DotProductOpNode;240;442.5474,-2534.417;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;22;1161.592,-1214.136;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.03,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;871;-748.9043,-493.942;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;860;854.7328,-1446.606;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;25,25;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;840;2186.58,376.9723;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;888;1975.206,-2250.151;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;980;1738.572,-1090.589;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;228;2025.544,-3.176147;Inherit;False;Property;_Maincolor;Main color;0;0;Create;True;0;0;0;False;0;False;0.2117647,0.5529412,0.5843138,1;0.2117626,0.5529411,0.5843138,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;247;913.7299,-2085.608;Inherit;False;233;Lightcolor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;750;1871.631,-2499.601;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.8;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;886;2172.718,-2274.143;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;94;-137.6477,-881.3301;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;843;1768.324,-2043.208;Inherit;False;Constant;_Float0;Float 0;29;0;Create;True;0;0;0;False;0;False;0.6;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;797;-763.8154,641.7365;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;4;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;930;3080.808,-515.9694;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;986;2414.04,-346.324;Inherit;False;Property;_Usereflection;Use reflection;26;0;Create;True;0;0;0;False;0;False;0;0;1;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;979;1507.606,-1030.456;Inherit;False;338;Foam;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;259;2605.865,-707.1708;Inherit;False;5;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;4;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;853;1448.179,-607.5709;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.05;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;12;-207.5247,-795.3911;Float;False;Property;_DepthColor;Depth Color;6;0;Create;True;0;0;0;False;0;False;0,0.04313726,0.4039216,1;0,0.04313587,0.4039196,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;793;-425.031,617.1488;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;724;2112.232,-1308.303;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;836;2335.09,-2103.787;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;93;492.9236,-749.451;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;800;-614.108,756.6668;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;-2;False;2;FLOAT;8;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;921;2464.058,-764.5395;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;231;1673.622,-738.3243;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;253;1457.827,-2194.804;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;1032;2651.173,-491.996;Inherit;False;SRP Additional Light;-1;;1;6c86746ad131a0a408ca599df5f40861;7,6,0,9,0,23,0,26,0,27,0,24,0,25,0;6;2;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;15;FLOAT3;0,0,0;False;14;FLOAT3;1,1,1;False;18;FLOAT;0.5;False;32;FLOAT4;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;991;1411.571,-278.981;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;835;687.1506,-281.3547;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleAddOpNode;905;3015.914,-703.915;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;862;1094.732,-1478.606;Inherit;True;Property;_GSparkles;(G) Sparkles;22;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Instance;668;Auto;Texture2D;8;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;919;3248.475,-732.1378;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;3;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;True;False;False;False;False;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;False;False;True;1;LightMode=DepthOnly;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;917;3248.475,-732.1378;Float;False;True;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;3;KumaBeer/URP_Anime_water;2992e84f91cbeb14eab234972e07ea9d;True;Forward;0;1;Forward;8;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;4;0;False;True;1;5;False;-1;10;False;-1;1;1;False;-1;10;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;False;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;2;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=UniversalForward;False;0;0;0;0;Standard;22;Surface;1;  Blend;0;Two Sided;1;Cast Shadows;0;  Use Shadow Threshold;0;Receive Shadows;0;GPU Instancing;1;LOD CrossFade;0;Built-in Fog;1;DOTS Instancing;1;Meta Pass;0;Extra Pre Pass;0;Tessellation;1;  Phong;0;  Strength;0.5,False,-1;  Type;1;  Tess;32,False,-1;  Min;0,False,-1;  Max;35,False,-1;  Edge Length;16,False,-1;  Max Displacement;25,False,-1;Vertex Position,InvertActionOnDeselection;1;0;5;False;True;False;True;False;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;916;3248.475,-732.1378;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;3;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;0;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;918;3248.475,-732.1378;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;3;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=ShadowCaster;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;920;3248.475,-732.1378;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;3;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
WireConnection;1000;0;166;0
WireConnection;999;0;1000;0
WireConnection;1;0;999;0
WireConnection;3;0;1;0
WireConnection;3;1;1031;4
WireConnection;89;0;3;0
WireConnection;794;0;89;0
WireConnection;816;0;794;0
WireConnection;896;0;897;0
WireConnection;477;0;721;0
WireConnection;814;0;813;0
WireConnection;814;1;818;0
WireConnection;814;2;816;0
WireConnection;475;0;477;0
WireConnection;475;1;896;0
WireConnection;829;0;477;0
WireConnection;829;1;896;0
WireConnection;17;1;829;0
WireConnection;17;5;814;0
WireConnection;23;1;475;0
WireConnection;23;5;814;0
WireConnection;826;0;23;0
WireConnection;826;1;17;0
WireConnection;261;0;826;0
WireConnection;242;0;261;0
WireConnection;242;1;249;0
WireConnection;505;0;242;0
WireConnection;375;0;505;0
WireConnection;365;0;363;0
WireConnection;364;0;365;0
WireConnection;364;1;375;0
WireConnection;106;0;704;0
WireConnection;706;0;363;0
WireConnection;684;0;89;0
WireConnection;116;0;106;0
WireConnection;116;1;364;0
WireConnection;668;1;116;0
WireConnection;686;0;684;0
WireConnection;686;1;706;0
WireConnection;690;0;686;0
WireConnection;333;0;89;0
WireConnection;333;1;111;0
WireConnection;333;2;668;3
WireConnection;115;0;333;0
WireConnection;115;1;735;0
WireConnection;688;0;690;0
WireConnection;712;0;688;0
WireConnection;712;2;713;0
WireConnection;335;0;115;0
WireConnection;438;0;335;0
WireConnection;438;1;712;0
WireConnection;113;0;438;0
WireConnection;338;0;113;0
WireConnection;926;0;89;0
WireConnection;453;0;339;0
WireConnection;453;1;447;0
WireConnection;737;0;736;0
WireConnection;87;0;88;0
WireConnection;87;1;10;0
WireConnection;227;0;225;0
WireConnection;227;1;224;0
WireConnection;872;0;2;0
WireConnection;872;1;242;0
WireConnection;894;0;737;0
WireConnection;894;1;893;0
WireConnection;265;0;244;0
WireConnection;265;1;250;0
WireConnection;265;2;254;0
WireConnection;893;0;794;0
WireConnection;893;1;898;0
WireConnection;893;2;899;0
WireConnection;848;0;89;0
WireConnection;758;0;760;0
WireConnection;758;1;255;0
WireConnection;224;0;770;0
WireConnection;224;1;400;0
WireConnection;834;0;826;0
WireConnection;105;1;22;0
WireConnection;255;0;252;0
WireConnection;400;0;834;0
WireConnection;400;1;98;0
WireConnection;770;0;166;1
WireConnection;770;1;166;2
WireConnection;88;0;89;0
WireConnection;88;1;6;0
WireConnection;885;0;862;2
WireConnection;885;1;668;3
WireConnection;885;2;105;1
WireConnection;885;3;729;0
WireConnection;98;0;826;0
WireConnection;98;1;97;0
WireConnection;244;0;242;0
WireConnection;244;1;254;0
WireConnection;753;0;758;0
WireConnection;890;0;885;0
WireConnection;252;0;240;0
WireConnection;252;1;265;0
WireConnection;841;0;247;0
WireConnection;841;1;227;0
WireConnection;841;2;843;0
WireConnection;233;0;232;0
WireConnection;251;0;246;0
WireConnection;722;0;105;1
WireConnection;722;1;729;0
WireConnection;722;2;853;0
WireConnection;722;3;980;0
WireConnection;722;4;232;2
WireConnection;792;0;793;0
WireConnection;210;0;228;0
WireConnection;210;1;227;0
WireConnection;210;2;840;0
WireConnection;240;0;261;0
WireConnection;240;1;238;0
WireConnection;22;0;894;0
WireConnection;871;0;794;0
WireConnection;860;0;891;0
WireConnection;860;1;872;0
WireConnection;840;0;244;0
WireConnection;840;1;226;0
WireConnection;840;2;848;0
WireConnection;888;0;890;0
WireConnection;980;0;979;0
WireConnection;750;0;753;0
WireConnection;886;0;750;0
WireConnection;886;1;888;0
WireConnection;94;0;87;0
WireConnection;797;0;364;0
WireConnection;930;0;926;0
WireConnection;930;1;453;0
WireConnection;986;1;985;0
WireConnection;986;0;210;0
WireConnection;259;0;921;0
WireConnection;259;1;453;0
WireConnection;259;2;836;0
WireConnection;259;3;724;0
WireConnection;259;4;986;0
WireConnection;853;0;871;0
WireConnection;793;0;794;0
WireConnection;793;1;797;0
WireConnection;793;2;800;0
WireConnection;724;0;722;0
WireConnection;724;2;247;0
WireConnection;836;1;841;0
WireConnection;836;2;886;0
WireConnection;93;0;12;0
WireConnection;93;2;94;0
WireConnection;800;0;688;0
WireConnection;921;0;253;0
WireConnection;231;0;93;0
WireConnection;231;1;232;0
WireConnection;253;0;255;0
WireConnection;253;1;251;0
WireConnection;253;2;247;0
WireConnection;835;0;826;0
WireConnection;905;0;259;0
WireConnection;905;1;231;0
WireConnection;905;2;1032;0
WireConnection;862;1;860;0
WireConnection;917;2;905;0
WireConnection;917;3;930;0
WireConnection;917;5;835;1
ASEEND*/
//CHKSM=FF1A5594F569420C4FBC097F64E01F2D80C97C72