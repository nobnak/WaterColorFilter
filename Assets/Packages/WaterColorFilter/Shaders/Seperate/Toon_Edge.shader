Shader "Hidden/Toon_Edge" {
	Properties {
		_MainTex("Texture", 2D) = "white" {}

		_EdgeSize ("Edge Size", Float) = 1
		_EdgePower ("Edge Power", Float) = 3
	}
	SubShader {
		Cull Off ZWrite Off ZTest Always

		CGINCLUDE			
			#include "UnityCG.cginc"
			#include "Common.cginc"

			float _EdgeSize;
			float _EdgePower;

			fixed4 frag(v2f i) : SV_Target {
				float2 uv_offset = _MainTex_TexelSize.xy * _EdgeSize;
				fixed4 src_l = tex2D(_MainTex, i.uv + float2(-uv_offset.x, 0));
				fixed4 src_r = tex2D(_MainTex, i.uv + float2(+uv_offset.x, 0));
				fixed4 src_b = tex2D(_MainTex, i.uv + float2(0, -uv_offset.y));
				fixed4 src_t = tex2D(_MainTex, i.uv + float2(0, +uv_offset.y));
				fixed4 src = tex2D(_MainTex, i.uv);

				fixed4 grad = abs(src_r - src_l) + abs(src_b - src_t);
				float intens = saturate(0.333 * (grad.x + grad.y + grad.z));
				float d = _EdgePower * intens + 1;
				return ColorMod(src, d);
			}
		ENDCG

		// Edge Darkening
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
}
