Shader "Hidden/Toon_Toon" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_ColorWarm ("Warm Color", Color) = (1, 1, 0.5, 1)
		_ColorCool ("Cool Color", Color) = (0.4, 0.7, 1, 1)
		_LightDir ("Light Dir", Vector) = (0, 0, 1, 0)
		_TonePower ("Tone Power", Float) = 1
	}
	SubShader {
		Cull Off ZWrite Off ZTest Always

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
				float2 uv_Depth : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _CameraDepthNormalsTexture;
			float4 _CameraDepthNormalsTexture_TexelSize;
			float4 _ColorWarm;
			float4 _ColorCool;
			float4 _LightDir;
			float _TonePower;

			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv_Main = v.uv;
				o.uv_Depth = v.uv;
				#ifdef UNITY_UV_STARTS_AT_TOP
				if (_CameraDepthNormalsTexture_TexelSize.y < 0)
					o.uv_Depth.y = 1 - o.uv_Depth.y;
				#endif
				return o;
			}
			fixed4 frag (v2f i) : SV_Target {
				float depth;
				float3 normal;
				DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, i.uv_Depth), depth, normal);
				//normal = normalize(normal);

				float4 src = tex2D(_MainTex, i.uv_Main);

				//return float4(0.5 * (normal + 1), 1);
				float4 tone = lerp(_ColorCool, _ColorWarm, saturate(0.5 * (dot(normal, _LightDir) + 1)));
				return src * tone * _TonePower;
			}
			ENDCG
		}
	}
}
