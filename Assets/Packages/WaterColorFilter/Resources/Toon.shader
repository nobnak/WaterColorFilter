Shader "Hidden/Toon" {
	Properties {}
	SubShader {
		Cull Off ZWrite Off ZTest Always

CGINCLUDE
#include "UnityCG.cginc"

struct appdata {
	float4 vertex : POSITION;
	float2 uv : TEXCOORD0;
};
struct v2f {
	float4 uv : TEXCOORD0;
	float4 uv1 : TEXCOORD1;
	float4 vertex : SV_POSITION;
};

sampler2D _MainTex;
float4 _MainTex_TexelSize;

sampler2D _CameraDepthNormalsTexture;
float4 _CameraDepthNormalsTexture_TexelSize;

// Toon
float4 _Toon_Warm;
float4 _Toon_Cool;
float4 _Toon_LightDir_Power;

// Edge
float4 _Edge_Size_Power;

// Paper
sampler2D _Paper_Tex;
float4 _Paper_Scale_Power;

// Wobb
sampler2D _Wobb_Tex;
float4 _Wobb_Scale_Power;

float4 ColorMod(float4 c, float d) {
	return c - (c - c * c) * (d - 1);
}
v2f vert (appdata v) {
	v2f o;
	o.vertex = UnityObjectToClipPos(v.vertex);
	o.uv = v.uv.xyxy;
	#ifdef UNITY_UV_STARTS_AT_TOP
	if (_CameraDepthNormalsTexture_TexelSize.y < 0)
		o.uv.w = 1 - o.uv.w;
	#endif
	return o;
}

float4 frag_toon (v2f i) : SV_Target {
	float depth;
	float3 normal;
	DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, i.uv.zw), depth, normal);

	float4 src = tex2D(_MainTex, i.uv.xy);
	float4 tone = lerp(_Toon_Cool, _Toon_Warm, saturate(0.5 * (dot(normal, _Toon_LightDir_Power.xyz) + 1)));
	return src * tone * _Toon_LightDir_Power.w;
}
float4 frag_edge(v2f i) : SV_Target {
	float2 uv_offset = _MainTex_TexelSize.xy * _Edge_Size_Power.xy;
	float4 src_l = tex2D(_MainTex, i.uv + float2(-uv_offset.x, 0));
	float4 src_r = tex2D(_MainTex, i.uv + float2(+uv_offset.x, 0));
	float4 src_b = tex2D(_MainTex, i.uv + float2(0, -uv_offset.y));
	float4 src_t = tex2D(_MainTex, i.uv + float2(0, +uv_offset.y));
	float4 src = tex2D(_MainTex, i.uv);

	float4 grad = abs(src_r - src_l) + abs(src_b - src_t);
	float intens = saturate(0.333 * (grad.x + grad.y + grad.z));
	float d = _Edge_Size_Power.z * intens + 1;
	return ColorMod(src, d);
}
float4 frag_paper(v2f i) : SV_Target {
	float4 src = tex2D(_MainTex, i.uv.xy);
	float paper = tex2D(_Paper_Tex, i.uv.xy * _Paper_Scale_Power.xy).x;

	float d = _Paper_Scale_Power.z * (paper - 0.5) + 1;
	return ColorMod(src, d);
}
float4 frag_wobb(v2f i) : SV_Target{
	float2 wobb = tex2D(_Wobb_Tex, i.uv.xy * _Wobb_Scale_Power.xy).wy * 2 - 1;
	float4 src = tex2D(_MainTex, i.uv.xy + wobb * _Wobb_Scale_Power.z);
	return src;
}
ENDCG

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_toon
			ENDCG
		}
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_edge
			ENDCG
		}
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_wobb
			ENDCG
		}
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_paper
			ENDCG
		}
	}
}
