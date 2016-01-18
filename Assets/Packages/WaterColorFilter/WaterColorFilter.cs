using UnityEngine;
using System.Collections;

namespace WaterColorFilterSystem {

	[ExecuteInEditMode]
	public class WaterColorFilter : MonoBehaviour {
		public Material filter;

		void OnRenderImage(RenderTexture src, RenderTexture dst) {
			Graphics.Blit(src, dst, filter);
		}
		
	}
}
