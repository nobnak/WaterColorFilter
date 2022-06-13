Shader "Hidden/Toon_Paper" {
	Properties {
		_PaperTex ("Paper", 2D) = "grey" {}
		_PaperScale ("Paper Scale", Float) = 1
		_PaperPower ("Paper Power", Float) = 1
	}
	SubShader {
		Cull Off ZWrite Off ZTest Always

		CGINCLUDE			
			#include "UnityCG.cginc"
			#include "Common.cginc"

			struct v2f_paper {
				float2 uv_Main : TEXCOORD0;
				float2 uv_Paper : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _PaperTex;
			float _PaperScale;
			float _PaperPower;

			v2f_paper vert_paper(appdata v) {
				float aspect = _ScreenParams.x / _ScreenParams.y;

				v2f_paper o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv_Main = v.uv;
				o.uv_Paper = v.uv * float2(aspect, 1) * _PaperScale;
				return o;
			}

			fixed4 frag(v2f_paper i) : SV_Target {
				fixed4 src = tex2D(_MainTex, i.uv_Main);
				fixed paper = tex2D(_PaperTex, i.uv_Paper).x;

				float d = _PaperPower * (paper - 0.5) + 1;
				return ColorMod(src, d);
			}
		ENDCG

		// Paper Layer
		Pass {
			CGPROGRAM
			#pragma vertex vert_paper
			#pragma fragment frag
			ENDCG
		}
	}
}
