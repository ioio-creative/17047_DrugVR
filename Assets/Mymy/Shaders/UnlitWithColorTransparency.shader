// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// http://www.alanzucconi.com/2015/06/10/a-gentle-introduction-to-shaders-in-unity3d/
// http://www.alanzucconi.com/2015/07/01/vertex-and-fragment-shaders-in-unity3d/
// https://unity3d.com/learn/tutorials/topics/graphics/making-transparent-shader

Shader "Unlit/UnlitWithColorTransparency"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_TintColor("Tint Color", Color) = (1, 1, 1, 1)  // RGBA
		_Transparency("Transparency", Range(0.0, 1.0)) = 0.5
	}
	
	SubShader
	{
		//Tags { "RenderType" = "Opaque" }
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Front// Cull Back = outside sphere;  Cull Front = inside sphere 

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			
			struct vertInput
			{
				float4 pos: POSITION;				
			};
		
			struct vertOutput
			{				
				float4 pos : SV_POSITION;
			};
	
			float4 _TintColor;
			float _Transparency;

			vertOutput vert(vertInput i)
			{
				vertOutput o;
				o.pos = UnityObjectToClipPos(i.pos);
				return o;
			}
		
			half4 frag(vertOutput i) : COLOR
			{
				fixed4 col = _TintColor;
				col.a = _Transparency;
				return col;
			}
			
			ENDCG
		}
	}
}
