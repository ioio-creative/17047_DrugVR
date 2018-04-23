// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// http://www.alanzucconi.com/2015/06/10/a-gentle-introduction-to-shaders-in-unity3d/
// http://www.alanzucconi.com/2015/07/01/vertex-and-fragment-shaders-in-unity3d/
// https://unity3d.com/learn/tutorials/topics/graphics/making-transparent-shader

// Based on Unlit shader, but culls the front faces instead of the back

Shader "Unlit/InsideVisibleWithTransparency"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}		
		_Transparency("Transparency", Range(0.0, 1.0)) = 1.0
	}

	SubShader
	{
		//Tags { "RenderType" = "Opaque" }
		Tags{ "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Front  // Cull Back = outside sphere;  Cull Front = inside sphere

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
			};

			// for mapping texture
			sampler2D _MainTex;
			float4 _MainTex_ST;

			// for setting color and alpha
			fixed4 _Color;
			float _Transparency;

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				// ADDED BY BERNIE:
				v.texcoord.x = 1 - v.texcoord.x;
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.texcoord);
				col.a = _Transparency;
				return col;
			}
			
			ENDCG
		}
	}
}
