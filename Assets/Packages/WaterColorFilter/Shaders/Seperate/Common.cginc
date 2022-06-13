#ifndef __COMMON_TOON__
#define __COMMON_TOON__

#include "UnityCG.cginc"

struct appdata {
	float4 vertex : POSITION;
	float2 uv : TEXCOORD0;
};
struct v2f {
	float2 uv : TEXCOORD0;
	float4 vertex : SV_POSITION;
};

sampler2D _MainTex;
float4 _MainTex_TexelSize;

float4 ColorMod(float4 c, float d) {
	return c - (c - c * c) * (d - 1);
}
v2f vert(appdata v) {
	v2f o;
	o.vertex = UnityObjectToClipPos(v.vertex);
	o.uv = v.uv;
	return o;
}

#endif