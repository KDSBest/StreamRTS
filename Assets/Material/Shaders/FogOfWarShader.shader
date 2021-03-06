﻿Shader "FullScreenEffects/FogOfWar" {
	Properties{
	_MainTex("Base (RGB)", 2D) = "white" {}
	_HeightTex("HeightMap Texture", 2D) = "white" {}
	_MapTex("Map Texture", 2D) = "white" {}
	_MaskTex("Mask texture", 2D) = "white" {}
	}
		SubShader{
		Pass {
		CGPROGRAM
		#pragma vertex vert_img
		#pragma fragment frag
		#include "UnityCG.cginc"

		uniform sampler2D _MainTex;
		uniform sampler2D _MapTex;
		uniform sampler2D _MaskTex;
		uniform sampler2D _HeightTex;

		fixed _maskBlend;
		fixed _maskSize;

		fixed4 frag(v2f_img i) : COLOR 
		{
			float display = tex2D(_MaskTex, i.uv).r;
			float height = tex2D(_HeightTex, i.uv).r;
			fixed4 base = tex2D(_MainTex, i.uv);
			fixed4 map = tex2D(_MapTex, i.uv);
			

			if (height - display > 0)
			{
				return fixed4(map.r * 0.5, map.g * 0.5, map.b * 0.5, 1);
			}
			
			return base;
		}
		ENDCG
		}
	}
}