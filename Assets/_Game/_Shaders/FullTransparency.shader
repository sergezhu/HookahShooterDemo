Shader "Unlit/FullTransparency"
{
    Properties
    {
        //_MainTex ("Texture", 2D) = "white" {}
    }
     SubShader {
        Tags { "RenderType"="TransparentCutout" "Queue"="Transparent" }
        LOD 200
        
        Pass {
            Cull Off
            Lighting Off
            ZWrite Off
            ZTest Always
            ColorMask RGB
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            //sampler2D _MainTex;
            
            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target {
                //fixed4 c = tex2D(_MainTex, i.uv);
                fixed4 c = fixed4(0,0,0,0);
                clip(c.a - 1.5); // Adjust the alpha threshold as needed
                
                return c;
            }
            ENDCG
        }
    }
    
    FallBack "Diffuse"
}
