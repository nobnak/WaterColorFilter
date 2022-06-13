Shader "Hidden/Toon_Wobb" {
	Properties {
		_WobbTex ("Wobbing", 2D) = "grey" {}
		_WobbScale ("Wob Tex Scale", Float) = 1
		_WobbPower ("Wobbing Power", Float) = 0.005
	}
	SubShader {
		Cull Off ZWrite Off ZTest Always

		CGINCLUDE			
			#include "UnityCG.cginc"
			#include "Common.cginc"

			struct v2f_wobb {
				float2 uv_Main : TEXCOORD0;
				float2 uv_Wobb : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _WobbTex;
			float _WobbScale;
			float _WobbPower;

			v2f_wobb vert_wobb(appdata v) {
				float aspect = _ScreenParams.x / _ScreenParams.y;

				v2f_wobb o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv_Main = v.uv;
				o.uv_Wobb = v.uv * float2(aspect, 1) * _WobbScale;
				return o;
			}

			fixed4 frag(v2f_wobb i) : SV_Target{
				fixed2 wobb = tex2D(_WobbTex, i.uv_Wobb).wy * 2 - 1;
				fixed4 src = tex2D(_MainTex, i.uv_Main + wobb * _WobbPower);
				return src;
			}
		ENDCG

		// Wobbing
		Pass {
			CGPROGRAM
			#pragma vertex vert_wobb
			#pragma fragment frag
			ENDCG
		}
	}
}
