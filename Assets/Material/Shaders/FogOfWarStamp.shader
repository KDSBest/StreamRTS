Shader "Custom/FogOfWarStamp" {
	Properties{
		heightmapScale("Hightposition Int to Float Scale", Range(0, 1000)) = 10
	}
		SubShader{
			Pass
		{
			Tags { "RenderType" = "Opaque" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			float heightmapScale;

		// vertex input: position, normal, tangent
		struct appdata {
			float4 vertex : POSITION;
		};

		struct v2f {
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
		};

		v2f vert(appdata v) {
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv.xy = mul(unity_ObjectToWorld, v.vertex).xy;
			return o;
		}

		fixed frag(v2f i) : SV_Target
		{
			return i.uv.y / heightmapScale;
		}
		ENDCG
			}
	}
	FallBack "Diffuse"
}
