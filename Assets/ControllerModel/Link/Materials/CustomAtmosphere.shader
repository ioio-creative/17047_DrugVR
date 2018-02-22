// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Atmosphere" {
    Properties {
        _RampTex ("Base (RGB)", 2D) = "white" {}  
        _AmbientColor  ("Ambient Color", Color) = (1,1,1,1)  
    }
   
    Category {
    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
    Blend One OneMinusSrcAlpha
 
   
        SubShader {
            Pass {
           
                CGPROGRAM
               
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"
               
                sampler2D _RampTex;
                float4 _AmbientColor;  
               
                struct v2f
                {
                     float4 pos : SV_POSITION;
                     fixed4 color : COLOR;
                     float3 normal : TEXCOORD0; //you don't need these semantics except for XBox360
                     float3 viewT : TEXCOORD1; //you don't need these semantics except for XBox360
                  };
       
                 v2f vert (appdata_base v)
                 {
                     v2f o;
                     o.pos = UnityObjectToClipPos(v.vertex);
                     o.normal = normalize(v.normal);
                     o.viewT = normalize(ObjSpaceViewDir(v.vertex));
 
                     return o;
                 }
 
                 fixed4 frag(v2f i) : COLOR0
                 {
                     float difLight = dot(i.normal, i.viewT);
                    float4 ramp = tex2D(_RampTex, difLight);
                   
                    ramp.rgb = ramp.a * _AmbientColor.rgb;
                    return ramp;
                }
               
                ENDCG
            }
        }
    }
}
