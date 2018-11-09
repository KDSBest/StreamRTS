Shader "Custom/FogOfWarStamp" {
	Properties {
	}
	SubShader {
		Tags { "RenderType"="Opaque" }

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf SimpleNoLightning

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input {
			float2 uv_MainTex;
		};

	half4 LightingSimpleNoLightning(SurfaceOutput s, half3 lightDir, half atten) {
		return half4(1, 1, 1, 1);
	}

	void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = half3(1, 1, 1, 1);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
