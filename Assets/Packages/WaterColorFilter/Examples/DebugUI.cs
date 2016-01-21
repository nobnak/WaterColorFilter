using UnityEngine;
using System.Collections;

namespace WaterColorFilterSystem {
	public class DebugUI : MonoBehaviour {
		ToneFilter _tone;
		WaterColorFilter _water;

		Rect _window = new Rect(10, 10, 80, 50);

		void OnEnable() {
			_tone = GetComponent<ToneFilter> ();
			_water = GetComponent<WaterColorFilter> ();
		}
		void OnGUI() {
			_window = GUILayout.Window (0, _window, Window, "Debug UI");
		}
		void Window(int id) {
			_tone.enabled = GUILayout.Toggle (_tone.enabled, "Tone-Based Shading");
			_water.enabled = GUILayout.Toggle (_water.enabled, "WaterColor Filter");

			GUI.DragWindow ();
		}
	}
}
