Shader "Game/ToonShaderVertexColor"
{
	// https://blog.csdn.net/wolf96/article/details/43019719

	Properties
	{
		_Color("Main Color",color) = (1,1,1,1)
		_Saturation("Saturation", Range(0.0, 5.0)) = 1
		[Toggle] _HasSaturation("HasSaturation", float) = 0

		_OutlineColor("Outline Color",color) = (1,1,1,1)
		_Outline("Thick of Outline",range(0,0.1)) = 0.02
		_Factor("Factor",range(0,1)) = 0.5
		_ToonEffect("Toon Effect",range(0,1)) = 0.5
		_Steps("Steps of toon",range(0,9)) = 3
	}

	CGINCLUDE
	float4 saturation(float4 c, float saturation)
	{
		float gray = dot(c.rgb, float3(0.2126, 0.7152, 0.0722));
		c.rgb = lerp(gray, c.rgb, saturation);
		return c;
	}
	ENDCG

	SubShader
	{
		Pass
		{
			Tags {"LightMode" = "Always"}
			Cull Front
			ZWrite On
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			float _Outline;
			float _Factor;
			float4 _OutlineColor;
			
			struct v2f
			{
				float4 pos:SV_POSITION;
			};

			v2f vert(appdata_full v)
			{
				v2f o;
				float3 dir = normalize(v.vertex.xyz);
				float3 dir2 = v.normal;
				float D = dot(dir,dir2);
				dir = dir * sign(D);
				dir = dir * _Factor + dir2 * (1 - _Factor);
				v.vertex.xyz += dir * _Outline;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}

			float4 frag(v2f i) :COLOR
			{
				float4 c = _OutlineColor;
				return c;
			}

			ENDCG
		}

		Pass
		{
			Tags {"LightMode" = "ForwardBase"}
			Cull Back
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			float4 _Color;
			float _Steps;
			float _ToonEffect;
			half _Saturation;
			float _HasSaturation;

			struct v2f {
				float4 pos:SV_POSITION;
				float3 lightDir:TEXCOORD0;
				float3 viewDir:TEXCOORD1;
				float3 normal:TEXCOORD2;
				float4 color : COLOR;
			};

			v2f vert(appdata_full v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.normal = v.normal;
				o.lightDir = ObjSpaceLightDir(v.vertex);
				o.viewDir = ObjSpaceViewDir(v.vertex);
				o.color = v.color;
				
				return o;
			}

			float4 frag(v2f i) :COLOR
			{
				float4 c = 1;
				float3 N = normalize(i.normal);
				float3 viewDir = normalize(i.viewDir);
				float3 lightDir = normalize(i.lightDir);
				float diff = max(0,dot(N,i.lightDir));
				diff = (diff + 1) / 2;
				diff = smoothstep(0,1,diff);
				float toon = floor(diff * _Steps) / _Steps;
				diff = lerp(diff,toon,_ToonEffect);

				float4 vc = i.color * _Color;
				if (_HasSaturation != 0)
					vc = saturation(vc, _Saturation);
				
				c = vc * diff;
				
				return c;
			}
			ENDCG
		}

		Pass
		{
			Tags{ "LightMode" = "ShadowCaster" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4 vert(float4 vertex:POSITION) : SV_POSITION
			{
				return UnityObjectToClipPos(vertex);
			}

			float4 frag(float4 vertex:SV_POSITION) : SV_TARGET
			{
				return 0;
			}

			ENDCG
		}
	}
}