// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Shader Forge/radialGradient" {
	Properties{
		_Radius("Radius", Range(0, 1)) = 0.5
		_Density("Density", Range(0, 1)) = 0.66
	}
		SubShader{
		Tags{
		"RenderType" = "Opaque"
	}
		Pass{
		Name "FORWARD"
		Tags{
		"LightMode" = "ForwardBase"
	}


		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#define UNITY_PASS_FORWARDBASE
#include "UnityCG.cginc"
#include "AutoLight.cginc"
#pragma multi_compile_fwdbase_fullshadows
#pragma multi_compile_fog
#pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
#pragma target 3.0
		uniform float4 _LightColor0;
	uniform float _Radius;
	uniform float _Density;
	struct VertexInput {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		float2 texcoord0 : TEXCOORD0;
	};
	struct VertexOutput {
		float4 pos : SV_POSITION;
		float2 uv0 : TEXCOORD0;
		float4 posWorld : TEXCOORD1;
		float3 normalDir : TEXCOORD2;
		LIGHTING_COORDS(3,4)
			UNITY_FOG_COORDS(5)
	};
	VertexOutput vert(VertexInput v) {
		VertexOutput o = (VertexOutput)0;
		o.uv0 = v.texcoord0;
		o.normalDir = UnityObjectToWorldNormal(v.normal);
		o.posWorld = mul(unity_ObjectToWorld, v.vertex);
		float3 lightColor = _LightColor0.rgb;
		o.pos = UnityObjectToClipPos(v.vertex);
		UNITY_TRANSFER_FOG(o,o.pos);
		TRANSFER_VERTEX_TO_FRAGMENT(o)
			return o;
	}
	float4 frag(VertexOutput i) : COLOR{
		i.normalDir = normalize(i.normalDir);
	/////// Vectors:
	float3 normalDirection = i.normalDir;
	float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
	float3 lightColor = _LightColor0.rgb;
	////// Lighting:
	float attenuation = LIGHT_ATTENUATION(i);
	float3 attenColor = attenuation * _LightColor0.xyz;
	/////// Diffuse:
	float NdotL = max(0.0,dot(normalDirection, lightDirection));
	float3 directDiffuse = max(0.0, NdotL) * attenColor;
	float3 indirectDiffuse = float3(0,0,0);
	indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
	float node_1970 = (1.0 - (distance(i.uv0,float2(0.5,0.5)) / _Radius));
	float node_1101_if_leA = step(node_1970,0.0);
	float node_1101_if_leB = step(0.0,node_1970);
	float node_7045 = 1.0;
	float node_145 = (1.0 - lerp((node_1101_if_leA*node_7045) + (node_1101_if_leB*(1.0 / pow(2.718,(node_1970*_Density)))),node_7045,node_1101_if_leA*node_1101_if_leB));
	float3 diffuseColor = float3(node_145,node_145,node_145);
	float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
	/// Final Color:
	float3 finalColor = diffuse;
	fixed4 finalRGBA = fixed4(finalColor,1);
	UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
	return finalRGBA;
	}
		ENDCG
	}
		Pass{
		Name "FORWARD_DELTA"
		Tags{
		"LightMode" = "ForwardAdd"
	}
		Blend One One


		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#define UNITY_PASS_FORWARDADD
#include "UnityCG.cginc"
#include "AutoLight.cginc"
#pragma multi_compile_fwdadd_fullshadows
#pragma multi_compile_fog
#pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
#pragma target 3.0
		uniform float4 _LightColor0;
	uniform float _Radius;
	uniform float _Density;
	struct VertexInput {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		float2 texcoord0 : TEXCOORD0;
	};
	struct VertexOutput {
		float4 pos : SV_POSITION;
		float2 uv0 : TEXCOORD0;
		float4 posWorld : TEXCOORD1;
		float3 normalDir : TEXCOORD2;
		LIGHTING_COORDS(3,4)
	};
	VertexOutput vert(VertexInput v) {
		VertexOutput o = (VertexOutput)0;
		o.uv0 = v.texcoord0;
		o.normalDir = UnityObjectToWorldNormal(v.normal);
		o.posWorld = mul(unity_ObjectToWorld, v.vertex);
		float3 lightColor = _LightColor0.rgb;
		o.pos = UnityObjectToClipPos(v.vertex);
		TRANSFER_VERTEX_TO_FRAGMENT(o)
			return o;
	}
	float4 frag(VertexOutput i) : COLOR{
		i.normalDir = normalize(i.normalDir);
	/////// Vectors:
	float3 normalDirection = i.normalDir;
	float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
	float3 lightColor = _LightColor0.rgb;
	////// Lighting:
	float attenuation = LIGHT_ATTENUATION(i);
	float3 attenColor = attenuation * _LightColor0.xyz;
	/////// Diffuse:
	float NdotL = max(0.0,dot(normalDirection, lightDirection));
	float3 directDiffuse = max(0.0, NdotL) * attenColor;
	float node_1970 = (1.0 - (distance(i.uv0,float2(0.5,0.5)) / _Radius));
	float node_1101_if_leA = step(node_1970,0.0);
	float node_1101_if_leB = step(0.0,node_1970);
	float node_7045 = 1.0;
	float node_145 = (1.0 - lerp((node_1101_if_leA*node_7045) + (node_1101_if_leB*(1.0 / pow(2.718,(node_1970*_Density)))),node_7045,node_1101_if_leA*node_1101_if_leB));
	float3 diffuseColor = float3(node_145,node_145,node_145);
	float3 diffuse = directDiffuse * diffuseColor;
	/// Final Color:
	float3 finalColor = diffuse;
	return fixed4(finalColor * 1,0);
	}
		ENDCG
	}
	}
		FallBack "Diffuse"
		CustomEditor "ShaderForgeMaterialInspector"
}