using UnityEngine;
using System.Collections;

namespace WaterColorFilterSystem {

	public class Rotator : MonoBehaviour {
		public float speed = 1f;
		public float freq = 1f;

		void Update() {
			var dt = Time.deltaTime * speed;
			var t = Time.timeSinceLevelLoad;
			transform.localRotation *= Quaternion.Euler(
				Noise(freq * t, 0f) * dt,
				Noise(freq * t, 10f) * dt,
				Noise(freq * t, 20f) * dt);

		}
		float Noise(float x, float y) {
			return Mathf.PerlinNoise(x, y) * 2f - 1f;
		}
	}
}
