Shader "Hidden/WaterColorFilter" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_WobbTex ("Wobbing", 2D) = "grey" {}
		_WobbScale ("Wob Tex Scale", Float) = 1
		_WobbPower ("Wobbing Power", Float) = 1
	}
	SubShader {
		Cull Off ZWrite Off ZTest Always

		CGINCLUDE
		ENDCG

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
			struct v2f {
				float2 uv_Main : TEXCOORD0;
				float2 uv_Wobb : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _WobbTex;
			float _WobbScale;
			float _WobbPower;

			v2f vert(appdata v) {
				float aspect = _ScreenParams.x / _ScreenParams.y;

				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv_Main = v.uv;
				o.uv_Wobb = v.uv * float2(aspect, 1) * _WobbScale;
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target {
				fixed2 wobb = tex2D(_WobbTex, i.uv_Wobb).wy * 2 - 1;
				fixed4 src = tex2D(_MainTex, i.uv_Main + wobb * _WobbPower);
				return src;
			}
			ENDCG
		}
	}
}
