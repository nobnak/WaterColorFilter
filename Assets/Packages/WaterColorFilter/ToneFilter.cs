using UnityEngine;
using System.Collections;

namespace WaterColorFilterSystem {

	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class ToneFilter : MonoBehaviour {
		public const string PROP_COLOR_WARM = "_ColorWarm";
		public const string PROP_COLOR_COOL = "_ColorCool";
		public const string PROP_TONE_POWER = "_TonePower";
		public const string PROP_LIGHT_DIR = "_LightDir";

		public Shader filter;

		public Color warm;
		public Color cool;
		public float tonePower = 1f;
		public Transform dirLight;

		Material _filterMat;
		Camera _cam;

		void OnEnable() {
			_cam = GetComponent<Camera>();
			_cam.depthTextureMode = DepthTextureMode.DepthNormals;

			_filterMat = new Material(filter);
		}
		void OnDisable() {
			DestroyImmediate(_filterMat);
		}
		void OnRenderImage(RenderTexture src, RenderTexture dst) {
			var lightDir = _cam.transform.InverseTransformDirection(dirLight == null ? Vector3.forward : dirLight.forward);
			lightDir.x *= -1f;
			lightDir.y *= -1f;
			_filterMat.SetColor(PROP_COLOR_WARM, warm);
			_filterMat.SetColor(PROP_COLOR_COOL, cool);
			_filterMat.SetFloat(PROP_TONE_POWER, tonePower);
			_filterMat.SetVector(PROP_LIGHT_DIR, lightDir);

			Graphics.Blit(src, dst, _filterMat);
		}
	}
}
